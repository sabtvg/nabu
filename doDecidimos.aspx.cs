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
    public partial class doDecidimos : System.Web.UI.Page
    {
        public Aplicacion app;
        public int saveTime = 10; //guardar arboles cada x minutos
        public int cleanTime = 20; //quito arbol de memoria si no se toca en 20 minutos

        private Random simRND = new Random();

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
                
                //limpio flores caducadas periodicamente de todos los usuarios 
                //verifyFloresCaducadas(); se verifica al crear al arbol personal

                //proceso peticiones
                Grupo grupo;
                Arbol a;
                string ret = "";

                if (actn != null)
                {
                    switch (actn.ToLower())
                    {
                        case "docomentar":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doComentar(int.Parse(Request["id"]), Request["grupo"], Request["email"], Request["comentario"], Request["objecion"] == "true"));
                            app.addLog("doComentar", Request.UserHostAddress, Request["grupo"], "", Request["comentario"]);
                            break;

                        //case "crearacta":
                        //    //devuelvo las propuestas de toda la rama
                        //    VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                        //    Response.Write(crearActa(Request["grupo"], Request["email"], Request));
                        //    break;

                        case "htmldocumento":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(HTMLDocumento(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "htmlpropuesta":
                            //devuelvo las propuestas de toda la rama
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(HTMLPropuesta(int.Parse(Request["id"]), Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "getarbolpersonal":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
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
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doVariante(int.Parse(Request["id"]), Request["modeloID"], Request["grupo"], Request["email"], int.Parse(Request["width"]), int.Parse(Request["finalID"])));
                            break;

                        case "prevista":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doPrevista(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"]), Request));
                            break;

                        case "revisar":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doRevisar(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"], int.Parse(Request["width"])));
                            break;

                        case "proponer":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doProponer(int.Parse(Request["id"]), Request["modelo"], Request["grupo"], Request["email"]));
                            app.addLog("proponer", Request.UserHostAddress, Request["grupo"], Request["email"], "Nueva propuesta recibida");
                            break;

                        case "seguimiento":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doSeguimiento(int.Parse(Request["docID"]), Request["grupo"], int.Parse(Request["width"])));
                            break;

                        case "toggleflor":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doToggleFlor(Request["email"], int.Parse(Request["id"]), float.Parse(Request["x"]), Request["grupo"]));
                            app.addLog("toggleFlor", Request.UserHostAddress, Request["grupo"], Request["email"], "Cambio de voto");
                            break;

                        case "updatearbol":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            a = updateArbol(Request["grupo"], int.Parse(Request["cantidadFlores"]), float.Parse(Request["minSiPc"]), float.Parse(Request["maxNoPc"]), Request["padreURL"], Request["padreNombre"], Request["idioma"], Request["tipoGrupo"]);
                            Response.Write("Arbol actualizado");
                            break;

                        case "documentsubmit":
                            VerificarUsuario(Request["grupo"], Request["email"], Request["clave"]);
                            Response.Write(doDocumentSubmit(Request["accion"], Request["parametro"], Request["grupo"], Request["email"], Request["modelo"], int.Parse(Request["id"]), int.Parse(Request["width"]), Request));
                            break;

                        case "crearsimulacion":
                            //creo grupo
                            Grupo g = new Grupo();
                            g.nombre = getSimName();
                            g.path = Server.MapPath("grupos/" + g.nombre);
                            g.URL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
                            g.objetivo = "simulacion";

                            //organizacion
                            g.organizacion = new nabu.organizaciones.Plataforma();

                            //arbol
                            a = new Arbol();
                            a.nombre = g.nombre;
                            a.simulacion = true;
                            a.raiz = new Nodo();
                            a.raiz.nombre = "Sim";
                            a.grupo = g;
                            g.arbol = a;

                            a.minSiPc = 100;
                            a.maxNoPc = 0;

                            //usuarios de prueba
                            Usuario admin = null;
                            for (int i = 0; i < 50; i++)
                            {
                                Usuario u = new Usuario();
                                u.nombre = "u" + i;
                                u.email = "u" + i;
                                u.lastLogin = DateTime.Now;
                                for (int q = 0; q < 5; q++)
                                    u.flores.Add(new Flor());
                                g.usuarios.Add(u);

                                if (admin == null)
                                {
                                    u.isAdmin = true;
                                    admin = u;
                                }
                            }

                            //escribo respuesta
                            List<Type> tipos = new List<Type>();
                            foreach (Modelo m in g.organizacion.getModelosDocumento(g.idioma)) tipos.Add(m.GetType());
                            foreach (ModeloEvaluacion m in g.organizacion.getModelosEvaluacion()) tipos.Add(m.GetType());
                            ret = "{\"arbolPersonal\": " + Tools.toJson(a.getArbolPersonal("u1")) + ",";
                            ret += "\"modelos\":" + Tools.toJson(g.organizacion.getModelosDocumento(g.idioma), tipos) + ",";
                            ret += "\"modelosEvaluacion\":" + Tools.toJson(g.organizacion.getModelosEvaluacion(), tipos) + "}";
                            lock (app.grupos)
                            {
                                app.grupos.Add(g);
                            }
                            Response.Write(ret);
                            app.addLog("crearSimulacion", Request.UserHostAddress, "", "", "Simulacion creada");
                            break;

                        case "simulacionnuevodebate":
                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                Usuario u = null;
                                //busco un usuario con flores
                                foreach(Usuario u2 in grupo.usuarios)
                                    if (u2.floresDisponibles().Count > 0)
                                    {
                                        u = u2;
                                        break;
                                    }

                                //agergo nodos iniciales
                                if (u != null)
                                {
                                    //el nombre de este nodo es la cantidad de dias
                                    Nodo NuevoTema = simAgregarNodo(grupo, u, grupo.arbol.raiz, 0); //introduccion al debate
                                    //el nombre de este nodo es la generacion
                                    Nodo n1 = simAgregarNodo(grupo, u, NuevoTema, 1);
                                }
                                ret = "{\"stop\": false, \"arbolPersonal\":" + Tools.toJson(grupo.arbol.getArbolPersonal("u1")) + "}";
                            }
                            Response.Write(ret);
                            break;

                        case "simulacionlive":
                            string x = Request["x"];

                            grupo = app.getGrupo(Request["grupo"]);
                            lock (grupo)
                            {
                                //guardo x
                                //&x=38=43,42=111,43=146
                                if (x != "")
                                {
                                    a = grupo.arbol;
                                    foreach (string s in x.Split(','))
                                    {
                                        Nodo n = a.getNodo(int.Parse(s.Split('=')[0]));
                                        if (n != null) 
                                            n.x = float.Parse(s.Split('=')[1]);
                                    }
                                }

                                //para cada debate live
                                int i = 0;
                                while (i < grupo.arbol.raiz.children.Count)
                                {
                                    Nodo n = grupo.arbol.raiz.children[i];
                                    int dias = int.Parse(n.nombre);
                                    simDebateLive(grupo, n, dias);
                                    n.nombre = (dias + 1).ToString();
                                    i++;
                                }
                                ret = "{\"stop\": false, \"arbolPersonal\":" + Tools.toJson(grupo.arbol.getArbolPersonal("u1")) + "}";
                            }
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

                Response.Write("Error=" + ex.Message);
                app.addLog("server exception", "", "", "", s);
            }
            Response.End();
        }

        public void simDebateLive(Grupo g, Nodo tema, float dias)
        {
            int cantDiscrepar = 0;
            for(int iu = 0; iu < g.usuarios.Count; iu++)
            {
                //limpio consensos
                Usuario u = g.usuarios[iu];
                foreach(Flor f in u.flores)
                    if (f.id != 0){
                        Nodo n = g.arbol.getNodo(f.id);
                        if (n == null)
                            f.id = 0;
                        else if (n.consensoAlcanzado)
                            g.arbol.quitarFlor(n, u);
                    }

                //actuo
                if (simHoyConsiente(dias / 30))
                    simConsentir(g, u, tema);
                else
                    if (cantDiscrepar < 30)  //evito que discrepen todos de golpe al principio
                    {
                        simDiscrepar(g, u, tema);
                        cantDiscrepar++;
                    }
            }
        }

        public void simConsentir(Grupo g, Usuario u, Nodo tema)
        {
            //muevo una flor a un nodo con generacion mayor
            List<Nodo> nodes = g.arbol.toList2(tema, new List<Nodo>());
            //si tengo flor en este tema la quito
            int generacion = 1;
            Nodo nViejo = null;
            foreach (Nodo n2 in nodes)
                if (u.getFlor(n2.id) != null)
                {
                    generacion = int.Parse(n2.nombre);
                    nViejo = n2;
                    break;
                }

            //busco nodo para apoyar
            Nodo nNuevo = null;
            int nivel = 1;
            foreach (Nodo n2 in nodes)
                //if (int.Parse(n2.nombre) > generacion && n2.nivel > 1 && !n2.consensoAlcanzado)
                if (n2.nivel > nivel && !n2.consensoAlcanzado)
                {
                    //generacion = int.Parse(n2.nombre);
                    nivel = n2.nivel;
                    nNuevo = n2;
                }
            if (nNuevo != null)
            {
                if (nViejo != null) 
                    g.arbol.quitarFlor(nViejo, u);  //quito la flor solo si hay nuevo candidato
                if (u.floresDisponibles().Count != 0)
                    g.arbol.asignarflor(u, nNuevo);
            }

        }

        public void simDiscrepar(Grupo g, Usuario u, Nodo tema)
        {
            //creo hermano o hijo
            List<Nodo> nodes = g.arbol.toList2(tema, new List<Nodo>());
            //si tengo flor en este tema entonces nada
            bool hayFlor = false;
            foreach (Nodo n2 in nodes)
                if (u.getFlor(n2.id) != null)
                {
                    hayFlor = true;
                }
            if (!hayFlor)
            {
                //seleccion nodo al azar dentro del subarbol de n
                Nodo selected = g.arbol.rndElement(nodes);
                if (!selected.consensoAlcanzado)
                {
                    int generacion = int.Parse(selected.nombre);
                    if (u.floresDisponibles().Count > 0)
                    {
                        if (simRND.NextDouble() <= 0.3)
                        {
                            //hijo
                            if (selected.nivel < 5)
                                simAgregarNodo(g, u, selected, generacion + 1);
                            else if (selected != tema)
                            {
                                //hermano
                                List<Nodo> path = g.arbol.getPath(selected.id);
                                if (path.Count > 2)
                                {
                                    Nodo padre = path[1];
                                    simAgregarNodo(g, u, padre, generacion + 1);
                                }
                                else if (selected.nivel < 5)
                                    //hijo
                                    simAgregarNodo(g, u, selected, generacion + 1);
                            }
                        }
                    }
                }
            }
        }

        public bool simHoyConsiente(float prob)
        {
            return simRND.NextDouble() <= prob;
        }

        public Nodo simAgregarNodo(Grupo g, Usuario u, Nodo selected, int generacion){
            //agrego nodo
            //creo texto segun nivel y modelo de documento
            Modelo m = g.organizacion.getModeloDocumento("nabu.plataforma.modelos.Accion");  //modelo de simulacion (Accion)
            Propuesta prop = new Propuesta();
            prop.email = u.email;
            prop.modeloID = m.id;
            prop.nivel = selected.nivel + 1;  //esta propuesta es para el hijo que voy a crear
            prop.nodoID = selected.id;
            prop.niveles = 5;
            prop.titulo = Tools.tr("Documento simulado", g.idioma);
            prop.etiqueta = generacion.ToString();

            //lleno datos de prueba
            foreach (Variable v in m.getVariables())
            {
                if (v.id == "s.etiqueta")
                    prop.bag.Add("s.etiqueta", "Sim");
                else if (v.id == "s.titulo")
                    prop.bag.Add("s.titulo", Tools.tr("Documento simulado", g.idioma));
                else
                    prop.bag.Add(v.id, Tools.tr("Simulacion", g.idioma));
            }

            Nodo nuevoNodo = g.arbol.addNodo(selected, prop);
            
            return nuevoNodo;
        }

        string doPrevista(int id, string modeloID, string grupo, string email, int width, HttpRequest req)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            Modelo m = g.organizacion.getModeloDocumento(modeloID);
            lock (g)
            {
                List<Propuesta> props = prepararDocumento(g, email, modeloID, id, req);

                //genro prevista para segurarme que defina etiqueta y titulo
                ret = m.toHTML(props, g, email, width, Modelo.eModo.prevista); //las propuesta debe ir en orden de nivel

                //guarpo prevista para poder crearla luego
                if (props.Count > 0)
                {
                    Usuario u = g.getUsuarioHabilitado(email);
                    Prevista prev = new Prevista();
                    prev.etiqueta = m.etiqueta;
                    prev.titulo = m.titulo;
                    prev.propuestas.Clear();
                    foreach(Propuesta p in props)
                        if (p.esPrevista())
                            prev.propuestas.Add(p);
                    u.prevista = prev;
                } //else no ha escrito nada nuevo
            }
            return ret;
        }

        public string doDocumentSubmit(string accion, string parametro, string grupo, string email, string modeloID, int id, int width, HttpRequest req)
        {
            string ret = "";
            Grupo g = app.getGrupo(grupo);
            Modelo m = g.organizacion.getModeloDocumento(modeloID);

            lock (g)
            {
                List<Propuesta> props = prepararDocumento(g, email, modeloID, id, req);
                
                //genro respuesta
                ret = m.documentSubmit(accion, parametro, props, g, email, width, Modelo.eModo.editar); //las propuesta debe ir en orden de nivel

                //guarpo prevista para poder crearla luego
                if (props.Count > 0)
                {
                    Usuario u = g.getUsuarioHabilitado(email);
                    Prevista prev = new Prevista();
                    prev.etiqueta = m.etiqueta;
                    prev.titulo = m.titulo;
                    prev.propuestas.Clear();
                    foreach (Propuesta p in props)
                        if (p.esPrevista())
                            prev.propuestas.Add(p);
                    u.prevista = prev;
                } //else no ha escrito nada nuevo
            }
            return ret;
        }

        private List<Propuesta> prepararDocumento(Grupo g, string email, string modeloID, int id, HttpRequest req)
        {
            //preparo propuestas de nodos ancestros
            List<Propuesta> props = new List<Propuesta>();
            Arbol a = g.arbol;
            List<Nodo> path = a.getPath(id);
            Modelo m = g.organizacion.getModeloDocumento(modeloID);

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
                        //p.niveles = m.niveles; los niveles se completan luego desde el modelo de documeto por si cambia con alguna condicion propia
                        //p.nodoID = id; nodoID=0 determina propuesta prevista, porque esta propuesta aun no tiene nodo
                        p.bag.Add(reqID, m.parse(reqID, Server.HtmlEncode(req[reqID])));

                        previstaProps.Add(p);
                        props.Add(p);
                    }
                }
            }

            return props;
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
            throw new Exception(Tools.tr("Usuario invalido o no habilitado",g.idioma));
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
                Modelo m = g.organizacion.getModeloDocumento(modeloID);
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
                if (p != null)
                {
                    Modelo m = g.organizacion.getModeloDocumento(p.modeloID);
                    ret = m.toHTML(p, g, email, width, Modelo.eModo.ver);
                }
            }
            return ret;
        }

        string doComentar(int id, string grupo, string email, string comentario, bool objecion)
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
                    c.objecion = objecion;
                    p.comentarios.Add(c);

                    Nodo n = a.getNodo(id);
                    n.comentario = true;
                    if (objecion)
                    {
                        n.objecion = true;
                    }
                }
                //retorno el nuevo html de todos los comentarios de ese nodo
                Modelo m = g.organizacion.getModeloDocumento(p.modeloID);
                ret = m.toHTMLComentarios(p.nivel, p, g, email, 330, true);
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
                Usuario u = g.getUsuarioHabilitado(email);
                Flor f = u.getFlor(id);
                List<Nodo> pathn = a.getPath(id);
                if (pathn == null)
                    throw new appException(Tools.tr("Seleccione un nodo",g.idioma));
                else
                {
                    Nodo n = pathn[0];

                    if (f == null)
                    {
                        n.x = x;

                        //no tiene flor en el nodo, la agrego
                        a.asignarflor(u, n);
                                               
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

        string doVariante(int id, string modeloID, string grupo, string email, int width, int finalID)
        {
            string ret = "";
            List<Propuesta> props = new List<Propuesta>();
            Grupo g = app.getGrupo(grupo);
            lock (g)
            {
                //preparo propuestas de nodos ancestros
                Arbol a = g.arbol;
                List<Nodo> path = a.getPath(finalID);
                Nodo nodo = a.getNodo(id); 
                Modelo m = g.organizacion.getModeloDocumento(modeloID);
                g.ts = DateTime.Now;
                foreach (Nodo n in path)
                {
                    Propuesta op = a.getPropuesta(n); //comparo textox con hermanos y resalto palarbas nuevas
                    if (n.nivel > 0 && op != null)
                    {
                        if (n.nivel >= nodo.nivel)
                        {
                            //marco la propuesta de id como prevista
                            op = op.clone(); //nueva propuesta
                            op.email = email;
                            op.nodoID = 0;  //nodoID=0 determina propuesta prevista, porque esta propuesta aun no tiene nodo       
                        }

                        props.Add(op);
                    }
                }

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
                Documento doc = Documento.load(g.path + "\\documentos\\" + ld.carpeta + "\\" + ld.docID.ToString("0000") + "\\" + ld.fname + ".json");
                doc.grupo = g;
                ret = doc.toHTMLSeguimiento(); 
            }
            return ret;
        }

        string doRevisar(int id, string modeloID, string grupo, string email, int width)
        {
            string ret = "";
            List<Propuesta> props = new List<Propuesta>();
            Grupo g = app.getGrupo(grupo);
            Modelo m = g.organizacion.getModeloDocumento(modeloID);

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
                if (u.prevista != null)
                    foreach (Propuesta p in u.prevista.propuestas)
                        props.Add(p);

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
                //obtengo cabeza del debate
                Nodo nodo = a.getNodo(id);
                Nodo debate;
                List<Nodo> path = a.getPath(id);
                if (path.Count >= 2)
                    debate = path[path.Count - 2];
                else
                    debate = nodo;

                if (nodo.consensoAlcanzado)
                    throw new appException(Tools.tr("Este debate ya ha alcanzado el acuerdo", g.idioma));
                else
                {
                    //agrego propuestas de la prevista guardada
                    Usuario u = g.getUsuario(email);
                    foreach (Propuesta p in u.prevista.propuestas)
                    {
                        if (nodo.nivel == 0)
                            p.etiqueta = u.prevista.etiqueta;
                        else
                            p.etiqueta = u.prevista.etiqueta + debate.lastEtiquetaID++;

                        p.titulo = u.prevista.titulo;

                        nodo = a.addNodo(nodo, p);
                        
                        if (nodo.nivel == 1) debate = nodo; //el nuveo nodo es el debate si es un debate nuevo
                    }
                }
                //devuelvo el arbolPersonal
                ret = Tools.toJson(a.getArbolPersonal(email, nodo.id));
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

        Arbol updateArbol(string grupo, int cantidadFlores, float minSiPc, float maxNoPc, string padreURL, string padreNombre, string idioma, string tipoGrupo)
        {
            Grupo g = app.getGrupo(grupo);

            if (minSiPc > 100)
                throw new appException(Tools.tr("Minimos usuarios implicados debe estar entre 50 y 100", g.idioma));
            if (maxNoPc > 50)
                throw new appException(Tools.tr("Máximos usuarios negados debe estar entre 0 y 50", g.idioma));
            if (cantidadFlores > 20 || cantidadFlores < 1)
                throw new appException(Tools.tr("Cantidad de flores", g.idioma));
            Arbol a;

            lock (g)
            {
                if (padreURL.EndsWith("/")) padreURL = padreURL.Substring(0, padreURL.Length - 1);
                if (padreURL == "") padreURL = Request.UrlReferrer.AbsoluteUri.Substring(0, Request.UrlReferrer.AbsoluteUri.LastIndexOf("/"));
                g.padreURL = padreURL;
                g.padreNombre = padreNombre;
                g.idioma = idioma;
                g.tipoGrupo = tipoGrupo;
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