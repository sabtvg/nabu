using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.modelos
{
    public class Accion: Modelo
    {
        public Accion()
        {
            icono = "res/documentos/accion.png";
            niveles = 5;
            nombre = "Accion";

            crearVariables();
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
            variables.Add(new Variable("s.materiales", 3000, 3));
            variables.Add(new Variable("s.software", 3000, 3));
            variables.Add(new Variable("s.rrhh", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.fases", 3000, 4));

            //nivel 5
            variables.Add(new Variable("s.presupuesto", 3000, 5));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if ((string)getValue("s.etiqueta", prop) == "")
                    {
                        addError(1, "La etiqueta determina el nombre con que aparece en el arbol, no puede ser vacia");
                        getVariable("s.etiqueta").className = "errorfino";
                    }
                    if ((string)getValue("s.titulo", prop) == "")
                    {
                        addError(1, "El titulo del documento no puede ser vacio");
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if ((string)getValue("s.introduccion", prop) == "")
                    {
                        addError(1, "La introduccion no puede ser vacia");
                        getVariable("s.introduccion").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if ((string)getValue("s.objetivo", prop) == "" 
                        && (string)getValue("s.descripcion", prop) == ""
                        && (string)getValue("s.aquien", prop) == "")
                    {
                        addError(2, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if ((string)getValue("s.materiales", prop) == ""
                        && (string)getValue("s.software", prop) == ""
                        && (string)getValue("s.rrhh", prop) == "")
                    {
                        addError(2, "La propuesta no puede estar completamente vacia");
                    }
                }
            }
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
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores;


            //validaciones de este nivel
            validar(prop);

            if (nivel == 1)
            {
                titulo = (string)getValue("s.titulo", prop);
                etiqueta = (string)getValue("s.etiqueta", prop);

                //titulo
                ret += "<div class='titulo1'><nobr>" + nombre + ":" + txt("s.titulo", prop, width - 250, tieneFlores) + "</nobr></div>";

                //etiqueta
                ret += "<div class='titulo2'><nobr>" + tr("Etiqueta") + ":" + txt("s.etiqueta", prop, 20 * 5, tieneFlores);
                if (prop == null)
                    ret += "<span style='color:gray;font-size:12px;'>" + tr("(Etiqueta en el arbol)") + "</span>";
                ret += "</nobr></div>";

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + tr("Fecha") + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + tr("Resumen y motivacion") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("El consenso es un proceso cooperativo. Somos constructivos con nuestras propuestas y consideramos el bien comun") + "</div>";

                ret += area("s.introduccion", prop, width, 120, tieneFlores);
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += "<div class='tema'>" + tr("Objetivo a lograr") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe que pretendes que logremos") + "</div>";

                ret += area("s.objetivo", prop, width, 120, tieneFlores);

                //Descripcion
                ret += "<div class='tema'>" + tr("Descripcion") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe con mayor detalle como sera") + "</div>";

                ret += area("s.descripcion", prop, width, 120, tieneFlores);

                //A quien va dirigido
                ret += "<div class='tema'>" + tr("A quien va dirigido") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Quienes se beneficiaran") + "</div>";

                ret += area("s.aquien", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Materiales
                ret += "<div class='tema'>" + tr("Materiales") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") + "</div>";

                ret += area("s.materiales", prop, width, 120, tieneFlores);

                //Software
                ret += "<div class='tema'>" + tr("Software") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") + "</div>";

                ret += area("s.software", prop, width, 120, tieneFlores);

                //RRHH
                ret += "<div class='tema'>" + tr("RRHH") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") + "</div>";

                ret += area("s.rrhh", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Fases") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Describe las fases que se deben alcanzar para lograr el objetivo") + "</div>";

                ret += area("s.fases", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + tr("Presupuesto y plazo de entrega") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Estimaci&oacute;n") + "</div>";

                ret += area("s.presupuesto", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else
            {
                throw new Exception("Nivel [" + nivel + "] no existe en este modelo");
            }
            
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
            //nada que hacer
        }


    }
}

