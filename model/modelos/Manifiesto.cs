using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.modelos
{
    public class Manifiesto: Modelo
    {
        public Manifiesto()
        {
            icono = "res/documentos/manifiesto.png";
            niveles = 4;
            nombre = "Manifiesto";

            crearVariables();
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.titulo", 150, 1));
            variables.Add(new Variable("s.vision", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.mision", 3000, 2));

            //nivel 3
            variables.Add(new Variable("s.objetivo", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.servicios", 3000, 4));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if ((string)getValue("s.titulo", prop) == "")
                    {
                        addError(1, "El titulo del manifiesto no puede ser vacio");
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if ((string)getValue("s.vision", prop) == "")
                    {
                        addError(1, "La vision no puede ser vacia");
                        getVariable("s.vision").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if ((string)getValue("s.mision", prop) == "")
                    {
                        addError(2, "La mision no ser vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if ((string)getValue("s.objetivo", prop) == "")
                    {
                        addError(2, "El objetivo no puede ser vacio");
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
                etiqueta = "Manifiesto";

                //titulo
                ret += "<div class='titulo1'><nobr>" + nombre + ":" + txt("s.titulo", prop, width - 250, tieneFlores) + "</nobr></div>";

                //etiqueta
                ret += "<div class='titulo2'><nobr>" + tr("Etiqueta") + ": Manifiesto ";
                if (prop == null)
                    ret += "<span style='color:gray;font-size:12px;'>" + tr("(Etiqueta en el arbol)") + "</span>";
                ret += "</nobr></div>";

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + tr("Fecha") + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + tr("Visi&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("La vision") + "</div>";

                ret += area("s.vision", prop, width, 120, tieneFlores);
            }
            else if (nivel == 2)
            {
                //Mision
                ret += "<div class='tema'>" + tr("Mision") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Mision") + "</div>";

                ret += area("s.mision", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

            }
            else if (nivel == 3)
            {
                //Objetivo
                ret += "<div class='tema'>" + tr("Objetivo") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Objetivo") + "</div>";

                ret += area("s.objetivo", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Servicios") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" + tr("Servicios") + "</div>";

                ret += area("s.servicios", prop, width, 120, tieneFlores);

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
            doc.propuestas.Sort();
            doc.grupo.objetivo = (string)getValue("s.titulo", doc.propuestas[0]);
            doc.grupo.URLEstatuto = doc.URLPath;
            doc.addLog(tr("Manifiesto actualizado en el grupo"));
        }


    }
}

