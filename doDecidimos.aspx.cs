using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//atiendo peticiones de arbol

namespace nabu
{
    public partial class doDecidimos : System.Web.UI.Page
    {
        public Aplicacion app;
        public int saveTime = 10; //guardar arboles cada x minutos
        public int cleanTime = 20; //quito arbol de memoria si no se toca en 20 minutos

        protected void Page_Load(object sender, EventArgs e)
        {
            string actn = Request["actn"];

            //verifico lista global de arboles
            if (Application["aplicacion"] == null)
            {
                Application.Lock();
                Application["aplicacion"] = new Aplicacion(Server, Request);
                Application.UnLock();
            }
            app = (Aplicacion)Application["aplicacion"];

            Tools.startupPath = Server.MapPath("");

            try
            {
                //limpio flores caducadas periodicamente
                verifyFloresCaducadas();

                //proceso peticiones
                Grupo grupo;
                Arbol a;
                string ret = "";
                string msg;

                if (actn != null)
                {
                    switch (actn.ToLower())
                    {
                        case "docomentar":
                            //devuelvo las propuestas de toda la rama
                            Response.Write(doComentar(int.Parse(Request["id"]), Request["grupo"], Request["email"], Request["comentario"]));
                            app.addLog("doComentar", Request.UserHostAddress, Request["grupo"], "", Request["comentario"]);
                            break;

                        //case "getpropuestasresaltadas":
                        //    //devuelvo las propuestas de toda la rama
                        //    Response.Write(getPropuestasResaltadas(int.Parse(Request["id"]), Request["grupo"]));
                        //    break;

                        //case "getpropuestas":
                        //    //devuelvo las propuestas de toda la rama
                        //    Response.Write(getPropuestas(int.Parse(Request["id"]), Request["grupo"]));
                        //    break;

                        case "htmldocumento":
                            //devuelvo las propuestas de toda la rama
                            Response.Write(HTMLDocumento(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "htmlpropuesta":
                            //devuelvo las propuestas de toda la rama
                            Response.Write(HTMLPropuesta(int.Parse(Request["id"]), Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "getarbolpersonal":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                //devuelvo el arbolPersonal
                                grupo.ts = DateTime.Now;
                                ret = Tools.toJson(grupo.arbol.getArbolPersonal(Request["email"]));
                            }
                            Response.Write(ret);
                            break;

                        case "variante":
                            Response.Write(doVariante(int.Parse(Request["id"]), Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "prevista":
                            Response.Write(doPrevista(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"]), Request));
                            break;

                        case "revisar":
                            Response.Write(doRevisar(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "proponer":
                            Response.Write(doProponer(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"]));
                            app.addLog("proponer", Request.UserHostAddress, Request["grupo"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "seguimiento":
                            Response.Write(doSeguimiento(int.Parse(Request["docID"]), Request["grupo"], int.Parse(Request["width"])));
                            break;

                        case "toggleflor":
                            Response.Write(doToggleFlor(Request["email"], int.Parse(Request["id"]), float.Parse(Request["x"]), Request["grupo"]));
                            app.addLog("toggleFlor", Request.UserHostAddress, Request["grupo"], Request["email"], "Cambio de voto");
                            break;

                        case "updatearbol":
                            a = updateArbol(Request["grupo"], int.Parse(Request["cantidadFlores"]), float.Parse(Request["minSiPc"]), float.Parse(Request["maxNoPc"]));
                            Response.Write("Arbol actualizado");
                            break;

                        //case "download":
                        //    Response.ContentType = "application/force-download";
                        //    grupo = app.getGrupo(Request["grupo"]);
                        //    a = grupo.arbol;
                        //    lock (a)
                        //    {
                        //        Nodo n = a.getNodo(int.Parse(Request["id"]));
                        //        Response.AddHeader("Content-Disposition", "Attachment;filename=" + a.nombre + "_" + n.nombre + ".txt");
                        //    }
                        //    Response.Write(download(int.Parse(Request["id"]), Request["grupo"]));
                        //    app.addLog("download", Request.UserHostAddress, Request["grupo"], "", "nodo=" + Request["id"]);
                        //    break;

                        case "simulacionlive":
                            string separador = (0.0f).ToString("0.0").Substring(1,1);
                            float coopProb = float.Parse(Request["coopProb"].Replace(".",separador));
                            string x = Request["x"];

                            //guardo las coordenadas x
                            if (x != "")
                            {
                                grupo = app.getGrupo(Request["grupo"]);
                                lock (grupo)
                                {
                                    a = grupo.arbol;
                                    foreach (string s in x.Split(','))
                                    {
                                        Nodo n = a.getNodo(int.Parse(s.Split('=')[0]));
                                        n.x = float.Parse(s.Split('=')[1]);
                                    }
                                }
                            }

                            //live
                            bool consensoAlcanzado = false;
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                for (int pasos = 0; pasos < 10 && !consensoAlcanzado; pasos++)
                                    consensoAlcanzado = consensoAlcanzado || doSimulacionLive(grupo, coopProb);
                            }
                            Response.Write("{\"stop\": " + (consensoAlcanzado ? "true" : "false") + ", \"arbolPersonal\":" + Tools.toJson(grupo.arbol.getArbolPersonal("Prueba")) + "}");
                            break;

                        case "crearsimulacion":
                            Grupo g = new Grupo();
                            g.nombre = getSimName();
                            g.path = Server.MapPath("grupos/" + g.nombre);
                            g.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
                            g.objetivo = "simulacion";

                            a = new Arbol();
                            a.nombre = g.nombre;
                            a.simulacion = true;
                            a.raiz = new Nodo();
                            a.raiz.nombre = "Sim";
                            a.grupo = g;
                            g.arbol = a;

                            lock (app.grupos)
                            {                               
                                app.grupos.Add(g);
                            }
                            a.minSiPc = 60;
                            a.maxNoPc = 50;

                            //usuario de prueba
                            Usuario u1 = new Usuario();
                            u1.nombre = "Prueba";
                            u1.email = "Prueba";
                            g.usuarios.Add(u1);
                            a.lastSimUsuario = u1;

                            //escribo respuesta
                            List<Type> tipos = new List<Type>();
                            foreach (Modelo m in Modelo.getModelos())
                            {
                                tipos.Add(m.GetType());
                            }
                            Response.Write("{\"arbolPersonal\": " + Tools.toJson(a.getArbolPersonal("Prueba")) + ",\"modelos\":" + Tools.toJson(a.modelos, tipos) + "}");
                            app.addLog("crearSimulacion", Request.UserHostAddress, "", "", "Simulacion creada");
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

        private string getSimName()
        {
            int index = 1;
            while (System.IO.Directory.Exists(Server.MapPath("grupos/__Sim" + index)))
                index +=1;
            return "__Sim" + index;
        }

        private string getClientBrowser(HttpRequest req)
        {
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                string s = "Type=" + browser.Type + ";"
                    + "Name=" + browser.Browser + ";"
                    + "Version=" + browser.Version + ";"
                    //+ "Major Version = " + browser.MajorVersion + "\n"
                    //+ "Minor Version = " + browser.MinorVersion + "\n"
                    + "Platform=" + browser.Platform + ";"
                    //+ "Is Beta = " + browser.Beta + ";"
                    //+ "Is Crawler = " + browser.Crawler + "\n"
                    //+ "Is AOL = " + browser.AOL + "\n"
                    //+ "Is Win16 = " + browser.Win16 + "\n"
                    //+ "Is Win32 = " + browser.Win32 + "\n"
                    //+ "Frames = " + browser.Frames + "\n"
                    //+ "Tables = " + browser.Tables + "\n"
                    + "Cookies=" + browser.Cookies;
                    //+ "VBScript = " + browser.VBScript + "\n"
                    //+ "JavaScript=" + browser.EcmaScriptVersion.ToString() + ";"
                    //+ "Java Applets = " + browser.JavaApplets + "\n"
                    //+ "ActiveX Controls = " + browser.ActiveXControls + "\n"
                    //+ "JSVersion=" + browser["JavaScriptVersion"];

                return s;
            }
        }

        //public string download(int id, string arbol)
        //{
        //    string ret = "";
        //    Grupo grupo = app.getGrupo(arbol);
        //    Arbol a = grupo.arbol;
        //    lock (a)
        //    {
        //        grupo.ts = DateTime.Now;
        //        List<Nodo> pathn = a.getPath(id);
        //        for(int i = pathn.Count - 1; i >=0 ; i--)
        //        {
        //            Nodo n = pathn[i];
        //            Propuesta op = a.getPropuesta(n.id);
        //            if (op != null)
        //            {
        //                if (ret == "")
        //                {
        //                    ModeloDocumento m = a.getModelo(op.modeloID);
        //                    ret += "Fecha: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\r\n";
        //                    ret += "Arbol: " + a.nombre + "\r\n";
        //                    ret += "Creado: " + a.born.ToShortDateString() + " " + a.born.ToShortTimeString() + "\r\n";
        //                    ret += "Objetivo: " + a.objetivo + "\r\n";
        //                    ret += "Consensos alcanzados: " + a.logDocumentos.Count + "\r\n";
        //                    ret += "Usuarios: " + a.usuarios.Count + "\r\n";
        //                    ret += "Admin: " + a.getAdmin().nombre + " (" + a.getAdmin().email + ")\r\n\r\n";

        //                    ret += "Modelo documento: " + m.nombre + "\r\n";
        //                    ret += "Titulo: " + op.titulo + "\r\n\r\n";
        //                }
        //                ret += "--------------------------------------------------------------- Nivel del arbol " + (pathn.Count - i - 1) + "\r\n";
        //                ret += "Fecha: " + op.ts.ToShortDateString() + " " + op.ts.ToShortTimeString() + "\r\n";

        //                foreach (TextoTema tt in op.textos)
        //                {
        //                    ret += "Titulo: " + tt.titulo + "\r\n";
        //                    ret += tt.texto.Replace("\n", "\r\n").Replace("&quot;", "\"") + "\r\n\r\n";
        //                }
        //                ret += "\r\n";
        //                ret += "\r\n";
        //            }
        //        }
        //   }
        //    return ret;
        //}

        public bool doSimulacionLive(Grupo g, float coopProb)
        {
            bool ret = false;
            lock (g)
            {
                Arbol a = g.arbol;
                float action = coopProb + (float)a.rnd.NextDouble() - 0.5f;

                if (action > 3f/4f) {
                    //cooperacion, muevo un voto del menor al mayor
                    Nodo mayor = a.getMayorAgregar(0);
                    Nodo menor = a.getMenorQuitar(mayor.id);

                    if (mayor != menor && menor.flores > 0 && !mayor.consensoAlcanzado)
                    {
                        Usuario u = a.quitarFlor(menor);
                        try { a.asignarflor(u, mayor); } catch {}
                        if (mayor.consensoAlcanzado) ret = true;
                    }
                }
                else if (action > 2f/4f) {
                    //voto a minorias, muevo un voto del mayor al menor
                    var mayor = a.getMayorQuitar(0);
                    var menor = a.getMenorAgregar(mayor.id);

                    if (mayor != menor && mayor.flores > 0 && !menor.consensoAlcanzado)
                    {
                        Usuario u = a.quitarFlor(mayor);
                        try { a.asignarflor(u, menor); } catch {}
                        if (menor.consensoAlcanzado) ret = true;
                    }
                }
                else if (action > 1f / 4f)
                {
                    //quito alguna flor
                    var mayor = a.getMayorQuitar(0);
                    if (mayor.flores > 0)
                    {
                        a.quitarFlor(mayor);
                    }
                }
                else
                {
                    //creo una rama nueva
                    //seleccion nodo al azar
                    List<Nodo> nodes = a.toList();
                    Nodo selected = a.rndElement(nodes);

                    //agrego nuevo nodo
                    if (!selected.consensoAlcanzado && selected.nivel < 5)
                    {
                        //agrego nodo
                        //creo texto segun nivel y modelo de documento
                        Modelo m = Modelo.getModelo("nabu.modelos.Accion");  //modelo de simulacion (Accion)
                        Propuesta prop = new Propuesta();
                        prop.email = g.usuarios[0].email;
                        prop.modeloID = m.id;
                        prop.nivel = selected.nivel + 1;  //esta propuesta es para el hijo que voy a crear
                        prop.nodoID = selected.id;
                        prop.titulo = "Documento simulado";

                        //lleno datos de prueba
                        foreach (Variable v in m.getVariables())
                        {
                            if (v.id == "s.etiqueta")
                                prop.bag.Add("s.etiqueta", "Sim");
                            else if (v.id == "s.titulo")
                                prop.bag.Add("s.titulo", "Documento simulado");
                            else
                                prop.bag.Add(v.id, "Simulacion!!!");
                        }

                        //me aseguro que el usuario tenga flores o agrego otro
                        if (a.lastSimUsuario.flores.Count < 5)
                            a.lastSimUsuario.flores.Add(new Flor());
                        else
                        {
                            //busco un usuario con flores
                            a.lastSimUsuario = a.getUsuarioConFloresDisponibles();
                        }
                        if (a.lastSimUsuario == null)
                        {
                            Usuario u = new Usuario();
                            u.nombre = "Sim" + g.usuarios.Count + 1;
                            u.email = "Sim" + g.usuarios.Count + 1;
                            u.flores.Add(new Flor());
                            g.usuarios.Add(u);
                            a.lastSimUsuario = u;
                        }

                        prop.email = a.lastSimUsuario.email;
                        Nodo nuevo = a.addNodo(selected, prop);

                        if (selected.nivel == 1)
                        {
                            selected.nombre = "Sim" + selected.id;
                        }
                    }
                }
            }
            return ret;
        }

        void verifyFloresCaducadas()
        {
            DateTime lastVerifyFloresCaducadas;
            Application.Lock();
            if (Application["lastVerifyFloresCaducadas"] == null)
                lastVerifyFloresCaducadas = DateTime.MinValue;
            else
                lastVerifyFloresCaducadas = (DateTime)Application["lastVerifyFloresCaducadas"];
            Application["lastVerifyFloresCaducadas"] = lastVerifyFloresCaducadas;
            Application.UnLock();


            if (DateTime.Now.Subtract(lastVerifyFloresCaducadas).TotalHours > 24)
            {
                app.addLog("verifyFloresCaducadas", Request.UserHostAddress, "", "", "verifyFloresCaducadas");

                lock (app.grupos)
                {
                    foreach (Grupo g in app.grupos)
                    {
                        Arbol a = g.arbol;
                        foreach (Usuario u in g.usuarios)
                        {                           
                            //verifico caducadas
                            foreach (Flor f in u.flores)
                            {
                                if (f.id != 0 && DateTime.Now.Subtract(f.born).TotalDays > 60)
                                {
                                    Nodo n = a.getNodo(f.id);
                                    if (n != null)
                                    {
                                        a.quitarFlor(n, u);
                                        app.addLog("verifyFloresCaducadas", Request.UserHostAddress, g.nombre, u.email, "Flor caducada. Nacida en: " + f.born);
                                    }                                    
                                }
                            }
                        }
                    }
                }
                Application.Lock();
                Application["lastVerifyFloresCaducadas"] = DateTime.Now;
                Application.UnLock();
            }
        }



        string HTMLDocumento(int id, string modeloID, string grupo, string email, int width)
        {
            string ret = "";
            List<Propuesta> l = new List<Propuesta>();
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                foreach(Nodo n in a.getPath(id))
                {
                    Propuesta op = a.getPropuesta(n); //comparo textox con hermanos y resalto palarbas nuevas
                    if (op != null)
                    {
                        l.Add(op);
                    }
                }
                Modelo m = Modelo.getModelo(modeloID);
                ret = m.toHTML(l, g, email, width, Modelo.eModo.editar); //las propuesta debe ir en orden de nivel
            }
            return ret;
        }

        string HTMLPropuesta(int id, string grupo, string email, int width)
        {
            string ret = "";
            List<Propuesta> l = new List<Propuesta>();
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                Propuesta p = a.getPropuesta(id);
                Modelo m = Modelo.getModelo(p.modeloID);
                ret = m.toHTML(p, g, email, width, Modelo.eModo.ver);
            }
            return ret;
        }

        string doComentar(int id, string grupo, string email, string comentario)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                Propuesta p = a.getPropuesta(id);
                if (comentario != "")
                {
                    Comentario c = new Comentario();
                    c.email = email;
                    c.texto = Server.HtmlEncode(comentario);
                    p.comentarios.Add(c);
                }
                //retorno el nuevo html de todos los comentarios de ese nodo
                Modelo m = Modelo.getModelo(p.modeloID);
                ret = m.toHTMLComentarios(p.nivel, p, g, email, 250, true);
            }
            
            return ret;
        }

        string doToggleFlor(string email, int id, float x, string grupo)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                //verifico que el usuario tiene una flor en ese nodo
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                Usuario u = g.getUsuario(email);
                Flor f = u.getFlor(id);
                List<Nodo> pathn = a.getPath(id);
                if (pathn == null)
                    throw new appException("Seleccione un nodo");
                else
                {
                    Nodo n = pathn[0];

                    if (f == null)
                    {
                        //no tiene flor en el nodo, la agrego
                        a.asignarflor(u, n);

                        n.x = x;
                        
                        //devuelvo el arbolPersonal
                        ret = Tools.toJson(a.getArbolPersonal(email));
                    }
                    else
                    {
                        //tiene flor, la quito
                        a.quitarFlor(n, u);

                        //devuelvo el arbolPersonal
                        ret = Tools.toJson(a.getArbolPersonal(email));
                    }
                }
            }
            return ret;
        }

        string doVariante(int id, string grupo, string email, int width)
        {
            string ret = "";
            List<Propuesta> props = new List<Propuesta>();
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                //preparo propuestas de nodos ancestros
                Arbol a = g.arbol;
                List<Nodo> path = a.getPath(id);
                Modelo m = Modelo.getModelo(path[0].modeloID);
                g.ts = DateTime.Now;
                foreach (Nodo n in path)
                {
                    Propuesta op = a.getPropuesta(n); //comparo textox con hermanos y resalto palarbas nuevas
                    if (n.nivel > 0 && op != null)
                    {
                        props.Add(op);
                    }
                }

                //marco la propuesta de id como prevista
                props[0] = props[0].clone(); //nueva propuesta
                props[0].email = email;
                props[0].nodoID = 0;  //nodoID=0 determina propuesta prevista, porque esta propuesta aun no tiene nodo       

                //muestro documento
                ret = m.toHTML(props, g, email, width, Modelo.eModo.editar); //las propuesta debe ir en orden de nivel
            }
            return ret;
        }

        string doSeguimiento(int docID, string grupo, int width)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                LogDocumento ld = g.arbol.getLogDocumento(docID);
                Documento doc = Documento.load(g.path + "\\documentos\\" + ld.fname + ".json");
                ret = doc.toHTMLSeguimiento();
            }
            return ret;
        }

        string doPrevista(int id, string modeloID, string grupo, string email, int width, HttpRequest req)
        {
            string ret = "";
            List<Propuesta> props = new List<Propuesta>();
            Modelo m = Modelo.getModelo(modeloID);
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                //preparo propuestas de nodos ancestros
                Arbol a = g.arbol;
                List<Nodo> path = a.getPath(id);
                g.ts = DateTime.Now;
                foreach (Nodo n in path)
                {
                    Propuesta op = a.getPropuesta(n); //comparo textox con hermanos y resalto palarbas nuevas
                    if (n.nivel > 0 && op != null)
                    {
                        props.Add(op);
                    }
                }

                //agrego las propuestas de prevista
                List<Propuesta> previstaProps = new List<Propuesta>();
                foreach (string reqID in req.Form.AllKeys)
                {
                    if (reqID != null && m.isVariable(reqID) && req[reqID] != "")
                    {
                        //este valor me lo guardo en la prpuesta para ese nivel
                        Variable v = m.getVariable(reqID);
                        bool found = false;
                        foreach (Propuesta p in previstaProps)
                        {
                            if (v.nivel == p.nivel)
                            {
                                p.bag.Add(reqID, m.parse(reqID, req[reqID]));
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            Propuesta p = new Propuesta();
                            p.email = email;
                            p.modeloID = m.id;
                            p.nivel = v.nivel;
                            //p.nodoID = id; nodoID=0 determina propuesta prevista, porque esta propuesta aun no tiene nodo
                            p.bag.Add(reqID, m.parse(reqID, Server.HtmlEncode(req[reqID])));

                            previstaProps.Add(p);
                            props.Add(p);
                        }
                    }
                }

                //genro prevista
                ret = m.toHTML(props, g, email, width, Modelo.eModo.prevista ); //las propuesta debe ir en orden de nivel

                //guarpo prevista para poder crearla luego
                if (props.Count > 0)
                {
                    Usuario u = g.getUsuario(email);
                    Prevista prev = new Prevista();
                    prev.etiqueta = m.etiqueta;
                    prev.titulo = m.titulo;
                    prev.propuestas = previstaProps;
                    u.prevista = prev;
                } //else no ha escrito nada nuevo
            }
            return ret;
        }

        string doRevisar(int id, string modeloID, string grupo, string email, int width)
        {
            string ret = "";
            List<Propuesta> props = new List<Propuesta>();
            Modelo m = Modelo.getModelo(modeloID);
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                //preparo propuestas de nodos ancestros
                Arbol a = g.arbol;
                List<Nodo> path = a.getPath(id);
                g.ts = DateTime.Now;
                foreach (Nodo n in path)
                {
                    Propuesta op = a.getPropuesta(n); //comparo textox con hermanos y resalto palarbas nuevas
                    if (n.nivel > 0 && op != null)
                    {
                        props.Add(op);
                    }
                }

                //agrego las propuestas de prevista
                Usuario u = g.getUsuario(email);
                foreach (Propuesta p in u.prevista.propuestas)
                {
                    props.Add(p);
                }

                //genro revision
                ret = m.toHTML(props, g, email, width, Modelo.eModo.revisar); //las propuesta debe ir en orden de nivel
            }
            return ret;
        }

        string doProponer(int id, string modeloID, string grupo, string email)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                Nodo padre = a.getNodo(id);
                if (padre.consensoAlcanzado)
                    throw new appException("Este debate ya ha alcanzado el consenso");
                else
                {
                    //agrego propuestas de la prevista guardada
                    Usuario u = g.getUsuario(email);
                    foreach (Propuesta p in u.prevista.propuestas)
                    {
                        if (padre.nivel == 0)
                            p.etiqueta = u.prevista.etiqueta;
                        else
                            p.etiqueta = a.getEtiqueta(u.prevista.etiqueta, padre);

                        p.titulo = u.prevista.titulo;

                        padre = a.addNodo(padre, p);
                    }
                }
                //devuelvo el arbolPersonal
                ret = Tools.toJson(a.getArbolPersonal(email, padre.id));
            }

            app.saveGrupos();

            return ret;
        }

        void verifyCantidadFlores()
        {
            //verifico cantidad de flores
            lock (app.grupos)
            {
                foreach (Grupo g in app.grupos)
                {
                    lock (g)
                    {
                        Arbol a = g.arbol;
                        foreach (Usuario u in g.usuarios)
                        {
                            //agrego si faltan
                            for (int q = u.flores.Count; q < a.cantidadFlores; q++)
                                u.flores.Add(new Flor());

                            //quito si sobran
                            while (u.flores.Count > a.cantidadFlores)
                            {
                                Flor f = u.flores[u.flores.Count - 1]; //quito desde la ultima que quizas no esta en uso
                                if (f.id != 0)
                                {
                                    a.quitarFlor(a.getNodo(f.id), u);
                                }
                                u.flores.RemoveAt(u.flores.Count - 1);
                            }
                        }
                    }
                }
            }
        }

        Arbol updateArbol(string grupo, int cantidadFlores, float minSiPc, float maxNoPc)
        {
            if (minSiPc > 100)
                throw new appException("Mínimos usuarios implicados debe estar entre 50 y 100");
            if (maxNoPc > 50)
                throw new appException("Máximos usuarios negados debe estar entre 0 y 50");
            if (cantidadFlores > 20 || cantidadFlores < 1)
                throw new appException("cantidad de flores debe estar entre 1 y 20");


            Grupo g = app.getGrupo(grupo);
            Arbol a;

            lock (g)
            {
                a = g.arbol;
                g.ts = DateTime.Now;
                a.cantidadFlores = cantidadFlores;
                a.minSiPc = minSiPc;
                a.maxNoPc = maxNoPc;
            }

            //verifico cantidd de flores de todos los arboles
            verifyCantidadFlores();

            //guardo a disco
            app.saveGrupos();

            return a;
        }
    }
}