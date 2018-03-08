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
using System.Linq;
using System.Web;

namespace nabu.plataforma.modelosEvaluacion
{
    public class Enlace: ModeloEvaluacion
    {
        private string temaNombre = "";
        private string temaURL = "";
        private string temaAutor = "";

        public Enlace()
        {
            modeloDocumento = "Enlace";
            nombre = "Enlace";
            titulo = "Evalucion de Enlace";
            preguntas = 5;
            icono = "res/enlace.png";


            crearVariables();
        }

        public override string getEvaluadoID()  //identificador del documento evaluado
        {
            if (evaluadoID == "")
            {   
                int ret = 0;
                foreach(char c in temaURL)
                    ret += c;
                evaluadoID = ret.ToString();
            }
            return evaluadoID;
        }

        public override List<Pregunta> getPreguntas(Propuesta prop)
        {
            List<Pregunta> ret = new List<Pregunta>();
            for (int i = 1; i <= preguntas; i++)
            {
                Pregunta p = new Pregunta();
                p.pregunta = Tools.tr("enlace.evaluacion.p" + i, grupo.idioma);
                p.respuesta = Convert.ToInt32((float)getValue("f.p" + i, prop));
                p.texto = (string)getValue("s.t" + i, prop);
                if (i == 3)
                {
                    p.minText = "No";
                    p.maxText = "Si";
                }
                else
                {
                    p.minText = "Si";
                    p.maxText = "No";
                } 
                ret.Add(p);
            }
            return ret;
        }

        public override string carpeta()
        {
            return "Enlace";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.nombre", 50));
            variables.Add(new Variable("s.URL", 150));
            variables.Add(new Variable("s.autor", 50));
            variables.Add(new Variable("f.p1", 0));
            variables.Add(new Variable("f.p2", 0));
            variables.Add(new Variable("f.p3", 0));
            variables.Add(new Variable("f.p4", 0));
            variables.Add(new Variable("f.p5", 0));

            variables.Add(new Variable("s.t1", 2000));
            variables.Add(new Variable("s.t2", 2000));
            variables.Add(new Variable("s.t3", 2000));
            variables.Add(new Variable("s.t4", 2000));
            variables.Add(new Variable("s.t5", 2000));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (getText("s.nombre", prop) == "")
                {
                    addError(1, "Se debe definir un nombre para la publicacion");
                    getVariable("s.nombre").className = "errorfino";
                }
                if (getText("s.URL", prop) == "")
                {
                    addError(1, "Se debe definir una URL para la publicacion");
                    getVariable("s.URL").className = "errorfino";
                }
                if (getText("s.autor", prop) == "")
                {
                    addError(1, "Se debe definir un autor para la publicacion");
                    getVariable("s.autor").className = "errorfino";
                } 
                
                for (int i = 1; i <= preguntas; i++)
                    if ((float)getValue("f.p" + i, prop) == 0)
                    {
                        addError(1, "Se deben responder todas las preguntas para completar la evalauci&oacute;n");
                        break;
                    }
            }
        }

        public override Propuesta createProp(Tema t)
        {
            Propuesta ret = new Propuesta();
            ret.bag["s.nombre"] = t.nombre;
            ret.bag["s.URL"] = t.URL;
            ret.bag["s.autor"] = t.autor;
            ret.bag["s.basadoEnTemaExistente"] = "si";
            return ret;
        }

        public override string getTemaAutor()
        {
            //obtenog titulo de tema
            return temaAutor;
        }

        public override string getTemaNombre()
        {
            //obtenog titulo de tema
            return temaNombre;
        }

        public override string getTemaIcono()
        {
            return "res/enlace.png";
        }

        public override string getTemaURL()
        {
            return temaURL;
        }

        override protected string toHTMLContenido(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);

            //validaciones de este nivel
            validar(prop);

            //titulo
            ret += "<br><div class='titulo1'>" + Tools.tr("Evaluacion de Enlace", g.idioma) + "</div><br>";

            //fecha
            if (modo == eModo.finalizado)
                ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //documento a evaluar
            if (getText("s.basadoEnTemaExistente", prop) == "")
            {
                ret += Tools.tr("Nombre de la publicacion", g.idioma) + ":" + HTMLText("s.nombre", prop, 150, g.idioma) + "<br>";
                ret += Tools.tr("URL de la publicacion", g.idioma) + ":" + HTMLText("s.URL", prop, 500, g.idioma) + "<br>";
                ret += Tools.tr("Autor de la publicacion", g.idioma) + ":" + HTMLText("s.autor", prop, 150, g.idioma) + "<br><br>";
            }
            else
            {
                ret += Tools.tr("Nombre de la publicacion", g.idioma) + ":" + prop.bag["s.nombre"] + "<br>";
                ret += Tools.tr("URL de la publicacion", g.idioma) + ":" + prop.bag["s.URL"] + "<br>";
                ret += Tools.tr("Autor de la publicacion", g.idioma) + ":" + prop.bag["s.autor"] + "<br><br>";
            }

            //defino valores internos
            temaNombre = (string)getValue("s.nombre", prop);
            temaURL = (string)getValue("s.URL", prop);
            temaAutor = (string)getValue("s.autor", prop);

            //preguntas
            ret += "<table style='width:" + width + "px'>";
            //pregunta 1
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'>" + Tools.tr("enlace.evaluacion.p1", g.idioma);
            if (Tools.tr("enlace.evaluacion.tip1", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("enlace.evaluacion.tip1", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p1", prop, "Si", "No") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t" + 1, prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 2
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'>" + Tools.tr("enlace.evaluacion.p2", g.idioma);
            if (Tools.tr("enlace.evaluacion.tip2", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("enlace.evaluacion.tip2", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p2", prop, "Si", "No") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t" + 2, prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 3
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'>" + Tools.tr("enlace.evaluacion.p3", g.idioma);
            if (Tools.tr("enlace.evaluacion.tip3", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("enlace.evaluacion.tip3", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p3", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t" + 3, prop, width, 70, g.idioma);
            ret += "</td></tr>";


            //pregunta 4
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'>" + Tools.tr("enlace.evaluacion.p4", g.idioma);
            if (Tools.tr("enlace.evaluacion.tip4", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("enlace.evaluacion.tip4", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p4", prop, "Si", "No") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t" + 4, prop, width, 70, g.idioma);
            ret += "</td></tr>";


            //pregunta 5
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'>" + Tools.tr("enlace.evaluacion.p5", g.idioma);
            if (Tools.tr("enlace.evaluacion.tip5", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("enlace.evaluacion.tip5", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p5", prop, "Si", "No") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t" + 5, prop, width, 70, g.idioma);
            ret += "</td></tr>";

            ret += "</table>";

            //mensajes de error
            if (errores.ContainsKey(1))
            {
                ret += "<div class='error' style='width:" + (width - 4) + "px'>" + errores[1] + "</div>";
            }
            return ret;
        }

        public override void evaluacionSubmit(string accion, string parametro, Propuesta prop, Grupo g, string email)
        {
            if (accion.StartsWith("f.p") && accion.EndsWith("_set"))
            {
                string id = accion.Split('_')[0];
                prop.bag[id] = float.Parse(parametro);
            }
        }

    }
}