using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//atiendo peticiones de arbol

namespace nabu
{
    public partial class doArbol : System.Web.UI.Page
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
                Application["aplicacion"] = new Aplicacion(Server);
                Application.UnLock();
            }
            app = (Aplicacion)Application["aplicacion"];

            try
            {
                //guardo lista de arboles periodicamente
                verifySave();

                //limpio flores caducadas periodicamente
                verifyFloresCaducadas();

                //proceso peticiones
                Arbol a;
                string ret = "";
                string msg;

                if (actn != null)
                {
                    switch (actn.ToLower())
                    {
                        case "cambiarclave":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                Usuario u = a.getUsuario(Request["email"]);
                                if (u == null)
                                    throw new appException("El usuario no existe");
                                else
                                {
                                    if (u.clave != Request["claveActual"])
                                        throw new appException("La clave actual no corresponde");
                                    else
                                    {
                                        if (Request["nuevaClave"] == "")
                                            throw new appException("La clave nueva no puede ser vacia");
                                        else
                                            u.clave = Request["nuevaClave"]; //no devuelvo mensaje
                                    }
                                }
                            }
                            break;

                        case "getusuarios":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                Response.Write(Tools.toJson(a.usuarios));
                            }
                            break;

                        case "getlistausuariosclaves":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                foreach (Usuario u in a.usuarios)
                                    ret += u.email + " - Clave:" + u.clave + " - LastLogin:" + u.lastLogin.ToShortDateString() + " " + u.lastLogin.ToShortTimeString() + "<br>";
                                Response.Write(ret);
                            }
                            break;

                        case "docomentar":
                            //devuelvo las propuestas de toda la rama
                            doComentar(int.Parse(Request["id"]), Request["arbol"], Request["comentario"]);
                            app.addLog("doComentar", Request.UserHostAddress, Request["arbol"], "", Request["comentario"]);
                            break;

                        case "getpropuestas":
                            //devuelvo las propuestas de toda la rama
                            Response.Write(getPropuestas(int.Parse(Request["id"]), Request["arbol"]));
                            break;

                        case "exception":
                            app.addLog("client exception", Request.UserHostAddress, Request["arbol"], Request["email"], Request["flag"] + " -- " + Request["message"] + " -- " + Request["stack"]);
                            break;

                        case "sendmailwelcome":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                string url = Request.UrlReferrer.OriginalString.Substring(0, Request.UrlReferrer.OriginalString.LastIndexOf("/"));
                                Usuario u = a.getUsuario(Request["email"]);
                                msg = Tools.sendMailWelcome(Request["arbol"], Request["email"], u.clave, url, Server.MapPath("mails"));
                            }
                            Response.Write(msg);
                            break;

                        case "sendmailalta":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                Usuario u = a.getAdmin();
                                msg = Tools.sendMailAlta(Request["arbol"], u.email, Request["nombre"], Request["email"], Server.MapPath("mails"));
                            }
                            Response.Write(msg);
                            break;

                        case "getconfig":
                            //plataformas cliente reconocidas
                            //Type=Chrome45;Name=Chrome;Version=45.0;Platform=WinNT;Cookies=True
                            //Type=Safari5;Name=Safari;Version=5.1;Platform=Unknown;Cookies=True;width=1300;height=854
                            //Type=InternetExplorer11;Name=InternetExplorer;Version=11.0;Platform=WinNT;Cookies=True;width=784;height=537


                            Config cnf = getConfig();
                            cnf.browser = Request.Browser.Browser;
                            cnf.type = Request.Browser.Type;
                            cnf.version = Request.Browser.Version;
                            cnf.width = int.Parse(Request["width"]);
                            cnf.height = int.Parse(Request["height"]);

                            ret = Tools.toJson(cnf);
                            Response.Write(ret);
                            app.addLog("getConfig", Request.UserHostAddress, "", "", getClientBrowser(Request) + ";width=" + Request["width"] + ";height=" + Request["height"]);
                            break;

                        case "desactivarmodelo":
                            int modeloID = int.Parse(Request["modeloID"]);
                            ModeloDocumento m;
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                m = a.getModelo(modeloID);
                                m.activo = false;
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Modelo desactivado\", \"modelos\":" + ret + "}");
                            app.addLog("desactivarModelo", Request.UserHostAddress, Request["arbol"], "", m.nombre);
                            break;

                        case "borrartema":
                            int modeloID4 = int.Parse(Request["modeloID"]);
                            int indexSeccion = int.Parse(Request["indexSeccion"]);
                            int indexTema = int.Parse(Request["indexTema"]);
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                m = a.getModelo(modeloID4);
                                if (m.enUso)
                                    throw new appException("El modelo de documento esta en uso, no se puede modificar");
                                else
                                {
                                    m.secciones[indexSeccion].temas.RemoveAt(indexTema);
                                }
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Tema borrado\", \"modelos\":" + ret + "}");
                            break;

                        case "newmodelo":
                            string nombre = Request["nombre"];
                            if (nombre == "")
                                throw new appException("El nombre no puede ser vacio");

                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;

                                //verifico nombre repetido
                                foreach (ModeloDocumento m1 in a.modelosDocumento)
                                    if (m1.nombre == nombre)
                                        throw new appException("Nombre repetido");

                                //creo
                                m = new ModeloDocumento();
                                m.nombre = nombre;

                                m.activo = false;
                                //obtengo nuevo id
                                int newID = 0;
                                foreach (ModeloDocumento m2 in a.modelosDocumento)
                                    if (newID <= m2.id)
                                        newID = m2.id + 1;
                                m.id = newID;
                                //creo 5 secciones vacias
                                m.secciones.Add(new Seccion());
                                m.secciones.Add(new Seccion());
                                m.secciones.Add(new Seccion());
                                m.secciones.Add(new Seccion());
                                m.secciones.Add(new Seccion());
                                a.modelosDocumento.Insert(0, m); //para que aparezca primero en la lista en el cliente
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Modelo creado\", \"modelos\":" + ret + "}");
                            app.addLog("newModelo", Request.UserHostAddress, Request["arbol"], "", m.nombre);
                            break;

                        case "activarmodelo":
                            int modeloID2 = int.Parse(Request["modeloID"]);
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                m = a.getModelo(modeloID2);
                                //valido
                                bool completo = true;
                                foreach (Seccion s in m.secciones)
                                    if (s.temas.Count == 0)
                                        completo = false;
                                if (!completo)
                                    throw new appException("No se puede activar un modelo de documento con secciones vacias");

                                m.activo = true;
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Modelo activado\", \"modelos\":" + ret + "}");
                            break;

                        case "borrarmodelo":
                            int modeloID3 = int.Parse(Request["modeloID"]);
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                m = a.getModelo(modeloID3);
                                if (m.enUso)
                                    throw new appException("No se puede borrar un modelo de documento en uso");
                                if (m.activo)
                                    throw new appException("No se puede borrar un modelo de documento activo");
                                a.modelosDocumento.Remove(m);
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Modelo borrado\", \"modelos\":" + ret + "}");
                            break;

                        case "creartemamodelo":
                            string titulo2 = Server.HtmlEncode(Request["titulo"]);
                            string tip = Server.HtmlEncode(Request["tip"]);
                            int maxLen = int.Parse(Request["maxLen"]);
                            int indexSeccion2 = int.Parse(Request["indexSeccion"]);

                            if (titulo2 == "")
                                throw new appException("Titulo no puede ser vac&iacute;o");
                            if (maxLen > 10000 || maxLen < 500)
                                throw new appException("Longitud maxima debe estar entre 500 y 10000");

                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                m = a.getModelo(int.Parse(Request["modeloID"]));
                                m.secciones[indexSeccion2].temas.Add(new Tema(titulo2,tip,maxLen));
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"Tema creado\", \"modelos\":" + ret + "}");
                            break;

                        case "getarbolpersonal":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                //devuelvo el arbolPersonal
                                a.ts = DateTime.Now;
                                ret = Tools.toJson(a.getArbolPersonal(Request["email"]));
                            }
                            Response.Write(ret);
                            break;

                        case "proponer":
                            Response.Write(doProponer(Request["email"], int.Parse(Request["id"]), Request["nombre"], Request["propuestas"], Request["arbol"]));
                            app.addLog("proponer", Request.UserHostAddress, Request["arbol"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "toggleflor":
                            Response.Write(doToggleFlor(Request["email"], int.Parse(Request["id"]), float.Parse(Request["x"]), Request["arbol"]));
                            app.addLog("toggleFlor", Request.UserHostAddress, Request["arbol"], Request["email"], "Cambio de voto");
                            break;

                        case "newarbol":
                            a = newArbol(Request["arbol"], Request["objetivo"], Request["URLEstatuto"], Request["nombreAdmin"], Request["email"], Request["clave"]);
                            lock (app.arboles)
                            {
                                app.arboles.Add(a);
                                //guardo a disco
                                saveArboles();
                            }
                            Response.Write("Arbol creado");
                            app.addLog("newArbol", Request.UserHostAddress, Request["arbol"], Request["email"], Request["objetivo"]);
                            break;

                        case "updatearbol":
                            a = updateArbol(Request["arbol"], Server.HtmlEncode(Request["objetivo"]), Request["URLEstatuto"], int.Parse(Request["cantidadFlores"]), float.Parse(Request["minSiPc"]), float.Parse(Request["maxNoPc"]));
                            Response.Write("Arbol actualizado");
                            break;

                        case "newusuario":
                            Usuario u2 = newUsuario(Request["nombre"], Request["email"], Request["clave"], Request["arbol"]);
                            Response.Write("Usuario [" + u2.email + "] creado");
                            app.addLog("newUsuario", Request.UserHostAddress, Request["arbol"], Request["email"], Request["nombre"]);
                            break;

                        case "removeusuario":
                            Usuario u3 = removeUsuario(Request["email"], Request["arbol"]);
                            Response.Write("Usuario [" + u3.email + "] borrado");
                            break;

                        case "getarboles":
                            Response.Write(getArboles());
                            break;

                        case "login":
                            Response.Write(doLogin(Request["email"], Request["clave"], Request["arbol"]));
                            break;

                        case "download":
                            Response.ContentType = "application/force-download";
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                Nodo n = a.getNodo(int.Parse(Request["id"]));
                                Response.AddHeader("Content-Disposition", "Attachment;filename=" + a.nombre + "_" + n.nombre + ".txt");
                            }
                            Response.Write(download(int.Parse(Request["id"]), Request["arbol"]));
                            app.addLog("download", Request.UserHostAddress, Request["arbol"], "", "nodo=" + Request["id"]);
                            break;

                        case "simulacionlive":
                            string separador = (0.0f).ToString("0.0").Substring(1,1);
                            float coopProb = float.Parse(Request["coopProb"].Replace(".",separador));
                            string x = Request["x"];

                            //guardo las coordenadas x
                            if (x != "")
                            {
                                a = getArbol(Request["arbol"]);
                                lock (a)
                                {
                                    foreach (string s in x.Split(','))
                                    {
                                        Nodo n = a.getNodo(int.Parse(s.Split('=')[0]));
                                        n.x = float.Parse(s.Split('=')[1]);
                                    }
                                }
                            }

                            //live
                            doSimulacionLive(Request["arbol"], coopProb);
                            doSimulacionLive(Request["arbol"], coopProb);
                            doSimulacionLive(Request["arbol"], coopProb);
                            doSimulacionLive(Request["arbol"], coopProb);
                            doSimulacionLive(Request["arbol"], coopProb);

                            Response.Write(doSimulacionLive(Request["arbol"], coopProb));
                            break;

                        case "getmodelosdocumento":
                            a = getArbol(Request["arbol"]);
                            lock (a)
                            {
                                a.ts = DateTime.Now;
                                a.actualizarModelosEnUso();
                                //envio
                                ret = Tools.toJson(a.modelosDocumento);
                            }
                            Response.Write("{\"msg\":\"\", \"modelos\":" + ret + "}");
                            break;

                        case "crearsimulacion":
                            a = new Arbol();
                            int index =1;
                            a.simulacion = true;
                            a.objetivo = "simulacion";
                            a.raiz = new Nodo();
                            a.raiz.nombre = "Sim";
                            a.setModelosDocumentoDefault();
                            a.path = Server.MapPath("cooperativas/" + a.nombre);

                            a.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
                            lock (app.arboles)
                            {
                                foreach (Arbol a1 in app.arboles)
                                {
                                    if (a.simulacion)
                                        index+=1;
                                }
                                app.arboles.Add(a);
                            }
                            a.nombre = "Sim" + index;
                            a.minSiPc = 60;
                            a.maxNoPc = 50;

                            //usuario de prueba
                            Usuario u1 = new Usuario();
                            u1.nombre = "Prueba";
                            u1.email = "Prueba";
                            a.usuarios.Add(u1);
                            a.lastSimUsuario = u1;

                            Response.Write("{\"arbolPersonal\": " + Tools.toJson(a.getArbolPersonal("Prueba")) + ",\"modelos\":" + Tools.toJson(a.modelosDocumento) + "}");
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

        public string download(int id, string arbol)
        {
            string ret = "";
            Arbol a = getArbol(arbol);
            lock (a)
            {
                a.ts = DateTime.Now;
                List<Nodo> pathn = a.getPath(id);
                for(int i = pathn.Count - 1; i >=0 ; i--)
                {
                    Nodo n = pathn[i];
                    Propuesta op = a.getPropuesta(n.id);
                    if (op != null)
                    {
                        if (ret == "")
                        {
                            ModeloDocumento m = a.getModelo(op.modeloID);
                            ret += "Fecha: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\r\n";
                            ret += "Arbol: " + a.nombre + "\r\n";
                            ret += "Creado: " + a.born.ToShortDateString() + " " + a.born.ToShortTimeString() + "\r\n";
                            ret += "Objetivo: " + a.objetivo + "\r\n";
                            ret += "Consensos alcanzados: " + a.logDocumentos.Count + "\r\n";
                            ret += "Usuarios: " + a.usuarios.Count + "\r\n";
                            ret += "Admin: " + a.getAdmin().nombre + " (" + a.getAdmin().email + ")\r\n\r\n";

                            ret += "Modelo documento: " + m.nombre + "\r\n";
                            ret += "Titulo: " + op.titulo + "\r\n\r\n";
                        }
                        ret += "--------------------------------------------------------------- Nivel del arbol " + (pathn.Count - i - 1) + "\r\n";
                        ret += "Fecha: " + op.ts.ToShortDateString() + " " + op.ts.ToShortTimeString() + "\r\n";

                        foreach (TextoTema tt in op.textos)
                        {
                            ret += "Titulo: " + tt.titulo + "\r\n";
                            ret += tt.texto.Replace("\n","\r\n").Replace("&quot;","\"") + "\r\n\r\n";
                        }
                        ret += "\r\n";
                        ret += "\r\n";
                    }
                }
           }
            return ret;
        }

        public string doSimulacionLive(string arbol, float coopProb)
        {
            string ret = "";
            Arbol a = getArbol(arbol);
            lock (a)
            {
                a.ts = DateTime.Now;
                float action = coopProb + (float)a.rnd.NextDouble() - 0.5f;

                if (action > 3f/4f) {
                    //cooperacion, muevo un voto del menor al mayor
                    Nodo mayor = a.getMayorAgregar(0);
                    Nodo menor = a.getMenorQuitar(mayor.id);

                    if (mayor != menor && menor.flores > 0 && !mayor.consensoAlcanzado)
                    {
                        Usuario u = a.quitarFlor(menor);
                        try { a.asignarflor(u, mayor); } catch {}
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
                    var selected = a.rndElement(nodes);

                    //agrego nuevo nodo
                    if (!selected.consensoAlcanzado && selected.nivel < 5)
                    {
                        //agrego nodo
                        List<TextoTema> tts = new List<TextoTema>();
                        TextoTema tt = new TextoTema();
                        tt.titulo = "Prueba";
                        tt.texto = "Prueba";
                        tts.Add(tt);

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
                            u.nombre = "Sim" + a.usuarios.Count + 1;
                            u.email = "Sim" + a.usuarios.Count + 1;
                            u.flores.Add(new Flor());
                            a.usuarios.Add(u);
                            a.lastSimUsuario = u;
                        }

                        Nodo nuevo = a.addNodo(selected, a.lastSimUsuario.email, "", tts, 1);

                        if (selected.nivel == 1)
                        {
                            selected.nombre = "Sim" + selected.id;
                        }
                    }
                }

                ret = Tools.toJson(a.getArbolPersonal("Prueba"));
            }
            return ret;
        }

        void verifySave()
        {
            DateTime lastSave;
            Application.Lock();
            if (Application["lastSave"] == null)
                lastSave = DateTime.MinValue;
            else
                lastSave = (DateTime)Application["lastSave"];
            Application["lastSave"] = lastSave;
            Application.UnLock();

            if (DateTime.Now.Subtract(lastSave).TotalMinutes > saveTime)
            {
                saveArboles();

                depurarMemoria();

                Application.Lock();
                Application["lastSave"] = DateTime.Now;
                Application.UnLock();
            }
        }

        private void depurarMemoria()
        {
            //depuro arboles viejos de memoria
            lock (app.arboles)
            {
                int index = 0;
                while (index < app.arboles.Count)
                {
                    if (DateTime.Now.Subtract(app.arboles[index].ts).TotalMinutes > cleanTime)
                    {
                        //este arbol se puede quitar de memoria
                        //se asume que ya fue guardado
                        app.arboles.RemoveAt(index);
                    }
                    else
                        index += 1;
                }
            }
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
                lock (app.arboles)
                {
                    foreach (Arbol a in app.arboles)
                    {
                        foreach (Usuario u in a.usuarios)
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

        void saveArboles()
        {
            lock (app.arboles)
            {
                foreach (Arbol a in app.arboles)
                {
                    if (!a.simulacion)
                    {
                        lock (a)
                        {
                            a.save(Server.MapPath("cooperativas/" + a.nombre));
                        }
                    }
                }
            }
        }

        string getPropuestas(int id, string arbol)
        {
            string ret = "";
            List<Propuesta> l = new List<Propuesta>();
            Arbol a = getArbol(arbol);
            lock (a)
            {
                a.ts = DateTime.Now;
                List<Nodo> pathn = a.getPath(id);
                foreach(Nodo n in pathn){
                    Propuesta op = a.getPropuesta(n.id);
                    if (op != null)
                        l.Add(op);
                }
                ret = Tools.toJson(l);
            }
            return ret;
        }

        Config getConfig()
        {
            Config ret = new Config();
            ret.arbolList = getArboles();
            return ret;
        }

        void doComentar(int id, string arbol, string comentario)
        {
            if (comentario != "")
            {
                Arbol a = getArbol(arbol);
                lock (a)
                {
                    a.ts = DateTime.Now;
                    Propuesta p = a.getPropuesta(id);
                    p.comentarios.Add(Server.HtmlEncode(comentario));
                }
            }
            else
                throw new appException("Un comentario no puede ser vacio");
        }

        string doToggleFlor(string email, int id, float x, string arbol)
        {
            string ret = "";
            Arbol a = getArbol(arbol);
            lock (a)
            {
                //verifico que el usuario tiene una flor en ese nodo
                a.ts = DateTime.Now;
                Usuario u = a.getUsuario(email);
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

        string doProponer(string email, int id, string nombre, string propuestas, string arbol)
        {
            string ret = "";
            List<Propuesta> props = Tools.fromJson<List<Propuesta>>(propuestas);

            Arbol a = getArbol(arbol);
            lock (a)
            {
                //valido
                //valido que no se inserte en un debate ya consensuado
                a.ts = DateTime.Now;
                Nodo padre = a.getNodo(id);
                if (padre.consensoAlcanzado)
                    throw new appException("Este debate ya ha alcanzado el consenso");

                //inserto
                foreach (Propuesta p in props)
                {
                    bool seccionValida = false;
                    p.ts = DateTime.Now;
                    foreach (TextoTema tt in p.textos)
                    {
                        if (tt.texto != "") seccionValida = true;
                        tt.texto = Server.HtmlEncode(tt.texto);
                    }

                    if (seccionValida)
                        padre = a.addNodo(padre, email, Server.HtmlEncode(nombre), p.textos, p.modeloID);
                    else
                        break; //inserto hasta econtrar un texto vacio
                }
                //devuelvo el arbolPersonal
                ret = Tools.toJson(a.getArbolPersonal(email));
            }

            saveArboles();

            return ret;
        }

        string doLogin(string email, string clave, string arbol)
        {
            string ret = "";
            Arbol a = getArbol(arbol);
            lock (a)
            {
                a.ts = DateTime.Now;
                Usuario u = a.getUsuario(email, clave);
                if (u != null)
                {
                    //login correcto
                    //devuelvo el arbol personal con las flores de este usuario y sus modelos
                    u.lastLogin = DateTime.Now;
                    a.actualizarModelosEnUso();
                    ret = "{\"msg\":\"\", \"modelos\":" + Tools.toJson(a.modelosDocumento) + ", \"arbolPersonal\":" + Tools.toJson(a.getArbolPersonal(u.email)) + "}";
                }
                else
                {
                    app.addLog("login", Request.UserHostAddress, arbol, email, "fail!");
                    throw new appException("usuario o clave incorrectos para el arbol [" + arbol + "]");
                }
            }
            return ret;
        }

        Arbol getArbol(string nombre)
        {
            Arbol ret = null;

            lock (app.arboles)
            {
                foreach (Arbol a in app.arboles)
                {
                    if (a.nombre == nombre)
                    {
                        ret = a;
                    }
                }

                if (ret == null)
                {
                    //no existe en la lista lo busco en la carpeta y lo cargo
                    string jsonpath = Server.MapPath("cooperativas/" + nombre + "/" + nombre + ".json");
                    if (System.IO.File.Exists(jsonpath))
                    {
                        System.IO.StreamReader fs = System.IO.File.OpenText(jsonpath);
                        string s = fs.ReadToEnd();
                        fs.Close();

                        List<Type> tipos = new List<Type>();
                        tipos.Add(typeof(Arbol));
                        tipos.Add(typeof(Usuario));
                        tipos.Add(typeof(Nodo));

                        ret = Tools.fromJson<Arbol>(s, tipos);
                        ret.path = Server.MapPath("cooperativas/" + nombre);
                        ret.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));

                        app.arboles.Add(ret);
                    }
                    else
                        throw new appException("El arbol no existe");
                }
            }
           
            return ret;
        }

        void verifyCantidadFlores()
        {
            //verifico cantidad de flores
            lock (app.arboles)
            {
                foreach (Arbol a in app.arboles)
                {
                    lock (a)
                    {
                        foreach (Usuario u in a.usuarios)
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

        Arbol newArbol(string arbol, string objetivo, string URLEstatuto, string nombreAdmin, string email, string clave)
        {
            if (objetivo == "")
                throw new appException("Objetivo no puede ser vacio");
            if (arbol == "")
                throw new appException("Nombre de arbol no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");

            //veo que no exista ya
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath("cooperativas"));

            foreach (System.IO.DirectoryInfo fi in di.GetDirectories())
            {
                if (fi.Name == arbol)
                {
                    //ya existe
                    throw new appException("El arbol ya existe");
                }
            }

            //creo
            Arbol a = new Arbol();
            a.nombre = arbol;
            a.objetivo = objetivo;
            a.raiz = new Nodo();
            a.raiz.nombre = a.nombre;
            a.setModelosDocumentoDefault();
            a.path = Server.MapPath("cooperativas/" + a.nombre);
            a.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
            a.URLEstatuto = URLEstatuto;

            //admin
            Usuario u = new Usuario(a.cantidadFlores);
            u.nombre = nombreAdmin;
            u.email = email;
            u.clave = clave;
            u.isAdmin = true;
            a.usuarios.Add(u);

            return a;
        }

        Arbol updateArbol(string arbol, string objetivo, string URLEstatuto, int cantidadFlores, float minSiPc, float maxNoPc)
        {
            if (objetivo == "")
                throw new appException("Objetivo no puede ser vacio");
            if (minSiPc > 100)
                throw new appException("Mínimos usuarios implicados debe estar entre 50 y 100");
            if (maxNoPc > 50)
                throw new appException("Máximos usuarios negados debe estar entre 0 y 50");
            if (cantidadFlores > 20 || cantidadFlores < 1)
                throw new appException("cantidad de flores debe estar entre 1 y 20");

            Arbol a = getArbol(arbol);

            if (a == null)
                throw new appException("El arbol no existe");
            else
            {
                lock (a)
                {
                    a.ts = DateTime.Now;
                    a.objetivo = objetivo;
                    a.cantidadFlores = cantidadFlores;
                    a.minSiPc = minSiPc;
                    a.maxNoPc = maxNoPc;
                    a.URLEstatuto = URLEstatuto;
                }
            }

            //verifico cantidd de flores de todos los arboles
            verifyCantidadFlores();

            //guardo a disco
            saveArboles();

            return a;
        }

        Usuario removeUsuario(string email, string arbol)
        {
            Arbol a = getArbol(arbol);

            if (a == null)
                throw new appException("El arbol no existe");
            else
            {
                Usuario u;
                lock (a)
                {
                    a.ts = DateTime.Now;
                    u = a.removeUsuario(email);

                    //guardo a disco
                    saveArboles();
                }
                return u;
            }
        }

        Usuario newUsuario(string nombre, string email, string clave, string arbol)
        {
            if (arbol == "")
                throw new appException("Nombre de arbol no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");

            Arbol a = getArbol(arbol);

            if (a.getUsuario(email) != null)
                throw new appException("El usuario ya existe");

            if (a == null)
                throw new appException("El arbol no existe");
            else
            {
                Usuario u;
                lock (a)
                {
                    a.ts = DateTime.Now;
                    u = new Usuario(a.cantidadFlores);
                    u.nombre = nombre;
                    u.email = email;
                    u.clave = clave;
                    a.usuarios.Add(u);

                    //guardo a disco
                    saveArboles();
                }
                return u;
            }
        }

        List<string> getArboles()
        {
            List<string> ret = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath("cooperativas"));

            foreach (System.IO.DirectoryInfo fi in di.GetDirectories())
            {
                ret.Add(fi.Name);
            }
            return ret;
        }
    }
}