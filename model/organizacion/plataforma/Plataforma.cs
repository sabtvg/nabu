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

namespace nabu.organizaciones
{
    public class Plataforma: Organizacion
    {
        public List<nabu.plataforma.GrupoTrabajo> gruposTrabajo = new List<nabu.plataforma.GrupoTrabajo>();
        public List<nabu.plataforma.Seguimiento> seguimientos = new List<nabu.plataforma.Seguimiento>();
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
                    string avance = req["avance"];
                    string luz = req["luz"];
                    string inicio = req["inicio"];
                    string fin = req["fin"];
                    string descrip = Tools.HTMLLinksBR(Tools.HtmlEncode(req.Form["descrip"]));
                    int favance = 0;
                    DateTime finicio;
                    DateTime ffin;
                    Usuario usuario = g.getUsuario(email);

                    try {favance = int.Parse(avance);} catch(Exception) {throw new appException("Valor inválido para avance");}
                    if (favance < 0 || favance > 100) throw new appException("Valor inválido para avance");
                    try {
                        string[] p = inicio.Split('/');
                        finicio = new DateTime(Convert.ToInt32(p[2]),
                            Convert.ToInt32(p[1]),
                            Convert.ToInt32(p[0])); 
                    }
                    catch (Exception) { throw new appException("Valor inválido para inicio"); } 
                    try {
                        string[] p = fin.Split('/');
                        ffin = new DateTime(Convert.ToInt32(p[2]),
                            Convert.ToInt32(p[1]),
                            Convert.ToInt32(p[0])); 

                    } 
                    catch (Exception) { throw new appException("Valor inválido para fin"); }
                    if (finicio > ffin) throw new appException("Plazo inválido");

                    foreach (nabu.plataforma.Seguimiento a in seguimientos)
                        if (a.EID == EID)
                        {
                            nabu.plataforma.Seguimiento.Estado e = new plataforma.Seguimiento.Estado();
                            e.EID = getEID();
                            e.estado = Tools.HtmlEncode(estado);
                            e.avance = favance;
                            e.luz = Tools.HtmlEncode(luz);
                            e.inicio = finicio;
                            e.fin = ffin;
                            e.descrip = descrip;
                            e.email = email;
                            if (usuario != null) e.nombreUsuario = usuario.nombre;
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
                            Hijo h = new Hijo();
                            h.URL = newg.URL;
                            h.nombre = newg.nombre;
                            g.hijos.Add(h);

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
                else if (accion == "seguimientoFinalizar")
                {
                    int EID = int.Parse(req["EID"]);
                    foreach (nabu.plataforma.Seguimiento a in seguimientos)
                        if (a.EID == EID)
                        {
                            //creo documento de resultado
                            LogDocumento ld = crearResultado(g, a, email);
                            g.logResultados.Add(ld);

                            //quito accion
                            seguimientos.Remove(a);

                            //alertas
                            foreach (Usuario u in g.getUsuariosHabilitados())
                                if (u.email != email)
                                    u.alertas.Add(new Alerta(Tools.tr("Accion finalizada [%1]", a.nombre, g.idioma)));
                            //guardo todo el grupo
                            g.save(Tools.server.MapPath("grupos/" + g.nombre));

                            ret = "{\"accion\":\"seguimientoFinalizar\",\"resultado\":\"ok\",\"operativo\":" + getOperativo(g) + "}";
                            break;
                        }
                }
            }
            return ret;
        }

