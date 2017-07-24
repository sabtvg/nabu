using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class Estrategia: Modelo
    {
        private string accion = ""; //para que existe en otros nivels mientras dibujo

        public Estrategia()
        {
            icono = "res/documentos/estrategia.png";
            niveles = 5;
            nombre = "Estrategia";
            descripcion = "Estrategia operativa";

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
            variables.Add(new Variable("s.objetivo", 3000, 2));
            variables.Add(new Variable("s.descripcion", 3000, 2));
            variables.Add(new Variable("s.aquien", 3000, 2));
			variables.Add(new Variable("s.consecuencias", 3000, 2));
			
            //nivel 3
            variables.Add(new Variable("s.definicion", 5000, 3));
            variables.Add(new Variable("s.conclusiones", 3000, 3));
			
            //nivel 4
            variables.Add(new Variable("s.grupoTrabajo", 60, 4));
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
                    if (getText("s.objetivo", prop) == ""
                        && getText("s.descripcion", prop) == ""
                        && getText("s.aquien", prop) == "" 
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
                    if (getText("r.accion", prop) == "nuevo" && getText("s.grupoTrabajo", prop) == "")
                    {
                        addError(4, "Debes seleccionar un grupo de trabajo");
                        getVariable("s.grupoTrabajo").className = "errorfino";
                    }

                    if (getText("s.implantacion", prop) == "" 
                        && getText("s.grupoTrabajo", prop) == "")
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

            //nombre nuevo o existente o borrar
            //nuevo
            ret += "<table>";
            ret += "<tr>";
            ret += "<td colspan=2 class='titulo2'><nobr>" + Tools.tr("accion", g.idioma) + "</nobr></td>";
            ret += "<td colspan=2 class='titulo2'><nobr>" + Tools.tr("Nombre", g.idioma) + "</nobr></td>";
            ret += "</tr>";

            ret += "<tr>";
            ret += "<td>" + HTMLRadio("r.accion", 1, prop, tieneFlores, "nuevo", g.idioma) + "</td>";
            ret += "<td style='vertical-align:middle'>" + Tools.tr("Crear nueva estrategia", g.idioma) + "</td>";
            ret += "<td class='titulo2'>";
            //nombre del grupo
            if (prop != null && accion == "nuevo")
            {
                ret += HTMLText("s.nombreProceso", prop, 40 * 8, tieneFlores, g.idioma);
            }
            ret += "</td>";
            ret += "</tr>";

            //existente
            string listaPRs = getListaPRs();
            if (listaPRs != "")
            {
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 2, prop, tieneFlores, "existente", g.idioma) + "</td>";
                ret += "<td style='vertical-align:middle'>" + Tools.tr("Modificar estrategia", g.idioma) + "</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "existente")
                {
                    ret += HTMLLista("s.nombreProceso", listaPRs, prop, 80 * 8, tieneFlores, g.idioma);
                }
                ret += "</tr>";

                //borrar
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 3, prop, tieneFlores, "borrar", g.idioma) + "</td>";
                ret += "<td style='vertical-align:middle'>" + Tools.tr("Eliminar estrategia", g.idioma) + "</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "borrar")
                {
                    ret += HTMLLista("s.nombreProceso", listaPRs, prop, 80 * 8, tieneFlores, g.idioma);
                }
                ret += "</tr>";
            }
            ret += "</table><br>";

            //etiqueta
            ret += "<div class='titulo2'><nobr>" + Tools.tr("Etiqueta", g.idioma) + ":" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores, g.idioma);
            if (prop == null)
                ret += "<span style='color:gray;font-size:12px;'>" + Tools.tr("(Etiqueta en el arbol)", g.idioma) + "</span>";
            ret += "</nobr></div>";
            return ret;
        }

        override protected string toHTMLContenido(int nivel, Propuesta prop, Grupo g, string email, int width)
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

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + Tools.tr("Fecha", g.idioma) + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + Tools.tr("Introduccion", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("proceso.introduccion", g.idioma) 
                        + "</div>";
                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 2)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("Consecuencias", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.consecuancias", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.consecuencias", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //Objetivo a lograr
                    ret += "<div class='tema'>" + Tools.tr("Objetivo a lograr", g.idioma) + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.objetivo", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores, g.idioma);

                    //Descripcion
                    ret += "<div class='tema'>" + Tools.tr("Descripcion", g.idioma) + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.descripcion", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores, g.idioma);

                    //A quien va dirigido
                    ret += "<div class='tema'>" + Tools.tr("A quien va dirigido", g.idioma) + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>" 
                            + Tools.tr("Quienes se beneficiaran con los resultados de utilizar este proceso operativo", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores, g.idioma);

                    //variante
                    if (puedeVariante) 
                        ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
                }
            }
            else if (nivel == 3)
            {
                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + Tools.tr("Conclusiones", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.conclusiones", g.idioma)
                            + "</div>";
                    ret += HTMLArea("s.conclusiones", prop, width, 120, tieneFlores, g.idioma);
                }
                else
                {
                    //definicion
                    ret += "<div class='tema'>" + Tools.tr("Definicion de la estrategia", g.idioma) + "</div>";
                    if (editar)  
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.definicion", g.idioma) 
                            + "</div>";
                    ret += HTMLArea("s.definicion", prop, width, 220, tieneFlores, g.idioma);
                }

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                //guardo la accion en cada nivel porque su representacion depende de este valor
                if (prop != null && accion != "")
                    prop.bag["r.accion"] = accion;
                else if (prop != null && accion == "" && prop.bag.ContainsKey("r.accion"))
                    accion = getText("r.accion", prop);

                if (accion == "nuevo")
                {
                    //grupo de trabajo
                    ret += "<div class='tema'>" + Tools.tr("Grupo de trabajo", g.idioma) + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + Tools.tr("proceso.grupo", g.idioma)
                            + "</div>";
                    ret += HTMLLista("s.grupoTrabajo", getListaGTs(), prop, width, tieneFlores, g.idioma);
                }

                //implantacion
                ret += "<div class='tema'>" + Tools.tr("Implantacion", g.idioma) + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("proceso.implantacion", g.idioma) 
                        + "</div>";
                ret += HTMLArea("s.implantacion", prop, width, 120, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + Tools.tr("Valoracion del resultado", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("proceso.valoracion", g.idioma)
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores, g.idioma);

                ret += "<div class='tema'>" + Tools.tr("Revision de valoracion del resultado", g.idioma) + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + Tools.tr("proceso.revision", g.idioma)
                        + "</div>";
                ret += HTMLLista("s.revision", ":Mensual:Trimestral:Semestral:Anual", prop, 250, tieneFlores, g.idioma);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + Tools.tr("Proponer variante", g.idioma) + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
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
                ret += "<div class='error' style='width:" + (width-4) + "px'>" + errores[nivel] + "</div>";
            }
            return ret;
        }

        public override void ejecutarConsenso(Documento doc)
        {
            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            if ((string)doc.getValor("r.accion") == "borrar")
            {
                //borro estrategia
                string grupoTrabajo = doc.titulo.Split('.')[0];
                string estrategia = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                    if (gt.nombre == grupoTrabajo)
                        foreach (nabu.plataforma.Estrategia pr in gt.estrategias)
                            if (pr.nombre == Tools.HTMLDecode(estrategia))
                            {
                                gt.estrategias.Remove(pr);
                                doc.addLog(Tools.tr("estrategia.eliminada", grupoTrabajo, doc.grupo.idioma));
                                break;
                            }
            }
            else if ((string)doc.getValor("r.accion") == "existente")
            {
                //creo/actualizo grupoTrabajo actual
                string grupoTrabajo = doc.titulo.Split('.')[0];
                string estrategia = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                    if (gt.nombre == grupoTrabajo)
                    {
                        foreach (nabu.plataforma.Estrategia pr in gt.estrategias)
                            if (pr.nombre == Tools.HTMLDecode(estrategia))
                            {
                                //actualizo
                                pr.docURL = doc.URLPath; //nuevo consenso
                                pr.docTs = DateTime.Now;
                                pr.revision = doc.getText("s.revision");
                                pr.objetivo = doc.getText("s.objetivo");
                                pr.definicion = doc.getText("s.definicion");

                                doc.addLog(Tools.tr("estrategia.actualizada", grupoTrabajo, doc.grupo.idioma));
                            }
                    }
            }
            else
            {//nuevo
                string grupoTrabajo = (string)doc.getValor("s.grupoTrabajo");
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                    if (gt.nombre == Tools.HTMLDecode(grupoTrabajo))
                    {
                        //creo
                        nabu.plataforma.Estrategia pr = new plataforma.Estrategia();
                        pr.EID = plataforma.getEID();
                        pr.nombre = doc.titulo;
                        pr.docURL = doc.URLPath;
                        pr.docTs = DateTime.Now;
                        pr.revision = doc.getText("s.revision");
                        pr.objetivo = doc.getText("s.objetivo");
                        pr.definicion = doc.getText("s.definicion");

                        gt.estrategias.Add(pr);

                        doc.addLog(Tools.tr("estrategia.creada", grupoTrabajo, doc.grupo.idioma));
                    }
            }
        }
        
        private string getListaPRs()
        {
            string ret = ":";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.GrupoTrabajo gt in pl.gruposTrabajo)
                foreach (plataforma.Estrategia pr in gt.estrategias)
                    ret += gt.nombre + "." + pr.nombre + ":";
            if (ret.EndsWith(":")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getListaGTs()
        {
            string ret = ":";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.GrupoTrabajo gt in pl.gruposTrabajo)
            {
                ret += gt.nombre + ":";
            }
            if (ret.EndsWith(":")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }
    }
}

