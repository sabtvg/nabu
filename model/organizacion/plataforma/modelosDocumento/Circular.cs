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

//Los modelos deben darse de alta en Modelo.getModelosDocumento()

namespace nabu.plataforma.modelos
{
    public class Circular: Modelo
    {
        public Circular()
        {
            icono = "res/documentos/Circular.png";
            niveles = 4;
            nombre = "Circular";
            descripcion = "Comunicado a todos los grupos hijo";
            tipo = "hijo";
            consensoMsg = "circular.consensoMsg";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Circular";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            variables.Add(new Variable("s.titulo", 70, 1));
            variables.Add(new Variable("s.introduccion", 4000, 1));

            //nivel 2
            variables.Add(new Variable("s.situacionactual", 4000, 2));

            //nivel 3
            variables.Add(new Variable("s.propuesta", 4000, 3));

            //nivel 4
            variables.Add(new Variable("s.situaciondeseada", 12000, 4));
        }

        private void validar(Propuesta prop, string idioma)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, Tools.tr("El titulo del comunicado no puede ser vacio", idioma));
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if (getText("s.introduccion", prop) == "")
                    {
                        addError(1, Tools.tr("La introduccion no puede ser vacia", idioma));
                        getVariable("s.vision").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("s.situacionactual", prop) == "")
                    {
                        addError(2, Tools.tr("La situacion actual no ser vacia", idioma));
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.propuesta", prop) == "")
                    {
                        addError(2, Tools.tr("La propuesta no puede ser vacia", idioma));
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.situaciondeseada", prop) == "")
                    {
                        addError(2, Tools.tr("La situacion deseada no puede ser vacia", idioma));
                    }
                }
            }
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


            //validaciones de este nivel
            validar(prop, g.idioma);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                if (g.hijos.Count == 0)
                    ret += "<div class='titulo1'>" + Tools.tr("No hay grupos hijos en este grupo", g.idioma) + "</div>";
                else
                {
                    //hijo
                    string listahijos = "|";
                    foreach (Hijo h in g.hijos)
                        listahijos += h.nombre + "|";

                    //tema
                    ret += "<div class='tema'>" + Tools.tr("alhijo.introduccion.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("alhijo.introduccion.tip", g.idioma)
                            + "</div>";

                    ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
                }
            }
            else if (nivel == 2)
            {
                if (g.hijos.Count != 0)
                {
                    //Mision
                    ret += "<div class='tema'>" + Tools.tr("alhijo.situacionactual.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>" + Tools.tr("alhijo.situacionactual.tip", g.idioma) + "</div>";

                    ret += HTMLArea("s.situacionactual", prop, width, 290, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
                }

            }
            else if (nivel == 3)
            {
                if (g.hijos.Count != 0)
                {
                    //Objetivo
                    ret += "<div class='tema'>" + Tools.tr("alhijo.propuesta.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>" + Tools.tr("alhijo.propuesta.tip", g.idioma) + "</div>";

                    ret += HTMLArea("s.propuesta", prop, width, 290, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
                }
            }
            else if (nivel == 4)
            {
                if (g.hijos.Count != 0)
                {
                    ret += "<div class='tema'>" + Tools.tr("alhijo.situaciondeseada.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>" + Tools.tr("alhijo.situaciondeseada.tip", g.idioma) + "</div>";

                    ret += HTMLArea("s.situaciondeseada", prop, width, 550, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
                }
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

        public override void ejecutarConsenso(Documento doc)
        {
            try
            {
                Grupo g = doc.grupo;

                string htmlpath = doc.path.Replace(".json", ".html");

                foreach (Hijo h in g.hijos)
                {
                    string ret = Tools.getFileHttp(h.URL + "/doAprendemos.aspx?actn=crearEvaluacionAlHijo"
                        + "&docnombre=" + doc.nombre
                        + "&doctitulo=" + doc.titulo
                        + "&docmodeloid=AlHijo"
                        + "&grupohijo=" + h.nombre
                        + "&grupopadre=" + doc.grupo.nombre,
                        htmlpath);

                    if (ret == "ok")
                        doc.addLog(Tools.tr("Documento publicado en Evaluamos en el grupo hijo [%1]", h.nombre, doc.grupo.idioma));
                    else
                        doc.addLog(Tools.tr("Respuesta del grupo hijo [%1]", h.nombre, doc.grupo.idioma) + ": " + ret);
                }

            }
            catch (Exception ex)
            {
                doc.addLog(Tools.tr("No se pudo publicar documento en el grupo hijo", doc.grupo.idioma) + ": " + ex.Message);
            }
        }


    }
}

