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
    public class Evento: Modelo
    {
        public Evento()
        {
            icono = "res/documentos/evento.png";
            niveles = 5;
            nombre = "Evento";
            descripcion = "Evento";
            tipo = "seguimiento";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Evento";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            variables.Add(new Variable("s.titulo", 150, 1));
            variables.Add(new Variable("s.introduccion", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.objetivo", 3000, 2));
            variables.Add(new Variable("s.descripcion", 3000, 2));
            variables.Add(new Variable("s.aquien", 3000, 2));

            //nivel 3
            variables.Add(new Variable("s.lugar", 3000, 3));
            variables.Add(new Variable("s.materiales", 3000, 3));
            variables.Add(new Variable("s.transporte", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.organizacion", 3000, 4));
            variables.Add(new Variable("s.responsable", 100, 4));
            variables.Add(new Variable("d.fecha", 0, 4));

            //nivel 5
            variables.Add(new Variable("s.eficiencia", 3000, 5));
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
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, "El titulo del documento no puede ser vacio");
                        getVariable("s.titulo").className = "errorfino";
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
                        && getText("s.aquien", prop) == "")
                    {
                        addError(2, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.lugar", prop) == ""
                        && getText("s.materiales", prop) == ""
                        && getText("s.transporte", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.organizacion", prop) == "")
                        addError(4, "Describe como organizar el evento");
                    if (getText("s.responsable", prop) == "")
                        addError(4, "Debe definr un responsable");
                    if (getDate("d.fecha", prop) == Tools.minValue)
                        addError(4, "Define una fecha para el evento");
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.eficiencia", prop) == "")
                    {
                        addError(5, "Define como se valorar&aacute el resultado del evento");
                    }
                }
            }
        }

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            bool editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
            editar = editar && !consensoAlcanzado;
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado;


            //validaciones de este nivel
            validar(prop);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //tema
                ret += "<div class='tema'>" + Tools.tr("Introduccion", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.introduccion", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += "<div class='tema'>" + Tools.tr("Objetivo a lograr", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.objetivo", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores, g.idioma);

                //Descripcion
                ret += "<div class='tema'>" + Tools.tr("Descripcion", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.descripcion", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores, g.idioma);

                //A quien va dirigido
                ret += "<div class='tema'>" + Tools.tr("A quien va dirigido", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("event.aquien", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Materiales
                ret += "<div class='tema'>" + Tools.tr("Lugar", g.idioma) + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.lugar", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.lugar", prop, width, 120, tieneFlores, g.idioma);

                //RRHH
                ret += "<div class='tema'>" + Tools.tr("Materiales", g.idioma) + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.materiales", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.materiales", prop, width, 120, tieneFlores, g.idioma);

                //Otros
                ret += "<div class='tema'>" + Tools.tr("Transportes", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.transportes", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.transporte", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + Tools.tr("Organizacion", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.organizacion", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.organizacion", prop, width, 120, tieneFlores, g.idioma);

                //responsable lista de seleccion de usuarios
                ret += "<div class='tema'>" + Tools.tr("Responsable", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("accion.responsable", g.idioma)
                        + "</div>";
                string lista = "|";
                foreach (Usuario u2 in g.usuarios)
                    lista += u2.email + "#" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLLista("s.responsable", lista, prop, 250, tieneFlores, g.idioma);
                ret += "<br>";
                ret += "<br>";

                //fecha
                ret += "<div class='tema'>";
                ret += Tools.tr("Fecha", g.idioma);
                ret += HTMLDate("d.fecha", prop, tieneFlores, g.idioma);
                if (editar)
                    ret += "<span class='smalltip' style='margin:5px'>"
                        + Tools.tr("evento.fecha", g.idioma)
                        + "</span>";
                ret += "</div>";

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + Tools.tr("Valoracion del resultado", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("evento.valoracion", g.idioma) 
                        + "</div>";

                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else
            {
                throw new Exception("Nivel [" + nivel + "] no existe en este modelo");
            }

            if (prop != null) prop.niveles = niveles; //esto es importante si cambian los niveles para que se traspase luego al nodo

            //fin nivel
            if (prop != null && prop.nodoID != 0 && modo != eModo.consenso)
                ret += HTMLFlores(g.arbol.getNodo(prop.nodoID), false, g.getUsuario(email));

            //mensajes de error
            if (errores.ContainsKey(nivel))
            {
                ret += "<div class='error' style='width:" + (width-4) + "px'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override void ejecutarConsenso(Documento doc)
        {
            try
            {
                nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
                nabu.plataforma.Evento evento = new nabu.plataforma.Evento();
                evento.EID = plataforma.getEID();
                evento.nombre = doc.titulo;
                evento.objetivo = doc.getText("s.objetivo");
                evento.docURL = doc.URLPath;
                evento.docTs = DateTime.Now;
                evento.responsable = doc.getText("s.responsable");
                evento.inicio = (DateTime)doc.getValor("d.fecha");

                plataforma.seguimientos.Add(evento);

                doc.addLog(Tools.tr("evento.creado", doc.grupo.idioma));
            }
            catch (Exception ex)
            {
                doc.addLog(Tools.tr("evento.error", doc.grupo.idioma) + ": " + ex.Message);
            }
        }


    }
}

