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

//modelo de evaluacion

namespace nabu
{
    public abstract class ModeloEvaluacion
    {
        public enum eModo { ver, editar, prevista, revisar, finalizado };

        public string modeloDocumento = ""; //nombre del modelo de documento al que aplica
        public int preguntas = 6;
        public bool activo = true;
        public string tipo = "resultado";
        public string icono = "res/doc.png";
        public string nombre = "";
        public eModo modo = eModo.editar;
        public Grupo grupo;
        protected List<Variable> variables = new List<Variable>();
        public string titulo = ""; //esto es parte del documento
        public DateTime fecha = DateTime.Now;

        protected abstract void crearVariables();
        protected abstract string toHTMLContenido(Propuesta prop, Grupo g, string email, int width);
        public abstract string carpeta();

        public string temaNombre = "";  //funcionalidad general para AlPadre
        public string temaAutor = "";  //funcionalidad general para AlPadre
        public string temaIcono = "";  //funcionalidad general para AlPadre
        public string temaURL = "";  //funcionalidad general para AlPadre

        public abstract List<Pregunta> getPreguntas(Propuesta prop);
        public abstract Propuesta createProp(Tema t);
        public abstract string getEvaluadoID();  //identificador del documento evaluado

        public string evaluadoID = "";   //identificador del documento evaluado


        public Dictionary<int, object> errores = new Dictionary<int, object>();

        public string id
        {
            get
            {
                return GetType().FullName;
            }
            set { }
        }

        public virtual string getTemaNombre(){
            return temaNombre;
        }
        public virtual string getTemaAutor()
        {
            return temaAutor;
        }

        public virtual string getTemaIcono()
        {
            return temaIcono;
        }

        public virtual string getTemaURL()
        {
            return temaURL;
        }


        public virtual void evaluacionSubmit(string accion, string parametro, Propuesta prop, Grupo g, string email)
        {
            //submit(accion, parametro, props, g, email, width, modo);
        }

        public string toHTML(Propuesta prop, Grupo g, string email, int width, eModo modo)
        {
            string ret = "";
            this.modo = modo;
            this.grupo = g;

            //contenido 
            ret += toHTMLContenido(prop, g, email, width);

            //botones de poantalla o firma de consenso
            ret += "<div>";
            if (modo == eModo.editar)
            {
                //modo muestra
                ret += "<input type='button' style='float:left;' class='btnok' value='" + Tools.tr("Prevista de evaluacion", g.idioma) + "' title='" + Tools.tr("Enseña vista previa antes de proponer", g.idioma) + "' onclick='doPrevistaEvaluacion(\"" + id + "\");' />";
                ret += "<input type='button' style='float:right;' class='btnnok' value='" + Tools.tr("Cerrar", g.idioma) + "' onclick='doCerrarDocumento();' />";
            }
            else if (modo == eModo.prevista)
            {
                if (!hayError())
                    ret += "<input type='button' class='btnok' value='" + Tools.tr("Crear evaluacion", g.idioma) + "' title='" + Tools.tr("Crea la propuesta", g.idioma) + "' onclick='doCrearEvaluacion(\"" + id + "\");' />";
                ret += "<input type='button' style='float:left;' class='btn' value='" + Tools.tr("Revisar evaluacion", g.idioma) + "' title='" + Tools.tr("Permite corregir errores", g.idioma) + "' onclick='doRevisarEvaluacion(\"" + id + "\");' />";
                ret += "<input type='button' style='float:right;' class='btnnok' value='" + Tools.tr("Cancelar", g.idioma) + "' onclick='doCerrarDocumento();' />";
            }
            else if (modo == eModo.revisar)
            {
                //permito prevista
                ret += "<input type='button' style='float:left;' class='btnok' value='" + Tools.tr("Prevista de evaluacion", g.idioma) + "' title='" + Tools.tr("Enseña vista previa antes de proponer", g.idioma) + "' onclick='doPrevistaEvaluacion(\"" + id + "\");' />";
                ret += "<input type='button' style='float:right;' class='btnnok' value='" + Tools.tr("Cancelar", g.idioma) + "' onclick='doCerrarDocumento();' />";
            }
            ret += "</div>";

            //ret += "<a id='btnDownload' href='' target='_blank'><font size='1'>Descargar esta versi&oacute;n</font></a>";
            return ret;
        }

