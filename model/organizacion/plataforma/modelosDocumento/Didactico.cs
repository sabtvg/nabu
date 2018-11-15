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
    public class Didactico : Modelo
    {
        private string donde = ""; //para que existe en otros nivels mientras dibujo
        private string que = ""; //para que existe en otros nivels mientras dibujo
        private string quemas = ""; //para que existe en otros nivels mientras dibujo
        private string costo = ""; //para que existe en otros nivels mientras dibujo

        public Didactico()
        {
            icono = "res/documentos/estrategia.png";
            niveles = 5;
            nombre = "Didactico";
            descripcion = "Modelo didactico";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Didactico";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            Variable v = new Variable("s.titulo", 30, 1);
            v.className = "textoBig";
            v.editClassName = "editarBig";
            variables.Add(v);

            //nivel 2
            variables.Add(new Variable("r.donde", 60, 2));

            //nivel 3
            variables.Add(new Variable("r.que", 60, 3));
			
            //nivel 4
            variables.Add(new Variable("r.quemas", 60, 4));
			
            //nivel 5
            variables.Add(new Variable("f.costo", 60, 5));
            variables.Add(new Variable("s.gastos", 3000, 5));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.etiqueta", prop) == "")
                    {
                        addError(2, "La etiqueta determina el nombre con que aparece en el arbol, no puede ser vacia");
                        getVariable("s.etiqueta").className = "errorfino";
                    }
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(2, "El titulo de la escapada no puede ser vacio");
                        getVariable("s.titulo").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("r.donde", prop) == "")
                    {
                        addError(2, "Completar todos los niveles");
                        getVariable("r.donde").className = "errorfino";
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("r.que", prop) == "")
                    {
                        addError(3, "Completar todos los niveles");
                        getVariable("r.que").className = "errorfino";
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("r.quemas", prop) == "")
                    {
                        addError(4, "Completar todos los niveles");
                        getVariable("r.quemas").className = "errorfino";
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getFloat("f.costo", prop) == 0)
                    {
                        addError(5, "Completar todos los niveles");
                        getVariable("f.costo").className = "errorfino";
                    }
                    if (getText("s.gastos", prop) == "")
                    {
                        addError(5, "Completar todos los niveles");
                        getVariable("s.gastos").className = "errorfino";
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

            titulo = getText("s.titulo", prop);
            etiqueta = getText("s.etiqueta", prop);

            //valor default para tipo
            organizaciones.Plataforma plata = (organizaciones.Plataforma)grupo.organizacion;

            //modelo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";        

            return ret;
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
            if (modo == eModo.prevista) validar(prop);

            //donde, que, quemas, costo
            if (prop != null)
            {
                if (getText("r.donde", prop) != "")
                    donde = getText("r.donde", prop);
                else
                    prop.bag["r.donde"] = donde;

                if (getText("r.que", prop) != "")
                    que = getText("r.que", prop);
                else
                    prop.bag["r.que"] = que;

                if (getText("r.quemas", prop) != "")
                    quemas = getText("r.quemas", prop);
                else
                    prop.bag["r.quemas"] = quemas;
            }

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //titulo
                ret += "<div class='titulo2'><nobr>" + Tools.tr("Titulo", g.idioma) + ":" + HTMLText("s.titulo", prop, 60 * 8, tieneFlores, g.idioma) + "</nobr></div>";

                //etiqueta
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ":" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma);
                if (prop == null)
                    ret += "<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
                ret += "</nobr></div><br><br>";
            }
            else if (nivel == 2)
            {
                //donde
                ret += "<div class='tema'><nobr>" + Tools.tr("Donde vamos?", g.idioma) + "</nobr></div>";
                ret += "<div>" + HTMLRadio("r.donde", 1, prop, tieneFlores, "Casa", g.idioma) + " " + Tools.tr("Nos quedamos en casa", g.idioma) + "</div>";
                ret += "<div>" + HTMLRadio("r.donde", 2, prop, tieneFlores, "Nacional", g.idioma) + " " + Tools.tr("Viaje nacional", g.idioma) + "</div>";
                ret += "<div>" + HTMLRadio("r.donde", 3, prop, tieneFlores, "Internacional", g.idioma) + " " + Tools.tr("Viaje internacional", g.idioma) + "</div>";
                ret += "<br>";

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 3)
            {
                ret += "<div class='tema'><nobr>" + Tools.tr("Que hacemos?", g.idioma) + "</nobr></div>";

                if (donde == "Casa")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.que", 1, prop, tieneFlores, "Pasear", g.idioma) + " " + Tools.tr("Pasear y descansar", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 2, prop, tieneFlores, "Tele", g.idioma) + " " + Tools.tr("Ver tele en el sofa", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 3, prop, tieneFlores, "Arreglar", g.idioma) + " " + Tools.tr("Arreglar cosas de la casa", g.idioma) + "</div>";
                }
                else if (donde == "Nacional")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.que", 1, prop, tieneFlores, "Galicia", g.idioma) + " " + Tools.tr("Galicia", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 2, prop, tieneFlores, "Madrid", g.idioma) + " " + Tools.tr("Madrid", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 3, prop, tieneFlores, "Cadiz", g.idioma) + " " + Tools.tr("Cadiz", g.idioma) + "</div>";
                }
                else if (donde == "Internacional")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.que", 1, prop, tieneFlores, "NY", g.idioma) + " " + Tools.tr("Nueva york", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 2, prop, tieneFlores, "Thailandia", g.idioma) + " " + Tools.tr("Thailandia", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.que", 3, prop, tieneFlores, "Japon", g.idioma) + " " + Tools.tr("Japon", g.idioma) + "</div>";
                }
                ret += "<br>";

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'><nobr>" + Tools.tr("Que hacemos? (mas concreto)", g.idioma) + "</nobr></div>";

                if (que == "Pasear")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Teatro", g.idioma) + " " + Tools.tr("Teatro y/o cine", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Gratuitos", g.idioma) + " " + Tools.tr("Espectaculos gratuitos", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Parques", g.idioma) + " " + Tools.tr("Parques y sitios naturales", g.idioma) + "</div>";
                }
                else if (que == "Tele")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Ficcion", g.idioma) + " " + Tools.tr("Ciancia ficcion", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Documentales", g.idioma) + " " + Tools.tr("Documentales", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Policiales", g.idioma) + " " + Tools.tr("Policiales", g.idioma) + "</div>";
                }
                else if (que == "Arreglar")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Limpiar", g.idioma) + " " + Tools.tr("Limpiar la casa", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Cocinar", g.idioma) + " " + Tools.tr("Cocinar para la semana", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Atrasadas", g.idioma) + " " + Tools.tr("Arreglar cosas atrasadas", g.idioma) + "</div>";
                }
                else if (que == "Galicia")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Tapas", g.idioma) + " " + Tools.tr("Tapas y sidra", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Playas", g.idioma) + " " + Tools.tr("Playas", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Montaña", g.idioma) + " " + Tools.tr("Montaña", g.idioma) + "</div>";
                }
                else if (que == "Madrid")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Tapas", g.idioma) + " " + Tools.tr("Tapas y noche", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Museos", g.idioma) + " " + Tools.tr("Museos y moumentos", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Manifestacion", g.idioma) + " " + Tools.tr("Manifestacion", g.idioma) + "</div>";
                }
                else if (que == "Cadiz")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Tapas", g.idioma) + " " + Tools.tr("Tapas y jerez", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Navegar", g.idioma) + " " + Tools.tr("Navegar", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Dormir", g.idioma) + " " + Tools.tr("Dormir la siesta bajo un arbol", g.idioma) + "</div>";
                }
                else if (que == "NY")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Recorrer", g.idioma) + " " + Tools.tr("Recorrer la ciudad", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Musicales", g.idioma) + " " + Tools.tr("Musicales", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Negocios", g.idioma) + " " + Tools.tr("Negocios", g.idioma) + "</div>";
                }
                else if (que == "Thailandia")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Mochila", g.idioma) + " " + Tools.tr("Mochila y autostop", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Playas", g.idioma) + " " + Tools.tr("Playas turisticas", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Bangkok", g.idioma) + " " + Tools.tr("Bangkok", g.idioma) + "</div>";
                }
                else if (que == "Japon")
                {
                    //que
                    ret += "<div>" + HTMLRadio("r.quemas", 1, prop, tieneFlores, "Conocer", g.idioma) + " " + Tools.tr("Conocer su cultura", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 2, prop, tieneFlores, "Comprar", g.idioma) + " " + Tools.tr("Comprar tecnologia", g.idioma) + "</div>";
                    ret += "<div>" + HTMLRadio("r.quemas", 3, prop, tieneFlores, "Sexual", g.idioma) + " " + Tools.tr("Turismo sexual", g.idioma) + "</div>";
                }
                ret += "<br>";

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'><nobr>" + Tools.tr("Costo asumido por cabeza", g.idioma) + "</nobr></div>";
                ret += HTMLFloat("f.costo", prop, tieneFlores, g.idioma, "0.00");
                ret += "<br>";
                ret += "<br>";

                //fases
                ret += HTMLSeccion("En que gastaremos el dinero", "Excursiones, fiesta nocturna, comidas en familia", "s.gastos", editar, prop, tieneFlores, g, width);

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
    }
}

