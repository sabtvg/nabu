using System;
using System.Collections.Generic;
using System.Web;

namespace nabu
{
    public abstract class Modelo
    {
        public enum eModo { ver, editar, prevista, revisar, consenso };

        public string nombre = "";
        public string icono = "res/doc.png";
        public int niveles = 0;
        public bool enUso = false;
        public bool activo = true;
        public string titulo = "";
        public string etiqueta = "";
        public string idioma = "ES";
        public string firmaConsenso = ""; //solo se usa para generar el consenso
        
        private static List<Modelo> modelos;

        protected List<Variable> variables = new List<Variable>();
        protected eModo modo = eModo.editar;
        protected bool consensoAlcanzado = false;
        protected Grupo grupo;

        protected abstract string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width);
        protected abstract void crearVariables();

        public virtual void ejecutarConsenso(Documento doc)
        {
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

        public string toHTML(Propuesta prop, Grupo g, string email, int width, eModo modo)
        {
            string ret = "";
            this.modo = modo;
            this.grupo = g;
            ret += toHTMLContenido(prop.nivel, prop, g, email, width);
            ret += "<br>Comentarios:";
            ret += toHTMLComentarios(prop.nivel, prop, g, email, width - 30, false);
            return ret;
        }

        public string toHTML(List<Propuesta> props, Grupo g, string email, int width, eModo modo)
        {
            string ret = "";
            this.modo = modo;
            this.grupo = g;

            //reinicio el modelo
            errores.Clear();
            crearVariables();
            consensoAlcanzado = false;

            bool tieneFlores = false;
            Usuario u = g.getUsuario(email);
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

            //todo el documento
            for (int q = 1; q <= niveles; q++)
            {
                Propuesta prop = getProp(q, props);
                bool editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                    || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
                
                if (prop != null && prop.consensoAlcanzado) consensoAlcanzado = true;

                //veo si falta la propuesta anterior (se ha saltado una)
                if (prop != null && q > 1 && getProp(q - 1, props) == null)
                {
                    //se ha saltado una propuesta
                    addError(q, "Las propuestas deben completarse por niveles correlativos, te has saltado el nivel anterior."); //esto evita que pueda proponer
                }

                ret += "<table style='width: " + (width - 10) + "px;'>";
                //mensaje de nivel
                if (q > 1 && modo != eModo.consenso)
                {
                    ret += "<tr>";
                    ret += "<td><hr></td>";
                    ret += "<td><span style='color:gray;font-size:10px;'>" + tr("Nivel en el arbol:") + " " + q + "</span></td>";
                    ret += "</tr>";
                }

                ret += "<tr>";
                //contenido de la propuesta
                int contenidoWidth = modo == eModo.consenso ? width : width - 340;
                ret += "<td ";
                ret += "style='width: " + contenidoWidth + "px;vertical-align:top;' ";
                if (editar && !consensoAlcanzado)
                    ret += "class='editar'";
                ret += ">";
                ret += toHTMLContenido(q, prop, g, email, contenidoWidth);
                ret += "    </td>";

                //comentarios
                if (modo != eModo.consenso)
                {
                    if (prop != null && !prop.esPrevista())
                    {
                        //se puede comentar
                        ret += "<td id='comentarios" + prop.nodoID + "' style='width: " + 250 + "px;vertical-align:top;' class='comentarios'>";
                        ret += toHTMLComentarios(q, prop, g, email, 246, !consensoAlcanzado);
                        ret += "</td>";
                    }
                    else
                    {
                        ret += "<td style='width: " + (250) + "px;vertical-align:top;' >";
                        ret += "</td>";
                    }
                }
                ret += "</tr>";
                ret += "</table>";
            }

            //botones de poantalla o firma de consenso
            ret += "<br>";
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
                    ret += "<div class='comentario'><b>" + tr("Consenso alcanzado") + "</b></div><br>";

                ret += "<input type='button' class='btn' value='" + tr("Cerrar") + "' onclick='doCerrarDocumento();' />";

                if (tieneFlores && !consensoAlcanzado)
                    ret += "<input type='button' class='btn' value='" + tr("Prevista de propuesta") + "' title='" + tr("Enseña vista previa antes de proponer") + "' onclick='doPrevista();' />";

            }
            else if (modo == eModo.prevista)
            {
                ret += "<input type='button' class='btn' value='" + tr("Cancelar") + "' onclick='doCerrarDocumento();' />";
                ret += "<input type='button' class='btn' value='" + tr("Revisar propuesta") + "' title='" + tr("Permite corregir errores") + "' onclick='doRevisar();' />";
                if (!hayError() && props[props.Count - 1].esPrevista())
                    //permito crear
                    //tiene que haber almenos una propuesa nueva para poder crear algo
                    ret += "<input type='button' class='btn' value='" + tr("Crear propuesta") + "' title='" + tr("Crea la propuesta") + "' onclick='doProponer();' />";
            }
            else if (modo == eModo.revisar)
            {
                //permito prevista
                ret += "<input type='button' class='btn' value='" + tr("Cancelar") + "' onclick='doCerrarDocumento();' />";
                if (tieneFlores && !consensoAlcanzado)
                    ret += "<input type='button' class='btn' value='" + tr("Prevista de propuesta") + "' title='" + tr("Enseña vista previa antes de proponer") + "' onclick='doPrevista();' />";
            }

            //ret += "<a id='btnDownload' href='' target='_blank'><font size='1'>Descargar esta versi&oacute;n</font></a>";
            return ret;
        }

