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

namespace nabu
{
    public abstract class Modelo
    {
        public enum eModo { ver, editar, prevista, revisar, consenso };

        public string nombre = "";
        public string descripcion = "";
        public string tipo = "otro";
        public string icono = "res/doc.png";
        public bool enUso = false;
        public bool activo = true;
        public string titulo = ""; //esto es parte del documento
        public string etiqueta = ""; //esto es parte del documento
        public string firmaConsenso = ""; //solo se usa para generar el consenso
        public string permisos = "";
        public string versionar = ""; //modelo o titulo

        protected string accion = ""; //para que existe en otros nivels mientras dibujo. Solo se usa en modelos que tengan ABM
        protected int niveles = 0;
        protected List<Variable> variables = new List<Variable>();
        protected eModo modo = eModo.editar;
        protected bool consensoAlcanzado = false;
        protected Grupo grupo;

        protected abstract string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width, Propuesta propFinal);
        protected abstract void crearVariables();

        public abstract string carpeta();

        public virtual void ejecutarConsenso(Documento doc)
        {
        }

        public virtual int getRevisionDias(List<Propuesta> props)
        {
            return 0; //por si el documento tiene revision periodica
        }


        public Dictionary<int, object> errores = new Dictionary<int, object>();

        public List<Variable> getVariables()
        {
            return variables;
        }

        public string id
        {
            get
            {
                return GetType().FullName;
            }
            set { }
        }

