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
    public class SubGrupo: Modelo
    {
        public SubGrupo()
        {
            icono = "res/documentos/SubGrupo.png";
            niveles = 5;
            nombre = "SubGrupo";
            descripcion = "Grupo de trabajo";
            tipo = "estructura";
            versionar = "titulo";
            consensoMsg = "subGrupo.consensoMsg";

            crearVariables();
        }

        public override string carpeta()
        {
            return "SubGrupo";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            Variable v = new Variable("s.nombreSubGrupo", 30, 1);
            v.className = "textoBig";
            v.editClassName = "editarBig";
            variables.Add(v);
            variables.Add(new Variable("r.accion", 6, 1));
            variables.Add(new Variable("s.introduccion", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.objetivo", 3000, 2));
            variables.Add(new Variable("s.aquien", 3000, 2));
            variables.Add(new Variable("s.consecuencias", 3000, 2));

            //nivel 3
            variables.Add(new Variable("s.recursos", 3000, 3));
            variables.Add(new Variable("s.capacidades", 3000, 3));
            variables.Add(new Variable("s.conclusiones", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.integrantes", 3000, 4));
            variables.Add(new Variable("r.hayUsuarios", 3000, 4));

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
                    if (getText("s.nombreSubGrupo", prop) == "")
                    {
                        addError(1, "El nombre del grupo de trabajo no puede ser vacio");
                        getVariable("s.nombreSubGrupo").className = "errorfino";
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
                    if (getText("s.integrantes", prop) == "" && getRadio("r.hayUsuarios", prop)=="")
                    {
                        addError(4, "Debes proponer integrantes o marcar la casilla");
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

            titulo = getText("s.nombreSubGrupo", prop);
            etiqueta = getText("s.etiqueta", prop);

            //valor default para tipo
            organizaciones.Plataforma plata = (organizaciones.Plataforma)grupo.organizacion;

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";

            //fecha
            if (modo == eModo.consenso)
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Fecha", g.idioma) + ": " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";
           
            //abm controls
            ret += HTMLABM("s.nombreSubGrupo", prop, width, tieneFlores, getListaGTs(), g.idioma);
            ret += "<br>";

            //etiqueta
            ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + "&nbsp;" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma);
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
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
                ret += "<div class='tema'>" + Tools.tr("SubGrupo.introduccion.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("SubGrupo.introduccion.tip", g.idioma)
                        + "</div>";
                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 2)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.consecuencias.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.consecuencias.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.consecuencias", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //Objetivo a lograr
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.objetivo.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.objetivo.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores, g.idioma);

                    //A quien va dirigido
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.aquien.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.aquien.tip", g.idioma)
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
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.conclusiones.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.conclusiones.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.conclusiones", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //Recursos
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.recursos.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.recursos.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.recursos", prop, width, 120, tieneFlores, g.idioma);

                    //Capacidades
                    ret += "<div class='tema'>" + Tools.tr("SubGrupo.capacidades.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("SubGrupo.capacidades.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.capacidades", prop, width, 120, tieneFlores, g.idioma);

                }

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + Tools.tr("SubGrupo.integrantes.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("SubGrupo.integrantes.tip", g.idioma)
                        + "</div>";

                //lista de seleccion de usuarios
                string lista = "";
                foreach (Usuario u2 in g.getUsuariosHabilitados())
                    lista += u2.email + ":" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLListaSeleccion("s.integrantes", prop, width - 50, 250, tieneFlores, lista,
                    Tools.tr("Pertenece al grupo", g.idioma),
                    Tools.tr("NO pertenece al grupo", g.idioma),
                    g.idioma);

                string integrantes = getText("s.integrantes", prop);
                if (integrantes != "" && prop != null)
                    prop.bag["r.hayUsuarios"] = "";


                if (integrantes == "" && (modo == eModo.editar || modo == eModo.revisar))
                {
                    ret += HTMLRadio("r.hayUsuarios", 1, prop, tieneFlores, "No", g.idioma);
                    ret += "<span>" + tr("No hay integrantes") + "</span>";
                }
                else if (integrantes == "")
                {
                    if (getText("r.hayUsuarios", prop) == "No")
                        ret += "<span>" + tr("No hay integrantes") + "</span>";
                }

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + Tools.tr("SubGrupo.valoracion.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("SubGrupo.valoracion.tip", g.idioma)
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores, g.idioma);

                ret += "<div class='tema'>" + Tools.tr("SubGrupo.revision.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("SubGrupo.revision.tip", g.idioma)
                        + "</div>";
                ret += HTMLLista("s.revision", "|Mensual|Trimestral|Semestral|Anual", prop, 250, tieneFlores, g.idioma, false);

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

            //string SubGrupoNombre = doc.titulo;
            //string SubGrupoOrganizacion = (string)doc.getValor("s.organizacion");

            ////creo
            //string retGrupo = Tools.getHttp(doc.grupo.URL + "/" 
            //    + "doMain.aspx?actn=newgrupoadmins?grupo=" + SubGrupoNombre
            //    + "&organizacion=" + SubGrupoOrganizacion
            //    + "&idioma=" + doc.grupo.idioma 
            //    + "&admins=" + admins
            //    + "&padreurl=" + doc.grupo.URL
            //    + "&padrenombre=" + doc.grupo.nombre);

            //doc.addLog(tr("Grupo remoto:" + retGrupo));

            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            if ((string)doc.getValor("r.accion") == "borrar")
            {
                //borro SubGrupo
                foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        plataforma.subgrupos.Remove(gt);
                        doc.addLog(Tools.tr("SubGrupo.eliminado", doc.grupo.idioma));
                        break;
                    }
                }
            }
            else if ((string)doc.getValor("r.accion") == "existente")
            {
                //creo/actualizo SubGrupo actual
                foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        //actualizo
                        gt.docURL = doc.URLPath; //nuevo consenso
                        gt.docTs = DateTime.Now;
                        gt.revision = (string)doc.getValor("s.revision");
                        gt.objetivo = (string)doc.getValor("s.objetivo");

                        if (doc.contains("s.integrantes"))
                        {
                            string integrantes = (string)doc.getValor("s.integrantes");
                            string[] usuarios = integrantes.Split('|');
                            gt.integrantes.Clear();
                            foreach (string usuario in usuarios)
                            {
                                gt.integrantes.Add(usuario.Split(':')[0]);
                            }
                        }
                        else
                            //no hay integrantes
                            gt.integrantes.Clear();

                        doc.addLog(Tools.tr("SubGrupo.actualizado", doc.grupo.idioma));
                    }
                }
            }
            else
            {
                //nuevo
                nabu.plataforma.SubGrupo gt = new plataforma.SubGrupo();
                gt.EID = plataforma.getEID();
                gt.grupoIdioma = doc.grupo.idioma;
                gt.nombre = doc.titulo;
                gt.docURL = doc.URLPath;
                gt.docTs = DateTime.Now;
                gt.revision = (string)doc.getValor("s.revision");
                gt.objetivo = (string)doc.getValor("s.objetivo");

                string integrantes = "";
                if (doc.contains("s.integrantes"))
                {
                    integrantes = (string)doc.getValor("s.integrantes");
                    string[] usuarios = integrantes.Split('|');
                    foreach (string usuario in usuarios)
                    {
                        doc.addLog(Tools.tr("%1 agregado", usuario.Split(':')[0], doc.grupo.idioma));
                        gt.integrantes.Add(usuario.Split(':')[0]);
                    }
                }
                else
                    doc.addLog(Tools.tr("No hay integrantes", doc.grupo.idioma));


                //agrego
                plataforma.subgrupos.Add(gt);

                doc.addLog(Tools.tr("SubGrupo.creado", doc.grupo.idioma));
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
            else if (accion == "r.accion_click" && parametro == "nuevo" && props.Count > 0)
            {
                for (int i = 1; i < props.Count - 1; i++)
                    props.RemoveAt(i);

                props[0].bag.Clear();
                props[0].bag["r.accion"] = "nuevo";
            }
            else if (accion == "r.accion_click" && parametro == "existente" && props.Count > 0)
            {
                //traer datos del coumento seleccionado si es una modificaicon
                string nombre;
                if (props[0].bag.ContainsKey("s.nombreSubGrupo"))
                    nombre = (string)props[0].bag["s.nombreSubGrupo"];
                else
                    nombre = getPrimerGT();
                    
                getContenidoDocumentoPrevio(nombre, props, g);
            }
            else if (accion == "s.nombreSubGrupo_click")
            {
                if ((string)props[0].bag["r.accion"] == "existente")
                {
                    //traer datos si es una modificaicon
                    string nombre = (string)props[0].bag["s.nombreSubGrupo"];
                    getContenidoDocumentoPrevio(nombre, props, g);
                }
            }

            return toHTML(props, g, email, width, modo);
        }

        private string getListaGTs()
        {
            string ret = "";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.SubGrupo gt in pl.subgrupos)
            {
                ret += gt.nombre + "|";
            }
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getPrimerGT()
        {
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            if (pl.subgrupos.Count > 0)
                return pl.subgrupos[0].nombre;
            else
                return "";
        }

        public void getContenidoDocumentoPrevio(string titulo, List<Propuesta> props, Grupo grupo)
        {
            //si es una modificaicon y el primero nivel esta vacio entonces traigo los datos del documento a modificar
            //busco en el logDocumentos
            LogDocumento versionAnterior = null;
            foreach (LogDocumento ldi in grupo.logDecisiones)
                if (ldi.modeloNombre == nombre && ldi.titulo == titulo)
                    versionAnterior = ldi; //me quedo con el utlimo

            if (versionAnterior != null)
            {
                //traigo datos de este doc
                if (System.IO.File.Exists(grupo.path + "\\" + versionAnterior.path))
                {
                    string json = System.IO.File.ReadAllText(grupo.path + "\\" + versionAnterior.path);
                    Documento doc = Tools.fromJson<Documento>(json);
                    //agrego contenido
                    props.Clear();
                    foreach (Propuesta prop in doc.propuestas)
                    {
                        prop.nodoID = 0;
                        prop.consensoAlcanzado = false;
                        props.Add(prop);
                    }
                    if (props.Count > 0)
                        props[0].bag["r.accion"] = "existente"; //este valor permanece
                }
            }
        }
    }

}