        public string toHTMLComentarios(int nivel, Propuesta prop, Grupo g, string email, int width, bool agregar)
        {
            string ret = "";
            if (prop != null)
            {
                foreach (Comentario c in prop.comentarios)
                {
                    ret += "<div class='comentario' style='overflow: auto;width:" + (width - 15) + "px'>";
                    ret += toHTML(c.texto);
                    ret += "</div>";
                    //fecha
                    ret += "<div style='text-align:right;color:gray;font-size:10px;width:" + (width - 10) + "px'>";
                    ret += c.fecha.ToShortDateString();
                    ret += "</div>";
                }

                //agregar
                if (agregar && !prop.esPrevista())
                {
                    ret += "<textarea id='comentario" + prop.nodoID + "' maxlength='200' class='editar' style='width: " + (width - 15) + "px; height: 70px;'>";
                    ret += "</textarea><br>";
                    ret += "<input type='button' class='btn2' value='" + tr("Enviar") + "' onclick='doComentar(" + prop.nodoID + ");'>";
                    ret += "&nbsp;<font size='1'>(max: 200)</font>";
                }
            }

            return ret;
        }

        public string addError(int nivel, string s)
        {
            s = tr(s);
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

        public static List<Modelo> getModelos()
        {
            //aqui se dan de alta los modelos existentes
            if (modelos != null)
                return modelos;
            else
            {
                modelos = new List<Modelo>();
                modelos.Add(new modelos.Manifiesto());
                modelos.Add(new modelos.Accion());
            }
            return modelos;
        }

        public static Modelo getModelo(string id)
        {
            foreach (Modelo m in getModelos())
            {
                if (m.id == id)
                    return m;
            }
            throw new Exception("Modelo [" + id + "] no existe");
        }

        private string toHTML(string s)
        {
            string ret = s;

            //reemplazo links
            int ini = s.ToLower().IndexOf("http://");
            int fin;
            string link;
            while (ini >= 0)
            {
                s = s.Substring(ini);
                fin = getSeparadorIndex(s);
                link = s.Substring(0, fin);
                ret = ret.Replace(link, "<a href='" + link + "' target='_blank'>" + link + "</a>");

                s = s.Substring(fin);
                ini = s.ToLower().IndexOf("http://");
            }

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

        public string area(string id, Propuesta prop, int width, int height, bool tieneFlores)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores)
            {
                if (modo != eModo.prevista && !consensoAlcanzado)
                {
                    ret += "<textarea id='" + id + "' ";
                    ret += "class='editar' ";
                    ret += "maxlength='" + v.len + "' ";
                    ret += "style='width:" + width + "px;height:" + height + "px;'>";
                    ret += "</textarea>";
                    ret += "<div style='text-align:right;width:" + (width + 14) + "px;font-size:10px;'>(" + tr("max") + ": " + v.len + ")</div>";
                    ret += "<br>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revisar
                ret += "<textarea id='" + id + "' ";
                ret += "class='editar' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;height:" + height + "px;'>";
                ret += getValue(id, prop);
                ret += "</textarea>";
                ret += "<div style='text-align:right;width:" + (width + 14) + "px;font-size:10px;'>(" + tr("max") + ": " + v.len + ")</div>";
                ret += "<br>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<div ";
                ret += "class='" + v.className + "' ";
                ret += "style='width:" + width + "px;'>";
                ret += toHTML((string)getValueResaltado(id, prop)) + "</div>";
                ret += "<br>";
            }
            else
            {
                ret += "<span style='color:gray;font-size:12px;padding:5px;'>" + tr("[No tiene flores para crear una propuesta]") + "</span>";
                ret += "<br>";
            }


            return ret;
        }

        public string txt(string id, Propuesta prop, int width, bool tieneFlores)
        {
            Variable v = getVariable(id);
            string ret = "";
            if (prop == null && tieneFlores && !consensoAlcanzado)
            {
                if (modo != eModo.prevista)
                {
                    //editar en blanco
                    ret += "<input id='" + id + "' type='text' ";
                    ret += "class='editar' ";
                    ret += "maxlength='" + v.len + "' ";
                    ret += "style='width:" + width + "px;'>";
                }
            }
            else if (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar))
            {
                //revision
                ret += "<input id='" + id + "' type='text' ";
                ret += "class='editar' ";
                ret += "maxlength='" + v.len + "' ";
                ret += "style='width:" + width + "px;' ";
                ret += "value='" + getValue(id, prop) + "'>";
            }
            else if (prop != null)
            {
                //ver
                ret += "<span type='text' readonly ";
                ret += "class='" + v.className + "' ";
                ret += "style='width:" + width + "px;'>";
                ret += getValue(id, prop) + "</span>";
            }
            else
                //sin flores
                ret += "<span style='color:gray;font-size:12px;'>" + tr("[No tiene flores para crear una propuesta]") + "</span>";

            return ret;
        }

