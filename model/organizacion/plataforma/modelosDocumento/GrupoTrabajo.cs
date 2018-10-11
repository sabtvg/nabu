///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo nabu@nabu.pt
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
///////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class GrupoTrabajo: Modelo
    {
        private string accion = ""; //para que existe en otros nivels mientras dibujo

        public GrupoTrabajo()
        {
            icono = "res/documentos/grupoTrabajo.png";
            niveles = 5;
            nombre = "GrupoTrabajo";
            descripcion = "Grupo de trabajo";
            tipo = "estructura";
            versionar = "titulo";

            crearVariables();
        }

        public override string carpeta()
        {
            return "GrupoTrabajo";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            Variable v = new Variable("s.nombreGrupoTrabajo", 30, 1);
            v.className = "textoBig";
            v.editClassName = "editarBig";
            variables.Add(v);
            variables.Add(new Variable("r.accion", 6, 1));
            variables.Add(new Variable("s.introduccion", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.objetivo", 3000, 2));
            variables.Add(new Variable("s.descripcion", 3000, 2));
            variables.Add(new Variable("s.aquien", 3000, 2));
            variables.Add(new Variable("s.consecuencias", 3000, 2));

            //nivel 3
            variables.Add(new Variable("s.recursos", 3000, 3));
            variables.Add(new Variable("s.capacidades", 3000, 3));
            variables.Add(new Variable("s.conclusiones", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.integrantes", 3000, 4));

            //nivel 5
            variables.Add(new Variable("s.eficiencia", 3000, 5));
            variables.Add(new Variable("s.revision", 0, 5));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.etiqueta", prop) == "")
                    {
                        addError(1, "La etiqueta determina el nombre con que aparece en el arbol, no puede ser vacia");
                        getVariable("s.etiqueta").className = "errorfino";
                    }
                    if (getText("s.nombreGrupoTrabajo", prop) == "")
                    {
                        addError(1, "El nombre del grupo de trabajo no puede ser vacio");
                        getVariable("s.nombreGrupoTrabajo").className = "errorfino";
                    }
                    if (getText("s.introduccion", prop) == "")
                    {
                        addError(1, "La introduccion no puede ser vacia");
                        getVariable("s.introduccion").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("s.objetivo", prop) == ""
                        && getText("s.descripcion", prop) == ""
                        && getText("s.aquien", prop) == "" 
                        && getText("s.consecuencias", prop) == "")
                    {
                        addError(2, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.capacidades", prop) == ""
                        && getText("s.recursos", prop) == ""
                        && getText("s.procesos", prop) == ""
                        && getText("s.conclusiones", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.integrantes", prop) == "")
                    {
                        addError(4, "Debes proponer integrantes");
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.revision", prop) == "")
                    {
                        addError(5, "Debes seleccionar un periodo de revisi&oacute;n");
                        getVariable("s.revision").className = "errorfino";
                    }


                    if (getText("s.eficiencia", prop) == "" && getText("s.revision", prop) == "")
                    {
                        addError(5, "La propuesta no puede estar completamente vacia");
                    }
                }
            }
        }

        override protected string HTMLEncabezado(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            titulo = getText("s.nombreGrupoTrabajo", prop);
            etiqueta = getText("s.etiqueta", prop);

            //valor default para tipo
            organizaciones.Plataforma plata = (organizaciones.Plataforma)grupo.organizacion;

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";
            //fecha
            if (modo == eModo.consenso)
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //nombre nuevo o existente o borrar
            //nuevo
            ret += "<table>";
            ret += "<tr>";
            ret += "<td colspan=2 class='titulo3'><nobr>" + Tools.tr("Accion", g.idioma) + "</nobr></td>";
            ret += "<td colspan=2 class='titulo3'><nobr>" + Tools.tr("Nombre", g.idioma) + "</nobr></td>";
            ret += "</tr>";

            ret += "<tr>";
            ret += "<td>" + HTMLRadio("r.accion", 1, prop, tieneFlores, "nuevo", g.idioma) + "</td>";
            ret += "<td style='vertical-align:middle'>" + Tools.tr("Crear nuevo grupo de trabajo", g.idioma) + "</td>";
            ret += "<td class='titulo2'>";
            //nombre del grupo
            if (prop != null && accion == "nuevo")
            {
                ret += HTMLText("s.nombreGrupoTrabajo", prop, 60 * 8, tieneFlores, g.idioma);
            }
            ret += "</td>";
            ret += "</tr>";

            //existente
            string listaGTs = getListaGTs();
            if (listaGTs != "")
            {
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 2, prop, tieneFlores, "existente", g.idioma) + "</td>";
                ret += "<td style='vertical-align:middle'>" + Tools.tr("Modificar grupo de trabajo", g.idioma) + "</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "existente")
                {
                    ret += HTMLLista("s.nombreGrupoTrabajo", listaGTs, prop, 60 * 8, tieneFlores, g.idioma);
                }
                ret += "</tr>";

                //borrar
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 3, prop, tieneFlores, "borrar", g.idioma) + "</td>";
                ret += "<td style='vertical-align:middle'>" + Tools.tr("Eliminar grupo de trabajo", g.idioma) + "</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "borrar")
                {
                    ret += HTMLLista("s.nombreGrupoTrabajo", getListaGTs(), prop, 60 * 8, tieneFlores, g.idioma);
                }
                ret += "</tr>";
            }
            ret += "</table><br>";

            //etiqueta
            ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ":" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma);
            if (prop == null)
                ret += "<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
            ret += "</nobr></div>";
            return ret;
        }

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width, Propuesta propFinal)
        {
            string ret = "";
            Usuario u = g.getUsuario(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            bool editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
            editar = editar && !consensoAlcanzado;
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado;

            //accion
            if (prop != null)
            {
                if (getText("r.accion", prop) != "")
                    accion = getText("r.accion", prop);
                else
                    prop.bag["r.accion"] = accion;
            }
            if (accion == "borrar")
                niveles = 3;
            else
                niveles = 5;

            //validaciones de este nivel
            if (modo == eModo.prevista) validar(prop);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //tema
                ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.introduccion.titulo", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip'>"
                        + Tools.tr("grupoTrabajo.introduccion.tip", g.idioma) 
                        + "</div>";
                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 2)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.consecuencias.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.consecuencias.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.consecuencias", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //Objetivo a lograr
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.objetivo.titulo", g.idioma) + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.objetivo.tip", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores, g.idioma);

                    //Descripcion
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.descripcion.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.descripcion.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores, g.idioma);

                    //A quien va dirigido
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.aquien.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.aquien.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores, g.idioma);
                }

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);

            }
            else if (nivel == 3)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.conclusiones.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.conclusiones.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.conclusiones", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //Recursos
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.recursos.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.recursos.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.recursos", prop, width, 120, tieneFlores, g.idioma);

                    //Capacidades
                    ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.capacidades.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("grupoTrabajo.capacidades.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.capacidades", prop, width, 120, tieneFlores, g.idioma);

                }

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.integrantes.titulo", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip'>"
                        + Tools.tr("grupoTrabajo.integrantes.tip", g.idioma) 
                        + "</div>";

                //lista de seleccion de usuarios
                string lista = "";
                foreach(Usuario u2 in g.usuarios) 
                    lista += u2.email + ":" + u2.nombre + "|";
                lista = lista .Substring(0, lista.Length-1);
                ret += HTMLListaSeleccion("s.integrantes", prop, width - 150, 250, tieneFlores, lista, 
                    Tools.tr("Pertenece al grupo", g.idioma), 
                    Tools.tr("NO pertenece al grupo", g.idioma), 
                    g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.valoracion.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("grupoTrabajo.valoracion.tip", g.idioma)
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores, g.idioma);

                ret += "<div class='tema'>" + Tools.tr("grupoTrabajo.revision.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("grupoTrabajo.revision.tip", g.idioma)
                        + "</div>";
                ret += HTMLLista("s.revision", "|Mensual|Trimestral|Semestral|Anual", prop, 250, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else
            {
                throw new Exception("Nivel [" + nivel + "] no existe en este modelo");
            }

            if (prop != null) prop.niveles = niveles; //esto es importante si cambian los niveles para que se traspase luego al nodo

            //fin nivel
            if (prop != null && prop.nodoID != 0 && modo != eModo.consenso && g.arbol.getNodo(prop.nodoID) != null)
                ret += HTMLFlores(g.arbol.getNodo(prop.nodoID), false, g.getUsuario(email));

            //mensajes de error
            if (errores.ContainsKey(nivel))
            {
                ret += "<div class='error'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override int getRevisionDias(List<Propuesta> props)
        {
            int ret = 0;
            foreach (Propuesta p in props)
                foreach (string key in p.bag.Keys)
                    if (key == "s.revision")
                        switch ((string)p.bag[key])
                        {
                            case "Mensual":
                                ret = 30;
                                break;
                            case "Trimestral":
                                ret = 90;
                                break;
                            case "Semestral":
                                ret = 180;
                                break;
                            case "Anual":
                                ret = 365;
                                break;
                        }
            return ret;
        }

        public override void ejecutarConsenso(Documento doc)
        {
            ////creo grupo remoto si no existe    
            ////usuarios
            //string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
            //string admins = "";
            //foreach(string usuario in usuarios)
            //{
            //    Usuario u = doc.grupo.getUsuario(usuario.Split(':')[1]);
            //    admins += u.nombre + ":" + u.email + ":" + u.clave + "|"; 
            //}
            //if (admins != "") admins = admins.Substring(0, admins.Length - 1);

            //string grupoTrabajoNombre = doc.titulo;
            //string grupoTrabajoOrganizacion = (string)doc.getValor("s.organizacion");

            ////creo
            //string retGrupo = Tools.getHttp(doc.grupo.URL + "/" 
            //    + "doMain.aspx?actn=newgrupoadmins?grupo=" + grupoTrabajoNombre
            //    + "&organizacion=" + grupoTrabajoOrganizacion
            //    + "&idioma=" + doc.grupo.idioma 
            //    + "&admins=" + admins
            //    + "&padreurl=" + doc.grupo.URL
            //    + "&padrenombre=" + doc.grupo.nombre);

            //doc.addLog(tr("Grupo remoto:" + retGrupo));

            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            if ((string)doc.getValor("r.accion") == "borrar")
            {
                //borro grupoTrabajo
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        plataforma.gruposTrabajo.Remove(gt);
                        doc.addLog(Tools.tr("grupoTrabajo.eliminado", doc.grupo.idioma));
                        break;
                    }
                }
            }
            else if ((string)doc.getValor("r.accion") == "existente")
            {
                //creo/actualizo grupoTrabajo actual
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        //actualizo
                        gt.docURL = doc.URLPath; //nuevo consenso
                        gt.docTs = DateTime.Now;
                        gt.revision = (string)doc.getValor("s.revision");
                        gt.objetivo = (string)doc.getValor("s.objetivo");

                        string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
                        gt.integrantes.Clear();
                        foreach (string usuario in usuarios)
                        {
                            gt.integrantes.Add(usuario.Split(':')[0]);
                        }
                        doc.addLog(Tools.tr("grupoTrabajo.actualizado", doc.grupo.idioma));
                    }
                }
            }
            else
            {
                //nuevo
                nabu.plataforma.GrupoTrabajo gt = new plataforma.GrupoTrabajo();
                gt.EID = plataforma.getEID();
                gt.grupoIdioma = doc.grupo.idioma;
                gt.nombre = doc.titulo;
                gt.docURL = doc.URLPath;
                gt.docTs = DateTime.Now;
                gt.revision = (string)doc.getValor("s.revision");
                gt.objetivo = (string)doc.getValor("s.objetivo");

                string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
                foreach (string usuario in usuarios)
                {
                    gt.integrantes.Add(usuario.Split(':')[0]);
                }
                plataforma.gruposTrabajo.Add(gt);

                doc.addLog(Tools.tr("grupoTrabajo.creado", doc.grupo.idioma));
            }
        }

        public override string documentSubmit(string accion, string parametro, List<Propuesta> props, Grupo g, string email, int width, Modelo.eModo modo)
        {
            this.grupo = g;

            if (accion == "s.integrantes_agregar" && getVariable("s.integrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.integrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.integrantes"];

                    if (value == "*")
                        prop.bag["s.integrantes"] = parametro; //caso inicial
                    else
                        prop.bag["s.integrantes"] += "|" + parametro;
                }
            }
            else if (accion == "s.integrantes_quitar" && getVariable("s.integrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.integrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.integrantes"];

                    //quito
                    string ret = "";
                    foreach (string item in value.Split('|'))
                    {
                        if (!item.StartsWith(parametro.Split(':')[0]))
                        {
                            ret += item + "|";
                        }
                    }
                    if (ret != "") ret = ret.Substring(0, ret.Length - 1);
                    prop.bag["s.integrantes"] = ret;
                }
            }
            else if (accion == "r.accion_click" && parametro == "existente" && props.Count == 1)
            {
                //traer datos del coumento seleccionado si es una modificaicon
                string nombre;
                if (props[0].bag.ContainsKey("s.nombreGrupoTrabajo"))
                    nombre = (string)props[0].bag["s.nombreGrupoTrabajo"];
                else
                    nombre = getPrimerGT();
                    
                getContenidoDocumentoPrevio(nombre, props, g);
            }
            else if (accion == "s.nombreGrupoTrabajo_click")
            {
                //traer datos si es una modificaicon
                string nombre = (string)props[0].bag["s.nombreGrupoTrabajo"];
                getContenidoDocumentoPrevio(nombre, props, g);
            }

            return toHTML(props, g, email, width, modo);
        }

        private string getListaGTs()
        {
            string ret = "";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.GrupoTrabajo gt in pl.gruposTrabajo)
            {
                ret += gt.nombre + "|";
            }
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getPrimerGT()
        {
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            if (pl.gruposTrabajo.Count > 0)
                return pl.gruposTrabajo[0].nombre;
            else
                return "";
        }
    }

}

