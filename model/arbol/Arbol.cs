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


namespace nabu
{
    public class Arbol
    {
        public string nombre = "";
        public int cantidadFlores = 5;
        public Grupo grupo;
        public Nodo raiz;
        public bool leftRight = false; //alterna agregado de nuevos nodos de un lado y del otro
        public List<Propuesta> propuestas = new List<Propuesta>();
        public DateTime born = DateTime.Now;
        public int lastDocID = 1;
        public int lastNodoID = 10;
        public bool simulacion = false;
        public Usuario lastSimUsuario;

        //condicion de consenso
        public float minSiPc = 60; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNoPc = 15; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso

        private Random rnd = new Random();

        public Arbol()
        {
            raiz = new Nodo();
        }

        public float getNexRandom(){
            return (float)rnd.NextDouble();
        }

        public void caerFlores(Usuario u){
            //caigo flores de u
            foreach (Flor f in u.flores)
            {
                if (f.id != 0)
                {
                    Nodo n = getNodo(f.id);
                    if (n != null)
                    {
                        quitarFlor(n, u);
                    }                                    
                }
            }
        }

        public Nodo getMayorAgregar(int notLikeId) {
            //busco un candidato con muchas flores para agregarle otra
            var nodes = toList();
            NodoComparerMayor dc = new NodoComparerMayor();
            nodes.Sort(dc); //de mayor a menor
            List<Nodo> temp = new List<Nodo>();

            //1er filtro de candidato
            foreach (Nodo n in nodes)
                if (!n.consensoAlcanzado && n.nivel == 5 && n.id != notLikeId && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (!n.consensoAlcanzado && n.id != notLikeId && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //3er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.id != notLikeId && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            return nodes[0];
        }

        public int getApoyos(Usuario u)
        {
    		return getApoyos(u, toList());
	    }

        private int getApoyos(Usuario u, List<Nodo> nodos)
        {
		int ret = 0;
		//obtengo nodos de este usuario
		List<int> mios = new List<int>();
		foreach (Nodo n in nodos)
		{
			if (n.email == u.email)
				mios.Add(n.id);
		}
		//obtengo apoyos
        foreach (Usuario u2 in grupo.usuarios)
		{
			foreach (int id in mios)
			{
				foreach (Flor f in u2.flores)
				if (f.id == id)
					ret++;
			}
		}
		return ret;
	}

        public void actualizarApoyos()
        {
		//recuento los apoyos que tiene cada usuario
		List<Nodo> nodos = toList();
        foreach (Usuario u in grupo.usuarios)
		{
			u.apoyos = getApoyos(u, nodos);		
		}
        }

        public Nodo getMayorQuitar(int notLikeId)
        {
            //busco un candidato con muchas flores para quitarle una
            var nodes = toList();
            NodoComparerMayor dc = new NodoComparerMayor();
            nodes.Sort(dc); //de mayor a menor
            List<Nodo> temp = new List<Nodo>();

            //1er filtro de candidato
            foreach (Nodo n in nodes)
                if (n.consensoAlcanzado && n.flores > 0 && n.id != notLikeId && n.nivel<5)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.consensoAlcanzado && n.flores > 0 && n.id != notLikeId)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.flores > 0 && n.id != notLikeId)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            return nodes[0];
        }

