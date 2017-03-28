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
                app.verifySave();

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

                        case "borrarhijo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                foreach (Tuple<string, string> hijo in grupo.hijos)
                                {
                                    if (hijo.Item1 == Request["hijoURL"])
                                    {
                                        grupo.hijos.Remove(hijo);
                                        break;
                                    }
                                }
                                Response.Write("Grupo hijo borrado");
                            }
                            break;

                        case "crearhijo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                grupo.hijos.Add(new Tuple<string, string>(Request["hijoURL"], Request["hijoNombre"]));
                                Response.Write("Nuevo grupo hijo agregado");
                            }
                            break;

                        case "getusuarios":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                a = grupo.arbol;
                                grupo.ts = DateTime.Now;
                                a.actualizarApoyos();
                                Response.Write(Tools.toJson(grupo.getUsuariosOrdenados()));
                            }
                            break;

                        case "crearusuarios":
                            grupo = app.getGrupo(Request["grupo"]);
                            List<Usuario> usuarios = Tools.fromJson<List<Usuario>>(Request["usuarios"]);
                            lock (grupo)
                            {
                                foreach(Usuario u in usuarios)
                                    if (grupo.getUsuario(u.email) == null)
                                        //este no existe, lo creo
                                        actualizarUsuario(u.nombre, u.email, u.clave, grupo.nombre, true, false, false);
                                Response.Write("Usuarios creados en [" + Request["grupo"] + "] desahibilitados, contacte al jardinero para habilitarlos");
                            }
                            break;

                        case "exception":
                            app.addLog("client exception", Request.UserHostAddress, Request["grupo"], Request["email"], Request["flag"] + " -- " + Request["message"] + " -- " + Request["stack"]);
                            break;

                        case "sendmailwelcome":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                a = grupo.arbol;
                                string url = Request.UrlReferrer.OriginalString.Substring(0, Request.UrlReferrer.OriginalString.LastIndexOf("/"));
                                url += "/?grupo=" + Request["grupo"];
                                Usuario u = grupo.getUsuario(Request["usuarioemail"]);
                                msg = Tools.sendMailWelcome(Request["grupo"], Request["usuarioemail"], u.clave, url, Server.MapPath("mails/modelos/" + grupo.idioma));
                            }
                            Response.Write(msg);
                            break;

                        case "sendmail":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            string email = Request["usuarioemail"];
                            string body = Request["body"];
                            string subject = Request["subject"];
                            msg = Tools.sendMail(email, subject, body.Replace("\n","<br>"));
                            Response.Write(msg);
                            break;

                        case "sendmailalta":
                            //peticion de alta al jardinero
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = grupo.getAdmin();
                                msg = Tools.sendMailAlta(Request["grupo"], u.email, Request["nombre"], Request["usuarioemail"], Server.MapPath("mails/modelos"));
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

                        //case "newgrupoadmins":
                        //    Grupo g = doNewGrupoFromPadre(Request["grupo"], Request["organizacion"], Request["admins"], Request["idioma"], Request["padreURL"], Request["idioma"]);
                        //    lock (app.grupos)
                        //    {
                        //        app.grupos.Add(g);
                        //        //guardo a disco
                        //        app.saveGrupos();
                        //    }
                        //    Response.Write("Grupo creado");
                        //    app.addLog("doNewGrupoFromPadre", Request.UserHostAddress, Request["grupo"], Request["email"], "");
                        //    break;

                        case "getbosque":
                            //subo buscando al padre
                            //VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            ret = "";
                            grupo = app.getGrupo(Request["grupo"]);

                            if (grupo.padreNombre == "")
                                //yo soy la cabeza del bosque, comienzo a bajar
                                ret = getBosque(Request["grupo"], Request["email"], "", "");
                            else
                                //pido al padre
                                ret = Tools.getHttp(grupo.padreURL + "/doMain.aspx?actn=getBosque&email=" + Request["email"]
                                    + "&grupo=" + grupo.padreNombre
                                    + "&clave=" + Request["clave"]);

                            Response.Write(ret);
                            break;

                        case "getbosque2":
                            //bajo armando el bosque
                            ret = getBosque(Request["grupo"], Request["email"], Request["padreURL"], Request["padreNombre"]);
                            Response.Write(ret);
                            break;

                        case "getoperativo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(getOperativo(Request["grupo"]));
                            break;

                        case "newgrupo":
                            Grupo g = doNewGrupo(Request["grupo"], Request["organizacion"], Request["nombreAdmin"], Request["email"], Request["clave"], Request["idioma"]);
                            lock (app.grupos)
                            {
                                app.grupos.Add(g);
                                //guardo a disco
                                app.saveGrupos();
                            }
                            Response.Write("Grupo creado");
                            app.addLog("newGrupo", Request.UserHostAddress, Request["grupo"], Request["email"], "");
                            break;

                        case "actualizarusuario":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Usuario u2 = actualizarUsuario(Server.HtmlEncode(Request["nuevonombre"]), Request["nuevoemail"], Request["nuevaclave"], Request["grupo"], 
                                Request["readOnly"] == "true" ? true : false,
                                Request["isAdmin"] == "true" ? true : false,
                                Request["habilitado"] == "true" ? true : false);
                            Response.Write("Usuario [" + u2.email + "] actualizado");
                            app.addLog("actualizarUsuario", Request.UserHostAddress, Request["grupo"], Request["email"], Request["nombre"]);
                            break;

                        case "removeusuario":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Usuario u3 = removeUsuario(Request["usuarioemail"], Request["grupo"]);
                            Response.Write("Usuario [" + u3.email + "] borrado");
                            break;

                        //case "getgrupos":
                        //    Response.Write(getGrupos());
                        //    break;

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

        public string getOperativo(string grupo)
        {
            //devuelvo el bosque de aqui hacia abajo
            Grupo g = app.getGrupo(grupo);
            return g.organizacion.getEstructura(g);
        }

        public string getBosque(string grupo, string email, string padreURL, string padreNombre)
        {
            //devuelvo el bosque de aqui hacia abajo
            Grupo g = app.getGrupo(grupo);

            string acceso;
            Usuario u = g.getUsuario(email);
            if (u == null)
                acceso = "NoExiste";
            else if (!u.habilitado)
                acceso = "NoHabilitado";
            else if (u.readOnly)
                acceso = "readOnly";
            else
                acceso = "si";
            
            string ret = "{\"nombre\":\"" + g.nombre + "\"," 
                + "\"usuarios\":" + g.usuarios.Count + ","
                + "\"acceso\":\"" + acceso + "\","
                + "\"objetivo\":\"" + g.objetivo + "\","
                + "\"padreVerificado\":\"" + (g.padreURL == padreURL && g.padreNombre == padreNombre ? "Ok" : "Error") + "\","
                + "\"hijos\":[";
            foreach (Tuple<string, string> hijo in g.hijos)
            {
                try
                {
                    string ret2 = Tools.getHttp(hijo.Item1 + "/doMain.aspx?actn=getBosque2&grupo=" + hijo.Item2 + "&padreURL=" + g.URL + "&padreNombre=" + g.nombre + "&email=" + email);
                    if (ret2.ToLower().StartsWith("error="))
                        ret += "{\"grupo\":\"" + hijo.Item2 + "\", \"exception\":\"" + ret2.Substring(6) + "\"},";
                    else
                        ret += ret2 + ",";
                }
                catch (Exception ex)
                {
                    //timeout
                    ret += "{\"grupo\":\"" + hijo.Item2 + "\", \"exception\":\"" + ex.Message + "\"},";
                }
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);

            ret += "]}";
            return ret;
        }

        public void VerificarUsuario(string grupo, string email, string clave)
        {
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                Usuario u = g.getUsuario(email);
                if (u.clave == clave)
                    return;
            }
            throw new Exception("Usuario inválido, operacion registrada!");                  
        }

        //Grupo doNewGrupoFromPadre(string grupo, string organizacion, string admins, string idioma, string padreURL, string padreNombre)
        //{
        //    string[] aadmins = admins.Split('|');
        //    Grupo g = null;

        //    foreach(string admin in aadmins)
        //    {
        //        string nombre = admin.Split(':')[0];
        //        string email = admin.Split(':')[1];
        //        string clave = admin.Split(':')[2];

        //        if (grupo == null)
        //        {
        //            //creo el grupo
        //            g = doNewGrupo(grupo, organizacion, nombre, email, clave, idioma);
        //            g.padreURL = padreURL;
        //            g.padreNombre = padreNombre;
        //        }
        //        else
        //        {
        //            //agrego admins
        //            Usuario u = new Usuario();
        //            u.nombre = nombre;
        //            u.isAdmin = true;
        //            u.email = email;
        //            u.clave = clave;
        //            g.usuarios.Add(u);
        //        }
        //    }
        //    return g;
        //}

        Grupo doNewGrupo(string grupo, string organizacion, string nombreAdmin, string email, string clave, string idioma)
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
            g.idioma = idioma;

            //organizacion
            switch (organizacion)
            {
                case "Plataforma":
                    g.organizacion = new organizaciones.Plataforma();
                    break;

                case "Cooperativa":
                    g.organizacion = new organizaciones.Cooperativa();
                    break;

                case "Colegio":
                    g.organizacion = new organizaciones.Colegio();
                    break;

                default:
                    throw new Exception("Modelo organizativo [" + organizacion + "] no existe");
            }

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
                if (u != null && u.habilitado)
                {
                    //login correcto
                    //devuelvo el arbol personal con las flores de este usuario y sus modelos
                    u.lastLogin = DateTime.Now;
                    a.actualizarModelosEnUso();
                    //knowtypes para modelos
                    List<Type> tipos = new List<Type>();
                    foreach (Modelo m in g.organizacion.getModelos()) tipos.Add(m.GetType());
                    ret = "{\"msg\":\"\", \"grupo\":" + g.toJson() + ", \"modelos\":" + Tools.toJson(a.getModelos(), tipos) + ", \"arbolPersonal\":" + Tools.toJson(a.getArbolPersonal(u.email)) + "}";
                }
                else if (u != null && !u.habilitado)
                {
                    app.addLog("login", Request.UserHostAddress, grupo, email, "fail!");
                    throw new appException("usuario no habilitado para el grupo [" + grupo + "]");
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

        Usuario actualizarUsuario(string nombre, string email, string clave, string grupo, bool readOnly, bool isAdmin, bool habilitado)
        {
            if (grupo == "")
                throw new appException("Nombre de arbol no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");

            Grupo g = app.getGrupo(grupo);
            Usuario u = g.getUsuario(email);

            lock (g)
            {
                if (u != null)
                {
                    //lo actualizo
                    u.nombre = nombre;
                    u.clave = clave;
                    u.readOnly = readOnly;
                    u.isAdmin = isAdmin;
                    u.habilitado = habilitado;
                }
                else
                {
                    g.ts = DateTime.Now;
                    u = new Usuario(g.arbol.cantidadFlores);
                    u.nombre = nombre;
                    u.email = email;
                    u.clave = clave;
                    u.readOnly = readOnly;
                    u.isAdmin = isAdmin;
                    u.habilitado = habilitado;
                    g.usuarios.Add(u);
                }
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
            ret += "<td style='text-align:center'><b>Admin</b></td>";
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
                    ret += "<td>" + g.getAdmin().nombre + " " + g.getAdmin().email + "</td>";
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