        public bool hayError()
        {
            return errores.Count > 0;
        }

        public string HTMLText(string id, Propuesta prop, int width, string idioma)
        {
            return input(id, prop, width, "text", getText(id, prop), idioma);
        }

        private string input(string id, Propuesta prop, int width, string tipo, string value, string idioma)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<input id='" + id + "' type='" + tipo + "' ";
                    ret += "class='" + v.editClassName + "' ";
                    ret += "maxlength='" + v.len + "' ";
                    ret += "style='width:" + width + "px;'>";
                }
            }
            else if (prop != null && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                ret += "<input id='" + id + "' type='" + tipo + "' ";
                ret += "class='" + v.editClassName + "' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;' ";
                ret += "value='" + value + "'>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<input type='text' readonly ";
                ret += "class='" + v.className + "' ";
                ret += "style='width:" + width + "px;' ";
                ret += "value='" + value + "'>";
            }


            return ret;
        }

        public List<int> evaluar(List<Evaluacion> evaluaciones)
        {
            //promedio respuestas
            List<int> ret = new List<int>();
            //sumo
            foreach (Evaluacion e in evaluaciones)
            {
                int index = 0;
                foreach (Pregunta p in e.preguntas)
                {
                    if (ret.Count < index + 1)
                        ret.Add(p.respuesta);
                    else
                        ret[index] += p.respuesta;

                    index += 1;
                }
            }
            //divido
            for (int i = 0; i < ret.Count; i++)
            {
                ret[i] = ret[i] / evaluaciones.Count;
            }
            return ret;
        }

        public string HTMLLista(string id, string valores, Propuesta prop, int width, string idioma, bool autopostback)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<select id='" + id + "'  ";
                    ret += "class='" + v.editClassName + "' ";
                    ret += "style='width:" + width + "px;'";
                    ret += autopostback ? " onchange=\"evaluacionSubmit('" + id + "_changed','','" + GetType().FullName + "')\" " : "";
                    ret += ">";
                    foreach (string l in valores.Split('|'))
                    {
                        string[] item = l.Split('#');
                        if (item.Length == 1)
                            ret += "<option id='" + item[0] + "'>" + item[0] + "</option>";
                        else
                            ret += "<option id='" + item[0] + "'>" + item[1] + "</option>";
                    }
                    ret += "</select>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                string value = getValue(id, prop).ToString();
                ret += "<select id='" + id + "' ";
                ret += "class='" + v.editClassName + "' ";
                ret += "style='width:" + width + "px;'";
                ret += autopostback ? " onchange=\"evaluacionSubmit('" + id + "_changed','','" + GetType().FullName + "')\" " : "";
                ret += ">";
                foreach (string l in valores.Split('|'))
                {
                    string[] item = l.Split('#');
                    if (item.Length == 1)
                        ret += "<option " + (value == item[0] ? "selected" : "") + " id='" + item[0] + "'>" + item[0] + "</option>";
                    else
                        ret += "<option " + (value == item[0] ? "selected" : "") + " id='" + item[0] + "'>" + item[1] + "</option>";
                }               
                ret += "</select>";
            }
            else if (prop != null)
            {
                //ver
                string value = getValue(id, prop).ToString();
                foreach (string l in valores.Split('|'))
                {
                    string[] item = l.Split('#');
                    if (value == item[0])
                    {
                        if (item.Length == 1)
                        {
                            ret += "<input type='text' readonly ";
                            ret += "class='" + v.className + "' ";
                            ret += "style='width:" + width + "px;' ";
                            ret += "value='" + item[0] + "'>";
                        }
                        else
                        {
                            ret += "<input type='text' readonly ";
                            ret += "class='" + v.className + "' ";
                            ret += "style='width:" + width + "px;' ";
                            ret += "value='" + item[1] + "'>";
                        }
                    }
                }
            }
            else
                //sin flores
                ret += "<div style='color:gray;font-size:12px;float:left;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";

            return ret;
        }

        public string getText(string id, Propuesta prop)
        {
            return getValue(id, prop).ToString();
        }

        public string addError(int nivel, string s)
        {
            if (errores.ContainsKey(nivel))
                errores[nivel] = errores[nivel] + "<br>" + s;
            else
                errores.Add(nivel, s);
            return s;
        }

        public string HTMLBarra(string id, Propuesta prop, string minText, string maxText)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null)
            {
                //editar
                if (modo != eModo.prevista)
                {
                    ret += "<table style='border-spacing: 0;'><tr>";
                    string color = "";
                    for (int i = 1; i <= 10; i++)
                    {
                        color = "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 0.4)";
                        ret += "<td style='cursor:pointer;border: 3px solid transparent;width:20px;background-color:" + color + "' ";
                        ret += "onclick=\"evaluacionSubmit('" + id + "_set','" + i + "','" + GetType().FullName + "')\" ";
                        ret += ">&nbsp;</td>";
                    }
                    ret += "</tr>";
                }
            }
            else if (prop != null && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revisar
                ret += "<table style='border-spacing: 0;'><tr>";
                string color = "";
                string border = "";
                for (int i = 1; i <= 10; i++)
                {
                    color = (float)getValue(id, prop) != i ? "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 0.4)" : "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 1)";
                    border = (float)getValue(id, prop) == i ? "border:3px solid blue;" : "border:3px solid transparent;";
                    ret += "<td style='cursor:pointer;" + border + ";width:20px;background-color:" + color + "' ";
                    ret += "onclick=\"evaluacionSubmit('" + id + "_set','" + i + "','" + GetType().FullName + "')\" ";
                    ret += ">&nbsp;</td>";
                }
                ret += "</tr>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<table style='border-spacing: 0;'><tr>";
                string color = "";
                string border = "";
                for (int i = 1; i <= 10; i++)
                {
                    color = "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 0.4)";
                    border = (float)getValue(id, prop) == i ? "border:3px solid blue;" : "border:3px solid transparent;";
                    ret += "<td style='" + border + ";width:20px;background-color:" + color + "' ";
                    ret += ">&nbsp;</td>";
                }
                ret += "</tr>";
            }
            ret += "<tr><td colspan=5 class='titulo3' style='text-align:left;'>" + minText + "</td><td colspan=5 class='titulo3' style='text-align:right;'>" + maxText + "</td></tr>";
            ret += "</table>";
            ret += "<input type='hidden' id='" + id + "' value='" + getValue(id, prop) + "'>";

            return ret;
        }

        public string HTMLArea(string id, Propuesta prop, int width, int height, string idioma)
        {
            //es reconocimiento de voz no funcion desde el servidor por motivos de seguridad. Solo funciona con HTTPS
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null)
            {
                if (modo != eModo.prevista)
                {
                    //ret += "<table style='width:" + width + "'>";
                    //ret += "<tr><td>";
                    ret += "<textarea id='" + id + "' ";
                    ret += "class='editarrtf' ";
                    ret += "maxlength='" + v.len + "' ";
                    ret += "style='width:" + width + "px;height:" + height + "px;'>";
                    ret += "</textarea>";
                    //ret += "</td><td style='width:25px;vertical-align:top;'>";
                    //ret += "<img src='res/mic.png' style='cursor:pointer;' onclick='startRecognition(\"" + id + "\");'>";
                    //ret += "</td></tr></table>";
                    ret += "<div style='text-align:right;width:100%;font-size:10px;'>(" + Tools.tr("max", idioma) + ": " + v.len + ")</div>";
                    ret += "<br>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revisar
                //ret += "<table style='width:" + width + "'>";
                //ret += "<tr><td>";
                ret += "<textarea id='" + id + "' ";
                ret += "class='editarrtf' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;height:" + height + "px;'>";
                ret += getValue(id, prop);
                ret += "</textarea>";
                //ret += "</td><td style='width:25px;vertical-align:top;'>";
                //ret += "<img src='res/mic.png' style='cursor:pointer;' onclick='startRecognition(\"" + id + "\");'>";
                //ret += "</td></tr></table>";
                ret += "<div style='text-align:right;width:100%;font-size:10px;'>(" + Tools.tr("max", idioma) + ": " + v.len + ")</div>";
                ret += "<br>";
            }
            else if (prop != null)
            {
                //ver
                string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(toHTMLText((string)getValue(id, prop))));
                ret += "<div ";
                ret += "class='" + v.className + "' ";
                ret += "style='width:100%;'>";
                ret += HTMLText + "</div>";
                ret += "<br>";
            }

            return ret;
        }

        public object parse(string id, string valor)
        {
            object ret;
            Variable v = getVariable(id);
            if (v.id.StartsWith("s."))
                ret = valor;
            else if (v.id.StartsWith("d."))
                try
                {
                    ret = DateTime.Parse(valor);
                }
                catch (Exception ex)
                {
                    ret = Tools.minValue;
                }
            else if (v.id.StartsWith("r."))
                ret = valor;
            else if (v.id.StartsWith("f."))
            {
                try
                {
                    ret = float.Parse(valor);
                }
                catch (Exception ex)
                {
                    ret = 0.0f;
                }
            }
            else
                throw new Exception("Modelo.parse: Tipo no implementado");
            return ret;
        }

        public bool isVariable(string id)
        {
            foreach (Variable v in variables)
                if (v.id == id)
                    return true;
            return false;
        }

        public object getValue(string id, Propuesta prop)
        {
            if (prop == null)
                return "";
            else
            {
                if (prop.bag.ContainsKey(id))
                {
                    //ejemplo "\"/Date(1445301615000-0700)/\"";
                    if (prop.bag[id].GetType().Name == "DateTime")
                        return prop.bag[id];
                    else
                    {
                        string value = prop.bag[id].ToString();
                        if (value.StartsWith("/Date("))
                            return Tools.getDateFromJSON(value);
                        else
                            return prop.bag[id];
                    }
                }
                else
                {
                    if (id.StartsWith("s."))
                        return "";
                    else if (id.StartsWith("f."))
                        return 0.0f;
                    else
                        return null;
                }
            }
        }

        public Variable getVariable(string id)
        {
            foreach (Variable v in variables)
                if (v.id == id)
                    return v;
            throw new Exception("Variable [" + id + "] no existe");
        }

        private string toHTMLText(string s)
        {
            string ret = s;

            ////reemplazo links
            //int ini = s.ToLower().IndexOf("http://");
            //int fin;
            //string link;
            //while (ini >= 0)
            //{
            //    s = s.Substring(ini);
            //    fin = getSeparadorIndex(s);
            //    link = s.Substring(0, fin);
            //    ret = ret.Replace(link, "<a href='" + link + "' target='_blank'>" + link + "</a>");

            //    s = s.Substring(fin);
            //    ini = s.ToLower().IndexOf("http://");
            //}

            //reemplazo \n
            ret = ret.Replace("\n", "<br>");

            return ret;
        }

        private int getSeparadorIndex(string s)
        {
            int ret = 0;
            while (ret < s.Length && s[ret] != '\n' && s[ret] != ' ' && s[ret] != ',')
                ret++;
            return ret;
        }
    }
}