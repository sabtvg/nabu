using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu.organizaciones
{
    public class Plataforma: Organizacion
    {
        public List<nabu.plataforma.GrupoTrabajo> gruposTrabajo = new List<nabu.plataforma.GrupoTrabajo>();
        public List<nabu.plataforma.Accion> acciones = new List<nabu.plataforma.Accion>();
        public string URLEstatuto = "";
        public string objetivo = "";

        public override string doAccion(Grupo g, string email, string accion, HttpRequest req)
        {
            string ret = "";
            lock (this)
            {
                if (accion == "accionAddEstado")
                {
                    int EID = int.Parse(req["EID"]);
                    string estado = req["estado"];
                    string descrip = req.Form["descrip"];
                    foreach (nabu.plataforma.Accion a in acciones)
                        if (a.EID == EID)
                        {
                            nabu.plataforma.Accion.Estado e = new plataforma.Accion.Estado();
                            e.EID = getEID();
                            e.estado = Tools.HtmlEncode(estado);
                            e.descrip = Tools.HtmlEncode(descrip).Replace("\n", "<br>");
                            e.email = email;
                            a.estados.Add(e);

                            //guardo todo el grupo
                            g.save(Tools.server.MapPath("grupos/" + g.nombre));

                            ret = "{\"accion\":\"accionAddEstado\",\"resultado\":\"ok\",\"operativo\":" + getOperativo(g) + "}";
                        }
                }
                else if (accion == "crearArbolParaGrupo")
                {
                    int EID = int.Parse(req["EID"]);
                    string grupoNombre = req["grupoNombre"];
                    Usuario usuarioActual = g.getUsuario(email);
                    foreach (nabu.plataforma.GrupoTrabajo gt in gruposTrabajo)
                        if (gt.EID == EID)
                        {
                            Grupo newg = Grupo.newGrupo(grupoNombre,
                                req["organizacion"],
                                usuarioActual.nombre,
                                usuarioActual.email,
                                usuarioActual.clave, 
                                g.idioma,
                                g.URL);

                            //creo los demas usuarios
                            foreach (string gtemail in gt.integrantes)
                            {
                                Usuario u = g.getUsuario(gtemail);
                                if (u != null && gtemail != email)
                                {
                                    Usuario newu = new Usuario(g.arbol.cantidadFlores);
                                    newu.nombre = u.nombre;
                                    newu.email = u.email;
                                    newu.clave = u.clave;
                                    newu.readOnly = u.readOnly;
                                    newu.isAdmin = u.isAdmin;
                                    newu.isSecretaria = u.isSecretaria;
                                    newu.habilitado = u.habilitado;
                                    newg.usuarios.Add(newu);
                                }
                            }

                            //padre
                            newg.padreURL = g.URL;
                            newg.padreNombre = g.nombre;
                            g.hijos.Add(new Tuple<string,string>(newg.URL, newg.nombre));

                            //apunto datos
                            gt.grupoNombre = newg.nombre;
                            gt.grupoURL = newg.URL;

                            //guardo ambos grupos
                            newg.save(Tools.server.MapPath("grupos/" + newg.nombre));
                            g.save(Tools.server.MapPath("grupos/" + g.nombre));

                            ret = "{\"accion\":\"crearArbolParaGrupo\",\"resultado\":\"ok\",\"operativo\":" + getOperativo(g) + "}";

                            break;
                        }
                }
                else if (accion == "borrarEnlace")
                {
                    int EID = int.Parse(req["EID"]);
                    foreach (nabu.plataforma.GrupoTrabajo gt in gruposTrabajo)
                        if (gt.EID == EID)
                        {
                            gt.grupoNombre = "";
                            gt.grupoURL = "";

                            //guardo 
                            g.save(Tools.server.MapPath("grupos/" + g.nombre));

                            ret = "{\"accion\":\"borrarEnlace\",\"resultado\":\"ok\",\"operativo\":" + getOperativo(g) + "}";

                            break;
                        }
                }
                else if (accion == "accionFinalizar")
                {
                    int EID = int.Parse(req["EID"]);
                    foreach (nabu.plataforma.Accion a in acciones)
                        if (a.EID == EID)
                        {
                            //creo documento de resultado
                            LogDocumento ld = crearResultado(g, a, email);
                            g.logResultados.Add(ld);

                            //quito accion
                            acciones.Remove(a);

                            //guardo todo el grupo
                            g.save(Tools.server.MapPath("grupos/" + g.nombre));

                            ret = "{\"accion\":\"accionFinalizar\",\"resultado\":\"ok\",\"operativo\":" + getOperativo(g) + "}";
                            break;
                        }
                }
            }
            return ret;
        }

        public LogDocumento crearResultado(Grupo gr, nabu.plataforma.Accion ac, string email)
        {
            int docID = 0;
            lock (gr.arbol)
            {
                docID = gr.arbol.lastDocID++;
            }
            string fname = "Resultado_" + docID.ToString("0000");
            string docPath = "resultados\\" + docID.ToString("0000");
            string URL = gr.URL + "/grupos/" + gr.nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

            //documento
            string html = "<html><head></head>";
            //firma Decision
            html = "<body>";
            html += "<h1>Resultado de Accion: " + ac.nombre + "</h1>";
            html += "<h2>Fecha: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</h2><br>";
            html += "<b>" + Tools.tr("Accion", gr.idioma) + ":</b> " + ac.nombre + "<br>";
            html += "<b>" + Tools.tr("Nacido", gr.idioma) + ":</b> " + ac.born + "<br>";
            html += "<b>" + Tools.tr("Decision", gr.idioma) + ":</b> <a href='" + ac.docURL + "' target='_blank'>" + Tools.getURLName(ac.docURL) + "</a><br>";
            html += "<b>" + Tools.tr("Objetivo", gr.idioma) + ":</b><br> " + ac.objetivo + "<br>";
            html += "<b>" + Tools.tr("Estado final", gr.idioma) + ":</b> " + ac.estado + "<br>";
            html += "<b>" + Tools.tr("Estado final fecha", gr.idioma) + ":</b> " + ac.estadoTs + "<br><br>";
                        
            html += "<b>Estados:</b><br>";
            foreach (nabu.plataforma.Accion.Estado es in ac.estados)
            {
                html += "<hr>";
                html += "Estado: " + es.estado + " " + es.email + " " +  es.estadoTs.ToShortDateString() + " " + es.estadoTs.ToShortTimeString() + "<br>";
                html += es.descrip;
            }
            html += "<hr>";
            html += "Documento publicado por: " + email + "<br>";
            html += "Grupo: " + gr.nombre + "<br>";
            html += "Documento ID:" + fname + "<br>";
            html += "Fecha de publicación: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br>";
            html += "Ubicaci&oacute;n: <a target='_blank' href='" + URL + "'>" + URL + "</a><br>";
            html += "Objetivo: " + gr.objetivo + "<br>";
            html += "Usuarios: " + gr.getUsuariosHabilitados().Count + "<br>";
            html += "Activos: " + gr.activos + "<br>";         
            //escribo
            if (!System.IO.Directory.Exists(gr.path + "\\" + docPath))
                System.IO.Directory.CreateDirectory(gr.path + "\\" + docPath);
            System.IO.File.WriteAllText(gr.path + "\\" + docPath + "\\" + fname + ".html", html, System.Text.Encoding.UTF8);

            //retorno meta data
            LogDocumento ld = new LogDocumento();
            ld.fecha = DateTime.Now;
            ld.titulo = "Resultado de Accion: " + ac.nombre;
            ld.icono = "res/documentos/accion.png";
            if (ld.titulo.Length > 50) ld.titulo = ld.titulo.Substring(0, 50);
            ld.modeloNombre = "Accion";
            ld.modeloID = "Accion";
            ld.x = 0;
            ld.docID = docID;
            ld.fname = fname;
            ld.arbol = gr.arbol.nombre;
            ld.objetivo = gr.objetivo;
            ld.flores = 0;
            ld.negados = 0;
            ld.carpeta = "resultados";
            ld.URL = URL;

            return ld;
        }

        public override string getOperativo(Grupo grupo)
        {
            string ret = "{\"objetivo\":\"" + objetivo + "\",";
            ret += "\"URLEstatuto\":\"" + URLEstatuto + "\",";

            Usuario admin = grupo.getAdmin();
            if (admin != null)
                ret += "\"admin\":\"" + admin.email + "\",";
            else
                ret += "\"admin\":\"\",";

            ret += "\"URLEstatuto\":\"" + URLEstatuto + "\",";
            ret += "\"gruposTrabajo\":[";

            foreach (nabu.plataforma.GrupoTrabajo gt in gruposTrabajo)
            {
                ret += "{\"nombre\":\"" + gt.nombre + "\",";
                ret += "\"EID\":" + gt.EID + ",";
                ret += "\"born\":\"" + gt.born.ToShortDateString() + " " + gt.born.ToShortTimeString() + "\",";
                ret += "\"docURL\":\"" + gt.docURL + "\",";
                ret += "\"docTs\":\"" + gt.docTs.ToShortDateString() + " " + gt.docTs.ToShortTimeString() + "\",";
                ret += "\"grupoURL\":\"" + gt.grupoURL + "\",";
                ret += "\"grupoNombre\":\"" + gt.grupoNombre + "\",";
                ret += "\"grupoIdioma\":\"" + gt.grupoIdioma + "\",";
                ret += "\"grupoOrganizacion\":\"" + gt.grupoOrganizacion + "\",";
                ret += "\"revision\":\"" + gt.revision + "\",";
                ret += "\"objetivo\":\"" + gt.objetivo + "\",";

                //usuarios
                ret += "\"usuarios\":[";
                foreach (string email in gt.integrantes)
                {
                    ret += "{\"email\":\"" + email + "\",";
                    Usuario u = grupo.getUsuario(email);
                    if (u == null)
                        ret += "\"estado\":\"NoExiste\"";
                    else
                    {
                        ret += "\"nombre\":\"" + u.nombre + "\"";
                    }
                    ret += "},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "],";

                //procesos
                ret += "\"estrategias\":[";
                foreach (nabu.plataforma.Estrategia pr in gt.estrategias)
                {
                    ret += "{\"nombre\":\"" + pr.nombre + "\",";
                    ret += "\"EID\":" + pr.EID + ",";
                    ret += "\"born\":\"" + pr.born.ToShortDateString() + " " + pr.born.ToShortTimeString() + "\",";
                    ret += "\"docURL\":\"" + pr.docURL + "\",";
                    ret += "\"docTs\":\"" + pr.docTs.ToShortDateString() + " " + pr.docTs.ToShortTimeString() + "\",";
                    ret += "\"revision\":\"" + pr.revision + "\",";
                    ret += "\"definicion\":\"" + pr.definicion + "\",";
                    ret += "\"objetivo\":\"" + pr.objetivo + "\"";
                    ret += "},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "]";
                ret += "},";
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            ret += "],";

            //acciones
            ret += "\"acciones\":[";
            foreach (nabu.plataforma.Accion ac in acciones)
            {
                ret += "{\"nombre\":\"" + ac.nombre + "\",";
                ret += "\"EID\":" + ac.EID + ",";
                ret += "\"born\":\"" + ac.born.ToShortDateString() + " " + ac.born.ToShortTimeString() + "\",";
                ret += "\"docURL\":\"" + ac.docURL + "\",";
                ret += "\"docTs\":\"" + ac.docTs.ToShortDateString() + " " + ac.docTs.ToShortTimeString() + "\",";
                ret += "\"estado\":\"" + ac.estado + "\",";
                ret += "\"estadoTs\":\"" + ac.estadoTs.ToShortDateString() + " " + ac.estadoTs.ToShortTimeString() + "\",";
                ret += "\"estadoEmail\":\"" + ac.estadoEmail + "\",";
                ret += "\"objetivo\":\"" + ac.objetivo + "\",";

                //estados
                ret += "\"estados\":[";
                foreach (nabu.plataforma.Accion.Estado es in ac.estados)
                {
                    ret += "{\"estado\":\"" + es.estado + "\",";
                    ret += "\"EID\":" + es.EID + ",";
                    ret += "\"estadoTs\":\"" + es.estadoTs + "\",";
                    ret += "\"email\":\"" + es.email + "\",";
                    ret += "\"descrip\":\"" + es.descrip + "\"},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "]";
                ret += "},";
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            ret += "]";
            ret += "}";

            return ret;
        }

        public override List<Modelo> getModelos()
        {
            List<Modelo> ret = new List<Modelo>();
            ret.Add(new plataforma.modelos.Manifiesto());
            ret.Add(new plataforma.modelos.GrupoTrabajo());
            ret.Add(new plataforma.modelos.Estrategia());
            ret.Add(new plataforma.modelos.Accion());
            //ret.Add(new plataforma.modelos.Evento());
            //ret.Add(new plataforma.modelos.Proceso());
            return ret;
        }

    }
}