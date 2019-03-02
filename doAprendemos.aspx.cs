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
using System.Web.UI;
using System.Web.UI.WebControls;

//atiendo peticiones de arbol

namespace nabu
{
    public partial class doAprendemos : System.Web.UI.Page
    {
        public Aplicacion app;
        public int saveTime = 10; //guardar arboles cada x minutos
        public int cleanTime = 20; //quito arbol de memoria si no se toca en 20 minutos

        protected void Page_Load(object sender, EventArgs e)
        {
            string actn = Request["actn"];

            Application.Lock();
            if (Application["aplicacion"] == null)
                Application["aplicacion"] = new Aplicacion(Server, Request);
            app = (Aplicacion)Application["aplicacion"];
            Application.UnLock();

            Tools.startupPath = Server.MapPath("");
            Tools.server = Server;

            try
            {
                //guardo lista de arboles periodicamente
                app.verifySave();
                
                //proceso peticiones
                Grupo grupo;
                Arbol a;
                string ret = "";

                if (actn != null)
                {
                    switch (actn.ToLower())
                    {
                        case "getquesopersonal":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                //pruebas
                                //grupo.queso = new Queso();
                                //app.saveGrupos();

                                //devuelvo el queso
                                List<Type> tipos = new List<Type>();
                                foreach (Modelo m in grupo.organizacion.getModelosDocumento()) tipos.Add(m.GetType());
                                foreach (ModeloEvaluacion m in grupo.organizacion.getModelosEvaluacion()) tipos.Add(m.GetType());
                                ret = Tools.toJson(grupo.queso.getQuesoPersonal(Request["email"]), tipos);
                            }
                            Response.Write(ret);
                            app.addLog("getQuesoPersonal", Request.UserHostAddress, Request["grupo"], "", "");
                            break;

                        case "evaluartema":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doEvaluarTema(Request["idTema"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "getquesoresultado":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(getQuesoResultado(Request["grupo"], int.Parse(Request["temaIndex"]), int.Parse(Request["preguntaIndex"])));
                            break;
                            
                        case "htmlevaluacion":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(HTMLEvaluacion(null, Request["modeloEvaluacion"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "prevista":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doPrevista(Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"]), Request));
                            break;

                        case "revisar":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doRevisar(Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "crearevaluacion":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doCrearEvaluacionUsuario(Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            app.addLog("crearevaluacion", Request.UserHostAddress, Request["grupo"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "crearevaluacionalpadre":
                            Response.Write(crearEvaluacionAlPadre(Request["grupopadre"], Request["grupohijo"], Request["docnombre"], Request["doctitulo"], Request["docmodeloid"], Request.Files[0]));
                            app.addLog("crearevaluacion", Request.UserHostAddress, Request["grupo"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "crearevaluacionalhijo":
                            Response.Write(crearEvaluacionAlHijo(Request["grupopadre"], Request["grupohijo"], Request["docnombre"], Request["doctitulo"], Request["docmodeloid"], Request.Files[0]));
                            app.addLog("crearevaluacion", Request.UserHostAddress, Request["grupo"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "evaluacionsubmit":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doDocumentSubmit(Request["accion"], Request["parametro"], Request["grupo"], Request["email"], Request["modelo"], int.Parse(Request["width"]), Request));
                            break;

                        default:
                            throw new appException("Peticion no reconocida");
                    }
                }
                else
                    throw new appException("Peticion no reconocida");
            }
            catch (appException ex)
            {
                Response.Write("Error=" + ex.Message);
            }
            catch (Exception ex)
            {
                string s = "Actn:" + actn.ToLower() + "<br>";
                s += "Message:" + ex.Message + "<br>";
                s += "REMOTE_ADDR:" + Request.ServerVariables["REMOTE_ADDR"] + "<br>";
                s += "Querystring:" + Request.QueryString.ToString() + "<br>";
                s += "Form:" + Request.Form.ToString() + "<br>";
                s += "Stack:" + ex.StackTrace;

                Response.Write("Error=" + ex.Message);
                app.addLog("server exception", "", "", "", s);
            }
            Response.End();
        }

        public string doDocumentSubmit(string accion, string parametro, string grupo, string email, string modeloID, int width, HttpRequest req)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(modeloID);
            Usuario u = g.getUsuarioHabilitado(email);

            lock (g)
            {
                Propuesta prop = prepararDocumento(g, email, modeloID, req);

                //proceso evento
                m.evaluacionSubmit(accion, parametro, prop, g, email); //las propuesta debe ir en orden de nivel

                //genro respuesta
                ret = m.toHTML(prop, g, email, width, ModeloEvaluacion.eModo.editar);

                //guarpo prevista para poder crearla luego
                Prevista prev = new Prevista();
                prev.titulo = m.titulo;
                prev.propuestas.Clear();
                prev.propuestas.Add(prop);
                u.prevista = prev;
            }
            return ret;
        }

        string crearEvaluacionAlHijo(string grupopadre, string grupohijo, string docnombre, string doctitulo, string docmodeloid, HttpPostedFile file)
        {
            //yo soy el hijo
            string ret = "";
            Grupo g = app.getGrupo(grupohijo);
            lock (g)
            {
                //compruebo grupo padre
                if (g.padreNombre == grupopadre)
                {
                    //escribo documento recibido
                    int docID = g.queso.lastEvalID++;
                    string fname = docmodeloid + "_" + docID.ToString("0000");
                    string docPath = "evaluaciones\\intergrupal\\" + grupopadre + "\\" + docID.ToString("0000");
                    string URL = g.URL + "/grupos/" + g.nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

                    //creo carpeta se salida
                    if (!System.IO.Directory.Exists(g.path + "\\" + docPath))
                    {
                        System.IO.Directory.CreateDirectory(g.path + "\\" + docPath);
                        System.IO.Directory.CreateDirectory(g.path + "\\" + docPath + "\\res\\documentos");
                        System.IO.File.Copy(g.path + "\\..\\..\\styles.css", g.path + "\\" + docPath + "\\styles.css");
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(g.path + "\\..\\..\\res\\documentos");
                        foreach (System.IO.FileInfo fi in di.GetFiles())
                        {
                            System.IO.File.Copy(fi.FullName, g.path + "\\" + docPath + "\\res\\documentos\\" + fi.Name);
                        }
                    }
                    //guardo documento recibido
                    file.SaveAs(g.path + "\\" + docPath + "\\" + fname + ".html");

                    //creo modelos de evaluacion con datos identificatorios
                    ModeloEvaluacion m = g.organizacion.getModeloEvaluacion("nabu.plataforma.modelosEvaluacion.AlHijo");
                    m.evaluadoID = (g.queso.lastEvalID++).ToString();
                    m.temaNombre = grupohijo + ": " + doctitulo;
                    m.temaIcono = "res/documentos/alhijo.png";
                    m.temaURL = URL;
                    m.temaAutor = grupohijo;

                    //creo evaluacion vacia para este documento nuevo
                    doCrearEvaluacion(m, g, "", null);

                    //alertas
                    foreach (Usuario u in g.getUsuariosHabilitados())
                        u.alertas.Add(new Alerta(Tools.tr("Nuevo documento intergrupal [%1]", doctitulo, g.idioma)));

                    ret = "ok";
                }
                else
                    ret = "Padre [" + grupopadre + "] no encontrado en [" + grupohijo + "]";
            }

            //guardo
            //app.saveGrupos();

            return ret;
        }

        string crearEvaluacionAlPadre(string grupopadre, string grupohijo, string docnombre, string doctitulo, string docmodeloid, HttpPostedFile file)
        {
            //yo soy el padre
            string ret = "";
            Grupo g = app.getGrupo(grupopadre);
            lock (g)
            {             
                //compruebo grupo hijo
                bool encontrado = false;
                foreach (Hijo h in g.hijos)
                    if (h.nombre == grupohijo)
                        encontrado = true;

                if (encontrado)
                {
                    //escribo documento recibido
                    int docID = g.queso.lastEvalID++;
                    string fname = docmodeloid + "_" + docID.ToString("0000");
                    string docPath = "evaluaciones\\intergrupal\\" + grupohijo + "\\" + docID.ToString("0000");
                    string URL = g.URL + "/grupos/" + g.nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

                    //creo carpeta se salida
                    if (!System.IO.Directory.Exists(g.path + "\\" + docPath))
                    {
                        System.IO.Directory.CreateDirectory(g.path + "\\" + docPath);
                        System.IO.Directory.CreateDirectory(g.path + "\\" + docPath + "\\res\\documentos");
                        System.IO.File.Copy(g.path + "\\..\\..\\styles.css", g.path + "\\" + docPath + "\\styles.css");
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(g.path + "\\..\\..\\res\\documentos");
                        foreach (System.IO.FileInfo fi in di.GetFiles())
                        {
                            System.IO.File.Copy(fi.FullName, g.path + "\\" + docPath + "\\res\\documentos\\" + fi.Name);
                        }
                    }
                    //guardo documento recibido
                    file.SaveAs(g.path + "\\" + docPath + "\\" + fname + ".html");

                    //creo modelos de evaluacion con datos identificatorios
                    ModeloEvaluacion m = g.organizacion.getModeloEvaluacion("nabu.plataforma.modelosEvaluacion.AlPadre");
                    m.evaluadoID = (g.queso.lastEvalID++).ToString();
                    m.temaNombre = grupohijo + ": " + doctitulo;
                    m.temaIcono = "res/documentos/alpadre.png";
                    m.temaURL = URL;
                    m.temaAutor = grupohijo;

                    //creo evaluacion vacia para este documento nuevo
                    doCrearEvaluacion(m, g, "", null);

                    //alertas
                    foreach (Usuario u in g.getUsuariosHabilitados())
                        u.alertas.Add(new Alerta(Tools.tr("Nuevo documento intergrupal [%1]", doctitulo, g.idioma)));

                    ret = "ok";
                }
                else
                    ret = "Hijo [" + grupohijo + "] no encontrado en [" + grupopadre + "]";
            }

            //guardo
            //app.saveGrupos();

            return ret;
        }

        string doCrearEvaluacionUsuario(string modeloID, string grupo, string email, int width)
        {
            //el usuario quiere crer una evaluacion
            Grupo g = app.getGrupo(grupo);
            Usuario u = null;
            Evaluacion ev = null;
            string ret = "";
            lock (g)
            {
                ModeloEvaluacion m;
                m = g.organizacion.getModeloEvaluacion(modeloID); 

                //genero evlacuacion HTML y guardo
                u = g.getUsuario(email);
                string html = m.toHTML(u.prevista.propuestas[0], g, email, width, ModeloEvaluacion.eModo.finalizado);

                //escribo
                int docID = g.queso.lastEvalID++;
                string fname = m.nombre + "_" + docID.ToString("0000");
                string docPath = "evaluaciones\\" + m.carpeta() + "\\" + docID.ToString("0000");
                string URL = g.URL + "/grupos/" + g.nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

                //creo carpeta se salida
                if (!System.IO.Directory.Exists(g.path + "\\" + docPath))
                {
                    System.IO.Directory.CreateDirectory(g.path + "\\" + docPath);
                    System.IO.Directory.CreateDirectory(g.path + "\\" + docPath + "\\res\\documentos");
                    System.IO.File.Copy(g.path + "\\..\\..\\styles.css", g.path + "\\" + docPath + "\\styles.css");
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(g.path + "\\..\\..\\res\\documentos");
                    foreach (System.IO.FileInfo fi in di.GetFiles())
                    {
                        System.IO.File.Copy(fi.FullName, g.path + "\\" + docPath + "\\res\\documentos\\" + fi.Name);
                    }
                }
                System.IO.File.WriteAllText(g.path + "\\" + docPath + "\\" + fname + ".html", html, System.Text.Encoding.UTF8);

                //crea evalaucion
                Propuesta prop = u.prevista.propuestas[0];
                ev = new Evaluacion();
                ev.email = email;
                ev.id = g.queso.lastEvalID++;
                ev.preguntas = m.getPreguntas(prop);

                string msg = doCrearEvaluacion(m, g, email, ev);

                //devuelvo el queso Personal
                ret = Tools.toJson(g.queso.getQuesoPersonal(email, msg));
            }

            //guardo
            //app.saveGrupos();

            return ret;
        }

        string doCrearEvaluacion(ModeloEvaluacion m, Grupo g, string email, Evaluacion ev)
        {
            string msg = "";

            //tema
            string idTema = m.nombre + ":" + m.getEvaluadoID();
            Tema t = g.queso.getTema(idTema);
            if (t == null)
            {
                t = new Tema();
                t.id = idTema;
                t.modeloEvaluacionID = m.id;
                t.nombre = m.getTemaNombre();
                t.icono = m.getTemaIcono();
                t.URL = m.getTemaURL();
                t.evaluadoID = m.getEvaluadoID();
                t.autor = m.getTemaAutor();

                g.queso.temas.Add(t);
            }

            //agrego evaluacion (reemplazo anterior si hay)
            if (ev != null)
            {
                msg = "";
                foreach (Evaluacion e in t.evaluaciones)
                    if (e.email == email)
                    {
                        t.evaluaciones.Remove(e);
                        msg = Tools.tr("La evaluacion anterior ha sido reemplazada", g.idioma);
                        break;
                    }
                t.evaluaciones.Add(ev);
            }

            //limpio
            Usuario u = g.getUsuario(email);
            if (u != null) u.prevista = null;

            //actualizo queso
            g.queso.evaluar();

            return msg;
        }

        string doRevisar(string modeloID, string grupo, string email, int width)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(modeloID);

            lock (g)
            {
                //preparo propuestas de nodos ancestros
                g.ts = DateTime.Now;

                //agrego las propuestas de prevista
                Usuario u = g.getUsuario(email);

                //genro revision
                ret = m.toHTML(u.prevista.propuestas[0], g, email, width, ModeloEvaluacion.eModo.revisar); //las propuesta debe ir en orden de nivel
            }
            return ret;
        }

        string doPrevista(string modeloID, string grupo, string email, int width, HttpRequest req)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(modeloID);
            lock (g)
            {
                Propuesta prop = prepararDocumento(g, email, modeloID, req);

                //genro prevista para segurarme que defina etiqueta y titulo
                ret = m.toHTML(prop, g, email, width, ModeloEvaluacion.eModo.prevista);

                //guarpo prevista para poder crearla luego
                Usuario u = g.getUsuarioHabilitado(email);
                Prevista prev = new Prevista();
                prev.titulo = m.titulo;
                prev.propuestas.Clear();
                prev.propuestas.Add(prop);
                u.prevista = prev;
            }
            return ret;
        }

        private Propuesta prepararDocumento(Grupo g, string email, string modeloID, HttpRequest req)
        {
            //preparo propuestas de nodos ancestros
            Propuesta prop;
            Arbol a = g.arbol;
            ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(modeloID);

            g.ts = DateTime.Now;

            Usuario u = g.getUsuarioHabilitado(email);
            if (u.prevista != null)
                prop = u.prevista.propuestas[0];
            else
                prop = new Propuesta();

            //agrego las propuestas de prevista
            foreach (string reqID in req.Form.AllKeys)
            {
                if (reqID != null && m.isVariable(reqID) && req[reqID] != "")
                {
                    //este valor me lo guardo en la prpuesta para ese nivel
                    Variable v = m.getVariable(reqID);
                    prop.bag[reqID] = m.parse(reqID, req[reqID]);
                }
            }

            return prop;
        }

        string doEvaluarTema(string idTema, string grupo, string email, int width)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                g.ts = DateTime.Now;

                //busco el tema
                foreach (Tema t in g.queso.temas)
                {
                    if (t.id == idTema)
                    {
                        ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(t.modeloEvaluacionID);
                        Propuesta prop = m.createProp(t);
                        ret = m.toHTML(prop, g, email, width, ModeloEvaluacion.eModo.editar); //las propuesta debe ir en orden de nivel

                        //guarpo prevista para poder crearla luego
                        Usuario u = g.getUsuarioHabilitado(email);
                        Prevista prev = new Prevista();
                        prev.titulo = m.titulo;
                        prev.propuestas.Clear();
                        prev.propuestas.Add(prop);
                        u.prevista = prev;
                    }
                }
            }
            return ret;
        }

        string getQuesoResultado(string grupo, int temaIndex, int preguntaIndex)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            List<string> textos = new List<string>();
            lock (g)
            {
                ret += "<div class='titulo1'>#pregunta#</div>";

                //NO SE PUEDEN ENSEÑAR PALABRAS CLAVE PROQUE TIENEN CODIGO HTML
                //kyewords
                ret += "<div>" + Tools.tr("Palabras clave", g.idioma) + ":</div><br>";
                ret += "#keywords#<br><br>";

                //textos
                ret += "<table style='width:100%'>";
                g.ts = DateTime.Now;                //separo info del queso
                var tema = g.queso.temas[temaIndex];
                var textoPregunta = "";
                foreach (Evaluacion ev in tema.evaluaciones) {
                    var pr = ev.preguntas[preguntaIndex];
                    textoPregunta = pr.pregunta;
                    ret += "<tr><td>" + ev.born.ToString("dd/MM/yy") + "</td><td style='width:240px'>" + HTMLBarra(pr.respuesta, pr.minText, pr.maxText) + "</td></tr>";

                    string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(pr.texto.Replace("\n", "<br>")));
                    ret += "<tr><td colspan=2 class='cuadro'>" + HTMLText + "</td></tr>";
                    textos.Add(pr.texto);
                }
                ret += "</table><br>";
                ret += "<input type='button' class='btn' style='float:unset;margin-left:calc(50% - 30px);' value='" + Tools.tr("Cerrar", g.idioma) + "' onclick='doCerrarDocumento();' />";

                //obtengo palkabras clave
                Dictionary<string, int> pals = new Dictionary<string, int>();
                string keywords = getPalabrasClave(textos, pals);

                ////resalto palarbas
                //foreach (KeyValuePair<string, int> e in pals)
                //    if (e.Value > 1)
                //        ret = ret.Replace(e.Key, "<span style='background-color:#CEE7FF'>" + e.Key + "</span>");

                //agrego la pregunta
                ret = ret.Replace("#pregunta#", textoPregunta);

                //agrego los keywords
                ret = ret.Replace("#keywords#", keywords);
                
            }
            return ret;
        }

        string getPalabrasClave(List<string> textos, Dictionary<string, int> pals)
        {
            //cuento palabras
            foreach (string s in textos)
            {
                string limpio = cleanHTML(Tools.HTMLDecode(Tools.HTMLDecode(s)));
                string[] textoPals = limpio.Split(' ');
                string pp;
                foreach (string p in textoPals)
                {
                    pp = p;
                    pp = pp.Replace(",", "");
                    pp = pp.Replace(".", "");
                    pp = pp.Replace(")", "");
                    pp = pp.Replace("(", "");
                    pp = pp.Replace("}", "");
                    pp = pp.Replace("{", "");
                    if (pp.Length > 5)
                    {
                        if (pals.ContainsKey(pp))
                            pals[pp]++;
                        else
                            pals[pp] = 1;
                    }
                }
            }

            //armo respuesta
            string ret = "";
            foreach (KeyValuePair<string, int> e in pals)
            {
                if (e.Value > 1 && e.Key.Length < 30)
                {
                    int size = 12 + 2 * e.Value;
                    if (size > 30) size = 30;
                    ret += "<span class='keyword' style='font-size:" + (size) + "px'>" + e.Key + "</span> ";
                }
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        string cleanHTML(string s)
        {
            string ret = "";
            bool inh = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '<')
                    inh = true;
                else if (s[i] == '>')
                {
                    inh = false;
                    ret += ' ';
                }
                else if (!inh)
                    ret += s[i];
            }
            return ret;
        }

        string HTMLBarra(int value, string minText, string maxText)
        {
            string ret = "";
            ret += "<table style='border-spacing: 0;'><tr>";
            string color = "";
            string border = "";
            for (int i = 1; i <= 10; i++)
            {
                color = value != i ? "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 0.4)" : "rgba(" + (100 - i * 10 - 10) + "%, " + (i * 10 - 10) + "%, 50%, 1)";
                border = value == i ? "border:3px solid blue;" : "border:3px solid transparent;";
                ret += "<td style='" + border + ";width:20px;background-color:" + color + "' ";
                ret += ">&nbsp;</td>";
            }
            ret += "</tr>";
            ret += "<tr><td colspan=5 style='text-align:left;font-size:12px'>" + minText + "</td><td colspan=5 style='text-align:right;font-size:12px'>" + maxText + "</td></tr>";
            ret += "</table>";
            return ret;
        }

        string HTMLEvaluacion(Propuesta prop, string modeloID, string grupo, string email, int width)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                ModeloEvaluacion m = g.organizacion.getModeloEvaluacion(modeloID);
                
                //limpio valores antiguos
                Usuario u = g.getUsuarioHabilitado(email);
                if (u.prevista != null)
                    u.prevista = null;

                ret = m.toHTML(prop, g, email, width, ModeloEvaluacion.eModo.editar); //las propuesta debe ir en orden de nivel
            }
            return ret;
        }

        public void VerificarUsuario(string grupo, string email, string clave)
        {
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Usuario u = g.getUsuarioHabilitado(email);
                if (u.clave == clave)
                    return;
            }
            throw new Exception(Tools.tr("Usuario invalido o no habilitado", g.idioma));
        }

    }
}