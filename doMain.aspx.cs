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

//atiendo peticiones de grupo

namespace nabu
{
    public partial class doMain : System.Web.UI.Page
    {
        public Aplicacion app;

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

                //envio algun mail si hay en cola
                app.mailBot.send();

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
                                Usuario u = grupo.getUsuarioHabilitado(Request["email"]);
                                if (u == null)
                                    throw new appException(Tools.tr("El usuario no existe o no esta habilitado", grupo.idioma));
                                else
                                {
                                    if (u.clave != Request["claveActual"])
                                        throw new appException(Tools.tr("La clave actual no corresponde", grupo.idioma));
                                    else
                                    {
                                        if (Request["nuevaClave"] == "")
                                            throw new appException(Tools.tr("La clave nueva no puede ser vacia", grupo.idioma));
                                        else
                                            u.clave = Request["nuevaClave"]; //no devuelvo mensaje
                                    }
                                }
                            }
                            break;

                        case "resumen":
                            Response.Write(getResumen());
                            break;

                        case "borrargrupo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = grupo.getUsuario(Request["email"]);
                                if (u != null && u.isAdmin)
                                {
                                    borrarCarpeta(Server.MapPath("grupos/" + grupo.nombre));
                                }
                                else
                                    throw new Exception("Debe ser coordinador");
                            }

                            app.grupos.Remove(grupo);

                            Response.Write("Grupo borrado");
                            break;

                        case "borrarhijo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                foreach (Hijo hijo in grupo.hijos)
                                {
                                    if (hijo.URL == Request["hijoURL"] && hijo.nombre == Request["hijoNombre"])
                                    {
                                        grupo.hijos.Remove(hijo);
                                        break;
                                    }
                                }
                                Response.Write(Tools.tr("Grupo hijo borrado", grupo.idioma));
                            }
                            break;

                        case "crearhijo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                string hijoURL = Request["hijoURL"];
                                if (hijoURL.EndsWith("/")) hijoURL = hijoURL.Substring(0, hijoURL.Length - 1);
                                if (hijoURL == "") hijoURL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
                                grupo.hijos.Add(new Hijo(hijoURL, Request["hijoNombre"]));
                                Response.Write(Tools.tr("Nuevo grupo hijo agregado", grupo.idioma));
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
                                        actualizarUsuario(u.nombre, u.email, u.clave, grupo.nombre, true, false, false, false, false, false, "", u.grupoDesde, "", "", "", "", "", "0", "0");
                                Response.Write(Tools.tr("Usuarios creados desahibilitados", Request["grupo"], grupo.idioma));
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
                                msg = Tools.sendMailWelcome(grupo, Request["usuarioemail"], u.clave, url, Server.MapPath("mails/modelos/" + grupo.idioma));
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
                            //peticion de alta al coordinador
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = grupo.getAdmin();
                                msg = Tools.sendMailAlta(grupo, u.email, Request["nombre"], Request["usuarioemail"], Server.MapPath("mails/modelos"));
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

                        case "clearbosque":
                            //subo buscando al padre
                            //VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            if (grupo.padreNombre == "")
                            {
                                grupo.bosque = new Bosque(grupo);
                                //ret = grupo.bosque.toJson();
                                ret = Tools.toJson(grupo.bosque);
                            }
                            else   
                                //pido al padre
                                ret = Tools.getHttp(grupo.padreURL + "/doMain.aspx?actn=clearBosque&grupo=" + grupo.padreNombre);

                            Response.Write(ret);
                            break;

                        case "getbosques":
                            //subo buscando al padre
                            List<string> gruposName = getGrupos();
                            List<Bosque> bosques = new List<Bosque>();
                            ret = "[";
                            foreach (string grupoName in gruposName)
                            {
                                grupo = app.getGrupo(grupoName);
                                if (grupo.padreNombre == "")
                                {
                                    //este grupo es padre
                                    ret += getBosque(grupoName) + ",";
                                }
                            }
                            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);

                            Response.Write(ret + "]");
                            break;

                        case "getbosque":
                            //subo buscando al padre
                            //VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(getBosque(Request["grupo"]));
                            break;

                        case "getnodo":
                            //devuelvo configuracion de hijos
                            try
                            {
                                grupo = app.getGrupo(Request["grupo"]);
                                Bosque.Nodo nodo = Bosque.crearNodo(grupo);
                                ret = Tools.toJson(nodo);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("getNodo(" + Request["grupo"] + ") > " + ex.Message);
                            }
                            Response.Write(ret);
                            break;

                        case "organizacion":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doOperativoAccion(Request["grupo"], Request["email"], Request["accion"], Request));
                            break;

                        case "getoperativo":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(getOperativo(Request["grupo"]));
                            break;

                        case "newgrupo":
                            Grupo g = Grupo.newGrupo(Request["grupo"], 
                                Request["organizacion"], 
                                Request["nombreAdmin"], 
                                Request["email"], 
                                Request["clave"],
                                Request["idioma"],
                                Request["tipoGrupo"],
                                Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/")));
                            lock (app.grupos)
                            {
                                app.grupos.Add(g);
                                //guardo a disco
                                app.saveGrupos();
                            }
                            Response.Write(Tools.tr("Grupo creado",g.idioma));
                            app.addLog("newGrupo", Request.UserHostAddress, Request["grupo"], Request["email"], "");
                            break;

                        case "actualizarusuario":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            Usuario u2 = actualizarUsuario(Server.HtmlEncode(Request["nuevonombre"]), Request["nuevoemail"], Request["nuevaclave"], Request["grupo"],
                                Request["habilitado"] == "true",
                                Request["readOnly"] == "true",
                                Request["isAdmin"] == "true",
                                Request["isSecretaria"] == "true",
                                Request["isRepresentante"] == "true",
                                Request["isFacilitador"] == "true",
                                Request["funcion"],
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null);
                            Response.Write(Tools.tr("Usuario [%1] actualizado", u2.email, grupo.idioma));
                            app.addLog("actualizarUsuario", Request.UserHostAddress, Request["grupo"], Request["email"], Request["nombre"]);
                            break;

                        case "actualizarperfilusuario":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            u2 = grupo.getUsuario(Request["email"]);
                            actualizarUsuario(Request["nombre"], u2.email, u2.clave, Request["grupo"],
                                u2.habilitado,
                                u2.readOnly,
                                u2.isAdmin,
                                u2.isSecretaria,
                                u2.isRepresentante,
                                u2.isFacilitador,
                                Request["funcion"],
                                null,
                                Request["mision"],
                                Request["capacidades"],
                                Request["expectativas"],
                                Request["participacion"],
                                Request["address"],
                                Request["lat"],
                                Request["lng"]);
                            Response.Write(Tools.tr("Usuario [%1] actualizado", u2.email, grupo.idioma));
                            app.addLog("actualizarUsuario", Request.UserHostAddress, Request["grupo"], Request["email"], Request["nombre"]);
                            break;

                        case "crearusuarioabierto":
                            grupo = app.getGrupo(Request["grupo"]);
                            u2 = grupo.getUsuario(Request["email"]);
                            if (u2 != null)
                                Response.Write("Error=" + Tools.tr("Este email ya existe", u2.email, grupo.idioma));
                            else
                            {
                                u2 = actualizarUsuario(Server.HtmlEncode(Request["nombre"]), Request["email"], Request["clave"], Request["grupo"],
                                    true,
                                    false,
                                    false,
                                    false,
                                    false,
                                    false,
                                    "",
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null);
                                Response.Write(Tools.tr("Usuario [%1] creado", u2.email, grupo.idioma));
                            }
                            app.addLog("crearusuarioabierto", Request.UserHostAddress, Request["grupo"], Request["email"], Request["nombre"]);
                            break;

                        case "removeusuario":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Usuario u3 = removeUsuario(Request["usuarioemail"], Request["grupo"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            Response.Write(Tools.tr("Usuario [%1] borrado", u3.email, grupo.idioma));
                            break;    

                        case "login":
                            Response.Write(doLogin(Request["email"], Request["clave"], Request["grupo"]));
                            break;

                        case "borraralertas":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = grupo.getUsuario(Request["email"]);
                                u.alertas.Clear();
                            }
                            Response.End(); //no hay respuesta
                            break;

                        case "getgrupopersonal":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            grupo = app.getGrupo(Request["grupo"]);
                            GrupoPersonal gp = null;
                            lock (grupo)
                            {
                                gp = grupo.getGrupoPersonal(Request["email"]);
                            }
                            Response.Write(Tools.toJson(gp)); 
                            break;

                        case "gettipogrupo":
                            grupo = app.getGrupo(Request["grupo"]);
                            string tipoGrupo = "";
                            lock (grupo)
                            {
                                tipoGrupo = grupo.tipoGrupo + ";" + grupo.URLEstatuto;
                            }
                            Response.Write(tipoGrupo);
                            break;

                        case "upload":
                            grupo = app.getGrupo(Request["grupo"]);
                            email = Request["email"];
                            string path = grupo.path + "\\usuarios\\" + email;
                            string base64 = Request["base64"];
                            base64 = base64.Split(',')[1];

                            if (!System.IO.Directory.Exists(path))
                                System.IO.Directory.CreateDirectory(path);

                            //base64 to binary
                            byte[] bytes = Convert.FromBase64String(base64);
                            System.Drawing.Image img = System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(bytes), true);

                            saveImage(img, path, 150, email + ".png");
                            saveImage(img, path, 50, email + ".small.png");
                            break;

                        case "dictionary":
                            List<string> eskeys = new List<string>();;
                            foreach (string item in Tools.dictionary) {
                                if (item.Split('|')[1] == "es")
                                    eskeys.Add(item.Split('|')[0]);
                            }
                            eskeys.Sort();
                            ret = "";
                            ret += dictionaryFill("es", eskeys);
                            ret += dictionaryFill("ct", eskeys);
                            ret += dictionaryFill("en", eskeys);
                            ret += dictionaryFill("fr", eskeys);
                            Response.Write(ret);
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

                Response.Write("Error=" + ex.Message + " [" + Tools.errordebug + "]");
                app.addLog("server exception", "", "", "", s);
            }
            Response.End();
        }

        public void saveImage(System.Drawing.Image img, string path, int maxSize, string name)
        {
            //resize
            float ratio = (float)img.Width / img.Height;
            int newWidth = img.Width, newHeight = img.Height;
            if (img.Width > maxSize && img.Width >= img.Height)
            {
                newWidth = maxSize;
                newHeight = (int)(newWidth / ratio);
            }
            else if (img.Height > maxSize && img.Height >= img.Width)
            {
                newHeight = maxSize;
                newWidth = (int)(newHeight * ratio);
            }

            //save
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            System.Drawing.Graphics grp = System.Drawing.Graphics.FromImage(bmp);
            System.Drawing.Rectangle rct = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            grp.DrawImage(img, rct, 0, 0, img.Width, img.Height, System.Drawing.GraphicsUnit.Pixel);
            if (System.IO.File.Exists(path + "\\" + name))
                System.IO.File.Delete(path + "\\" + name);
            bmp.Save(path + "\\" + name, System.Drawing.Imaging.ImageFormat.Png);  // Or Png
        }

        public string getBosque(string grupoNombre)
        {
            string ret="";
            Grupo grupo = app.getGrupo(grupoNombre);

            if (grupo.padreNombre == "")
            {
                //yo soy la cabeza del bosque
                try
                {
                    if (grupo.bosque == null || DateTime.Now.Subtract(grupo.bosque.born).TotalHours >= 24)
                        grupo.bosque = new Bosque(grupo);
                    //ret = grupo.bosque.toJson();
                    ret = Tools.toJson(grupo.bosque);
                }
                catch (Exception ex)
                {
                    throw new Exception("Comunidad [" + grupo.nombre + "] > " + ex.Message);
                }
            }
            else
                //pido al padre
                try
                {
                    ret = Tools.getHttp(grupo.padreURL + "/doMain.aspx?actn=getBosque&grupo=" + grupo.padreNombre);
                }
                catch (Exception ex)
                {
                    throw new Exception("No se puede alcanzar la cabeza de la comunidad desde [" + grupo.nombre + "] > " + ex.Message);
                }
            return ret;
        }

        public string dictionaryFill(string idioma, List<string> eskeys) {
            string ret = "";
            var found = false;
            foreach (string l in eskeys) {
                found = false;
                foreach (string item in Tools.dictionary) {
                    if (item.Split('|')[1] == idioma && item.Split('|')[0] == l) {
                        ret += "\"" + l + "|" + idioma + "|" + DictionaryEncode(item.Split('|')[2]) + "\",\r\n";
                        found = true;
                    }
                }
                if (!found) {
                    ret += "\"" + l + "|" + idioma + "|?\",\r\n";
                }
            }
            ret += "\r\n";
            return ret;
        }

        string DictionaryEncode(string str) {
            string result = "";
            for (int i = 0; i < str.Length; i++) {
                int chrcode = Convert.ToInt32(str[i]);
                if (chrcode == 38)
                    result += "&amp;";
                else if (chrcode == 60)
                    result += "&lt;";
                else if (chrcode == 62)
                    result += "&gt;";
                else
                    result += str.Substring(i, 1);
            }
            return result;
        }

        public void borrarCarpeta(string basePath)
        {
            if (System.IO.Directory.Exists(basePath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(basePath);
                foreach (System.IO.DirectoryInfo di2 in di.GetDirectories())
                    borrarCarpeta(di.FullName);
                foreach (System.IO.FileInfo fi in di.GetFiles())
                    System.IO.File.Delete(fi.FullName);
                System.IO.Directory.Delete(basePath);
            }
        }

        public string doOperativoAccion(string grupo, string email, string accion, HttpRequest req)
        {
            //devuelvo el bosque de aqui hacia abajo
            Grupo g = app.getGrupo(grupo);
            return  g.organizacion.doAccion(g, email, accion, req);
        }

        public string getOperativo(string grupo)
        {
            //devuelvo el bosque de aqui hacia abajo
            Grupo g = app.getGrupo(grupo);
            return g.organizacion.getOperativo(g);
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
            throw new Exception("Usuario no existe o no habilitado, operacion registrada!");                  
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
                        Tools.encolarMailInactivo(g, u.email);
                        u.notificado = DateTime.Now;
                        u.alertas.Add(new Alerta(Tools.tr("Tu estado es inactivo", g.idioma)));
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
                    a.actualizarModelosEnUso();
                    //knowtypes para modelos
                    List<Type> tipos = new List<Type>();
                    tipos.Add((new Alerta()).GetType());
                    foreach (Modelo m in g.organizacion.getModelosDocumento()) tipos.Add(m.GetType());
                    foreach (ModeloEvaluacion m in g.organizacion.getModelosEvaluacion()) tipos.Add(m.GetType());
                    ret = "{\"msg\":\"\", \"grupo\":" + g.toJson() + ", ";
                    ret += "\"modelos\":" + Tools.toJson(g.organizacion.getModelosDocumento(), tipos) + ", ";
                    ret += "\"modelosEvaluacion\":" + Tools.toJson(g.organizacion.getModelosEvaluacion(), tipos) + ", ";
                    ret += "\"arbolPersonal\":" + Tools.toJson(a.getArbolPersonal(u.email)) + "}";
                    u.lastLogin = DateTime.Now;
                }
                else if (u != null && !u.habilitado)
                {
                    app.addLog("login", Request.UserHostAddress, grupo, email, "fail!");
                    throw new appException(Tools.tr("Usuario no habilitado", grupo, g.idioma));
                }
                else
                {
                    app.addLog("login", Request.UserHostAddress, grupo, email, "fail!");
                    throw new appException(Tools.tr("Usuario o clave incorrectos", grupo, g.idioma));
                }

                //envio mails a usuarios inactivos
                verifyInactivos(g);
                return ret;
            }
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

        Usuario actualizarUsuario(string nombre, string email, string clave, string grupo, bool habilitado, bool readOnly, 
            bool isAdmin, bool isSecretaria, bool isRepresentante, bool isFacilitador, string funcion, string grupoDesde,
            string mision, string capacidades, string expectativas, string participacion, string address, string lat, string lng)
        {
            string idioma = "es";
            Grupo g;
            try
            {
                g = app.getGrupo(grupo);
                idioma = g.idioma;
            }
            catch (Exception ex) {}

            if (grupo == "")
                throw new appException(Tools.tr("Nombre de arbol no puede ser vacio",idioma));
            else if (email == "")
                throw new appException(Tools.tr("Email no puede ser vacio",idioma));
            else if (clave == "")
                throw new appException(Tools.tr("Clave no puede ser vacio", idioma));
            else if (isAdmin && isRepresentante)
                throw new appException(Tools.tr("No puede ser coordinador y representante", idioma));
            else if (isAdmin && !habilitado)
                throw new appException(Tools.tr("No se puede deshabilitar al admin", idioma));
            g = app.getGrupo(grupo);
            Usuario u = null;
            lock (g)
            {
                //limpio roles anteriores
                if (isAdmin)
                    foreach (Usuario u2 in g.usuarios)
                        if (u2.isAdmin)
                            u2.isAdmin = false;
                if (isSecretaria)
                    foreach (Usuario u2 in g.usuarios)
                        if (u2.isSecretaria)
                            u2.isSecretaria = false;
                if (isFacilitador)
                    foreach (Usuario u2 in g.usuarios)
                        if (u2.isFacilitador)
                            u2.isFacilitador = false;

                //actualizo
                u = g.getUsuario(email);
                if (u != null)
                {
                    //lo actualizo
                    u.nombre = nombre;
                    u.funcion = funcion;
                    u.clave = clave;
                    u.habilitado = habilitado;
                    u.readOnly = readOnly;
                    u.isAdmin = isAdmin;
                    u.isSecretaria = isSecretaria;
                    u.isRepresentante = isRepresentante;
                    u.isFacilitador = isFacilitador;
                    if (grupoDesde != null) u.grupoDesde = grupoDesde;
                    if (mision != null) u.mision = mision;
                    if (capacidades != null) u.capacidades = capacidades;
                    if (expectativas != null) u.expectativas = expectativas;
                    if (participacion != null) u.participacion = participacion;
                    if (address != null) u.address = address;
                    if (lat != null) u.lat = Tools.ParseF(lat);
                    if (lng != null) u.lng = Tools.ParseF(lng);
                }
                else
                {
                    g.ts = DateTime.Now;
                    u = new Usuario(g.arbol.cantidadFlores);
                    u.nombre = nombre;
                    u.funcion = funcion;
                    u.email = email;
                    u.clave = clave;
                    u.habilitado = habilitado;
                    u.readOnly = readOnly;
                    u.isAdmin = isAdmin;
                    u.isSecretaria = isSecretaria;
                    u.isRepresentante = isRepresentante;
                    u.isFacilitador = isFacilitador;
                    if (grupoDesde != null) u.grupoDesde = grupoDesde;
                    if (mision != null) u.mision = mision;
                    if (capacidades != null) u.capacidades = capacidades;
                    if (expectativas != null) u.expectativas = expectativas;
                    if (participacion != null) u.participacion = participacion;
                    if (address != null) u.address = address;
                    if (lat != null) u.lat = double.Parse(lat);
                    if (lng != null) u.lng = double.Parse(lng);
                    g.usuarios.Add(u);
                }

                //caen flores?
                if (!u.habilitado)
                    g.arbol.caerFlores(u);
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
            ret += "<td style='text-align:center'><b>Decisiones</b></td>";
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
                    ret += tdSize(g.logDecisiones.Count);
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
            if (size > 35) size = 45;
            return "<td style='text-align:center;font-size:" + size + "px'>" + i + "</td>";
        }
    }
}