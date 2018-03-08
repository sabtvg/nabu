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
    public class Evento: ModeloEvaluacion
    {
        public Evento()
        {
            modeloDocumento = "Evento";
            nombre = "Evento";
            titulo = "Evalucion de Evento";
            preguntas = 6;
            icono = "res/documentos/evento.png";


            crearVariables();
        }

        public override List<Pregunta> getPreguntas(Propuesta prop)
        {
            List<Pregunta> ret = new List<Pregunta>();
            for (int i = 1; i <= preguntas; i++)
            {
                Pregunta p = new Pregunta();
                p.pregunta = Tools.tr("evento.evaluacion.p" + i, grupo.idioma);
                p.respuesta = Convert.ToInt32((float)getValue("f.p" + i, prop));
                p.texto = (string)getValue("s.t" + i, prop);
                if (i == 6)
                {
                    p.minText = "Malo";
                    p.maxText = "Bueno";
                }
                else
                {
                    p.minText = "No";
                    p.maxText = "Si";
                } 
                ret.Add(p);
            }
            return ret;
        }

        public override string carpeta()
        {
            return "Evento";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("f.evaluadoID", 0));
            variables.Add(new Variable("f.p1", 0));
            variables.Add(new Variable("f.p2", 0));
            variables.Add(new Variable("f.p3", 0));
            variables.Add(new Variable("f.p4", 0));
            variables.Add(new Variable("f.p5", 0));
            variables.Add(new Variable("f.p6", 0));

            variables.Add(new Variable("s.t1", 2000));
            variables.Add(new Variable("s.t2", 2000));
            variables.Add(new Variable("s.t3", 2000));
            variables.Add(new Variable("s.t4", 2000));
            variables.Add(new Variable("s.t5", 2000));
            variables.Add(new Variable("s.t6", 2000));
        }

        public override string getEvaluadoID()  //identificador del documento evaluado
        {
            return evaluadoID;
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (getText("f.evaluadoID", prop) == "" || getText("f.evaluadoID", prop) == "0")
                {
                    addError(1, "No hay documento de resultado seleccionado");
                    getVariable("f.evaluadoID").className = "errorfino";
                }

                for (int i = 1; i <= preguntas; i++)
                    if ((float)getValue("f.p" + i, prop) == 0)
                    {
                        addError(1, "Se deben responder todas las preguntas para completar la evaluaci&oacute;n");
                        break;
                    }
            }
        }

        public override Propuesta createProp(Tema t)
        {
            Propuesta ret = new Propuesta();
            ret.bag["f.evaluadoID"] = t.evaluadoID;
            ret.bag["s.basadoEnTemaExistente"] = "si";
            return ret;
        }

        public override string getTemaNombre()
        {
            //obtenog titulo de tema
            foreach (LogDocumento ld in grupo.logResultados)
                if (ld.modeloID == modeloDocumento && ld.docID.ToString() == getEvaluadoID())
                {
                    return ld.modeloNombre + ": " + ld.documentoNombre; //nombre del documento de consenso
                }
            return "";
        }

        public override string getTemaAutor()
        {
            //obtenog titulo de tema
            foreach (LogDocumento ld in grupo.logResultados)
                if (ld.modeloID == modeloDocumento && ld.docID.ToString() == getEvaluadoID())
                    return ld.autor;
            return "";
        }

        public override string getTemaIcono()
        {
            //obtenog titulo de tema
            foreach (LogDocumento ld in grupo.logResultados)
                if (ld.modeloID == modeloDocumento && ld.docID.ToString() == getEvaluadoID())
                    return ld.icono;
            return "";
        }

        public override string getTemaURL()
        {
            //obtenog titulo de tema
            foreach (LogDocumento ld in grupo.logResultados)
                if (ld.modeloID == modeloDocumento && ld.docID.ToString() == getEvaluadoID())
                    return ld.URL;
            return "";
        }

        override protected string toHTMLContenido(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);

            //validaciones de este nivel
            validar(prop);

            //titulo
            ret += "<div class='titulo1'>" + Tools.tr("Evaluacion de Evento", g.idioma) + "</div><br>";

            //fecha
            if (modo == eModo.finalizado)
                ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //documento de Accon a evaluar
            string valores = getListaResultados();
            ret += "<div>" + Tools.tr("Documento de resultado a evaluar", g.idioma) + "</div>";
            if (getText("s.basadoEnTemaExistente", prop) == "")
                ret += HTMLLista("f.evaluadoID", valores, prop, 450, g.idioma, true);
            ret += "<br>";

            //defino valores internos
            evaluadoID = getValue("f.evaluadoID", prop).ToString();

            //enalce a documento evaluado
            if (evaluadoID != "" && getLD(Convert.ToInt32(evaluadoID)) != null)
            {
                LogDocumento ld = getLD(Convert.ToInt32(evaluadoID));
                ret += "<table class='smalltip' style='margin: 0 auto;background:wheat;'>";
                ret += "<tr><td>";
                ret += "<img src='" + ld.icono + "' style='width:32px;height:40px'></td>";
                ret += "<td style='text-align:left;'>";
                ret += ld.fname + "<br>";
                ret += ld.fecha.ToString("dd/MM/yy") + "<br>";
                ret += "<a href='" + ld.URL + "' target='_blank'>" + ld.titulo + "</a></td>";
                ret += "</tr></table>";
            }


            //preguntas
            ret += "<table style='width:" + width + "px'>";
            //pregunta 1
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p1", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip1", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip1", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p1", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t1", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 2
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p2", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip2", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip2", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p2", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t2", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 3
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p3", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip3", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip3", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p3", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t3", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 4
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p4", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip4", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip4", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p4", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t4", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 5
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p5", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip5", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip5", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p5", prop, "No", "Si") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t5", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            //pregunta 6
            ret += "<tr>";
            ret += "<td class='tema' style='vertical-align:top'><b>" + Tools.tr("evento.evaluacion.p6", g.idioma) + "</b>";
            if (Tools.tr("evento.evaluacion.tip6", g.idioma) != "")
                ret += "<div class='smalltip' style='width:90%'>" + Tools.tr("evento.evaluacion.tip6", g.idioma) + "</div>";
            ret += "</td>";
            ret += "<td style='width:200px;vertical-align:middle;text-align:right'>" + HTMLBarra("f.p6", prop, "Malo", "Bueno") + "</td>";
            ret += "</tr>";
            //texto
            ret += "<tr><td colspan='2'>";
            ret += HTMLArea("s.t6", prop, width, 70, g.idioma);
            ret += "</td></tr>";

            ret += "</table>";


            //mensajes de error
            if (errores.ContainsKey(1))
            {
                ret += "<div class='error' style='width:" + (width - 4) + "px'>" + errores[1] + "</div>";
            }
            return ret;
        }

        public override void evaluacionSubmit(string evento, string parametro, Propuesta prop, Grupo g, string email)
        {
            if (evento.StartsWith("f.p") && evento.EndsWith("_set"))
            {
                string id = evento.Split('_')[0];
                prop.bag[id] = float.Parse(parametro);
            }
            else if (evento == "f.evaluadoID_changed")
            {
                evaluadoID = prop.bag["f.evaluadoID"].ToString();
            }
        }

        private bool exists(string id, Propuesta prop)
        {
            return prop != null && prop.bag.ContainsKey(id);
        }

        private LogDocumento getLD(int docID)
        {
            for (int i = grupo.logResultados.Count - 1; i >= 0; i--)
            {
                LogDocumento ld = grupo.logResultados[i];
                if (ld.docID == docID)
                {
                    return ld;
                }
            }
            return null;
        }

        private string getListaResultados()
        {
            string ret = "#|";
            for (int i = grupo.logResultados.Count - 1; i >= 0; i--)
            {
                LogDocumento ld = grupo.logResultados[i];
                if (ld.modeloID == "Evento")
                {
                    ret += ld.docID + "#" + ld.titulo + " (" + ld.fecha.ToString("dd/MM/yy") + ")|";
                }
            }
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }
    }
}