        public Nodo getMenorAgregar(int notLikeId)
        {
            //busco un candidato con pocas flores para agregarle una
            List<Nodo> nodes = toList();
            NodoComparerMenor dc = new NodoComparerMenor();
            nodes.Sort(dc); //de mayor a menor
            List<Nodo> temp = new List<Nodo>();

            //1er filtro de candidato
            foreach (Nodo n in nodes)
                if (!n.consensoAlcanzado && n.id != notLikeId && n.nivel == 5 && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (!n.consensoAlcanzado && n.id != notLikeId && n.children.Count == 0 && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //2er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (!n.consensoAlcanzado && n.id != notLikeId && n.flores < minSiPc)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //3er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.id != notLikeId)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            return nodes[0];
        }

        public Nodo getMenorQuitar(int notLikeId)
        {
            //busco un candidato con pocas flores para quitarle una
            var nodes = toList();
            NodoComparerMenor dc = new NodoComparerMenor();
            nodes.Sort(dc); //de mayor a menor
            List<Nodo> temp = new List<Nodo>();

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.consensoAlcanzado && n.flores > 0 && n.id != notLikeId)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //1er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.flores > 0 && n.id != notLikeId && n.nivel > 2)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            //2er filtro de candidato
            temp.Clear();
            foreach (Nodo n in nodes)
                if (n.flores > 0 && n.id != notLikeId)
                    temp.Add(n);
            if (temp.Count > 0) return rndElement(temp);

            return nodes[0];
        }

        public Nodo rndElement(List<Nodo> nodes)
        {
            int index = (int)Math.Ceiling(rnd.NextDouble() * nodes.Count) - 1;
            return nodes[index];
        }

        public List<Nodo> toList() {
            var nodes = toList2(raiz, new List<Nodo>());
            return nodes;
        }

        public string getEtiqueta(string prefijo, Nodo n)
        {
            //busco un id libre en este subarbol
            List<Nodo> subarbol = toList(n);
            int ret = 0;
            bool found = true;
            while (found)
            {
                found = false;
                ret++;
                foreach (Nodo hijo in subarbol)
                    if (hijo.nombre == prefijo + ret.ToString())
                        found = true;
            }
            return prefijo + ret.ToString();
        }

        public List<Nodo> toList(Nodo n)
        {
            var nodes = toList2(n, new List<Nodo>());
            return nodes;
        }

        private List<Nodo> scrumble(List<Nodo> nodes) {
            //scrumble
            for(int i = 0; i<nodes.Count / 3; i++)
            {
                int swapIndex = (int)Math.Ceiling(rnd.NextDouble() * nodes.Count) - 1;
                Nodo swap = nodes[swapIndex];
                nodes[swapIndex] = nodes[i];
                nodes[i] = swap;
            }
            return nodes;
        }

        public List<Nodo> toList2(Nodo node, List<Nodo> l) {
            List<Nodo> ret = l;
            ret.Add(node);
            if (node.children.Count > 0) {
                foreach (Nodo n in node.children) 
                {
                    ret = toList2(n, ret);
                }
            }
            return ret;
        }

        public float minSiValue
        {
            get
            {
                return (float)Math.Ceiling(grupo.getUsuariosHabilitadosActivos().Count * minSiPc / 100);
            }
        }

        public float maxNoValue
        {
            get
            {
                return (float)Math.Ceiling(minSiValue * maxNoPc / 100);
            }
        }

        private bool comprobarConsenso(string email)
        {
            List<Nodo> nodos = toList();
            foreach (Nodo n in nodos)
                if (n.nivel > 0)
                    if (comprobarConsenso(n, email))
                        return true;
            return false;
        }

        public LogDocumento getLogDocumento(int docID)
        {
            foreach (LogDocumento ld in grupo.logDecisiones)
            {
                if (ld.docID == docID)
                    return ld;
            }
            return null;
        }

        private bool comprobarConsenso(Nodo n, string email)
        {
            bool ret = false;
            Modelo m = grupo.organizacion.getModeloDocumento(n.modeloID);
            List<Nodo> pathn = getPath(n.id);

            if (m != null && n.nivel == n.niveles)
            {
                //es una hoja de documento completo, verifico condicion
                n.negados = getNegados(n);

                //condicion de consenso
                if (!n.consensoAlcanzado &&
                    n.flores >= minSiValue &&
                    n.negados <= maxNoValue)
                {
                    //esta rama ha alcanzado el consenso
                    //genero documento
                    DateTime fdate = DateTime.Now;
                    int docID = lastDocID++;
                    string fname = m.nombre + "_" + docID.ToString("0000");
                    string docPath = "documentos\\" + m.carpeta() + "\\" + docID.ToString("0000");
                    string URL = grupo.URL + "/grupos/" + nombre + "/" + docPath.Replace('\\', '/') + "/" + fname + ".html";

                    //creo carpeta se salida
                    //crearCarpeta(docPath);
                    if (!System.IO.Directory.Exists(grupo.path + "\\" + docPath))
                    {
                        System.IO.Directory.CreateDirectory(grupo.path + "\\" + docPath);
                        System.IO.Directory.CreateDirectory(grupo.path + "\\" + docPath + "\\res\\documentos");
                        System.IO.File.Copy(grupo.path + "\\..\\..\\styles.css", grupo.path + "\\" + docPath + "\\styles.css");
                        //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(grupo.path + "\\..\\..\\res\\documentos");
                        //foreach (System.IO.FileInfo fi in di.GetFiles())
                        //{
                        //    System.IO.File.Copy(fi.FullName, grupo.path + "\\" + docPath + "\\res\\documentos\\" + fi.Name);
                        //}
                    }

                    //titulo
                    Propuesta p = getPropuesta(n.id);  //obtengo el titulo del debate de cualquiera de las propuestas 
                    string titulo = p.titulo;

                    //version
                    //cuento los documentos historicos con mismo modelo y mismo titulo
                    int version = 1;
                    foreach (LogDocumento ldi in grupo.logDecisiones)
                    {
                        if (m.versionar=="titulo" && ldi.modeloNombre == m.nombre && ldi.titulo == titulo)
                            version++;
                        else if (m.versionar == "modelo" && ldi.modeloNombre == m.nombre)
                            version++;
                    }

                    //guardo HTML. Debo hacerlo antes de ejecutar consenso por si usa el fichero HTML
                    generarDocumentoHTML(n, fdate, fname, docPath, URL, version, titulo);

                    //creo documento
                    Documento doc = crearDocumento(n, fdate, fname, docPath, URL);
                    doc.version = version;

                    //ejecuto proceso de consenso alcanzado
                    try
                    {
                        doc.grupo = grupo;
                        doc.EjecutarConsenso();
                    }
                    catch (Exception ex)
                    {
                        doc.addLog("EjecutarConsenso: <font color=red>" + ex.Message + "</font>");
                    }
                    //guardo el documento
                    doc.save();

                    //vuelvo a guarda HTML para agregarle los logs
                    generarDocumentoHTML(n, fdate, fname, docPath, URL, version, titulo, doc.logs);

                    //notifico via email a todos los socios
                    if (!simulacion)
                    {
                        foreach (Usuario u in grupo.usuarios)
                            if (u.email != email)
                            {
                                Tools.encolarMailNuevoConsenso(grupo, u.email, n.flores, n.negados, URL);
                                u.alertas.Add(new Alerta(Tools.tr("Nueva decision [%1]", doc.nombre + ": " + doc.titulo, grupo.idioma)));
                            }
                    }

                    //cruzo modelo con valores
                    List<Propuesta> props = new List<Propuesta>();
                    foreach (Nodo n1 in pathn)
                    {
                        p = getPropuesta(n1);
                        if (p != null) //la raiz
                            props.Add(p);
                    }

                    //guardo el log historico en el arbol
                    LogDocumento ld = new LogDocumento();
                    ld.fecha = fdate;
                    ld.titulo = titulo;
                    ld.icono = grupo.organizacion.getModeloDocumento(n.modeloID).icono;
                    if (ld.titulo.Length > 50) ld.titulo = ld.titulo.Substring(0, 50);
                    ld.modeloNombre = grupo.organizacion.getModeloDocumento(n.modeloID).nombre;
                    ld.modeloID = n.modeloID;
                    ld.x = n.x; if (ld.x == 0) ld.x = 90;
                    ld.docID = docID;
                    ld.fname = fname;
                    ld.arbol = nombre;
                    ld.objetivo = grupo.objetivo;
                    ld.flores = n.flores;
                    ld.negados = n.negados;
                    ld.carpeta = m.carpeta();
                    ld.URL = URL;
                    ld.revisionDias = m.getRevisionDias(props);
                    ld.version = doc.version;
                    ld.path = doc.path;
                    grupo.logDecisiones.Add(ld);

                    //marco a todos los nodos del debate y sus propuestas
                    for (int i = 0; i < pathn.Count - 1; i++) //menos la raiz
                    {
                        pathn[i].consensoAlcanzado = true;
                        pathn[i].consensoFecha = DateTime.Now;
                        getPropuesta(pathn[i]).consensoAlcanzado = true;
                        foreach (Nodo n2 in pathn[i].children)
                            marcarConsenso(n2);
                    }

                    grupo.save(grupo.path + "\\" + docPath); //guardo copia del arbol

                    ret = true;
                }
            }
            return ret;
        }

        private void marcarConsenso(Nodo n)
        {
            //marco sus hijos
            n.consensoAlcanzado = true;
            n.consensoFecha = DateTime.Now;
            getPropuesta(n).consensoAlcanzado = true;
            foreach (Nodo n2 in n.children)
            {
                n2.consensoAlcanzado = true;
                n2.consensoFecha = DateTime.Now;
                getPropuesta(n2).consensoAlcanzado = true;
                marcarConsenso(n2);
            }
        }

        private Documento crearDocumento(Nodo n, DateTime now, string fname, string docPath, string URL)
        {
            Modelo m = grupo.organizacion.getModeloDocumento(n.modeloID);
            Documento doc = new Documento();
            doc.fecha = now;
            doc.nombre = m.nombre;
            doc.fname = fname;
            doc.modeloID = n.modeloID;
            doc.path = docPath + "\\" + fname + ".json";
            doc.URLPath = URL;
            //obtengo el titulo
            //debo dibujar el documento
            //junto propuestas
            List<Propuesta> props = new List<Propuesta>();
            foreach (Nodo n1 in getPath(n.id))
            {
                Propuesta p = getPropuesta(n1);
                if (p != null) //la raiz
                    props.Add(p);
            }
            //armo HTML
            m.toHTML(props, this.grupo, "", 1024, Modelo.eModo.consenso);
            doc.titulo = m.titulo;
            
            //guardo propuestas
            doc.propuestas = props;
            
            return doc;
        }

        private void generarDocumentoHTML(Nodo n, DateTime now, string fname, string docPath, string URL, int version, string titulo)
        {
            generarDocumentoHTML(n, now, fname, docPath, URL, version, titulo, null);
        }

        private void generarDocumentoHTML(Nodo n, DateTime now, string fname, string docPath, string URL, int version, string titulo, List<Documento.Log> logs)
        {
            List<Nodo> pathn = getPath(n.id);
            Modelo m = grupo.organizacion.getModeloDocumento(n.modeloID);
            DateTime inicio = DateTime.MaxValue;

            //junto propuestas
            List<Propuesta> props = new List<Propuesta>();
            foreach (Nodo n1 in pathn)
            {
                Propuesta p = getPropuesta(n1);
                if (p != null) //la raiz
                {
                    props.Add(p);
                    if (p.born < inicio) inicio = p.born;
                }
            }
            //firma consenso
            string ret = "";
            ret += "<div style='clear:left;float:left;width:100%;font-size:12px;'>";
            ret += "<hr>";
            ret += "Documento escrito de forma cooperativa.<br>";
            ret += "Titulo: " + titulo + "<br>";
            if (m.versionar != "")
                ret += "Version: " + version + "<br>";
            ret += "Grupo: " + this.nombre + "<br>";
            ret += "Documento ID:" + fname + "<br>";
            ret += "Inicio de debate: " + inicio.ToString("dd/MM/yy") + "<br>";
            ret += "Fecha de consenso: " + DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToShortTimeString() + "<br>";
            ret += "Ubicaci&oacute;n: <a target='_blank' href='" + URL + "'>" + URL + "</a><br>";
            ret += "Objetivo: " + this.grupo.objetivo + "<br>";
            ret += "Usuarios: " + this.grupo.getUsuariosHabilitados().Count + "<br>";
            ret += "Activos: " + this.grupo.activos + "<br>";
            ret += "Si: (&ge; " + this.minSiPc + "%): " + n.flores + "<br>";
            ret += "No: (&le; " + this.maxNoPc + "%): " + n.negados + "<br>";
            
            //admin
            if (this.grupo.getAdmin() != null) ret += "Coordinador: " + grupo.getAdmin().nombre + "<br>";
            
            //representates
            ret += "Representantes: ";
            foreach(Usuario rep in this.grupo.getRepresentantes())
            {
                ret += rep.nombre + ",";
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            ret += "<br>";
            
            //secretaria
            if (this.grupo.getSecretaria() != null) ret += "Secretaria: " + grupo.getSecretaria().nombre + "<br>";
            
            //facilitador
            if (this.grupo.getFacilitador() != null) ret += "Facilitador: " + grupo.getFacilitador().nombre + "<br>";

            //log consenso
            if (logs != null)
            {
                ret += "<br>";
                ret += "Log consenso: <br>";
                foreach (Documento.Log l in logs)
                    ret += "&nbsp;&nbsp;" + l.msg + "<br>";
                ret += "</div>";
            }

            //armo HTML
            string html = "";
            try
            {
                m.firmaConsenso = ret;
                html = m.toHTML(props, this.grupo, "", 1024, Modelo.eModo.consenso);
            }
            catch (Exception ex)
            {
                throw new Exception("generarDocumentoHTML():toHTML():" + m.nombre + ":" + ex.Message + " " + ex.StackTrace);
            }
           
            //escribo
            System.IO.File.WriteAllText(grupo.path + "\\" + docPath + "\\" + fname + ".html", html, System.Text.Encoding.UTF8);
        }

        private int getNegados(Nodo n)
        {
            int ret = 0;
            List<Nodo> pathn = getPath(n.id);
            for (int i = 1; i < pathn.Count - 1; i++) //la raiz me la salto porque solo miro en el  mismo debate
            {
                foreach (Nodo n2 in pathn[i].children)
                    if (n2.id != pathn[i - 1].id) //busco en todos los hijos del padre que no sea yo mismo
                        ret += getHijosNegados(n2);
            }
            return ret;
        }

        private int getHijosNegados(Nodo n)
        {
            int ret = n.flores;
            foreach (Nodo n2 in n.children)
                ret += getHijosNegados(n2);
            return ret;
        }

        public ArbolPersonal getArbolPersonal(string email)
        {
            return getArbolPersonal(email, 0);
        }

        public ArbolPersonal getArbolPersonal(string email, int nuevoNodoID)
        {
            Usuario u = grupo.getUsuarioHabilitado(email);
            if (u != null)
            {
                if (verificarFloresCaducadas())
                {
                    //notifico por mail al usuario
                    Usuario admin = grupo.getAdmin();
                    Tools.encolarMailCaido(grupo, u.email, admin.email, Tools.MapPath("mails/modelos/" + grupo.idioma));
                    u.alertas.Add(new Alerta(Tools.tr("Tus flores han caido", grupo.idioma)));
                    //app.addLog("verifyFloresCaducadas", "", grupo.nombre, u.email, "Flor caducada. Usuario lastLogin: " + u.lastLogin);
                }
                comprobarConsenso(email);

                ArbolPersonal ap = new ArbolPersonal();
                ap.raiz = raiz; //referecia cruzada pero no importa porque este objeto es para serializar
                ap.objetivo = grupo.objetivo;
                ap.URLEstatuto = grupo.URLEstatuto;
                ap.nombre = grupo.nombre;
                ap.usuarios = grupo.getUsuariosHabilitados().Count;
                ap.cantidadFlores = cantidadFlores;
                ap.activos = grupo.activos;
                ap.simulacion = simulacion;
                ap.nuevoNodoID = nuevoNodoID;
                ap.born = born;
                ap.documentos = grupo.logDecisiones.Count;
                ap.idioma = grupo.idioma.ToLower();
                ap.organizacion = grupo.organizacion.GetType().Name;
                ap.padreNombre = grupo.padreNombre;
                ap.padreURL = grupo.padreURL;
                ap.tipoGrupo = grupo.tipoGrupo;

                foreach(Hijo hijo in grupo.hijos)
                {
                    ap.hijos.Add(hijo);//referecia cruzada pero no importa porque este objeto es para serializar
                }

                ap.usuario = u;
                ap.minSiPc = minSiPc;
                ap.maxNoPc = maxNoPc;

                ap.minSiValue = minSiValue;
                ap.maxNoValue = maxNoValue;

                ap.logDecisiones = grupo.logDecisiones;
                ap.logResultados = grupo.logResultados;

                return ap;
            }
            else
                throw new appException("El usuario no existe o no esta habilitado");
        }

        public Nodo addNodo(Nodo padre, Propuesta prop){
            //verifico que el usuario tiene al menos una flor disponible
            Usuario u = grupo.getUsuarioHabilitado(prop.email);
            if (prop.etiqueta == "")
                throw new appException(Tools.tr("Nombre de nodo no puede ser vacio", grupo.idioma));
            else if (u == null)
                throw new appException(Tools.tr("El usuario no existe o no esta habilitado", grupo.idioma));
            else if (padre == null)
                throw new appException(Tools.tr("El nodo no existe", grupo.idioma));
            else
            {
                //agrego nuevo nodo
                Nodo nuevo = new Nodo();
                nuevo.nombre = prop.etiqueta;
                nuevo.id = lastNodoID++;
                nuevo.modeloID = prop.modeloID;
                nuevo.email = prop.email;
                nuevo.niveles = prop.niveles;

                try
                {
                    //agrego al arbol
                    if (leftRight)
                        padre.children.Add(nuevo);
                    else
                        padre.children.Insert(0, nuevo);

                    //seguridad
                    if (prop.nivel != getPath(nuevo.id).Count - 1)  //quito la raiz
                        throw new Exception("El nivel de la propuesta no coincide con el del arbol");

                    //fijo nivel
                    nuevo.nivel = prop.nivel;

                    leftRight = !leftRight;

                    //agrego la propuesta
                    prop.nodoID = nuevo.id;  //ahora si tiene nodo
                    propuestas.Add(prop);

                    //consumo una flor
                    asignarflor(u, nuevo);
                }
                catch (Exception ex)
                {
                    //no se pudo agregar, quito nodo nuevo
                    padre.children.Remove(nuevo);
                    throw ex;
                }
                return nuevo;
            }
        }

        public Usuario getUsuarioConFloresDisponibles()
        {
            Usuario ret = null;
            foreach (Usuario u in grupo.usuarios)
            {
                if (u.habilitado && u.floresDisponibles().Count > 0)
                {
                    ret = u;
                    break;
                }
            }
            return ret;
        }

        public Usuario quitarFlor(Nodo n)
        {
            Usuario u = null;
            Flor f = null;

            if (n.flores > 0)
            {
                //busco un usuario que haya votado ese nodo
                foreach (Usuario u2 in grupo.usuarios)
                {
                    foreach (Flor f2 in u2.flores)
                        if (f2.id == n.id)
                        {
                            f = f2;
                            u = u2;
                        }
                }

                //libero la flor al usuario
                n.flores -= 1;
                f.id = 0;

                //borro la parte de la rama que no tenga flores
                verifyNodoSinFlores(n.id);

                //actualizo negados
                actualizarNegados();

                //veo si algun nodo alcanza el consenso
                //comprobarConsenso(); se comprueba al crear el arbolpersonal
            }
            else
                throw new Exception("El nodo no tiene flores para quitar");
            return u;
        }


        public bool verificarFloresCaducadas()
        {
            bool ret = false;
            foreach (Usuario u in grupo.usuarios)
                ret = ret || verificarFloresCaducadas(u);
            return ret;
        }

        public bool verificarFloresCaducadas(Usuario u)
        {
            //verifico caducadas
            bool caido = false;
            foreach (Flor f in u.flores)
            {
                //15 dias sin entrar o 60 dias la flor
                Nodo n = getNodo(f.id);
                if (n != null)
                {
                    if (f.id != 0 && (DateTime.Now.Subtract(u.lastLogin).TotalDays > 15 ||
                                  DateTime.Now.Subtract(f.born).TotalDays > 60)) 
                    {
                        quitarFlor(n, u);
                        string msg = Tools.tr("Flor caducada para [%1]", n.nombre, grupo.idioma);
                        if (n.flores <= 0 && n.children.Count == 0)
                            msg += "<br>" + Tools.tr("El tema ha caido", grupo.idioma);

                        u.alertas.Add(new Alerta(msg));
                        caido = true;
                    }
                    else if (n.consensoAlcanzado && DateTime.Now.Subtract(n.consensoFecha).TotalDays > 5)
                        quitarFlor(n, u);
                }
            }
            return caido;
        }

        public bool quitarFlor(Nodo n, Usuario u)
        {
            bool ret = false;
            Flor f = u.getFlor(n.id);

            if (f == null)
                throw new Exception("El usuario no tiene esa flor para quitar");

            //libero la flor al usuario
            n.flores -= 1;
            f.id = 0;
            f.born = DateTime.Now;

            //borro la parte de la rama que no tenga flores
            ret = verifyNodoSinFlores(n.id);

            //actualizo negados
            actualizarNegados();

            //veo si algun nodo alcanza el consenso
            //comprobarConsenso(); se comprueba al crear el arbolpersonal

            return ret;
        }

        public void asignarflor(Usuario u, Nodo n)
        {
            //si tiene una flor en nivel anterior la subo, si no uso una de las disponibles
            //si hay flor en pathn entonces la subo
            bool subida = false;

            if (n.consensoAlcanzado)
                throw new appException("Este debate ya ha alcanzado el consenso");

            foreach (Nodo padre in getPath(n.id))
            {
                Flor usada = u.getFlor(padre.id);
                if (usada != null && padre != n)
                {
                    //hay flor en un nodo anterior la subo
                    padre.flores -= 1;
                    usada.id = n.id;
                    usada.born = DateTime.Now;
                    n.flores += 1;
                    subida = true;

                    break;
                }
            }
            if (!subida)
            {
                //uso una flor disponible
                List<Flor> disponibles = u.floresDisponibles();
                if (disponibles.Count > 0)
                {
                    disponibles[0].id = n.id;
                    disponibles[0].born = DateTime.Now;
                    n.flores += 1;
                }
                else
                    throw new appException("No tienes flores disponibles");
            }
            //compruebo consenso alcanzado
            //comprobarConsenso(); se comprueba al crear el arbolpersonal

            //actualizo negados
            actualizarNegados();
        }

        private void actualizarNegados()
        {
            List<Nodo> nodos = toList();
            foreach (Nodo n in nodos)
            {
                if (n.nivel == n.niveles)
                {
                    //es una hoja de final de documento
                    n.negados = getNegados(n);
                }
            }
        }

        private bool verifyNodoSinFlores(int id)
        {
            //borro la parte de la rama que no tenga flores
            List<Nodo> pathn = getPath(id);
            bool ret = false;
            Nodo n = pathn[0];
            while (n.flores <= 0 && n.children.Count == 0)
            {
                removeNodo(n.id);
                ret = true;  //se ha quitado el nodo
                pathn.RemoveAt(0);
                if (pathn.Count > 0)
                    n = pathn[0];
                else
                    break;
            }
            return ret;
        }

        public Propuesta getPropuesta(int id)
        {
            foreach (Propuesta op in propuestas)
            {
                if (op.nodoID == id)
                    return op;
            }
            return null;
        }

        private void removeNodo(int id)
        {
            //quito el nodo
            removeNodo(raiz, id);

            //quito la propuesta
            foreach (Propuesta op in propuestas)
            {
                if (op.nodoID == id)
                {
                    propuestas.Remove(op);
                    break;
                }
            }
        }

        private void removeNodo(Nodo padre, int id)
        {
            foreach (Nodo hijo in padre.children)
            {
                if (hijo.id == id)
                {
                    padre.children.Remove(hijo);
                    break;
                }
                else
                    removeNodo(hijo, id);
            }
        }

        public Nodo getNodo(int id)
        {
            List<Nodo> p = getPath(id);
            if (p != null && p.Count > 0)
                return p[0];
            else
                return null;
        }

        public List<Nodo> getPath(int id)
        {
            //lista de ancestros. incluye la raiz y incluye al propio nodo
            //index=0 es el nodo buscado
            //index=count-1 es la raiz
            List<Nodo> ret = new List<Nodo>();
            getPath(raiz, id, ret);
            return ret;
        }

        public Propuesta getPropuesta(Nodo n)
        {
            //devuelvo la propuesta resultado de comparar el texto con sus hermanos
            Propuesta op = getPropuesta(n.id);
            return op;
        }

        private void getPath(Nodo padre, int id, List<Nodo> ret)
        {      
            if (padre.id == id)
            {               
                ret.Add(padre);
            }
            else
            {
                foreach (Nodo hijo in padre.children)
                {
                    int count = ret.Count;
                    getPath(hijo, id, ret);
                    if (ret.Count > count) //encontrado
                    {
                        //agrego padres
                        ret.Add(padre);
                    }
                }
            }
        }

        public void actualizarModelosEnUso()
        {
            //marco los modelos de documentos que estan en uso
            foreach (Modelo m in grupo.organizacion.getModelosDocumento())
            {
                m.enUso = false;
                foreach (Propuesta p in propuestas)
                    if (p.modeloID == m.id)
                    {
                        m.enUso = true;
                        break;
                    }
            }
        }


    }
}