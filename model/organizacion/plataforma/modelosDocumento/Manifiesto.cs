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
            variables.Add(new Variable("s.vision", 4000, 1));

            //nivel 2
            variables.Add(new Variable("s.mision", 4000, 2));

            //nivel 3
            variables.Add(new Variable("s.objetivo", 4000, 3));

            //nivel 4
            variables.Add(new Variable("s.servicios", 4000, 4));
        }

        private void validar(Propuesta prop, string idioma)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.titulo", prop) == "")
                    {
                        addError(1, Tools.tr("El titulo del manifiesto no puede ser vacio", idioma));
                        getVariable("s.titulo").className = "errorfino";
                    }
                    if (getText("s.vision", prop) == "")
                    {
                        addError(1, Tools.tr("La vision no puede ser vacia", idioma));
                        getVariable("s.vision").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("s.mision", prop) == "")
                    {
                        addError(2, Tools.tr("La mision no ser vacia", idioma));
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("s.objetivo", prop) == "")
                    {
                        addError(2, Tools.tr("El objetivo no puede ser vacio", idioma));
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

            if (prop != null)
                prop.bag["s.etiqueta"] = "Manifiesto";

            titulo = getText("s.titulo", prop);
            etiqueta = Tools.tr("Manifiesto", g.idioma);

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div><br><br>";

            ret += "<div class='titulo3'><nobr>" + Tools.tr("Titulo", g.idioma) + ":" + HTMLText("s.titulo", prop, 70 * 8, tieneFlores, g.idioma);
            if (prop == null)
                ret += "<br><span style='color:gray;font-size:12px;'>" + Tools.tr("(Aparece en el pie del arbol)", g.idioma);
            ret += "</nobr></div>";

            //etiqueta
            ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ": " + Tools.tr("Manifiesto", g.idioma);
            etiqueta = Tools.tr("Manifiesto", g.idioma);
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
            ret += "</nobr></div><br>";
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
            validar(prop, g.idioma);

            if (nivel == 1)
            {
                ret += HTMLEncabezado(prop, g, email, width);

                //enseño manifiesto anterior
                LogDocumento anterior = null;
                foreach (LogDocumento ld in g.logDecisiones)
                {
                    if (ld.modeloNombre == this.nombre && ((prop!=null && ld.fecha < prop.born) || prop == null))
                        anterior = ld; //me quedo con el ultimo
                }
                if (anterior != null)
                {
                    ret += "<table class='smalltip' style='margin: 0 auto;background:wheat;width:250px'><tr>";
                    ret += "<td colspan=2 style='text-align:center;'><b>" + Tools.tr("Este manifiesto reemplaza al anterior", g.idioma) + "</b></td>";
                    ret += "<tr><td>";
                    ret += "<img src='" + anterior.icono + "' style='width:32px;height:40px'></td>";
                    ret += "<td style='text-align:left;'>";
                    ret += anterior.fname + "<br>";
                    ret += anterior.fecha.ToString("dd/MM/yy") + "<br>";
                    ret += "<a href='" + anterior.URL + "' target='_blank'>" + anterior.titulo + "</a></td>";
                    ret += "</tr></table>";
                }

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + Tools.tr("manifiesto.vision.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("manifiesto.vision.tip", g.idioma) + "</div>";

                ret += HTMLArea("s.vision", prop, width, 290, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g);
            }
            else if (nivel == 2)
            {
                //Mision
                ret += "<div class='tema'>" + Tools.tr("manifiesto.mision.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("manifiesto.mision.tip", g.idioma) + "</div>";

                ret += HTMLArea("s.mision", prop, width, 290, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g);

            }
            else if (nivel == 3)
            {
                //Objetivo
                ret += "<div class='tema'>" + Tools.tr("manifiesto.objetivos.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("manifiesto.objetivos.tip", g.idioma) + "</div>";

                ret += HTMLArea("s.objetivo", prop, width, 290, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g);
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + Tools.tr("manifiesto.servicios.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>" + Tools.tr("manifiesto.servicios.tip", g.idioma) + "</div>";

                ret += HTMLArea("s.servicios", prop, width, 550, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g);
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
            doc.propuestas.Sort();
            doc.grupo.objetivo = getText("s.titulo", doc.propuestas[0]);
            doc.grupo.URLEstatuto = doc.URLPath;
            doc.addLog(Tools.tr("Manifiesto actualizado en el grupo", doc.grupo.idioma));

            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            plataforma.URLEstatuto = doc.URLPath;
            plataforma.objetivo = doc.grupo.objetivo;
            doc.addLog(Tools.tr("Estructura organizativa actualizada", doc.grupo.idioma));
        }


    }
}

