using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class Manifiesto: Modelo
    {
        public Manifiesto()
        {
            icono = "res/documentos/manifiesto.png";
            niveles = 4;
            nombre = "Manifiesto";
            descripcion = "Manifiesto";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Manifiesto";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.titulo", 70, 1));
            variables.Add(new Variable("s.vision", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.mision", 3000, 2));

            //nivel 3
            variables.Add(new Variable("s.objetivo", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.servicios", 9000, 4));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, "El titulo del manifiesto no puede ser vacio");
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if (getText("s.vision", prop) == "")
                    {
                        addError(1, "La vision no puede ser vacia");
                        getVariable("s.vision").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("s.mision", prop) == "")
                    {
                        addError(2, "La mision no ser vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.objetivo", prop) == "")
                    {
                        addError(2, "El objetivo no puede ser vacio");
                    }
                }
            }
        }

        override protected string HTMLEncabezado(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuario(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            if (prop != null)
                prop.bag["s.etiqueta"] = "Manifiesto";

            titulo = getText("s.titulo", prop);
            etiqueta = "Manifiesto";

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";
            ret += "<div class='titulo2'><nobr>" + tr("T&iacute;tulo") + ":" + HTMLText("s.titulo", prop, 70 * 8, tieneFlores);
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + tr("(Aparece en el pi&eacute; del &aacute;rbol)");
            ret += "</nobr></div>";

            //etiqueta
            ret += "<div class='titulo2'><nobr>" + tr("Etiqueta") + ": Manifiesto";
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + tr("(Etiqueta en el &aacute;rbol)") + "</span>";
            ret += "</nobr></div>";
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
            validar(prop);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //enseño manifiesto anterior
                LogDocumento anterior = null;
                foreach (LogDocumento ld in g.arbol.logDocumentos)
                {
                    if (ld.modeloNombre == this.nombre && ((prop!=null && ld.fecha < prop.born) || prop == null))
                        anterior = ld; //me quedo con el ultimo
                }
                if (anterior != null)
                {
                    ret += "<table class='smalltip' style='margin: 0 auto;background:wheat;'><tr>";
                    ret += "<td colspan=2 style='text-align:center;'><b>" + tr("Este manifiesto reemplaza al anterior") + "</b></td>";
                    ret += "<tr><td>";
                    ret += "<img src='" + anterior.icono + "' style='width:32px;height:40px'></td>";
                    ret += "<td style='text-align:left;'>";
                    ret += anterior.fname + "<br>";
                    ret += anterior.fecha.ToShortDateString() + "<br>";
                    ret += "<a href='" + anterior.URL + "' target='_blank'>" + anterior.titulo + "</a></td>";
                    ret += "</tr></table>";
                }

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + tr("Fecha") + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + tr("Visi&oacute;n") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("La visi&oacute;n tiene un car&aacute;cter inspirador y motivador para el grupo. ¿Que visi&oacute;n tenemos del mundo?, ¿Como deber&iacute;an ser las cosas?") + "</div>";

                ret += HTMLArea("s.vision", prop, width, 220, tieneFlores);
            }
            else if (nivel == 2)
            {
                //Mision
                ret += "<div class='tema'>" + tr("Mision") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("La misi&oacute;n define cual es nuestra labor,  ¿qué hacemos?, ¿a qué nos dedicamos?, ¿en que sector social sucede?, ¿quiénes son nuestro p&uacute;blico objetivo?, ¿cuál es nuestro ámbito geogr&aacute;fico de acci&oacute;n?") + "</div>";

                ret += HTMLArea("s.mision", prop, width, 220, tieneFlores);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Objetivo
                ret += "<div class='tema'>" + tr("Valores") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Los valores son principios éticos del grupo, son la base de nuestras pautas de comportamiento. Definen la personalidad del grupo. ¿cómo somos?, ¿en qué creemos?") + "</div>";

                ret += HTMLArea("s.objetivo", prop, width, 220, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Servicios") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("¿Que servicios prestamos para alcanzar nuestra misi&oacute;n?. Estos servicios son el punto partida para la creaci&oacute;n de procesos operativos y grupos de trabajo. Lista los servicios con nombre y una breve descripci&oacute;n") + "</div>";

                ret += HTMLArea("s.servicios", prop, width, 550, tieneFlores);

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
            doc.propuestas.Sort();
            doc.grupo.objetivo = getText("s.titulo", doc.propuestas[0]);
            doc.grupo.URLEstatuto = doc.URLPath;
            doc.addLog(tr("Manifiesto actualizado en el grupo"));

            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            plataforma.URLEstatuto = doc.URLPath;
            plataforma.objetivo = doc.grupo.objetivo;
            doc.addLog(tr("Estructura organizativa actualizada"));
        }


    }
}

