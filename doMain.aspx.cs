using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//atiendo peticiones de arbol

namespace nabu
{
    public partial class doMain : System.Web.UI.Page
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
                //guardo lista de arboles periodicamente
                verifySave();

                //proceso peticiones
                Grupo grupo;
                Arbol a;
                string ret = "";
                string msg;

                if (actn != null)
                {
                    switch (actn.ToLower())
                    {
                        case "cambiarclave":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                grupo.ts = DateTime.Now;
                                Usuario u = grupo.getUsuario(Request["email"]);
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

                        case "resumen":
                            Response.Write(getResumen());
                            break;

                        case "getusuarios":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                a = grupo.arbol;
                                grupo.ts = DateTime.Now;
                                a.actualizarApoyos();
                                Response.Write(Tools.toJson(grupo.usuarios));
                            }
                            break;

                        case "getlistausuariosclaves":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                grupo.ts = DateTime.Now;
                                foreach (Usuario u in grupo.usuarios)
                                    ret += u.email + " - Clave:" + u.clave + " - LastLogin:" + u.lastLogin.ToShortDateString() + " " + u.lastLogin.ToShortTimeString() + "<br>";
                                Response.Write(ret);
                            }
                            break;

                        case "exception":
                            app.addLog("client exception", Request.UserHostAddress, Request["grupo"], Request["email"], Request["flag"] + " -- " + Request["message"] + " -- " + Request["stack"]);
                            break;