        public object parse(string id, string valor)
        {
            Variable v = getVariable(id);
            if (v.id.StartsWith("s"))
                return valor;
            else if (v.id.StartsWith("f"))
                return float.Parse(valor);
            else
                throw new Exception("Modelo.parse: Tipo no implementado");
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

        public object getValue(string id, Propuesta prop)
        {
            if (prop == null)
                return "";
            else
            {
                if (prop.bag.ContainsKey(id))
                    return prop.bag[id];
                else
                    return "";
            }
        }

        public string HTMLFlores(Nodo n, bool showVariante, Usuario u) {
            string ret;
            ret = "<table style='width:100%;'><tr>";
            ret += "<td class='votos'  style='vertical-align:center;'>";
            ret += "<img src='res/icono.png'>";
            ret += "&nbsp;" + n.getFloresTotales();
            ret += "</td><td style='text-align:right;'>";
            if (n.nivel > 0 && showVariante) {
                if (u.floresDisponibles().Count == 0)
                    ret += "<input type='button' class='btnDis' value='Crear variante' title='No tienes flores disponibles' disabled>";
                else
                    ret += "<input type='button' class='btn' value='Crear variante' title='Crea otra propuesta basada en esta' onclick='doVariante(" + n.id + ");'>";
            }
            ret += "</tr></table>";
            return ret;
        }
    }
}

