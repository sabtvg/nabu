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
    public class Acta: Modelo
    {
        public Acta()
        {
            icono = "res/documentos/acta.png";
            niveles = 1;
            nombre = "Acta";
            descripcion = "Acta";
            permisos = "isSecretaria";
            tipo = "seguimiento";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Acta";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.tipo", 200, 1));
            variables.Add(new Variable("s.subgrupoIntro", 200, 1));
            variables.Add(new Variable("s.subgrupoIntegrantes", 200, 1));
            variables.Add(new Variable("s.subgrupo", 200, 1));

            variables.Add(new Variable("f.temas", 8, 1));
            variables.Add(new Variable("d.fecha", 12, 1));
            variables.Add(new Variable("d.fechaProxima", 12, 1));
            variables.Add(new Variable("s.titulo", 200, 1));
            variables.Add(new Variable("s.lugar", 200, 1));
            variables.Add(new Variable("s.participan", 200, 1));
            variables.Add(new Variable("s.apertura", 4000, 1));
            variables.Add(new Variable("s.logisticos", 4000, 1));
            variables.Add(new Variable("s.ordendeldia", 4000, 1));
            variables.Add(new Variable("s.evaluacion", 4000, 1));

            for (int q = 0; q < 10; q++)
            {
                variables.Add(new Variable("s.tituloTema" + q, 100, 1));
                variables.Add(new Variable("s.textoTema" + q, 3000, 1));
            }

        }

        private void validar(Propuesta prop, string idioma)
        {
            if (prop != null)
            {
                string tipo = getText("s.tipo", prop);
                if (prop.nivel == 1 && tipo == "subgrupo")
                {
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, Tools.tr("Definir un titulo de Acta", idioma));
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if (getText("s.subgrupoIntro", prop) == "")
                    {
                        addError(1, Tools.tr("Definir una introduccion", idioma));
                        getVariable("s.subgrupoIntro").className = "errorfino";
                    }
                    if (getText("s.subgrupoIntegrantes", prop) == "")
                    {
                        addError(1, Tools.tr("Se deben definir los integrantes", idioma));
                        getVariable("s.subgrupoIntegrantes").className = "errorfino";
                    }
                }
                else if (prop.nivel == 1 && tipo == "Reunion")
                {
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, Tools.tr("Definir un titulo de Acta", idioma));
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if (getText("s.participan", prop) == "")
                    {
                        addError(1, Tools.tr("Se deben definir los participantes", idioma));
                        getVariable("s.participan").className = "errorfino";
                    }
                    if (getText("s.apertura", prop) == "")
                    {
                        addError(1, Tools.tr("Definir apertura", idioma));
                        getVariable("s.apertura").className = "errorfino";
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

            if (prop != null)
                prop.bag["s.etiqueta"] = "Acta";

            //titulo
            titulo = getText("s.titulo", prop);

            //fecha
            if (modo == eModo.consenso)
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Fecha", g.idioma) + ": " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            etiqueta = "Acta";

            //nombre modelo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";

            //tipo
            ret += HTMLTabs("Reunion|Subgrupo", "s.tipo", prop, g);
            ret += "<br>";
            ret += "<br>";

            //titulo y etiqueta
            ret += "<table>";
            ret += "<tr>";
            ret += "<td class='titulo3'>" + Tools.tr("Titulo", g.idioma) + "</td>";
            ret += "<td colspan=2>" + HTMLText("s.titulo", prop, 60 * 8, tieneFlores, g.idioma) + "</td>";
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td class='titulo3'>" + Tools.tr("Etiqueta", g.idioma) + "</td>";
            ret += "<td class='titulo3' style='width:80px'>" + Tools.tr("Acta", g.idioma) + "</td>";
            ret += "<td>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</td>";
            ret += "</tr>";
            ret += "</table>";
            ret += "<br>";

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
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado && u.isSecretaria;


            //validaciones de este nivel
            validar(prop, g.idioma);

            string tipo = "Reunion";
            if (prop != null) tipo = getText("s.tipo", prop);
            if (tipo == "") tipo = "Reunion";
            if (nivel == 1 && tipo == "Subgrupo")
            {
                ret += HTMLEncabezado(prop, g, email, width);

                string listaGTs = getListaGTs();

                if (listaGTs == "")
                    ret += "<div class='tema'>" + Tools.tr("No hay subgrupos", g.idioma) + "</div>";
                else
                {
                    //introduccion
                    ret += HTMLSeccion("acta.subgrupo.introduccion", "acta.subgrupo.introduccion.tip", "s.subgrupoIntro", editar, prop, tieneFlores, g, width);

                    //subgrupo
                    ret += "<div class='tema'>" + Tools.tr("acta.subgrupo.subgrupo", g.idioma) + "</div>";
                    ret += HTMLLista("s.subgrupo", listaGTs, prop, 40 * 8, tieneFlores, g.idioma);

                    //Integrantes
                    ret += "<div class='tema'>" + Tools.tr("Integrantes", g.idioma) + "</div>";
                    //lista de seleccion de usuarios
                    string lista = "";
                    foreach (Usuario u2 in g.getUsuariosHabilitados())
                        lista += u2.email + ":" + u2.nombre + "|";
                    lista = lista.Substring(0, lista.Length - 1);
                    ret += HTMLListaSeleccion("s.subgrupoIntegrantes", prop, width - 150, 250, tieneFlores, lista,
                        Tools.tr("Es integrantes", g.idioma),
                        Tools.tr("NO es integrantes", g.idioma),
                        g.idioma);
                }

            }
            else if (nivel == 1 && tipo == "Reunion")
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //fecha
                ret += "<div class='tema'>" + Tools.tr("Fecha celebracion", g.idioma) + "</div>";
                ret += HTMLDate("d.fecha", prop, tieneFlores, g.idioma);
                ret += "<br>";
                ret += "<br>";

                //fecha proxima reunion
                ret += "<div class='tema'>" + Tools.tr("Fecha proxima celebracion", g.idioma) + "</div>";
                ret += HTMLDate("d.fechaProxima", prop, tieneFlores, g.idioma);
                ret += "<br>";
                ret += "<br>";

                //lugar
                ret += "<div class='tema'>" + Tools.tr("Lugar", g.idioma) + "</div>";
                ret += HTMLText("s.lugar", prop, width - 250, tieneFlores, g.idioma);
                ret += "<br>";
                ret += "<br>";

                //participan
                ret += "<div class='tema'>" + Tools.tr("Participantes", g.idioma) + "</div>";
                //lista de seleccion de usuarios
                string lista = "";
                foreach (Usuario u2 in g.usuarios)
                    lista += u2.email + ":" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLListaSeleccion("s.participan", prop, width - 250, 250, tieneFlores, lista,
                    Tools.tr("Presente", g.idioma),
                    Tools.tr("NO presente", g.idioma),
                    g.idioma);

                //apertura
                ret += HTMLSeccion("acta.apertura.titulo", "acta.apertura.tip", "s.apertura", editar, prop, tieneFlores, g, width);

                //Aspectos logisticos
                ret += HTMLSeccion("acta.logisticos.titulo", "acta.logisticos.tip", "s.logisticos", editar, prop, tieneFlores, g, width);

                //Orden del dia
                ret += HTMLSeccion("acta.ordendeldia.titulo", "acta.ordendeldia.tip", "s.ordendeldia", editar, prop, tieneFlores, g, width);

                //temas
                float temas = 0;
                if (prop != null && prop.bag.ContainsKey("f.temas"))
                {
                    temas = float.Parse(prop.bag["f.temas"].ToString());
                    if (temas > 10) temas = 10;
                    for (int q = 0; q < temas; q++)
                    {
                        ret += "<div class='tema'>" + Tools.tr("Tema", g.idioma) + " " + (q + 1) + ":&nbsp;" + HTMLText("s.tituloTema" + q, prop, 70 * 8, tieneFlores, g.idioma) + "</div>";
                        ret += HTMLArea("s.textoTema" + q, prop, width, 220, tieneFlores, g.idioma);
                    }
                }
                if (editar && temas < 10)
                    ret += "<input type='button' class='btn' value='" + Tools.tr("Agregar tema", g.idioma) + "' onclick=\"documentSubmit('agregarTema','')\">";
                ret += "<input type='hidden' id='f.temas' value='" + temas.ToString("0") + "'>"; //guardo valor de Temas en el propio documento

                //Evaluacion
                ret += HTMLSeccion("acta.evaluacion.titulo", "acta.evaluacion.tip", "s.evaluacion", editar, prop, tieneFlores, g, width);
                //ret += "<div class='tema'>" + Tools.tr("acta.evaluacion.titulo", g.idioma) + "</div>";
                //if (editar)
                //    ret += "<div class='smalltip'>" + Tools.tr("acta.evaluacion.tip", g.idioma) + "</div>";
                //ret += HTMLArea("s.evaluacion", prop, width, 290, tieneFlores, g.idioma);
                //ret += "<br>";

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else
            {
                throw new Exception(Tools.tr("Nivel [%1] no existe en este modelo", nivel.ToString(), g.idioma));
            }

            if (prop != null) prop.niveles = niveles; //esto es importante si cambian los niveles para que se traspase luego al nodo

            //fin nivel
            if (prop != null && prop.nodoID != 0 && modo != eModo.consenso)
                ret += HTMLFlores(g.arbol.getNodo(prop.nodoID), false, g.getUsuario(email));

            //mensajes de error
            if (errores.ContainsKey(nivel))
            {
                ret += "<div class='error'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override string documentSubmit(string accion, string parametro, List<Propuesta> props, Grupo g, string email, int width, Modelo.eModo modo)
        {
            if (accion == "Reunion_click" && props.Count == 1)
            {
                Propuesta p = props[0];
                p.bag["s.tipo"] = "Reunion";
            }
            else if (accion == "Subgrupo_click" && props.Count == 1)
            {
                Propuesta p = props[0];
                p.bag["s.tipo"] = "Subgrupo";
            } 
            else if (accion == "agregarTema" && props.Count == 1)
            {
                Propuesta p = props[0];
                float temas = (float)p.bag["f.temas"];     
                if (temas < 10)
                    p.bag["f.temas"] = temas + 1.0;
            }
            else if (accion == "s.participan_agregar" && getVariable("s.participan").nivel <= props.Count)
            {
                Variable v = getVariable("s.participan");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.participan"];

                    if (value == "*")
                        prop.bag["s.participan"] = parametro; //caso inicial
                    else
                        prop.bag["s.participan"] += "|" + parametro;
                }
            }
            else if (accion == "s.participan_quitar" && getVariable("s.participan").nivel <= props.Count)
            {
                Variable v = getVariable("s.participan");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.participan"];

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
                    prop.bag["s.participan"] = ret;
                }
            }
            else if (accion == "s.subgrupoIntegrantes_agregar" && getVariable("s.subgrupoIntegrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.subgrupoIntegrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.subgrupoIntegrantes"];

                    if (value == "*")
                        prop.bag["s.subgrupoIntegrantes"] = parametro; //caso inicial
                    else
                        prop.bag["s.subgrupoIntegrantes"] += "|" + parametro;
                }
            }
            else if (accion == "s.subgrupoIntegrantes_quitar" && getVariable("s.subgrupoIntegrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.subgrupoIntegrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.subgrupoIntegrantes"];

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
                    prop.bag["s.subgrupoIntegrantes"] = ret;
                }
            }

            return toHTML(props, g, email, width, modo);
        }

        public override void ejecutarConsenso(Documento doc)
        {
            //creo/actualizo SubGrupo actual
            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
            {
                string subgrupo = doc.getText("s.subgrupo");
                if (gt.nombre == subgrupo)
                {
                    //actualizo
                    gt.docURL = doc.URLPath; //nuevo consenso
                    gt.docTs = DateTime.Now;

                    if (doc.contains("s.subgrupoIntegrantes"))
                    {
                        string integrantes = (string)doc.getValor("s.subgrupoIntegrantes");
                        string[] usuarios = integrantes.Split('|');
                        gt.integrantes.Clear();
                        foreach (string usuario in usuarios)
                        {
                            gt.integrantes.Add(usuario.Split(':')[0]);
                            doc.addLog(Tools.tr("%1 agregado", usuario.Split(':')[0], doc.grupo.idioma));
                        }
                    }
                    else
                        //no hay integrantes
                        gt.integrantes.Clear();

                    doc.addLog(Tools.tr("SubGrupo.actualizado", doc.grupo.idioma));
                }
            }

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
    }
}