                        case "sendmailwelcome":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                a = grupo.arbol;
                                string url = Request.UrlReferrer.OriginalString.Substring(0, Request.UrlReferrer.OriginalString.LastIndexOf("/"));
                                url += "/?grupo=" + Request["grupo"];
                                Usuario u = grupo.getUsuario(Request["email"]);
                                msg = Tools.sendMailWelcome(Request["grupo"], Request["email"], u.clave, url, Server.MapPath("mails/modelos/" + grupo.idioma));
                            }
                            Response.Write(msg);
                            break;

                        case "sendmail":
                            string email = Request["email"];
                            string body = Request["body"];
                            string subject = Request["subject"];
                            msg = Tools.sendMail(email, subject, body.Replace("\n","<br>"));
                            Response.Write(msg);
                            break;

                        case "sendmailalta":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = grupo.getAdmin();
                                msg = Tools.sendMailAlta(Request["grupo"], u.email, Request["nombre"], Request["email"], Server.MapPath("mails/modelos"));
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

                        case "newgrupo":
                            Grupo g = doNewGrupo(Request["grupo"], Request["nombreAdmin"], Request["email"], Request["clave"]);
                            lock (app.grupos)
                            {
                                app.grupos.Add(g);
                                //guardo a disco
                                app.saveGrupos();
                            }
                            Response.Write("Grupo creado");
                            app.addLog("newGrupo", Request.UserHostAddress, Request["grupo"], Request["email"], Request["objetivo"]);
                            break;

                        case "newusuario":
                            Usuario u2 = newUsuario(Server.HtmlEncode(Request["nombre"]), Request["email"], Request["clave"], Request["grupo"]);
                            Response.Write("Usuario [" + u2.email + "] creado");
                            app.addLog("newUsuario", Request.UserHostAddress, Request["grupo"], Request["email"], Request["nombre"]);
                            break;

                        case "removeusuario":
                            Usuario u3 = removeUsuario(Request["email"], Request["grupo"]);
                            Response.Write("Usuario [" + u3.email + "] borrado");
                            break;

                        case "getgrupos":
                            Response.Write(getGrupos());
                            break;

                        case "login":
                            Response.Write(doLogin(Request["email"], Request["clave"], Request["grupo"]));
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

        Grupo doNewGrupo(string grupo, string nombreAdmin, string email, string clave)
        {
            if (grupo == "")
                throw new appException("Nombre de grupo no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");
            if (Server.HtmlEncode(grupo) != grupo)
                throw new appException("Nombre de grupo inv&aacute;lido. Evite acentos y caracteres especiales");
            if (Server.HtmlEncode(email) != email)
                throw new appException("Email inv&aacute;lido. Evite acentos y caracteres especiales");
            if (Server.HtmlEncode(clave) != clave)
                throw new appException("Clave inv&aacute;lida. Evite acentos y caracteres especiales");

            //veo que no exista ya
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath("grupos"));

            foreach (System.IO.DirectoryInfo fi in di.GetDirectories())
            {
                if (fi.Name == grupo)
                {
                    //ya existe
                    throw new appException("El grupo ya existe");
                }
            }

            //creo
            Grupo g = new Grupo();
            g.nombre = grupo;
            g.path = Server.MapPath("grupos/" + g.nombre);
            g.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));

            Arbol a = new Arbol();
            a.nombre = grupo;
            a.raiz = new Nodo();
            a.raiz.nombre = Server.HtmlEncode(a.nombre);
            a.grupo = g; //referencia ciclica, no se pude serializar
            g.arbol = a;

            //admin
            Usuario u = new Usuario(a.cantidadFlores);
            u.nombre = Server.HtmlEncode(nombreAdmin);
            u.email = email;
            u.clave = clave;
            u.isAdmin = true;
            g.usuarios.Add(u);

            return g;
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

        void verifyInactivos(Grupo g)
        {
            DateTime lastInactivos;
            Application.Lock();
            if (Application["lastInactivos"] == null)
                lastInactivos = DateTime.MinValue;
            else
                lastInactivos = (DateTime)Application["lastInactivos"];
            Application["lastInactivos"] = lastInactivos;
            Application.UnLock();

            if (DateTime.Now.Subtract(lastInactivos).TotalHours > 24)
            {
                foreach (Usuario u in g.usuarios)
                {
                    if (u.lastLogin < u.notificado && DateTime.Now.Subtract(u.lastLogin).TotalDays > 7)
                    {
                        //notifico
                        Tools.encolarMailInactivo(u.email);
                        u.notificado = DateTime.Now;
                    }
                }
                Application.Lock();
                Application["lastInactivos"] = DateTime.Now;
                Application.UnLock();
            }
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
                app.saveGrupos();

                depurarMemoria();

                Application.Lock();
                Application["lastSave"] = DateTime.Now;
                Application.UnLock();
            }
        }

        private void depurarMemoria()
        {
            //depuro arboles viejos de memoria
            lock (app.grupos)
            {
                int index = 0;
                while (index < app.grupos.Count)
                {
                    if (DateTime.Now.Subtract(app.grupos[index].ts).TotalMinutes > cleanTime)
                    {
                        //este grupo se puede quitar de memoria
                        //se asume que ya fue guardado
                        //si es simulacion borro temporales
                        Grupo g = app.grupos[index];
                        Arbol a = g.arbol;
                        if (a.simulacion)
                            if (System.IO.Directory.Exists(g.path))
                                System.IO.Directory.Delete(g.path,true);

                        app.grupos.RemoveAt(index);
                    }
                    else
                        index += 1;
                }
            }
        }

        Config getConfig()
        {
            Config ret = new Config();
            ret.grupos = getGrupos();
            return ret;
        }

        string doLogin(string email, string clave, string grupo)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Arbol a = g.arbol;
                g.ts = DateTime.Now;
                Usuario u = g.getUsuario(email, clave);
                if (u != null)
                {
                    //login correcto
                    //devuelvo el arbol personal con las flores de este usuario y sus modelos
                    u.lastLogin = DateTime.Now;
                    a.actualizarModelosEnUso();
                    //knowtypes para modelos
                    List<Type> knowntypes = new List<Type>();
                    foreach (Modelo m in a.modelos)
                    {
                        knowntypes.Add(m.GetType());
                    }
                    ret = "{\"msg\":\"\", \"grupo\":" + g.toJson() + ", \"modelos\":" + Tools.toJson(a.modelos, knowntypes) + ", \"arbolPersonal\":" + Tools.toJson(a.getArbolPersonal(u.email)) + "}";
                }
                else
                {
                    app.addLog("login", Request.UserHostAddress, grupo, email, "fail!");
                    throw new appException("usuario o clave incorrectos para el grupo [" + grupo + "]");
                }

                //envio mails a usuarios inactivos
                verifyInactivos(g);
            }
            return ret;
        }

        Usuario removeUsuario(string email, string grupo)
        {
            Grupo g = app.getGrupo(grupo);

            if (g == null)
                throw new appException("El grupo no existe");
            else
            {
                Usuario u;
                lock (g)
                {
                    g.ts = DateTime.Now;
                    u = g.removeUsuario(email);
                }
                
                //guardo a disco
                app.saveGrupos();

                return u;
            }
        }

        Usuario newUsuario(string nombre, string email, string clave, string grupo)
        {
            if (grupo == "")
                throw new appException("Nombre de arbol no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");

            Grupo g = app.getGrupo(grupo);
            Usuario u;

            if (g.getUsuario(email) != null)
                throw new appException("El usuario ya existe");

            lock (g)
            {
                g.ts = DateTime.Now;
                u = new Usuario(g.arbol.cantidadFlores);
                u.nombre = nombre;
                u.email = email;
                u.clave = clave;
                g.usuarios.Add(u);

            }

            //guardo a disco
            app.saveGrupos();
            
            return u;
        }

        List<string> getGrupos()
        {
            List<string> ret = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath("grupos"));

            foreach (System.IO.DirectoryInfo fi in di.GetDirectories())
            {
                if (!fi.Name.StartsWith("__"))
                    ret.Add(fi.Name);
            }
            return ret;
        }

        string getResumen()
        {
            string ret = "<table style='font-size:16px;'>";
            ret += "<td style='text-align:center'><b>Grupo</b></td>";
            ret += "<td style='text-align:center'><b>Dias</b></td>";
            ret += "<td style='text-align:center'><b>Usuarios</b></td>";
            ret += "<td style='text-align:center'><b>Activos</b></td>";
            ret += "<td style='text-align:center'><b>Consensos</b></td>";
            ret += "<td style='text-align:center'><b>Nodos</b></td>";
            ret += "<td style='text-align:center'><b>Last Login Dias</b></td>";
            foreach (string grupo in getGrupos())
            {
                try
                {
                    Grupo g = app.loadGrupo(grupo);
                    ret += "<tr>";
                    ret += "<td><b>" + g.nombre + "</b></td>";
                    ret += tdSize((int)DateTime.Now.Subtract(g.born).TotalDays);
                    ret += tdSize(g.usuarios.Count);
                    ret += tdSize(g.activos);
                    ret += tdSize(g.arbol.logDocumentos.Count);
                    ret += tdSize(g.arbol.toList().Count);
                    ret += tdSize((int)DateTime.Now.Subtract(g.lastLogin).TotalDays);
                    ret += "</tr>";
                }
                catch (Exception ex)
                {
                    ret += "<tr>";
                    ret += "<td><b>" + grupo + "</b></td>";
                    ret += "<td colspan=6 style='color:red;'>" + ex.Message + "</td>";
                    ret += "</tr>";
                }
            }
            return ret + "</table>";
        }

        string tdSize(int i)
        {
            int size = 10 + i;
            if (size > 45) size = 45;
            return "<td style='text-align:center;font-size:" + size + "px'>" + i + "</td>";
        }
    }
}