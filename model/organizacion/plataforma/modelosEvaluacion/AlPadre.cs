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
    public class AlPadre: ModeloEvaluacion
    {
        public AlPadre()
        {
            modeloDocumento = "AlPadre";
            nombre = "AlPadre";
            titulo = "Evalucion de documento generado por grupo hijo";
            preguntas = 5;
            activo = false;
            icono = "res/documentos/alpadre.png";


            crearVariables();
        }

        public override List<Pregunta> getPreguntas(Propuesta prop)
        {
            List<Pregunta> ret = new List<Pregunta>();
            for (int i = 1; i <= preguntas; i++)
            {
                Pregunta p = new Pregunta();
                p.pregunta = Tools.tr("alpadre.evaluacion.p" + i, grupo.idioma);
                p.respuesta = Convert.ToInt32((float)getValue("f.p" + i, prop));
                p.texto = (string)getValue("s.t" + i, prop);
                p.minText = "No";
                p.maxText = "Si";
                ret.Add(p);
            }
            return ret;
        }

        public override string carpeta()
        {
            return "AlPadre";
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

            variables.Add(new Variable("s.t1", 2000));
            variables.Add(new Variable("s.t2", 2000));
            variables.Add(new Variable("s.t3", 2000));
            variables.Add(new Variable("s.t4", 2000));
            variables.Add(new Variable("s.t5", 2000));
        }

        public override string getEvaluadoID()  //identificador del documento evaluado
        {
            return evaluadoID;
        }

        private void validar(Propuesta prop)
        {
            //if (prop != null)
            //{
            //    if (getText("f.evaluadoID", prop) == "" || getText("f.evaluadoID", prop) == "0")
            //    {
            //        addError(1, "No hay documento de resultado seleccionado");
            //        getVariable("f.evaluadoID").className = "errorfino";
            //    }

            //    for (int i = 1; i <= preguntas; i++)
            //        if ((float)getValue("f.p" + i, prop) == 0)
            //        {
            //            addError(1, "Se deben responder todas las preguntas para completar la evalauci&oacute;n");
            //            break;
            //        }
            //}
        }

        public override Propuesta createProp(Tema t)
        {
            Propuesta ret = new Propuesta();
            ret.bag["f.evaluadoID"] = t.evaluadoID;
            ret.bag["s.basadoEnTemaExistente"] = "si";
            return ret;
        }

        override protected string toHTMLContenido(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);

            //validaciones de este nivel
            validar(prop);

            //titulo
            ret += "<div class='titulo1'>" + Tools.tr("Evaluaci&oacute;n de comunicado intergrupal", g.idioma) + "</div><br>";

            //fecha
            if (modo == eModo.finalizado)
                ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //documento de Accon a evaluar
            string valores = getListaResultados();
            ret += "<div class='titulo3'>" + Tools.tr("Documento de comunicado a evaluar", g.idioma) + "</div>";
            if (getText("s.basadoEnTemaExistente", prop) == "")
                ret += HTMLLista("f.evaluadoID", valores, prop, 450, g.idioma, true);
            ret += "<br>";

            //defino valores internos
            evaluadoID = getValue("f.evaluadoID", prop).ToString();

            //enalce a documento evaluado
            if (evaluadoID != "" && getLD(Convert.ToInt32(evaluadoID)) != null)
            {
                LogDocumento ld = getLD(Convert.ToInt32(evaluadoID));
                ret += "<table class='smalltip' style='margin: 0 auto;background:wheat;width:200px;'>";
                ret += "<tr><td>";
                ret += "<img src='" + ld.icono + "' style='width:32px;height:40px'></td>";
                ret += "<td style='text-align:left;'>";
                ret += ld.fname + "<br>";
                ret += ld.fecha.ToString("dd/MM/yy") + "<br>";
                ret += "<a href='" + ld.URL + "' target='_blank'>" + ld.titulo + "</a></td>";
                ret += "</tr></table>";
            }

            //preguntas
            for (var q = 1; q <= 5; q++)
            {
                ret += "<div class='tema' style='clear:left;float:left;vertical-align:top'><b>" + Tools.tr("alpadre.evaluacion.p" + q, g.idioma) + "</b></div>";
                ret += "<div style='float:right;vertical-align:middle;text-align:right;margin:0.1vw;margin-right:1vw;'>";
                ret += HTMLBarra("f.p" + q, prop, "No", "Si");
                ret += "</div>";

                if (modo != eModo.prevista && Tools.tr("alpadre.evaluacion.tip" + q, g.idioma) != "")
                    ret += "<div class='smalltip' style='float:left;width:-webkit-fill-available;margin: auto'>" + Tools.tr("alpadre.evaluacion.tip" + q, g.idioma) + "</div>";
                //texto
                ret += "<div style='float:left;width:-webkit-fill-available'>" + HTMLArea("s.t" + q, prop, 0, 120, g.idioma) + "</div>";
            }

            //mensajes de error
            if (errores.ContainsKey(1))
            {
                ret += "<div class='error'>" + errores[1] + "</div>";
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
            //else if (accion == "f.evaluadoID_changed")
            //{
            //    evaluadoID = prop.bag["f.evaluadoID"].ToString();
            //}
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
                if (ld.modeloID == "Accion")
                {
                    ret += ld.docID + "#" + ld.titulo + " (" + ld.fecha.ToString("dd/MM/yy") + ")|";
                }
            }
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }
    }
}