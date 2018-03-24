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
                if (prop.nivel == 1)
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
                    if (getText("s.ordendeldia", prop) == "")
                    {
                        addError(1, Tools.tr("Definir orden del dia", idioma));
                        getVariable("s.ordendeldia").className = "errorfino";
                    }
                    if (getText("s.logisticos", prop) == "")
                    {
                        addError(1, Tools.tr("Aspectos logisticos", idioma));
                        getVariable("s.logisticos").className = "errorfino";
                    }
                    if (getText("s.evaluacion", prop) == "")
                    {
                        addError(1, Tools.tr("Definir evaluacion", idioma));
                        getVariable("s.evaluacion").className = "errorfino";
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

            titulo = getText("s.titulo", prop);
            etiqueta = "Acta";

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";
            ret += "<br>";
            ret += "<br>";

            ret += "<div class='titulo3'><nobr>" + Tools.tr("Titulo", g.idioma) + ":" + HTMLText("s.titulo", prop, 70 * 8, tieneFlores, g.idioma);
            ret += "</nobr></div>";

            //etiqueta
            ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ": " + Tools.tr("Acta", g.idioma);
            etiqueta = Tools.tr("Manifiesto", g.idioma);
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
            ret += "</nobr></div>";
            ret += "<br>";
            ret += "<br>";
            return ret;
        }

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuario(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            bool editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
            editar = editar && !consensoAlcanzado;
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado;


            //validaciones de este nivel
            validar(prop, g.idioma);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);            

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

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
                ret += HTMLText("s.lugar", prop, width, tieneFlores, g.idioma);
                ret += "<br>";
                ret += "<br>";

                //participan
                ret += "<div class='tema'>" + Tools.tr("Participantes", g.idioma) + "</div>";
                //lista de seleccion de usuarios
                string lista = "";
                foreach (Usuario u2 in g.usuarios)
                    lista += u2.email + ":" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLListaSeleccion("s.participan", prop, width - 150, 250, tieneFlores, lista, "Presente", "NO presente", g.idioma);

                //apertura
                ret += "<div class='tema'>" + Tools.tr("Ronda de apertura", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("acta.apertura", g.idioma) + "</div>";
                ret += HTMLArea("s.apertura", prop, width, 290, tieneFlores, g.idioma);

                //Aspectos logisticos
                ret += "<div class='tema'>" + Tools.tr("Aspectos logisticos", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("acta.logisticos", g.idioma) + "</div>";
                ret += HTMLArea("s.logisticos", prop, width, 290, tieneFlores, g.idioma);
                ret += "<br>";

                //Orden del dia
                ret += "<div class='tema'>" + Tools.tr("Orden del dia", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("acta.ordendeldia", g.idioma) + "</div>";
                ret += HTMLArea("s.ordendeldia", prop, width, 290, tieneFlores, g.idioma);

                //temas
                float temas = 0;
                if (prop != null)
                {
                    temas = float.Parse(prop.bag["f.temas"].ToString());
                    for (int q = 0; q < temas; q++)
                    {
                        ret += "<div class='tema'>" + Tools.tr("Tema", g.idioma) + " " + (q + 1) + ":" + HTMLText("s.tituloTema" + q, prop, 70 * 8, tieneFlores, g.idioma) + "</div>";
                        ret += HTMLArea("s.textoTema" + q, prop, width, 220, tieneFlores, g.idioma);
                    }
                }
                if (editar)
                    ret += "<input type='button' class='btn' value='" + Tools.tr("Agregar tema", g.idioma) + "' onclick=\"documentSubmit('agregarTema','')\">";
                ret += "<input type='hidden' id='f.temas' value='" + temas.ToString("0") + "'>"; //guardo valor de Temas en el propio documento
                ret += "<br>";
                ret += "<br>";

                //Evaluacion
                ret += "<div class='tema'>" + Tools.tr("Evaluacion", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("acta.evaluacion", g.idioma) + "</div>";
                ret += HTMLArea("s.evaluacion", prop, width, 290, tieneFlores, g.idioma);
                ret += "<br>";

                //variante
                if (puedeVariante && u.isSecretaria)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

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
            if (accion == "agregarTema" && props.Count == 1)
            {
                Propuesta p = props[0];
                float temas = (float)p.bag["f.temas"];     
                if (temas < 9)
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

            return toHTML(props, g, email, width, modo);
        }

    }
}

