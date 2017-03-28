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
            variables.Add(new Variable("s.materiales", 3000, 3));
            variables.Add(new Variable("s.rrhh", 3000, 3));
            variables.Add(new Variable("s.otros", 3000, 3));

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
                    if (getText("s.materiales", prop) == ""
                        && getText("s.software", prop) == ""
                        && getText("s.rrhh", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.fases", prop) == "")
                    {
                        addError(4, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.presupuesto", prop) == "")
                    {
                        addError(5, "La propuesta no puede estar completamente vacia");
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
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado;


            //validaciones de este nivel
            validar(prop);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + tr("Fecha") + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + tr("Introducci&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("¿Cual es la situación que requiere de esta propuesta de acci&oacute;n? ¿porqu&aacute; se debe actuar? ¿que mejorar&iacute;a? Ten en cuenta que la situati&oacute;n que describes represente al grupo") 
                        + "</div>";

                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores);
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += "<div class='tema'>" + tr("Objetivo a lograr") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe que pretendes que logremos como resultado de realizar la acci&oacute;n propuesta. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?") 
                        + "</div>";

                ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores);

                //Descripcion
                ret += "<div class='tema'>" + tr("Descripcion") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe brevemente como sera la acci&oacute;n a realizar. Esfuerzos, plazos, costos, desplazamientos, implicaciones, fases.") 
                        + "</div>";

                ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores);

                //A quien va dirigido
                ret += "<div class='tema'>" + tr("A quien va dirigido") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Quienes se beneficiaran con los resultados") 
                        + "</div>";

                ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Materiales
                ret += "<div class='tema'>" + tr("Materiales") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") 
                        + "</div>";

                ret += HTMLArea("s.materiales", prop, width, 120, tieneFlores);

                //RRHH
                ret += "<div class='tema'>" + tr("RRHH") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") 
                        + "</div>";

                ret += HTMLArea("s.rrhh", prop, width, 120, tieneFlores);

                //Otros
                ret += "<div class='tema'>" + tr("Software") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.") 
                        + "</div>";

                ret += HTMLArea("s.otros", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Fases") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe las fases que se deben alcanzar para lograr el objetivo") 
                        + "</div>";

                ret += HTMLArea("s.fases", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + tr("Presupuesto y plazo de entrega") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Estimaci&oacute;n de costos y plazos, el grupo debe conocer y aprobar estos valores") 
                        + "</div>";

                ret += HTMLArea("s.presupuesto", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
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
            //nada que hacer
        }


    }
}