        protected string HTMLVariante(int nodoID, Grupo g, int nodoFinalID)
        {
            string ret = "";
            ret += "<div style='width:100%;text-align:right;padding-bottom:4px;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + nodoID + "," + nodoFinalID + ")'></div>";
            return ret;
        }

        protected string HTMLABM(string id, Propuesta prop, bool tieneFlores, string lista, string idioma)
        {
            string ret = "";
            //nombre nuevo o existente o borrar
            if (modo == eModo.editar)
            {
                //nuevo
                ret += "<table class='abm'>";
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 1, prop, tieneFlores, "nuevo", idioma) + "</td>";
                ret += "<td style='height:35px;'>" + Tools.tr("Crear", idioma) + "</td>";
                ret += "<td class='titulo3' style='width:350px;'>";
                if (prop != null && accion == "nuevo")
                {
                    ret += HTMLText(id, prop, 40 * 8, tieneFlores, idioma);
                }
                ret += "</td>";
                ret += "</tr>";

                //existente
                if (lista != "")
                {
                    ret += "<tr>";
                    ret += "<td>" + HTMLRadio("r.accion", 2, prop, tieneFlores, "existente", idioma) + "</td>";
                    ret += "<td style='height:35px;'>" + Tools.tr("Modificar", idioma) + "</td>";
                    ret += "<td class='titulo3'>";
                    //nombre del grupo
                    if (prop != null && accion == "existente")
                    {
                        ret += HTMLLista(id, lista, prop, 40 * 8, tieneFlores, idioma);
                    }
                    ret += "</tr>";

                    //borrar
                    ret += "<tr>";
                    ret += "<td>" + HTMLRadio("r.accion", 3, prop, tieneFlores, "borrar", idioma) + "</td>";
                    ret += "<td style='height:35px;'>" + Tools.tr("Eliminar", idioma) + "</td>";
                    ret += "<td class='titulo3'>";
                    //nombre del grupo
                    if (prop != null && accion == "borrar")
                    {
                        ret += HTMLLista(id, lista, prop, 40 * 8, tieneFlores, idioma);
                    }
                    ret += "</tr>";
                }
                ret += "</table>";
            }
            else
            {
                if (prop != null)
                {
                    string labelAccion = "";
                    ret += "<table class='abm'>";
                    if (accion == "nuevo") labelAccion = Tools.tr("Crear", idioma);
                    if (accion == "existente") labelAccion = Tools.tr("Modificar", idioma);
                    if (accion == "borrar") labelAccion = Tools.tr("Eliminar", idioma);
                    ret += "<tr>";
                    ret += "<td style='height:35px;'>" + labelAccion + "</td>";
                    ret += "<td class='titulo3' style='width:350px;'>" + getText(id, prop) + "</td>";
                    ret += "</tr>";
                    ret += "</table>";
                }
            }
            return ret;
        }

        protected string HTMLSeccion(string tituloid, string tipid, string id, bool editar, Propuesta prop, bool tieneFlores, Grupo g, int width)
        {
            string ret = "";
            string value = (string)getValue(id, prop);
            if ((value != "" && (modo == eModo.consenso || modo == eModo.prevista)) || (modo != eModo.consenso && modo != eModo.prevista))
            {
                ret = "<div class='tema' style='vertical-align:top'><b>" + Tools.tr(tituloid, g.idioma) + "</b></div>";
                if (editar) ret += "<div class='smalltip'>" + Tools.tr(tipid, g.idioma) + "</div>";
                ret += "<div style='width:-webkit-fill-available'>" + HTMLArea(id, prop, width, 120, tieneFlores, g.idioma) + "</div>";
            }
            return ret;
        }

        protected virtual string HTMLEncabezado(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            titulo = getText("s.titulo", prop);
            etiqueta = getText("s.etiqueta", prop);

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div><br>";
            
            //fecha
            if (modo == eModo.consenso)
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //titulo y etiqueta
            ret += "<table>";
            ret += "<tr>";
            ret += "<td class='titulo3'>" + Tools.tr("Titulo", g.idioma) + "</td>";
            ret += "<td colspan=2>" + HTMLText("s.titulo", prop, 60 * 8, tieneFlores, g.idioma) + "</td>";
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td class='titulo3'>" + Tools.tr("Etiqueta", g.idioma) + "</td>";
            ret += "<td style='width:120px'>" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma) + "</td>";
            ret += "<td>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</td>";
            ret += "</tr>";
            ret += "</table>";
            ret += "<br>";
            return ret;
        }

        protected virtual void iniciarDocumento(List<Propuesta> props, Grupo g)
        {
        }

        public string toHTML(Propuesta prop, Grupo g, string email, int width, eModo modo)
        {
            string ret = "";
            this.modo = modo;
            this.grupo = g;

            if (prop != null && prop.nivel != 1)
                ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";
            
            //contenido
            ret += toHTMLContenido(prop.nivel, prop, g, email, width, prop);

            //comentarios
            if (prop != null && modo != eModo.consenso)
            {
                ret += "<br>";
                ret += "<div style='vertical-align:top;' class='comentarios2'>";
                ret += toHTMLComentarios(prop.nivel, prop, g, email, width, false);
                ret += "</div>";
            }
            return ret;
        }

        public string toHTML(List<Propuesta> props, Grupo g, string email, int width, eModo modo)
        {
            string ret = "";
            this.modo = modo;
            this.grupo = g;

            //reinicio el modelo
            errores.Clear();
            consensoAlcanzado = false;

            bool tieneFlores = false;
            Usuario u = g.getUsuarioHabilitado(email);
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            //ordeno las propuestas por nivel
            props.Sort();

            if (modo == eModo.consenso)
            {
                //header HTML
                ret += "<html>";
                ret += "<head>";
                ret += "<title></title>";
                ret += "<meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1' />";
                ret += "<link rel='stylesheet' type='text/css' href='styles.css'>";
                ret += "</head>";
                ret += "<body>";
            }

            iniciarDocumento(props, grupo);

            //todo el documento
            bool editar = false;
            for (int q = 1; q <= niveles; q++)
            {
                Propuesta prop = getProp(q, props);
                Propuesta propFinal = props.Count > 0 ? props[props.Count - 1] : null; //el ultimo nodo de la version seleccionada

                editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                    || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
                
                if (prop != null && prop.consensoAlcanzado) consensoAlcanzado = true;

                //veo si falta la propuesta anterior (se ha saltado una)
                if (prop != null && q > 1 && getProp(q - 1, props) == null)
                {
                    //se ha saltado una propuesta
                    addError(q, "Las propuestas deben completarse por niveles correlativos, te has saltado el nivel anterior."); //esto evita que pueda proponer
                }

                //mensaje de nivel
                if (q > 1 && modo != eModo.consenso)
                {
                    ret += "<div style='float:clear;float:left;color:gray;font-size:10px;width:-webkit-fill-available;border-top:1px solid gray;margin:3px;text-align:right;'>";
                    ret += Tools.tr("Nivel en el arbol", g.idioma) + ": " + q;
                    ret += "</div>";
                }


                //if (editar && !consensoAlcanzado)
                //    ret += "class='seccion'";
                //else
                //    ret += "class='seccion'";
                //contenido de la propuesta
                ret += "<div class='seccion'>";
                ret += toHTMLContenido(q, prop, g, email, width, propFinal);
                ret += "</div>";

                //comentarios
                if (modo != eModo.prevista && modo != eModo.consenso && prop != null && !prop.esPrevista() && !consensoAlcanzado)
                {
                    //se puede comentar
                    ret += "<div id='comentarios" + prop.nodoID + "' style='vertical-align:top;' class='comentarios'>";
                    ret += toHTMLComentarios(q, prop, g, email, 326, !consensoAlcanzado);
                    ret += "</div>";
                }
            }

            //botones de poantalla o firma de consenso
            ret += "<div style='clear:left;float:left;margin-top:15px;'></div>"; //separador

            if (modo == eModo.consenso)
            {
                //firma
                ret += firmaConsenso;
                ret += "</body>";
                firmaConsenso = ""; //por las dudas
            }
            else if (modo == eModo.editar)
            {
                //modo muestra
                if (consensoAlcanzado)
                    ret += "<div class='mensaje' style='clear:left;float:left;width:100%'><b>" + Tools.tr("Consenso alcanzado", g.idioma) + "</b></div><br>";

                ret += "<input type='button' style='clear:left;float:left;' class='btn' value='" + Tools.tr("Cerrar", g.idioma) + "' onclick='doCerrarDocumento();' />";

                if (tieneFlores && !consensoAlcanzado && editar)
                    ret += "<input type='button' style='float:left;' class='btn' value='" + Tools.tr("Prevista de propuesta", g.idioma) + "' title='" + Tools.tr("Enseña vista previa antes de proponer", g.idioma) + "' onclick='doPrevista();' />";

            }
            else if (modo == eModo.prevista)
            {
                ret += "<input type='button' style='clear:left;float:left;' class='btn' value='" + Tools.tr("Cancelar", g.idioma) + "' onclick='doCerrarDocumento();' />";
                ret += "<input type='button' style='float:left;' class='btn' value='" + Tools.tr("Revisar propuesta", g.idioma) + "' title='" + Tools.tr("Permite corregir errores", g.idioma) + "' onclick='doRevisar();' />";
                if (!hayError() && props.Count > 0 && props[props.Count - 1].esPrevista())
                    //permito crear
                    //tiene que haber almenos una propuesa nueva para poder crear algo
                    ret += "<input type='button' style='float:left;' class='btn' value='" + Tools.tr("Crear propuesta", g.idioma) + "' title='" + Tools.tr("Crea la propuesta", g.idioma) + "' onclick='doProponer();' />";
            }
            else if (modo == eModo.revisar)
            {
                //permito prevista
                ret += "<input type='button' style='clear:left;float:left;' class='btn' value='" + Tools.tr("Cancelar", g.idioma) + "' onclick='doCerrarDocumento();' />";
                if (tieneFlores && !consensoAlcanzado)
                    ret += "<input type='button' style='float:left;' class='btn' value='" + Tools.tr("Prevista de propuesta", g.idioma) + "' title='" + Tools.tr("Enseña vista previa antes de proponer", g.idioma) + "' onclick='doPrevista();' />";
            }

            //ret += "<a id='btnDownload' href='' target='_blank'><font size='1'>Descargar esta versi&oacute;n</font></a>";
            return ret;
        }

        public string toHTMLComentarios(int nivel, Propuesta prop, Grupo g, string email, int width, bool agregar)
        {
            string ret = "";
            if (prop != null)
            {
                //titulo si hay contenido
                if (prop.comentarios.Count > 0 || (agregar && !prop.esPrevista()))
                    ret += "<div class='titulo4' style='padding-left:3px'>" + Tools.tr("Comentarios", g.idioma) + ":</div>";

                foreach (Comentario c in prop.comentarios)
                {
                    if (c.objecion)
                        ret += "<div class='comentario' style='background-color:#FFD2E3'>";
                    else
                        ret += "<div class='comentario' style='background-color:#D6F5DB'>";
                    ret += toHTMLText(c.texto);

                    //fecha
                    ret += "<div style='text-align:right;color:gray;font-size:10px;'>";
                    ret += c.fecha.ToString("dd/MM/yy");
                    ret += "</div>"; 
                    
                    ret += "</div>";
                    //objecion

                }

                //agregar
                if (agregar && !prop.esPrevista())
                {
                    ret += "<textarea id='comentario" + prop.nodoID + "' maxlength='300' class='editarComentario' style='height: 50px;width:-webkit-fill-available'>";
                    ret += "</textarea><br>";
                    ret += "<input type='button' class='btn2' style='margin:0px;' value='" + Tools.tr("Enviar", g.idioma) + "' onclick='doComentar(" + prop.nodoID + ");'>";
                    ret += "&nbsp;<font size='1'>(max: 300)</font>";
                    ret += "&nbsp;<input type='checkbox' id='objecion" + prop.nodoID + "'><span style='font-size:10px'>" + Tools.tr("Objecion", g.idioma) + "</span>";
                }
            }

            return ret;
        }

        public string addError(int nivel, string s)
        {
            if (errores.ContainsKey(nivel))
                errores[nivel] = errores[nivel] + "<br>" + s;
            else
                errores.Add(nivel,s);
            return s;
        }

        public bool hayError()
        {
            return errores.Count > 0;
        }

        public string tr(string msg)
        {
            //traduce segun el idioma
            return msg;
        }

        protected Propuesta getProp(int nivel, List<Propuesta> props)
        {
            foreach (Propuesta p in props)
                if (p.nivel == nivel)
                    return p;
            return null;
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

        private int getSeparadorIndex(string s){
            int ret = 0;
            while (ret < s.Length && s[ret] != '\n' && s[ret] != ' ' && s[ret] != ',')
                ret++;
            return ret;
        }

        public string HTMLLista(string id, string valores, Propuesta prop, int width, bool tieneFlores, string idioma)
        {
            return HTMLLista(id, valores, prop, width, tieneFlores, idioma, true);
        }

        public string HTMLLista(string id, string valores, Propuesta prop, int width, bool tieneFlores, string idioma, bool autopostback)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores && !consensoAlcanzado)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<select id='" + id + "'  ";
                    ret += "class='" + v.editClassName + "' ";
                    if (autopostback) ret += "onchange=\"documentSubmit('" + id + "_click','')\" ";
                    ret += "style='width:" + width + "px;'>";
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
                string value = (string)getValue(id, prop);
                ret += "<select id='" + id + "' ";
                ret += "class='" + v.editClassName + "' ";
                if (autopostback) ret += "onchange=\"documentSubmit('" + id + "_click','" + value + "')\" ";
                ret += "style='width:" + width + "px;'>";
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
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";

            return ret;
        }

        public string HTMLArea(string id, Propuesta prop, int width, int height, bool tieneFlores, string idioma)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores)
            {
                if (modo != eModo.prevista && !consensoAlcanzado)
                {
                    ret += "<textarea id='" + id + "' ";
                    ret += "class='editarrtf' ";
                    ret += "maxlength='" + v.len + "' ";
                    ret += "style='width:" + width + "px;height:" + height + "px;'>";
                    ret += "</textarea>";
                    ret += "<div style='text-align:right;width:100%;font-size:10px;'>(" + Tools.tr("max", idioma) + ": " + v.len + ")</div>";
                    ret += "<br>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revisar
                string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(toHTMLText((string)getValue(id, prop))));
                ret += "<textarea id='" + id + "' ";
                ret += "class='editarrtf' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;height:" + height + "px;'>";
                ret += HTMLText;
                //ret += getValue(id, prop);
                ret += "</textarea>";
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
                if (prop.consensoAlcanzado)
                    ret += HTMLText + "</div>";
                    //ret += toHTMLText((string)getValue(id, prop)) + "</div>";
                else
                    ret += HTMLText + "</div>";
                    //ret += Tools.HTMLDecode(Tools.HTMLDecode(toHTMLText((string)getValueResaltado(id, prop)))) + "</div>";
                ret += "<br>";
            }
            else
            {
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";
            }


            return ret;
        }

        public virtual string documentSubmit(string accion, string parametro, List<Propuesta> props, Grupo g, string email, int width, Modelo.eModo modo)
        {
            //submit(accion, parametro, props, g, email, width, modo);
            return toHTML(props, g, email, width, modo);
        }
       
        public string HTMLListaSeleccion(string id, Propuesta prop, int width, int height, bool tieneFlores, string lista, string pertenece, string NoPertenece, string idioma)
        {

            Variable v = getVariable(id);
            string value = getText(id, prop);
            //if (value == "") value = "*"; //no vacio para que lo envie en el submit
            string ret = "";

            if (prop == null && tieneFlores)
            {
                if (modo != eModo.prevista && !consensoAlcanzado)
                {
                    //edicion
                    //seleccionados
                    ret += "<table style='width:" + width + "px;height:" + height + "px;'>";
                    ret += "<tr><td><b>" + pertenece + "</b></td><td><b>" + NoPertenece + "</b></td></tr>";
                    ret += "<tr><td class='" + v.editClassName + "' style='overflow:scroll;width:" + (width / 2 - 15) + "px;vertical-align:top;'>";
                    if (value != "" && value != "*")
                        foreach (string item in value.Split('|'))
                        {
                            ret += "<span style='vertical-align:middle; padding:3px;'>";
                            ret += "<img src='res/quitar.png' ";
                            ret += "onclick=\"documentSubmit('" + id + "_quitar','" + item + "')\" style='cursor:pointer;vertical-align:bottom;'>&nbsp;";
                            ret += item.Split(':')[1];
                            ret += "</span>";
                            ret += "<br>";
                        }
                    ret += "</td>"; 

                    //disponibles
                    ret += "<td class='" + v.editClassName + "' style='float:left;width:" + (width / 2 - 15) + "px;height:" + height + "px;overflow:scroll;vertical-align:top;'>";
                    if (lista != "")
                        foreach (string item in lista.Split('|'))
                        {
                            if (value.IndexOf(item.Split(':')[0]) == -1)
                            {
                                ret += "<span style='vertical-align:middle; padding:3px;'>";
                                ret += "<img src='res/agregar.png' ";
                                ret += "onclick=\"setNotEmpyList('" + id + "');documentSubmit('" + id + "_agregar','" + item + "')\" style='cursor:pointer;vertical-align:bottom;'>&nbsp;";
                                ret += item.Split(':')[1];
                                ret += "</span>";
                                ret += "<br>";
                            }
                        }
                    ret += "</td></tr>";
                    ret += "</table>";
                    //pongo input invisible para guardar el valor como cualquier otro
                    ret += "<br><input type='hidden' id='" + id + "' value='" + value + "' size=90>"; //no vacio para que lo envie en el submit
                    ret += "<br>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revisar
                //seleccionados
                ret += "<table style='width:" + width + "px;height:" + height + "px;'>";
                ret += "<tr><td><b>" + pertenece + "</b></td><td><b>" + NoPertenece + "</b></td></tr>";
                ret += "<tr><td class='" + v.editClassName + "' ";
                ret += "style='overflow:scroll;width:" + (width / 2 - 15) + "px;vertical-align:top;'>";
                if (value != "" && value != "*")
                    foreach (string item in value.Split('|'))
                    {
                        ret += "<nobr>";
                        ret += "<img src='res/quitar.png' onclick=\"documentSubmit('" + id + "_quitar','" + item + "')\" style='cursor:pointer;vertical-align:bottom;'>&nbsp;";
                        ret += item.Split(':')[1];
                        ret += "</nobr>";
                        ret += "<br>";
                    }
                ret += "</td>";

                //disponibles
                ret += "<td class='" + v.editClassName + "' style='float:left;width:" + (width / 2 - 15) + "px;height:" + height + "px;overflow:scroll;vertical-align:top;'>";
                if (lista != "")
                    foreach (string item in lista.Split('|'))
                    {
                        if (value.IndexOf(item.Split(':')[0]) == -1)
                        {
                            ret += "<nobr>";
                            ret += "<img src='res/agregar.png' ";
                            ret += "onclick=\"setNotEmpyList('" + id + "');documentSubmit('" + id + "_agregar','" + item + "')\" style='cursor:pointer;vertical-align:bottom;'>&nbsp;";
                            ret += item.Split(':')[1];
                            ret += "</nobr>";
                            ret += "<br>";
                        }
                    }
                ret += "</td></tr>";
                ret += "</table>";
                //pongo input invisible para guardar el valor como cualquier otro
                ret += "<br><input type='hidden' id='" + id + "' value='" + value + "' size=90>"; //no vacio para que lo envie en el submit
                ret += "<br>";
            }
            else if (prop != null)
            {
                //ver
                //seleccionados
                if (value == "*")
                    ret += "(vac&iacute;o)";
                else
                {
                    string divitem = "";
                    try
                    {
                        divitem = "<div ";
                        divitem += "class='" + v.className + "'>";
                        if (value != "")
                        {
                            foreach (string item in value.Split('|'))
                            {
                                divitem += "<nobr>";
                                divitem += item.Split(':')[1]; ;
                                divitem += "</nobr>";
                                divitem += "<br>";
                            }
                        }
                        divitem += "</div>";
                        ret += divitem;
                    }
                    catch (Exception)
                    {
                        //el valor guardado no tiene el formato correcto, lo descarto
                    }
                }
                ret += "<br>";
            }
            else
            {
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";
            }


            return ret;
        }

        public string HTMLFloat(string id, Propuesta prop, bool tieneFlores, string idioma, string format)
        {
            float d = getFloat(id, prop);
            string value = d.ToString(format);

            return input(id, prop, 150, tieneFlores, "number", value, idioma);
        }

        public string HTMLDate(string id, Propuesta prop,  bool tieneFlores, string idioma)
        {
            DateTime d = getDate(id, prop);
            string value = "";
            if (d == Tools.minValue)
                //valor nulo
                value ="";
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                value = d.Year.ToString("0000") + "-" + d.Month.ToString("00") + "-" + d.Day.ToString("00");
            }
            else
                //ver
                value = d.Day.ToString("00") + "/" + d.Month.ToString("00") + "/" + d.Year.ToString("0000");
             
            return input(id, prop, 150, tieneFlores, "date", value, idioma);
        }

        public string HTMLText(string id, Propuesta prop, int width, bool tieneFlores, string idioma)
        {
            return input(id, prop, width, tieneFlores, "text", getText(id, prop), idioma);
        }

        public string HTMLCheck(string id, Propuesta prop, bool tieneFlores, bool value, string idioma)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores && !consensoAlcanzado)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<input id='" + id + "' name='" + id + "' type='checkbox' ";
                    ret += "class='" + v.editClassName + "' ";
                    ret += "value='" + value + "' ";
                    ret += "onclick=\"documentSubmit('" + id + "_click','" + value + "')\" style='cursor:pointer;width:15px;height:15px;'>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                ret += "<input id='" + id + "' name='" + id + "' type='checkbox' ";
                ret += "class='" + v.editClassName + "' ";
                ret += " " + (value ? "checked" : "") + " ";
                ret += "onclick=\"documentSubmit('" + id + "_click','" + value + "')\" style='cursor:pointer;width:15px;height:15px;'>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<input type='checkbox' ";
                ret += "class='" + v.editClassName + "' ";
                ret += " " + (value ? "checked" : "") + " ";
                ret += "value='" + value + "' disabled='true' style='width:15px;height:15px;'>";
            }
            else
                //sin flores
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";

            return ret;
        }

        public string HTMLRadio(string id, int index, Propuesta prop, bool tieneFlores, string value, string idioma)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores && !consensoAlcanzado)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<input id='" + id + index + "' name='" + id + "' type='radio' ";
                    ret += "class='" + v.editClassName + "' ";
                    ret += "value='" + value + "' ";
                    ret += "onclick=\"documentSubmit('" + id + "_click','" + value + "')\" style='cursor:pointer;width:15px;height:15px;'>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                ret += "<input id='" + id + index + "' name='" + id + "' type='radio' ";
                ret += "class='" + v.editClassName + "' ";
                ret += "value='" + value + "' ";
                if (getText(id, prop) == value) ret += "checked ";
                ret += "onclick=\"documentSubmit('" + id + "_click','" + value + "')\" style='cursor:pointer;width:15px;height:15px;'>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<input type='radio' ";
                ret += "class='" + v.editClassName + "' ";
                if (value == getText(id, prop)) ret += "checked ";
                ret += "value='" + value + "' disabled='true' style='width:15px;height:15px;'>";
            }
            else
                //sin flores
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";

            return ret;
        }

        public string HTMLTabs(string names, string id, Propuesta prop, Grupo g)
        {
            string ret  ="";
            string tipo = "";
            string[] anames = names.Split('|');
            if (anames.Length > 0) tipo = anames[0];
            if (prop != null) tipo = getText(id, prop);

            if (modo == eModo.editar)
            {
                foreach (string name in anames)
                {
                    ret += "<span class='titulo3' ";
                    if (tipo.ToLower() != name.ToLower()) ret += "style='color:blue;text-decoration: underline;cursor:pointer' ";
                    ret += "onclick=\"documentSubmit('" + name + "_click','')\">" + Tools.tr(name, g.idioma) + "</span>";
                }
            }
            else
                ret += "<span class='titulo3'>" + Tools.tr(tipo, g.idioma) + "</span>";

            //pongo input invisible para guardar el valor como cualquier otro
            ret += "<input type='hidden' id='" + id + "' value='" + tipo + "'>"; //no vacio para que lo envie en el submit

            return ret;
        }

        private string input(string id, Propuesta prop, int width, bool tieneFlores, string tipo, string value, string idioma)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores && !consensoAlcanzado)
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
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                ret += "<input id='" + id + "' type='" + tipo + "' ";
                ret += "class='" + v.editClassName + "' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;' ";
                if (tipo == "number") value = value.Replace(",", ".");
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
            else
                //sin flores
                ret += "<div style='color:gray;font-size:12px;'>" + Tools.tr("No tiene flores para crear una propuesta", idioma) + "</div>";

            return ret;
        }

        public object parse(string id, string valor)
        {
            object ret;
            Variable v = getVariable(id);
            if (v.id.StartsWith("s."))
                ret= valor;
            else if (v.id.StartsWith("b."))
                ret = valor != "false";
            else if (v.id.StartsWith("d."))
                try
                {
                    ret = DateTime.Parse(valor);
                }
                catch(Exception ex)
                {
                    ret = Tools.minValue;
                }
            else if (v.id.StartsWith("r."))
                ret = valor;
            else if (v.id.StartsWith("f."))
            {
                try
                {
                    //se asume formato español
                    ret = float.Parse(valor.Replace(".",","));
                }
                catch(Exception ex)
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

        public Variable getVariable(string id)
        {
            foreach (Variable v in variables)
                if (v.id == id)
                    return v;
            throw new Exception("Variable [" + id + "] no existe");
        }

        public string getValueResaltado(string id, Propuesta prop)
        {
            //devuelvo la propuesta resultado de comparar el texto con sus hermanos
            if (id.StartsWith("s."))
            {
                if (prop != null)
                {
                    if (prop.nodoID != 0)
                    {
                        //creo propuesta de retorno con textos comparados
                        List<Nodo> pathn = grupo.arbol.getPath(prop.nodoID);
                        if (pathn.Count > 2) //tiene padre no raiz
                        {
                            Nodo nodo = pathn[0];
                            Nodo padre = pathn[1];
                            return Comparador.comparar((string)getValue(id, prop), getTextosHermanos(padre, nodo, id));
                        }
                        else
                            //esta propuesta es de nivel 1 (no se compara con sus hermanos)
                            return (string)getValue(id, prop);
                    }
                    else
                        return (string)getValue(id, prop);
                }
                else
                    return "";
            }
            else
                throw new Exception("getValueResaltado(): Solo para strings: " + id);
        }

        private List<string> getTextosHermanos(Nodo padre, Nodo n, string id)
        {
            //preparao lista de textos de los hermanos
            List<string> ret = new List<string>();
            foreach (Nodo hno in padre.children)
                if (hno != n)
                {
                    Propuesta op = grupo.arbol.getPropuesta(hno.id);
                    ret.Add((string)getValue(id, op));
                }
            return ret;
        }

        public DateTime getDate(string id, Propuesta prop)
        {
            try
            {
                return (DateTime)getValue(id, prop);
            }
            catch (Exception)
            {
                return Tools.minValue;
            }
        }

        public float getFloat(string id, Propuesta prop)
        {
            try
            {
                return (float)getValue(id, prop);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string getText(string id, Propuesta prop)
        {
            return (string)getValue(id, prop);
        }

        public bool getBool(string id, Propuesta prop)
        {
            return (bool)getValue(id, prop);
        }

        public object getDefaultValue(string id)
        {
            if (id.StartsWith("b."))
                return false;
            else if (id.StartsWith("f."))
                return 0f;
            else
                return "";
        }

        private object getValue(string id, Propuesta prop)
        {
            if (prop == null)
                return getDefaultValue(id);
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
                    return getDefaultValue(id);
            }
        }

        public string HTMLFlores(Nodo n, bool showVariante, Usuario u) {
            string ret;
            ret = "<div class='votos' style='clear:right;float:right;vertical-align:center;'><nobr>&nbsp;";
            ret += n.born.ToString("dd/MM/yy");
            ret += "&nbsp;<img src='res/icono.png' style='vertical-align:middle'>";
            ret += "&nbsp;" + n.getFloresTotales();
            ret += "&nbsp;</nobr></div>";
            if (n.nivel > 0 && showVariante) {
                if (u.floresDisponibles().Count == 0)
                    ret += "<input type='button' class='btnDis' style='float:right;' value='Crear variante' title='No tienes flores disponibles' disabled>";
                else
                    ret += "<input type='button' class='btn' style='float:right;' value='Crear variante' title='Crea otra propuesta basada en esta' onclick='doVariante(" + n.id + ");'>";
            }
            return ret;
        }
    }
}

