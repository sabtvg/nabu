using System;
using System.Collections.Generic;
using System.Web;

//Los modelos deben darse de alta en Modelo.getModelos()

namespace nabu.plataforma.modelos
{
    public class GrupoTrabajo: Modelo
    {
        private string accion = ""; //para que existe en otros nivels mientras dibujo

        public GrupoTrabajo()
        {
            icono = "res/documentos/grupoTrabajo.png";
            niveles = 5;
            nombre = "GrupoTrabajo";
            descripcion = "Grupo de trabajo";

            crearVariables();
        }

        public override string carpeta()
        {
            return "GrupoTrabajo";
        }

        protected override void crearVariables()
        {
            variables.Clear();
            variables.Add(new Variable("s.etiqueta", 12, 1));
            Variable v = new Variable("s.nombreGrupoTrabajo", 30, 1);
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
            variables.Add(new Variable("s.recursos", 3000, 3));
            variables.Add(new Variable("s.capacidades", 3000, 3));
            variables.Add(new Variable("s.conclusiones", 3000, 3));

            //nivel 4
            variables.Add(new Variable("s.integrantes", 3000, 4));

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
                    if (getText("s.nombreGrupoTrabajo", prop) == "")
                    {
                        addError(1, "El nombre del grupo de trabajo no puede ser vacio");
                        getVariable("s.nombreGrupoTrabajo").className = "errorfino";
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
                    if (getText("s.capacidades", prop) == ""
                        && getText("s.recursos", prop) == ""
                        && getText("s.procesos", prop) == ""
                        && getText("s.conclusiones", prop) == "")
                    {
                        addError(3, "La propuesta no puede estar completamente vacia");
                    }
                }
                else if (prop.nivel == 4)
                {
                    if (getText("s.integrantes", prop) == "")
                    {
                        addError(4, "Debes proponer integrantes");
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

            titulo = getText("s.nombreGrupoTrabajo", prop);
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
            ret += "<td style='vertical-align:middle'>Crear nuevo grupo de trabajo</td>";
            ret += "<td class='titulo2'>";
            //nombre del grupo
            if (prop != null && accion == "nuevo")
            {
                ret += HTMLText("s.nombreGrupoTrabajo", prop, 60 * 8, tieneFlores);
            }
            ret += "</td>";
            ret += "</tr>";

            //existente
            string listaGTs = getListaGTs();
            if (listaGTs != "")
            {
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 2, prop, tieneFlores, "existente") + "</td>";
                ret += "<td style='vertical-align:middle'>Modificar grupo de trabajo</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "existente")
                {
                    ret += HTMLLista("s.nombreGrupoTrabajo", listaGTs, prop, 60 * 8, tieneFlores);
                }
                ret += "</tr>";

                //borrar
                ret += "<tr>";
                ret += "<td>" + HTMLRadio("r.accion", 3, prop, tieneFlores, "borrar") + "</td>";
                ret += "<td style='vertical-align:middle'>Eliminar grupo de trabajo</td>";
                ret += "<td class='titulo2'>";
                //nombre del grupo
                if (prop != null && accion == "borrar")
                {
                    ret += HTMLLista("s.nombreGrupoTrabajo", getListaGTs(), prop, 60 * 8, tieneFlores);
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
                        + tr("¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos un nuevo grupo de trabajo?, ¿Que problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente al grupo") 
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
                            + tr("Describe brevemente las actividades a realizar para alcanzar los objetivos.")
                            + "</div>";
                    ret += HTMLArea("s.descripcion", prop, width, 120, tieneFlores);

                    //A quien va dirigido
                    ret += "<div class='tema'>" + tr("A quien va dirigido") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("Quienes se beneficiaran con los resultados")
                            + "</div>";
                    ret += HTMLArea("s.aquien", prop, width, 120, tieneFlores);
                }

                //variante
                if (puedeVariante) 
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";

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
                    //Recursos
                    ret += "<div class='tema'>" + tr("Recursos") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("¿Que recursos ser&aacute;n necesarios para el nuevo grupo de trabajo? Mobiliario, ordenadores, sitio de trabajo, etc.")
                            + "</div>";
                    ret += HTMLArea("s.recursos", prop, width, 120, tieneFlores);

                    //Capacidades
                    ret += "<div class='tema'>" + tr("Capacidades") + "</div>";
                    if (editar)
                        ret += "<div class='smalltip' style='width:" + width + "px'>"
                            + tr("¿Que capacidades deben tener las personas que componen el nuevo grupo? Conocimientos, experiencias, etc.")
                            + "</div>";
                    ret += HTMLArea("s.capacidades", prop, width, 120, tieneFlores);

                }

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 4)
            {
                ret += "<div class='tema'>" + tr("Integrantes") + "</div>";
                if (editar) 
                    ret += "<div class='smalltip' style='width:" + width + "px'>" 
                        + tr("¿Quienes ser&aacute;n los integrantes del nuevo grupo de trabajo? Estos nombres sirven de propuesta o referencia. No implican realmente a las personas. Si no quieres implicar nombres reales di 'a definir'.") 
                        + "</div>";

                //lista de seleccion de usuarios
                string lista = "";
                foreach(Usuario u2 in g.usuarios) 
                    lista += u2.email + ":" + u2.nombre + "|";
                lista = lista .Substring(0, lista.Length-1);
                ret += HTMLListaSeleccion("s.integrantes", prop, width - 150, 250, tieneFlores, lista,"Pertenece al grupo","NO pertenece al grupo");

                //variante
                if (puedeVariante)
                    ret += "<div style='width:" + width + "px;text-align:right;'><input type='button' class='btn' value='" + tr("Proponer variante") + "' onclick='doVariante(" + prop.nodoID + ")'></div>";
            }
            else if (nivel == 5)
            {
                ret += "<div class='tema'>" + tr("Valoraci&oacute;n del resultado") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + tr("¿Como se medir&aacute; el resultado de este grupo de trabajo? ¿cantidad de procesos ejecutados? ¿m&eacute;tricas de eficiancia? ¿reconocimiento de otras personas? ¿resultados concretos?. <br>La valoraci&oacute;n del desempeño del grupo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.")
                        + "</div>";
                ret += HTMLArea("s.eficiencia", prop, width, 120, tieneFlores);

                ret += "<div class='tema'>" + tr("Revisi&oacute;n de valoraci&oacute;n del resultado") + "</div>";
                if (editar)
                    ret += "<div class='smalltip' style='width:" + width + "px'>"
                        + tr("¿Con que periodicidad deben evaluarse los resultados alcanzados.")
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
            ////creo grupo remoto si no existe    
            ////usuarios
            //string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
            //string admins = "";
            //foreach(string usuario in usuarios)
            //{
            //    Usuario u = doc.grupo.getUsuario(usuario.Split(':')[1]);
            //    admins += u.nombre + ":" + u.email + ":" + u.clave + "|"; 
            //}
            //if (admins != "") admins = admins.Substring(0, admins.Length - 1);

            //string grupoTrabajoNombre = doc.titulo;
            //string grupoTrabajoOrganizacion = (string)doc.getValor("s.organizacion");

            ////creo
            //string retGrupo = Tools.getHttp(doc.grupo.URL + "/" 
            //    + "doMain.aspx?actn=newgrupoadmins?grupo=" + grupoTrabajoNombre
            //    + "&organizacion=" + grupoTrabajoOrganizacion
            //    + "&idioma=" + doc.grupo.idioma 
            //    + "&admins=" + admins
            //    + "&padreurl=" + doc.grupo.URL
            //    + "&padrenombre=" + doc.grupo.nombre);

            //doc.addLog(tr("Grupo remoto:" + retGrupo));

            nabu.organizaciones.Plataforma plataforma = (nabu.organizaciones.Plataforma)doc.grupo.organizacion;
            if ((string)doc.getValor("r.accion") == "borrar")
            {
                //borro grupoTrabajo
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        plataforma.gruposTrabajo.Remove(gt);
                        doc.addLog(tr("Grupo de trabajo eliminado de la estructura organizativa"));
                        break;
                    }
                }
            }
            else
            {
                //creo/actualizo grupoTrabajo actual
                bool found = false;
                foreach (nabu.plataforma.GrupoTrabajo gt in plataforma.gruposTrabajo)
                {
                    if (gt.nombre == doc.titulo)
                    {
                        found = true;
                        //actualizo
                        gt.docURL = doc.URLPath; //nuevo consenso
                        gt.docTs = DateTime.Now;
                        gt.revision = (string)doc.getValor("s.revision");
                        gt.objetivo = (string)doc.getValor("s.objetivo");

                        string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
                        gt.integrantes.Clear();
                        foreach (string usuario in usuarios)
                        {
                            gt.integrantes.Add(usuario.Split(':')[1]);
                        }
                        doc.addLog(tr("Grupo de trabajo actualizado en la estructura organizativa"));
                    }
                }
                //creo
                if (!found)
                {
                    nabu.plataforma.GrupoTrabajo gt = new plataforma.GrupoTrabajo();
                    gt.idioma = doc.grupo.idioma;
                    gt.nombre = doc.titulo;
                    gt.docURL = doc.URLPath;
                    gt.docTs = DateTime.Now;
                    gt.revision = (string)doc.getValor("s.revision");
                    gt.objetivo = (string)doc.getValor("s.objetivo");

                    string[] usuarios = ((string)doc.getValor("s.integrantes")).Split('|');
                    foreach (string usuario in usuarios)
                    {
                        gt.integrantes.Add(usuario.Split(':')[1]);
                    }
                    plataforma.gruposTrabajo.Add(gt);

                    doc.addLog(tr("Grupo de trabajo creado en la estructura organizativa"));
                }
            }
        }

        public override string documentSubmit(string accion, string parametro, List<Propuesta> props, Grupo g, string email, int width, Modelo.eModo modo)
        {
            if (accion == "s.integrantes_agregar" && getVariable("s.integrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.integrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.integrantes"];

                    if (value == "*")
                        prop.bag["s.integrantes"] = parametro; //caso inicial
                    else
                        prop.bag["s.integrantes"] += "|" + parametro;
                }
            }
            else if (accion == "s.integrantes_quitar" && getVariable("s.integrantes").nivel <= props.Count)
            {
                Variable v = getVariable("s.integrantes");
                Propuesta prop;
                prop = props[v.nivel - 1];
                if (prop.nivel == v.nivel)
                {
                    string value = (string)prop.bag["s.integrantes"];

                    //quito
                    string ret = "";
                    foreach (string item in value.Split('|'))
                    {
                        if (!item.StartsWith(parametro.Split(':')[0]))
                        {
                            ret += item + "|";
                        }
                    }
                    if (ret != "") ret = ret.Substring(0, ret.Length - 1);
                    prop.bag["s.integrantes"] = ret;
                }
            }

            return toHTML(props, g, email, width, modo);
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