        public LogDocumento crearResultado(Grupo gr, nabu.plataforma.Seguimiento ac, string email)
        {
            int docID = 0;
            lock (gr.arbol)
            {
                docID = gr.arbol.lastDocID++;
            }
            string fname = "Resultado_" + docID.ToString("0000");
            string docPath = "resultados\\" + docID.ToString("0000");
            string URL = gr.URL + "/grupos/" + gr.nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

            //creo carpeta
            if (!System.IO.Directory.Exists(gr.path + "\\" + docPath))
                System.IO.Directory.CreateDirectory(gr.path + "\\" + docPath);

            //creo documento json
            Documento doc = new Documento();
            doc.fecha = DateTime.Now;
            doc.nombre = ac.nombre;
            doc.fname = fname;
            doc.modeloID = "";
            doc.path = gr.path + "\\" + docPath + "\\" + fname + ".json";
            doc.URLPath = URL;
            doc.titulo = "Resultado de: " + ac.nombre;

            Propuesta prop = new Propuesta();
            prop.bag["fecha"] = DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString();
            prop.bag["Seguimiento"] = ac.nombre;
            prop.bag["Nacido"] = ac.born;
            prop.bag["Decision"] = ac.docURL;
            prop.bag["Objetivo"] = ac.objetivo;
            prop.bag["Estado final"] = ac.estado.estado;
            prop.bag["Estado final fecha"] = ac.estado.estadoTs;
            prop.bag["Autor"] = email;

            int q = 1;
            for (int i = ac.estados.Count - 1; i >= 0; i--)
            {
                nabu.plataforma.Seguimiento.Estado es = ac.estados[i];
                prop.bag["Estado" + i] = es.estado;
                prop.bag["Email" + i] = es.email;
                prop.bag["EstadoTs" + i] = es.estadoTs.ToString("dd/MM/yy") + " " + es.estadoTs.ToShortTimeString();
            }
            List<Propuesta> props = new List<Propuesta>();
            props.Add(prop);
            doc.propuestas = props;
            doc.save();

            //HTML
            string html = "<html><head></head>";
            //firma Decision
            html = "<body>";
            html += "<h1>Resultado de: " + ac.nombre + "</h1>";
            html += "<h2>Fecha: " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</h2><br>";
            html += "<b>" + Tools.tr("Accion", gr.idioma) + ":</b> " + ac.nombre + "<br>";
            html += "<b>" + Tools.tr("Nacido", gr.idioma) + ":</b> " + ac.born + "<br>";
            html += "<b>" + Tools.tr("Decision", gr.idioma) + ":</b> <a href='" + ac.docURL + "' target='_blank'>" + Tools.getURLName(ac.docURL) + "</a><br>";

            string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(ac.objetivo.Replace("\n", "<br>")));
            html += "<b>" + Tools.tr("Objetivo", gr.idioma) + ":</b><br> " + HTMLText + "<br>";
            html += "<b>" + Tools.tr("Estado final", gr.idioma) + ":</b> " + ac.estado.estado + "<br>";
            html += "<b>" + Tools.tr("Estado final fecha", gr.idioma) + ":</b> " + ac.estado.estadoTs + "<br><br>";
            html += "<b>" + Tools.tr("Autor", gr.idioma) + ":</b> " + email + "<br><br>";
                        
            html += "<b>Estados:</b><br>";
            for (int i = ac.estados.Count - 1; i >=0; i--) 
            {
                nabu.plataforma.Seguimiento.Estado es = ac.estados[i];
                html += "<hr>";
                html += "Estado: " + es.estado + " " + es.email + " " + es.estadoTs.ToString("dd/MM/yy") + " " + es.estadoTs.ToShortTimeString() + "<br>";
                html += es.descrip;
            }
            html += "<hr>";
            html += "Documento publicado por: " + email + "<br>";
            html += "Grupo: " + gr.nombre + "<br>";
            html += "Documento ID:" + fname + "<br>";
            html += "Fecha de publicación: " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "<br>";
            html += "Ubicaci&oacute;n: <a target='_blank' href='" + URL + "'>" + URL + "</a><br>";
            html += "Objetivo: " + gr.objetivo + "<br>";
            html += "Usuarios: " + gr.getUsuariosHabilitados().Count + "<br>";
            html += "Activos: " + gr.activos + "<br>";         
            //escribo
            System.IO.File.WriteAllText(gr.path + "\\" + docPath + "\\" + fname + ".html", html, System.Text.Encoding.UTF8);

