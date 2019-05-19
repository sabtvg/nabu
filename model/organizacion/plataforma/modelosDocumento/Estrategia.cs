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
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class Estrategia: Modelo
    {
        public Estrategia()
        {
            icono = "res/documentos/estrategia.png";
            niveles = 5;
            nombre = "Estrategia";
            descripcion = "Estrategia operativa";
            tipo = "estructura";
            versionar = "titulo";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Estrategia";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            Variable v = new Variable("s.nombreProceso", 30, 1);
            v.className = "textoBig";
            v.editClassName = "editarBig";
            variables.Add(v);
            variables.Add(new Variable("r.accion", 6, 1));

            variables.Add(new Variable("s.introduccion", 3000, 1));

            //nivel 2
            variables.Add(new Variable("s.aquien", 3000, 2));
			variables.Add(new Variable("s.consecuencias", 3000, 2));
			
            //nivel 3
            variables.Add(new Variable("s.definicion", 5000, 3));
            variables.Add(new Variable("s.conclusiones", 3000, 3));
			
            //nivel 4
            variables.Add(new Variable("s.SubGrupo", 60, 4));
            variables.Add(new Variable("s.implantacion", 3000, 4));

            //nivel 5
            variables.Add(new Variable("s.eficiencia", 3000, 5));
            variables.Add(new Variable("s.revision", 0, 5));
        }

        private void validar(Propuesta prop)
        {
            if (prop != null)
            {
                if (prop.nivel == 1)
                {
                    if (getText("s.etiqueta", prop) == "")
                    {
                        addError(1, "La etiqueta determina el nombre con que aparece en el arbol, no puede ser vacia");
                        getVariable("s.etiqueta").className = "errorfino";
                    }
                    if (getText("s.nombreProceso", prop) == "")
                    {
                        addError(1, "El nombre del proceso no puede ser vacio");
                        getVariable("s.nombreProceso").className = "errorfino";
                    }
                    if (getText("s.introduccion", prop) == "")
                    {
                        addError(1, "La introduccion no puede ser vacia");
                        getVariable("s.introduccion").className = "errorfino";
                    }
                }
                else if (prop.nivel == 2)
                {
                    if (getText("s.aquien", prop) == "" 
                        && getText("s.consecuencias", prop) == "")
                    {
                        addError(2, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 3)
                {
                    if (getText("r.accion", prop) != "borrar" && getText("s.definicion", prop) == "")
                    {
                        addError(3, "Debes definir el proceso");
                        getVariable("s.definicion").className = "errorfino";
                    }

                    if (getText("s.definicion", prop) == ""
                        && getText("s.conclusiones", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("r.accion", prop) == "nuevo" && getText("s.SubGrupo", prop) == "")
                    {
                        addError(4, "Debes seleccionar un grupo de trabajo");
                        getVariable("s.SubGrupo").className = "errorfino";
                    }

                    if (getText("s.implantacion", prop) == "" 
                        && getText("s.SubGrupo", prop) == "")
                    {
                        addError(4, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.revision", prop) == "")
                    {
                        addError(5, "Debes seleccionar un periodo de revisi&oacute;n");
                        getVariable("s.revision").className = "errorfino";
                    }

                    if (getText("s.eficiencia", prop) == "" && getText("s.revision", prop) == "")
                    {
                        addError(5, "La propuesta no puede estar completamente vacia");
                    }
                }
            }
        }

        override protected string HTMLEncabezado(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuarioHabilitado(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            titulo = getText("s.nombreProceso", prop);
            etiqueta = getText("s.etiqueta", prop);

            //valor default para tipo
            organizaciones.Plataforma plata = (organizaciones.Plataforma)grupo.organizacion;

            //titulo
            ret += "<div class='titulo1'><nobr>" + nombre + "</nobr></div>";

            //fecha
            if (modo == eModo.consenso)
                ret += "<div class='titulo3'><nobr>" + Tools.tr("Fecha", g.idioma) + ": " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

            //abm controls
            ret += HTMLABM("s.nombreProceso", prop, width, tieneFlores, getListaPRs(), g.idioma);
            ret += "<br>";

            //etiqueta
            ret += "<div class='titulo3'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ": " + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma);
            if (prop == null)
                ret += "&nbsp;<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
            ret += "</nobr></div><br><br>";
            return ret;
        }

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width, Propuesta propFinal)
        {
            string ret = "";
            Usuario u = g.getUsuario(email);
            bool tieneFlores = false;
            if (u != null) tieneFlores = u.floresDisponibles().Count > 0;

            bool editar = (prop == null && tieneFlores && modo != eModo.prevista && modo != eModo.consenso)
                || (prop != null && prop.esPrevista() && (modo == eModo.revisar || modo == eModo.editar));
            editar = editar && !consensoAlcanzado;
            bool puedeVariante = prop != null && !prop.esPrevista() && modo == eModo.editar && tieneFlores && !consensoAlcanzado;


            //accion
            if (prop != null)
            {
                if (getText("r.accion", prop) != "")
                    accion = getText("r.accion", prop);
                else
                    prop.bag["r.accion"] = accion;
            }
            if (accion == "borrar")
                niveles = 3;
            else
                niveles = 5;

            //validaciones de este nivel
            if (modo == eModo.prevista) validar(prop);

            if (nivel == 1)
            {              
                ret += HTMLEncabezado(prop, g, email, width);

                //tema
                ret += "<div class='tema'>" + Tools.tr("proceso.introduccion.titulo", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip'>"
                        + Tools.tr("proceso.introduccion.tip", g.idioma) 
                        + "</div>";
                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 2)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("proceso.consecuencias.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("proceso.consecuancias.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.consecuencias", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //A quien va dirigido
                    ret += "<div class='tema'>" + Tools.tr("proceso.aquien.titulo", g.idioma) + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip'>"
                            + Tools.tr("proceso.aquien.tip", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
                }
            }
            else if (nivel == 3)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("proceso.conclusiones.titulo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("proceso.conclusiones.tip", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.conclusiones", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //definicion
                    ret += "<div class='tema'>" + Tools.tr("proceso.definicion.titulo", g.idioma) + "</div>";
                    if (editar)  
                        ret += "<div class='smalltip'>"
                            + Tools.tr("proceso.definicion.tip", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.definicion", prop, width, 220, tieneFlores, g.idioma);
                }

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 4)
            {
                //guardo la accion en cada nivel porque su representacion depende de este valor
                if (prop != null && accion != "")
                    prop.bag["r.accion"] = accion;
                else if (prop != null && accion == "" && prop.bag.ContainsKey("r.accion"))
                    accion = getText("r.accion", prop);

                //grupo de trabajo
                ret += "<div class='tema'>" + Tools.tr("proceso.grupo.titulo", g.idioma) + "</div>";
                if (accion == "nuevo")
                {
                    if (editar)
                        ret += "<div class='smalltip'>"
                            + Tools.tr("proceso.grupo.tip", g.idioma)
                            + "</div>";
                    ret += HTMLLista("s.SubGrupo", getListaGTs(), prop, 300, tieneFlores, g.idioma, false);
                }
                else
                {
                    ret += HTMLListaReadonly("s.SubGrupo", getListaGTs(), prop, 300, true, g.idioma);
                }

                //implantacion
                ret += "<div class='tema'>" + Tools.tr("proceso.implantacion.titulo", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip'>"
                        + Tools.tr("proceso.implantacion.tip", g.idioma) 
                        + "</div>";
                ret += HTMLArea("s.implantacion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + Tools.tr("proceso.valoracion.titulo", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("proceso.valoracion.tip", g.idioma)
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores, g.idioma);

                ret += "<div class='tema'>" + Tools.tr("Revision de valoracion del resultado", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip'>"
                        + Tools.tr("proceso.revision.tip", g.idioma)
                        + "</div>";
                ret += HTMLLista("s.revision", "|Mensual|Trimestral|Semestral|Anual", prop, 250, tieneFlores, g.idioma, false);

                //variante
                if (puedeVariante) ret += HTMLVariante(prop.nodoID, g, propFinal.nodoID);
            }
            else
            {
                throw new Exception("Nivel [" + nivel + "] no existe en este modelo");
            }

            if (prop != null) prop.niveles = niveles; //esto es importante si cambian los niveles para que se traspase luego al nodo

            //fin nivel
            if (prop != null && prop.nodoID != 0 && modo != eModo.consenso)
                ret += HTMLFlores(g.arbol.getNodo(prop.nodoID), false, g.getUsuario(email));

            //mensajes de error
            if (errores.ContainsKey(nivel))
            {
                ret += "<div class='error'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override int getRevisionDias(List<Propuesta> props)
        {
            int ret = 0;
            foreach (Propuesta p in props)
                foreach (string key in p.bag.Keys)
                    if (key == "s.revision")
                        switch ((string)p.bag[key])
                        {
                            case "Mensual":
                                ret = 30;
                                break;
                            case "Trimestral":
                                ret = 90;
                                break;
                            case "Semestral":
                                ret = 180;
                                break;
                            case "Anual":
                                ret = 365;
                                break;
                        }
            return ret;
        }

        public override void ejecutarConsenso(Documento doc)
        {
            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            if ((string)doc.getValor("r.accion") == "borrar")
            {
                //borro estrategia
                string SubGrupo = doc.titulo.Split('.')[0];
                string estrategia = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
                    if (gt.nombre == SubGrupo)
                        foreach (nabu.plataforma.Estrategia pr in gt.estrategias)
                            if (pr.nombre == Tools.HTMLDecode(estrategia))
                            {
                                gt.estrategias.Remove(pr);
                                doc.addLog(Tools.tr("estrategia.eliminada", SubGrupo, doc.grupo.idioma));
                                break;
                            }
            }
            else if ((string)doc.getValor("r.accion") == "existente")
            {
                //creo/actualizo SubGrupo actual
                string SubGrupo = doc.titulo.Split('.')[0];
                string estrategia = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
                    if (gt.nombre == SubGrupo)
                    {
                        foreach (nabu.plataforma.Estrategia pr in gt.estrategias)
                            if (pr.nombre == Tools.HTMLDecode(estrategia))
                            {
                                //actualizo
                                pr.docURL = doc.URLPath; //nuevo consenso
                                pr.docTs = DateTime.Now;
                                pr.revision = doc.getText("s.revision");
                                pr.definicion = doc.getText("s.definicion");

                                doc.addLog(Tools.tr("estrategia.actualizada", SubGrupo, doc.grupo.idioma));
                            }
                    }
            } 
            else
            {
                //nuevo
                string SubGrupo = (string)doc.getValor("s.SubGrupo");
                foreach (nabu.plataforma.SubGrupo gt in plataforma.subgrupos)
                    if (gt.nombre == Tools.HTMLDecode(SubGrupo))
                    {
                        //creo
                        nabu.plataforma.Estrategia pr = new plataforma.Estrategia();
                        pr.EID = plataforma.getEID();
                        pr.nombre = doc.titulo;
                        pr.docURL = doc.URLPath;
                        pr.docTs = DateTime.Now;
                        pr.revision = doc.getText("s.revision");
                        pr.definicion = doc.getText("s.definicion");

                        gt.estrategias.Add(pr);

                        doc.addLog(Tools.tr("estrategia.creada", SubGrupo, doc.grupo.idioma));
                    }
            }
        }
        
        private string getListaPRs()
        {
            string ret = "";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.SubGrupo gt in pl.subgrupos)
                foreach (plataforma.Estrategia pr in gt.estrategias)
                    ret += gt.nombre + "." + pr.nombre + "|";
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getListaGTs()
        {
            string ret = "|";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.SubGrupo gt in pl.subgrupos)
            {
                ret += gt.nombre + "|";
            }
            if (ret.EndsWith("|")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getPrimerPR()
        {
            string prs = getListaPRs();
            if (prs != "")
                return prs.Split('|')[0];
            else
                return "";
        }

        public override string documentSubmit(string accion, string parametro, List<Propuesta> props, Grupo g, string email, int width, Modelo.eModo modo)
        {
            this.grupo = g;

            if (accion == "r.accion_click" && parametro == "nuevo" && props.Count > 0)
            {
                for (int i = 1; i < props.Count - 1; i++)
                    props.RemoveAt(i);

                props[0].bag.Clear();
                props[0].bag["r.accion"] = "nuevo";
            }
            else if (accion == "r.accion_click" && parametro == "existente" && props.Count > 0)
            {
                //traer datos del coumento seleccionado si es una modificaicon
                string nombre;
                if (props[0].bag.ContainsKey("s.nombreProceso"))
                    nombre = (string)props[0].bag["s.nombreProceso"];
                else
                    nombre = getPrimerPR();

                getContenidoDocumentoPrevio(nombre, props, g);
            }
            else if (accion == "s.nombreProceso_click")
            {
                if ((string)props[0].bag["r.accion"] == "existente")
                {
                    //traer datos si es una modificaicon
                    string nombre = (string)props[0].bag["s.nombreProceso"];
                    getContenidoDocumentoPrevio(nombre, props, g);
                }
            }

            return toHTML(props, g, email, width, modo);
        }

        public void getContenidoDocumentoPrevio(string grupoTitulo, List<Propuesta> props, Grupo grupo)
        {
            //si es una modificaicon y el primero nivel esta vacio entonces traigo los datos del documento a modificar
            //busco en el logDocumentos la ultima version
            Documento lastDoc = null;
            if (grupoTitulo != "" && grupoTitulo.IndexOf(".") > 0)
            {
                for (int i = grupo.logDecisiones.Count -1; i > 0; i--)
                {
                    LogDocumento ldi = grupo.logDecisiones[i];
                    ////path fix if server is different
                    //string path = ldi.path;
                    //if (!path.StartsWith(Tools.startupPath))
                    //    //fix
                    //    path = Tools.startupPath + path.Substring(path.IndexOf("\\nabu\\") + 5);

                    if (ldi.modeloNombre == nombre && System.IO.File.Exists(grupo.path + "\\" + ldi.path))
                    {
                        try
                        {
                            string json = System.IO.File.ReadAllText(grupo.path + "\\" + ldi.path);
                            Documento doc = Tools.fromJson<Documento>(json);
                            //string eSubGrupo = doc.getText("s.SubGrupo");
                            string eNombreProceso = doc.getText("s.nombreProceso");
                            //string titulo = grupoTitulo.Split('.')[1];
                            //string subGrupo = grupoTitulo.Split('.')[0];
                            //if (Tools.HTMLDecode(eSubGrupo) == Tools.HTMLDecode(subGrupo) && Tools.HTMLDecode(eNombreProceso) == Tools.HTMLDecode(titulo))
                            if (Tools.HTMLDecode(eNombreProceso) == Tools.HTMLDecode(grupoTitulo))
                            {
                                if (lastDoc == null)
                                    lastDoc = doc;
                                else if (lastDoc.fecha < ldi.fecha)
                                    lastDoc = doc;
                            }
                        }
                        catch (Exception ex)
                        {}
                    }                 
                }
                //traigo datos de este doc
                //server debug only
                //if (ldi.path.StartsWith("h:")) ldi.path = grupo.path + "\\" + ldi.path.Substring(33); //debug only
                //server debug only
                //esta es la estretagia que me piden
                //traigo datos de este doc
                //agrego contenido
                if (lastDoc != null)
                {
                    props.Clear();
                    foreach (Propuesta prop in lastDoc.propuestas)
                    {
                        prop.nodoID = 0;
                        prop.consensoAlcanzado = false;
                        props.Add(prop);
                    }
                    props[0].bag["s.nombreProceso"] = grupoTitulo;

                    if (props.Count > 0)
                        props[0].bag["r.accion"] = "existente"; //este valor permanece
                }
                else
                    props.Clear();
            }
        }
    }
}

