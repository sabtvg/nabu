using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class Proceso: Modelo
    {
        private string accion = ""; //para que existe en otros nivels mientras dibujo

        public Proceso()
        {
            icono = "res/documentos/proceso.png";
            niveles = 5;
            nombre = "Proceso";
            descripcion = "Proceso operativo";

            crearVariables();
        }

        public override string carpeta()
        {
            return "Proceso";
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
            variables.Add(new Variable("s.entradas", 3000, 3));
            variables.Add(new Variable("s.definicion", 3000, 3));
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
                    if (getText("s.entradas", prop) == ""
						&& getText("s.definicion", prop) == ""
                        && getText("s.conclusiones", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.implantacion", prop) == "")
                    {
                        addError(4, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 5)
                {
                    if (getText("s.eficiencia", prop) == "")
                    {
                        addError(5, "La propuesta no puede estar completamente vacia");
                    }
                }
            }
        }

        override protected string HTMLEncabezado(Propuesta prop, Grupo g, string email, int width)
        {
            string ret = "";
            Usuario u = g.getUsuario(email);
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
            ret += "<td colspan=2 class='titulo2'><nobr>" + tr("Acci&oacute;n") + "</nobr></td>";
            ret += "<td colspan=2 class='titulo2'><nobr>" + tr("Nombre") + "</nobr></td>";
            ret += "</tr>";

            ret += "<tr>";
            ret += "<td>" + HTMLRadio("r.accion", 1, prop, tieneFlores, "nuevo") + "</td>";
            ret += "<td style='vertical-align:middle'>Crear nuevo proceso</td>";
            ret += "<td class='titulo2'>";
            //nombre del grupo
            if (prop != null && accion == "nuevo")
            {
                ret += HTMLText("s.nombreProceso", prop, 40 * 8, tieneFlores);
            }
            ret += "</td>";
            ret += "</tr>";

            //existente
            string listaPRs = getListaPRs();
            if (listaPRs != "")
            {
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 2, prop, tieneFlores, "existente") + "</td>";
                ret += "<td style='vertical-align:middle'>Modificar proceso</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "existente")
                {
                    ret += HTMLLista("s.nombreProceso", listaPRs, prop, 80 * 8, tieneFlores);
                }
                ret += "</tr>";

                //borrar
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 3, prop, tieneFlores, "borrar") + "</td>";
                ret += "<td style='vertical-align:middle'>Eliminar proceso</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "borrar")
                {
                    ret += HTMLLista("s.nombreProceso", listaPRs, prop, 80 * 8, tieneFlores);
                }
                ret += "</tr>";
            }
            ret += "</table><br>";

            //etiqueta
            ret += "<div class='titulo2'><nobr>" + tr("Etiqueta") + ":" + HTMLText("s.etiqueta", prop, 20 * 5, tieneFlores);
            if (prop == null)
                ret += "<span style='color:gray;font-size:12px;'>" + tr("(Etiqueta en el arbol)") + "</span>";
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


            //validaciones de este nivel
            if (modo == eModo.prevista) validar(prop);

            if (nivel == 1)
            {
                //me guardo el valor de r.accion para poder usarlo en otros nivels tambien
                accion = getText("r.accion", prop);
                if (accion == "borrar")
                    niveles = 3;
                else
                    niveles = 5;

                ret += HTMLEncabezado(prop, g, email, width);

                //fecha
                if (modo == eModo.consenso)
                    ret += "<div class='titulo2'><nobr>" + tr("Fecha") + ":" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</nobr></div>";

                //tema
                ret += "<div class='tema'>" + tr("Introducci&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos un nuevo proceso operativo?, ¿Que problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente un problema real del grupo") 
                        + "</div>";
                ret += HTMLArea("s.introduccion", prop, width, 120, tieneFlores);
            }
            else if (nivel == 2)
            {
                //guardo la accion en cada nivel porque su representacion depende de este valor
                if (prop != null && accion != "")
                    prop.bag["r.accion"] = accion;
                else if (prop != null && accion == "" && prop.bag.ContainsKey("r.accion"))
                    accion = getText("r.accion", prop); 

                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + tr("Consecuencias") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("¿Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?")
                            + "</div>";
                    ret += HTMLArea("s.consecuencias", prop, width, 120, tieneFlores);
                }
                else
                {
                    //Objetivo a lograr
                    ret += "<div class='tema'>" + tr("Objetivo a lograr") + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>" 
                            + tr("¿Cuales son los objetivos del nuevo grupo de trabajo?") 
                            + "</div>";
                    ret += HTMLArea("s.objetivo", prop, width, 120, tieneFlores);

                    //Descripcion
                    ret += "<div class='tema'>" + tr("Descripcion") + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>" 
                            + tr("Describe brevemente los pasos a realizar al ejecutar el proceso operativo. Menciona como se tratar&aacute;n los casos excepcionales. Un proceso operativo siempre debe generar un resultado, aunque sea un registro de fallo para su posterior analisis.") 
                            + "</div>";
                    ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores);

                    //A quien va dirigido
                    ret += "<div class='tema'>" + tr("A quien va dirigido") + "</div>";
                    if (editar) 
                        ret += "<div class='smalltip' style='width:" + width + "px'>" 
                            + tr("Quienes se beneficiaran con los resultados de utilizar este proceso operativo") 
                            + "</div>";
                    ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores);

                    //variante
                    if (puedeVariante) 
                        ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
                }
            }
            else if (nivel == 3)
            {
                //guardo la accion en cada nivel porque su representacion depende de este valor
                if (prop != null && accion != "")
                    prop.bag["r.accion"] = accion;
                else if (prop != null && accion == "" && prop.bag.ContainsKey("r.accion"))
                    accion = getText("r.accion", prop);

                if (accion == "borrar")
                {
                    ret += "<div class='tema'>" + tr("Conclusiones") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("¿Que hemos aprendido con esta experiencia?")
                            + "</div>";
                    ret += HTMLArea("s.conclusiones", prop, width, 120, tieneFlores);
                }
                else
                {
                //Entradas de proceso
                ret += "<div class='tema'>" + tr("Entradas de proceso") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + tr("¿Como se provoca la ejecuci&oacute;n de este proceso? ¿Que necesita este proceso para poder ejecutarse? Un nuevo socio, un documento, una peticion personal, etc.")
                        + "</div>";
                ret += HTMLArea("s.entradas", prop, width, 120, tieneFlores);

                //definicion
                ret += "<div class='tema'>" + tr("Definici&oacute;n del proceso operativo") + "</div>";
                if (editar)  
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("Define paso por paso las tareas a realizar cuando se ejecute este proceso. Define como se tratar&aacute;n las excepciones. Un proceso operativo es un flujo de condiciones y tareas que contempla todas las alternativas posibles de manera que su ejecusi&oacute;n no admita discusiones sobre sus resultados. Un proceso operativo bien definido es parte importante de la definici&oacute;n operativa del grupo. Todas las actividades habituales del grupo deben definirse como procesos operativos.") 
                        + "</div>";
                ret += HTMLArea("s.definicion", prop, width, 120, tieneFlores);

                }

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
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
                    ret += "<div class='tema'>" + tr("Grupo de trabajo") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("¿Que grupo de trabajo usa este proceso operativo?")
                            + "</div>";
                    ret += HTMLLista("s.grupoTrabajo", getListaGTs(), prop, width, tieneFlores);
                }

                //implantacion
                ret += "<div class='tema'>" + tr("Implantaci&oacute;n") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("¿Que se debe hacer para implantar este proceso? Recursos, sitio fis&iacute;co, conocimientos y capacidades de las personas, formularios/hojas de datos/folletos informativos, software, etc.") 
                        + "</div>";
                ret += HTMLArea("s.implantacion", prop, width, 120, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + tr("Valoraci&oacute;n del resultado") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + tr("¿Como se medir&aacute; el resultado de este proceso operativo? ¿cantidad de procesos ejecutados? ¿resultados obtenidos? ¿facilita la necesidad? ¿es facil de ejecutar?. <br>La valoraci&oacute;n de un proceso operativo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.")
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores);

                ret += "<div class='tema'>" + tr("Revisi&oacute;n de valoraci&oacute;n del resultado") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + tr("¿Cuando se revisar&aacute; la definici&oacute;n de este proceso operativo?")
                        + "</div>";
                ret += HTMLLista("s.revision", ":Mensual:Trimestral:Semestral:Anual", prop, 250, tieneFlores);

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
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
                //borro proceso
                string grupoTrabajo = doc.titulo.Split('.')[0];
                string proceso = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                    if (gt.nombre == grupoTrabajo)
                        foreach (nabu.plataforma.Proceso pr in gt.procesos)
                            if (pr.nombre == proceso)
                            {
                                gt.procesos.Remove(pr);
                                doc.addLog(tr("Proceso eliminado del grupo de trabajo [" + grupoTrabajo + "] en la estructura organizativa"));
                                break;
                            }
            }
            else
            {
                //creo/actualizo grupoTrabajo actual
                bool found = false;
                string grupoTrabajo = doc.titulo.Split('.')[0];
                string proceso = doc.titulo.Split('.')[1];
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                    if (gt.nombre == grupoTrabajo)
                    {
                        foreach (nabu.plataforma.Proceso pr in gt.procesos)
                            if (pr.nombre == proceso)
                            {
                                found = true;
                                //actualizo
                                pr.docURL = doc.URLPath; //nuevo consenso
                                pr.docTs = DateTime.Now;
                                pr.revision = (string)doc.getValor("s.revision");
                                pr.objetivo = (string)doc.getValor("s.objetivo");
                                pr.entradas = (string)doc.getValor("s.entradas");
                                pr.definicion = (string)doc.getValor("s.definicion");

                                doc.addLog(tr("Proceso actualizado en el grupo de trabajo [" + grupoTrabajo + "] en la estructura organizativa"));
                            }

                        //creo
                        if (!found)
                        {
                            grupoTrabajo = (string)doc.getValor("s.grupoTrabajo");
                            nabu.plataforma.Proceso pr = new plataforma.Proceso();
                            pr.nombre = doc.titulo;
                            pr.docURL = doc.URLPath;
                            pr.docTs = DateTime.Now;
                            pr.revision = (string)doc.getValor("s.revision");
                            pr.objetivo = (string)doc.getValor("s.objetivo");
                            pr.entradas = (string)doc.getValor("s.entradas");
                            pr.definicion = (string)doc.getValor("s.definicion");

                            gt.procesos.Add(pr);

                            doc.addLog(tr("Proceso creado en el grupo de trabajo [" + grupoTrabajo + "] en la estructura organizativa"));
                        }
                    }
            }
        }
        
        private string getListaPRs()
        {
            string ret = "";
            nabu.organizaciones.Plataforma pl = (nabu.organizaciones.Plataforma)grupo.organizacion;
            foreach (plataforma.GrupoTrabajo gt in pl.gruposTrabajo)
                foreach (plataforma.Proceso pr in gt.procesos)
                    ret += gt.nombre + "." + pr.nombre + ":";
            if (ret.EndsWith(":")) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string getListaGTs()
        {
            string ret = "";
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

