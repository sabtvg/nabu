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
    public class Accion: Modelo
    {
        public Accion()
        {
            icono = "res/documentos/accion.png";
            niveles = 5;
            nombre = "Accion";
            descripcion= "Accion";           
            tipo = "seguimiento";
            consensoMsg = "accion.consensoMsg";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Accion";
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
            variables.Add(new Variable("s.recursos", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.fases", 3000, 4));

            //nivel 5
            variables.Add(new Variable("s.presupuesto", 3000, 5));
            variables.Add(new Variable("s.responsable", 3000, 5));
            variables.Add(new Variable("s.revision", 3000, 5));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null && modo == eModo.prevista)
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
                        addError(2, "Se debe completar algún valor");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.recursos", prop) == "")
                    {
                        addError(3, "Se debe completar algún valor");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.fases", prop) == "")
                    {
                        addError(4, "Se debe completar algún valor");
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.presupuesto", prop) == "" && getText("s.responsable", prop) == "" && getText("s.revision", prop) == "")
                    {
                        addError(5, "Se debe completar algún valor");
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

                //tema
                ret += HTMLSeccion("accion.introduccion.titulo", "accion.introduccion.tip", "s.introduccion", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += HTMLSeccion("accion.objetivo.titulo", "accion.objetivo.tip", "s.objetivo", editar, prop, tieneFlores, g, width);

                //Descripcion
                ret += HTMLSeccion("accion.descripcion.titulo", "accion.descripcion.tip", "s.descripcion", editar, prop, tieneFlores, g, width);

                //A quien va dirigido
                ret += HTMLSeccion("accion.aquien.titulo", "accion.aquien.tip", "s.aquien", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);

            }
            else if (nivel == 3)
            {
                //Recursos
                ret += HTMLSeccion("accion.recursos.titulo", "accion.recursos.tip", "s.recursos", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                //fases
                ret += HTMLSeccion("accion.fases.titulo", "accion.fases.tip", "s.fases", editar, prop, tieneFlores, g, width);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                //presupuesto
                ret += HTMLSeccion("accion.presupuesto.titulo", "accion.presupuesto.tip", "s.presupuesto", editar, prop, tieneFlores, g, width);

                ret += "<div class='tema'>" + Tools.tr("accion.responsable.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("accion.responsable.tip", g.idioma)
                        + "</div>";

                //lista de seleccion de usuarios
                string lista = "|";
                foreach (Usuario u2 in g.usuarios)
                    lista += u2.email + "#" + u2.nombre + "|";
                lista = lista.Substring(0, lista.Length - 1);
                ret += HTMLLista("s.responsable", lista, prop, 250, tieneFlores, g.idioma);

                //revision
                ret += "<div class='tema'>" + Tools.tr("accion.revision.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("accion.revision.tip", g.idioma)
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
            if (prop != null && prop.nodoID != 0 && modo != eModo.consenso)
                ret += HTMLFlores(g.arbol.getNodo(prop.nodoID), false, g.getUsuario(email));

            //mensajes de error
            if (errores.ContainsKey(nivel) && modo == eModo.prevista)
            {
                ret += "<div class='error'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override int getRevisionDias(List<Propuesta> props)
        {
            int ret = 0;
            foreach(Propuesta p in props)
                foreach(string key in p.bag.Keys)
                    if (key == "s.revision")
                        switch((string)p.bag[key]){
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
            try
            {
                nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
                nabu.plataforma.Accion accion = new nabu.plataforma.Accion();
                accion.EID = plataforma.getEID();
                accion.nombre = doc.titulo;
                accion.objetivo = doc.getText("s.objetivo");
                accion.docURL = doc.URLPath;
                accion.docTs = DateTime.Now;
                accion.responsable = doc.getText("s.responsable");

                plataforma.seguimientos.Add(accion);

                doc.addLog(Tools.tr("accion.creada", doc.grupo.idioma));
            }
            catch (Exception ex)
            {
                doc.addLog(Tools.tr("accion.error", doc.grupo.idioma) + ": " + ex.Message);
            }
        }


    }
}