            //retorno meta data
            LogDocumento ld = new LogDocumento();
            ld.fecha = DateTime.Now;
            ld.titulo = "Resultado de: " + ac.nombre;
            ld.documentoNombre = ac.nombre;
            ld.icono = ac.icono;
            if (ld.titulo.Length > 70) ld.titulo = ld.titulo.Substring(0, 70);
            ld.modeloNombre = ac.modeloNombre;
            ld.modeloID = ac.modeloID;
            ld.x = 0;
            ld.docID = docID;
            ld.fname = fname;
            ld.arbol = gr.arbol.nombre;
            ld.objetivo = gr.objetivo;
            ld.flores = 0;
            ld.negados = 0;
            ld.carpeta = "resultados";
            ld.URL = URL;
            ld.autor = email;

            return ld;
        }

        public override string getOperativo(Grupo grupo)
        {
            string ret = "{\"objetivo\":" + Tools.toJson(objetivo) + ",";
            ret += "\"URLEstatuto\":\"" + URLEstatuto + "\",";

            Usuario admin = grupo.getAdmin();
            if (admin != null)
                ret += "\"admin\":{\"nombre\":\"" + admin.nombre + "\",\"email\":\"" + admin.email + "\",\"funcion\":\"" + admin.funcion + "\"},";
            else
                ret += "\"admin\":null,";

            Usuario secretaria = grupo.getSecretaria();
            if (secretaria != null)
                ret += "\"secretaria\":{\"nombre\":\"" + secretaria.nombre + "\",\"email\":\"" + secretaria.email + "\",\"funcion\":\"" + secretaria.funcion + "\"},";
            else
                ret += "\"secretaria\":null,";

            Usuario facilitador = grupo.getFacilitador();
            if (facilitador != null)
                ret += "\"facilitador\":{\"nombre\":\"" + facilitador.nombre + "\",\"email\":\"" + facilitador.email + "\",\"funcion\":\"" + facilitador.funcion + "\"},";
            else
                ret += "\"facilitador\":null,";

            List<Usuario> reps = grupo.getRepresentantes();
            if (reps.Count == 0)
                ret += "\"representantes\":[],";
            else
            {
                ret += "\"representantes\":[";
                foreach(Usuario rep in reps)
                    ret += "{\"nombre\":\"" + rep.nombre + "\",\"email\":\"" + rep.email + "\",\"funcion\":\"" + rep.funcion + "\"},";
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "],";
            }

            ret += "\"URLEstatuto\":\"" + URLEstatuto + "\",";
            ret += "\"gruposTrabajo\":[";

            foreach (nabu.plataforma.GrupoTrabajo gt in gruposTrabajo)
            {
                ret += "{\"nombre\":\"" + gt.nombre + "\",";
                ret += "\"EID\":" + gt.EID + ",";
                ret += "\"born\":\"" + gt.born.ToString("dd/MM/yy") + " " + gt.born.ToShortTimeString() + "\",";
                ret += "\"docURL\":\"" + gt.docURL + "\",";
                ret += "\"docTs\":\"" + gt.docTs.ToString("dd/MM/yy") + " " + gt.docTs.ToShortTimeString() + "\",";
                ret += "\"grupoURL\":\"" + gt.grupoURL + "\",";
                ret += "\"grupoNombre\":\"" + gt.grupoNombre + "\",";
                ret += "\"grupoIdioma\":\"" + gt.grupoIdioma + "\",";
                ret += "\"grupoOrganizacion\":\"" + gt.grupoOrganizacion + "\",";
                ret += "\"revision\":\"" + gt.revision + "\",";

                string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(gt.objetivo.Replace("\n", "<br>")));
                ret += "\"objetivo\":" + Tools.toJson(HTMLText) + ",";

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
                        ret += "\"nombre\":\"" + u.nombre + "\",";
                        ret += "\"funcion\":\"" + u.funcion + "\"";
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
                    ret += "\"born\":\"" + pr.born.ToString("dd/MM/yy") + " " + pr.born.ToShortTimeString() + "\",";
                    ret += "\"docURL\":\"" + pr.docURL + "\",";
                    ret += "\"docTs\":\"" + pr.docTs.ToString("dd/MM/yy") + " " + pr.docTs.ToShortTimeString() + "\",";
                    ret += "\"revision\":\"" + pr.revision + "\",";

                    HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(pr.definicion.Replace("\n", "<br>")));
                    ret += "\"definicion\":" + Tools.toJson(HTMLText) + ",";

                    HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(pr.objetivo.Replace("\n", "<br>")));
                    ret += "\"objetivo\":" + Tools.toJson(HTMLText);
                    ret += "},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "]";
                ret += "},";
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            ret += "],";

            //seguimientos
            ret += "\"seguimientos\":[";
            foreach (nabu.plataforma.Seguimiento ac in seguimientos)
            {

                ret += "{\"nombre\":\"" + ac.nombre + "\",";
                ret += "\"EID\":" + ac.EID + ",";
                ret += "\"born\":\"" + ac.born.ToString("dd/MM/yy") + " " + ac.born.ToShortTimeString() + "\",";
                ret += "\"docURL\":\"" + ac.docURL + "\",";
                ret += "\"docTs\":\"" + ac.docTs.ToString("dd/MM/yy") + " " + ac.docTs.ToShortTimeString() + "\",";
                ret += "\"estado\":" + Tools.toJson(ac.estado) + ",";
                ret += "\"estadoTs\":\"" + ac.estado.estadoTs.ToString("dd/MM/yy") + " " + ac.estado.estadoTs.ToShortTimeString() + "\",";
                ret += "\"estadoEmail\":\"" + ac.estadoEmail + "\",";

                string HTMLText = Tools.HTMLDecode(Tools.HTMLDecode(ac.objetivo.Replace("\n", "<br>")));
                ret += "\"objetivo\":" + Tools.toJson(HTMLText) + ",";
                ret += "\"responsable\":\"" + ac.responsable + "\",";

                //estados
                ret += "\"estados\":[";
                foreach (nabu.plataforma.Seguimiento.Estado es in ac.estados)
                {
                    ret += Tools.toJson(es) + ",";
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

        public override List<object> getSeriealizableObjects()
        {
            List<object> ret = new List<object>();
            ret.Add(new plataforma.Accion());
            ret.Add(new plataforma.Evento());
            return ret;
        }

        public override List<Modelo> getModelosDocumento()
        {
            List<Modelo> ret = new List<Modelo>();
            ret.Add(new plataforma.modelos.Manifiesto());
            ret.Add(new plataforma.modelos.GrupoTrabajo());
            ret.Add(new plataforma.modelos.Estrategia());
            ret.Add(new plataforma.modelos.Accion());
            ret.Add(new plataforma.modelos.Evento());
            ret.Add(new plataforma.modelos.AlPadre());
            ret.Add(new plataforma.modelos.AlHijo());
            ret.Add(new plataforma.modelos.Acta());
            //ret.Add(new plataforma.modelos.Proceso());
            return ret;
        }

        public override List<ModeloEvaluacion> getModelosEvaluacion()
        {
            List<ModeloEvaluacion> ret = new List<ModeloEvaluacion>();
            ret.Add(new plataforma.modelosEvaluacion.Accion());
            ret.Add(new plataforma.modelosEvaluacion.Evento());
            ret.Add(new plataforma.modelosEvaluacion.Enlace());
            ret.Add(new plataforma.modelosEvaluacion.AlPadre());
            ret.Add(new plataforma.modelosEvaluacion.AlHijo());
            return ret;
        }
    }
}