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
                    {
                        addError(4, "Describe como organizar el evento");
                    }
                    if (getDate("d.fecha", prop) == Tools.minValue)
                    {
                        addError(4, "Define una fecha para el evento");
                    }
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
                        + tr("¿Cual es la situación que requiere de esta propuesta de evento? ¿porqu&eacute; se debe hacer? ¿cual es la motivaci&oacute;n? Ten en cuenta que la situati&oacute;n que describes represente al grupo") 
                        + "</div>";

                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores);
            }
            else if (nivel == 2)
            {
                //Objetivo a lograr
                ret += "<div class='tema'>" + tr("Objetivo a lograr") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe que pretendes que logremos realizando este evento. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?") 
                        + "</div>";

                ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores);

                //Descripcion
                ret += "<div class='tema'>" + tr("Descripci&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe como ser&aacute; el evento. ¿show? ¿presentaciones? ¿ponentes? ¿musica? ¿actividades? ¿payasos? ¿catering?") 
                        + "</div>";

                ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores);

                //A quien va dirigido
                ret += "<div class='tema'>" + tr("A quien va dirigido") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("A quienes va dirigido el evento y quienes se beneficiaran con los resultados") 
                        + "</div>";

                ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Materiales
                ret += "<div class='tema'>" + tr("Lugar") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") 
                        + "</div>";

                ret += HTMLArea("s.lugar", prop, width, 120, tieneFlores);

                //RRHH
                ret += "<div class='tema'>" + tr("Materiales") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado") 
                        + "</div>";

                ret += HTMLArea("s.materiales", prop, width, 120, tieneFlores);

                //Otros
                ret += "<div class='tema'>" + tr("Transportes") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.") 
                        + "</div>";

                ret += HTMLArea("s.transporte", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Organizaci&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Describe como se debe organizar este evento, pasos a seguir, tiempos, desplazamientos, contrataciones, etc.") 
                        + "</div>";

                ret += HTMLArea("s.organizacion", prop, width, 120, tieneFlores);

                ret += "<div class='tema'>";
                ret += tr("Fecha");
                ret += HTMLDate("d.fecha", prop, tieneFlores);
                if (editar)
                    ret += "<span class='smalltip' style='margin:5px'>"
                        + tr("Haz una propuesta de fecha para el evento")
                        + "</span>";
                ret += "</div>";

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + tr("Valoraci&oacute;n del resultado") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("¿Como se medir&aacute; el resultado de este evento? ¿entradas vendidas? ¿cantidad de personas presentes? ¿reconocimiento alcanzado? ¿efectos sociales? ¿publicdad?. <br>La valoraci&oacute;n del evento debe ser cuantificable y facil de comprender. Debe quedar claro si ha ido bien o no.") 
                        + "</div>";

                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores);

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

