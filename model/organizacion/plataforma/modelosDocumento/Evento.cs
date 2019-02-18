﻿///////////////////////////////////////////////////////////////////////////
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

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width, Propuesta propFinal)
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

                //introduccion
                ret += HTMLSeccion("evento.introduccion.titulo", "evento.introduccion.tip", "s.introduccion", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += HTMLSeccion("evento.objetivo.titulo", "evento.objetivo.tip", "s.objetivo", editar, prop, tieneFlores, g, width);
   
                //Descripcion
                ret += HTMLSeccion("evento.descripcion.titulo", "evento.descripcion.tip", "s.descripcion", editar, prop, tieneFlores, g, width);

                //A quien va dirigido
                ret += HTMLSeccion("evento.aquien.titulo", "evento.aquien.tip", "s.aquien", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);

            }
            else if (nivel == 3)
            {
                //Lugar
                ret += HTMLSeccion("evento.lugar.titulo", "evento.lugar.tip", "s.lugar", editar, prop, tieneFlores, g, width);

                //materiales
                ret += HTMLSeccion("evento.materiales.titulo", "evento.materiales.tip", "s.materiales", editar, prop, tieneFlores, g, width);

                //Otros
                ret += HTMLSeccion("evento.transportes.titulo", "evento.transportes.tip", "s.transporte", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                //organizacion
                ret += HTMLSeccion("evento.organizacion.titulo", "evento.organizacion.tip", "s.organizacion", editar, prop, tieneFlores, g, width);

                //responsable lista de seleccion de usuarios
                ret += "<div class='tema'>" + Tools.tr("evento.responsable.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("evento.responsable.tip", g.idioma)
                        + "</div>";
                string lista = "|";
                foreach (Usuario u2 in g.usuarios)
                    lista += u2.email + "#" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLLista("s.responsable", lista, prop, 250, tieneFlores, g.idioma);
                ret += "<br>";

                //fecha
                ret += "<div class='tema'>" + Tools.tr("evento.organizacion.fecha", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("evento.fecha.tip", g.idioma)
                        + "</div>";
                ret += HTMLDate("d.fecha", prop, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                //valoracion
                ret += HTMLSeccion("evento.valoracion.titulo", "evento.valoracion.tip", "s.eficiencia", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
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
                ret += "<div class='error'>" + errores[nivel] + "</div>";
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

