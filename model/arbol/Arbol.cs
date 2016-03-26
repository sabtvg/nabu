using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace nabu
{
    public class Arbol
    {
        public string nombre = "";
        public string objetivo = "";
        public int cantidadFlores = 5;
        public List<Usuario> usuarios = new List<Usuario>();
        public Nodo raiz;
        public bool leftRight = false; //alterna agregado de nuevos nodos de un lado y del otro
        public List<Propuesta> propuestas = new List<Propuesta>();
        public DateTime born = DateTime.Now;
        public string path = ""; //ruta fisica en el servidor
        public int lastDocID = 1;
        public int lastNodoID = 10;
        public bool simulacion = false;
        public Usuario lastSimUsuario;
        public DateTime ts = DateTime.Now;
        public string URLEstatuto = "";
        public string URL = ""; //url base del arbol
        public DateTime lastBackup = DateTime.Now.AddHours(-1);

        //modelos de documento para este arbol
        public List<ModeloDocumento> modelosDocumento = new List<ModeloDocumento>();

        //log de consensos alcanzados
        public List<LogDocumento> logDocumentos = new List<LogDocumento>();

        //condicion de consenso
        public float minSiPc = 60; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNoPc = 15; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso

        [NonSerialized]
        public Random rnd = new Random();

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

        private List<Nodo> toList2(Nodo node, List<Nodo> l) {
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

        public int activos
        {
            get
            {
                int ret = 0;
                foreach (Usuario u2 in usuarios)
                    if (u2.isActive)
                        ret += 1;
                return ret;
            }
        }

        public float minSiValue
        {
            get
            {
                return (float)Math.Ceiling(usuarios.Count * minSiPc / 100);
            }
        }

        public float maxNoValue
        {
            get
            {
                return (float)Math.Ceiling(minSiValue * maxNoPc / 100);
            }
        }

        public Usuario getAdmin()
        {
            foreach (Usuario u in usuarios)
            {
                if (u.isAdmin)
                    return u;
            }
            return null;
        }

        private bool comprobarConsenso()
        {
            List<Nodo> nodos = toList();
            foreach (Nodo n in nodos)
                if (comprobarConsenso(n))
                    return true;
            return false;
        }

        private bool comprobarConsenso(Nodo n)
        {
            bool ret = false;
            ModeloDocumento m = getModelo(n.modeloID);
            List<Nodo> pathn = getPath(n.id);

            if (m != null && pathn.Count - 1 == m.secciones.Count)
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
                    //if (!simulacion)
                    //{
                        generarDocumentoHTML(n, fdate, fname);
                        generarDocumentoJSON(n, fdate, fname);
                        save(path + "\\documentos\\" + nombre + ".json"); //guardo copia del arbol
                    //}

                    //guardo el log historico en el arbol
                    Propuesta p = getPropuesta(pathn[0].id);  //obtengo el titulo del debate de cualquiera de las propuestas 
                    LogDocumento ld = new LogDocumento();
                    ld.fecha = fdate;
                    ld.titulo = p.titulo;
                    ld.nombre = getModelo(n.modeloID).nombre;
                    ld.x = n.x;
                    ld.docID = docID;
                    ld.fname = fname;
                    ld.arbol = nombre;
                    ld.objetivo = objetivo;
                    ld.URL = URL + "/cooperativas/" + nombre + "/documentos/" + fname + ".html";
                    logDocumentos.Add(ld);
                    
                    //marco a todos los nodos del debate
                    for (int i = 0; i < pathn.Count - 1; i++) //menos la raiz
                    {
                        pathn[i].consensoAlcanzado = true;
                        foreach (Nodo n2 in pathn[i].children)
                            marcarConsenso(n2);
                    }

                    ret = true;
                }
            }
            return ret;
        }

        private void marcarConsenso(Nodo n)
        {
            //marco sus hijos
            n.consensoAlcanzado = true;
            foreach (Nodo n2 in n.children)
            {
                n2.consensoAlcanzado = true;
                marcarConsenso(n2);
            }
        }

        public void save(string folderPath)
        {
            string json = Tools.toJson(this);
            string filepath = folderPath + "\\" + nombre + ".json";

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);
            
            //copa de seguridad
            if (DateTime.Now.Subtract(lastBackup).TotalDays >= 1)
            {
                string date = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + " " + DateTime.Now.Hour.ToString("00") + "-" + DateTime.Now.Minute.ToString("00"); 
                string bkpath = folderPath + "/" + nombre + " " + date + ".json";
                System.IO.File.Copy(filepath, bkpath);
                lastBackup = DateTime.Now;
            }

            System.IO.StreamWriter fs = System.IO.File.CreateText(filepath);
            fs.Write(json);
            fs.Close();
        }

        private void generarDocumentoJSON(Nodo n, DateTime now, string fname)
        {
            ModeloDocumento m = getModelo(n.modeloID);
            Documento doc = new Documento();
            doc.fecha = now;
            doc.nombre = m.nombre;
            foreach(Nodo n2 in getPath(n.id))
                doc.propuestas.Add(getPropuesta(n2.id));
            doc.raiz = raiz;

            //guardo
            System.IO.File.WriteAllText(path + "\\documentos\\" + fname + ".json", Tools.toJson(doc));
        }

        private void generarDocumentoHTML(Nodo n, DateTime now, string fname)
        {
            List<Nodo> pathn = getPath(n.id);
            ModeloDocumento m = getModelo(n.modeloID);

            if (!System.IO.Directory.Exists(path + "\\documentos"))
                System.IO.Directory.CreateDirectory(path + "\\documentos");

            if (!System.IO.File.Exists(path + "\\documentos\\styles.css"))
                System.IO.File.Copy(path + "\\..\\..\\styles.css", path + "\\documentos\\styles.css");

            //armo HTML
            string html = "<html>";
            html += "<head>";
            html += "<title></title>";
            html += "<meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1' />";
            html += "<link rel='stylesheet' type='text/css' href='styles.css'>";
            html += "</head>";
            html += "<body>";

            Propuesta p = getPropuesta(pathn[0].id);  //obtengo el titulo del debate de cualquiera de las propuestas 
            if (p == null)
                html += "<div class='titulo0'>" + m.nombre + ":</div>";
            else
                html += "<div class='titulo0'>" + m.nombre + ":" + p.titulo + "</div>";

            html += "<div class='titulo2'>" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</div>";

            for(int i = pathn.Count - 2; i >= 0; i--) //escribo en orden inverso
            {
                p = getPropuesta(pathn[i].id);

                foreach(TextoTema tt in p.textos)
                {
                    html += "<div class='titulo1'>" + tt.titulo + "</div><br>";
                    html += tt.texto + "<br><br>";
                }
                html += "<hr>";
            }

            //firma
            html += "Documento escrito de forma cooperativa.<br>";
            html += "Documento ID:" + fname + "<br>";
            html += "Fecha de consenso: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br>";
            html += "Ubicaci&oacute;n: <a target='_blank' href='" + URL + "/cooperativas/" + nombre + "/documentos/" + fname + ".html'>" + URL + "/cooperativas/" + nombre + "/documentos/" + fname + ".html</a><br>";
            html += "Cooperativa: " + this.nombre + "<br>";
            html += "Objetivo: " + this.objetivo + "<br>";
            html += "Usuarios: " + this.usuarios.Count + "<br>";
            html += "Activos: " + this.activos + "<br>";
            html += "A favor (Si &ge; " + this.minSiPc + "%): " + n.flores + "<br>";
            html += "En contra (No &le; " + this.maxNoPc + "%): " + n.negados + "<br>";

            html += "</body>";

            System.IO.File.WriteAllText(path + "\\documentos\\" + fname + ".html", html);
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

        public void setModelosDocumentoDefault()
        {
            //creo modelos de documentos default

            ModeloDocumento d = new ModeloDocumento();  ////modelo de simulacion !!!
            d.id = 1;
            d.nombre = "Accion";
            d.crear(0, "Resumen y motivacion", "El consenso es un proceso cooperativo. Somos constructivos con nuestras propuestas y consideramos el bien comun", 3000);
            d.crear(1, "Objetivo a lograr", "Describe que pretendes que logremos", 2000);
            d.crear(1, "Descripcion", "Describe con mayor detalle como sera", 3500);
            d.crear(1, "A quien va dirigido", "Quienes se beneficiaran", 1000);
            d.crear(2, "Materiales", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(2, "Software", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(2, "RRHH", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(3, "Fases", "Describe las fases que se deben alcanzar para lograr el objetivo", 3000);
            d.crear(4, "Presupuesto y plazo de entrega", "Adjunta un documento PDF detallando la propuesta, puedes usar graficos para entendernos mejor", 500);
            modelosDocumento.Add(d);

            d = new ModeloDocumento();
            d.id = 2;
            d.nombre = "Comision";
            d.crear(0, "Resumen y motivacion", "&iquest;porque neceistamos una nueva comision? &iquest;Que actividades realizara?", 2000);
            d.crear(1, "Objetivo de la comision", "&iquest;Que debe lograr la nueva comision?", 3500);
            d.crear(1, "Descripcion de actividades de la comision", "Detalla las actividades a realizar", 3500);
            d.crear(1, "A quien van dirigidas sus actuaciones", "", 3500);
            d.crear(2, "Capacidades necesarias", "", 3500);
            d.crear(3, "Composicion de la comision", "", 3500);
            d.crear(4, "Como medir su eficiencia", "", 3500);
            modelosDocumento.Add(d);

            d = new ModeloDocumento();
            d.id = 3;
            d.nombre = "Evento";
            d.crear(0, "Resumen y motivacion", "&iquest;Como sera el evento?", 3500);
            d.crear(1, "Objetivo del evento", "", 3500);
            d.crear(1, "Descripcion", "", 3500);
            d.crear(1, "A quien va dirigido el evento", "", 3500);
            d.crear(2, "Lugar", "", 3500);
            d.crear(2, "Materiles", "", 3500);
            d.crear(2, "Transporte", "", 3500);
            d.crear(3, "Organizacion del evento", "", 3500);
            d.crear(4, "Como medir su eficiencia", "", 3500);
            modelosDocumento.Add(d);

            d = new ModeloDocumento();
            d.id = 4;
            d.nombre = "Metodologia";
            d.crear(0, "Resumen y motivacion", "&iquest;Como sera la metodologia?", 3500);
            d.crear(1, "Para que sirve", "", 3500);
            d.crear(1, "Descripcion", "", 3500);
            d.crear(1, "A quien va dirigida", "", 3500);
            d.crear(2, "Definicion de la metodologia", "", 4500);
            d.crear(3, "Como medir su eficiencia", "", 3500);
            d.crear(3, "Como implantarla", "", 3500);
            d.crear(4, "Fases del desarrollo", "", 3500);
            d.crear(4, "Tiempo de desarrollo", "", 3500);
            modelosDocumento.Add(d);
        }

        public ArbolPersonal getArbolPersonal(string email)
        {
            Usuario u = getUsuario(email);
            if (u != null)
            {
                ArbolPersonal ap = new ArbolPersonal();
                ap.raiz = raiz;
                ap.objetivo = objetivo;
                ap.URLEstatuto = URLEstatuto;
                ap.nombre = nombre;
                ap.usuarios = usuarios.Count;
                ap.cantidadFlores = cantidadFlores;
                ap.activos = activos;
                ap.simulacion = simulacion;

                ap.usuario = u;
                ap.minSiPc = minSiPc;
                ap.maxNoPc = maxNoPc;

                ap.minSiValue = minSiValue;
                ap.maxNoValue = maxNoValue;

                ap.logDocumentos = logDocumentos;

                return ap;
            }
            else
                throw new appException("El usuario no existe");
        }

        public Nodo addNodo(Nodo padre, string email, string nombre, List<TextoTema> tts, int modeloID){
            //verifico que el usuario tiene al menos una flor disponible
            Usuario u = getUsuario(email);
            if (nombre == null)
                throw new appException("Nombre de nodo no puede ser vacio");
            else if (u == null)
                throw new appException("El usuario no existe");
            else if (tts.Count == 0)
                throw new appException("Los textos no pueden ser vacios");
            else if (padre == null)
                throw new appException("El nodo no existe");
            else
            {
                //agrego nuevo nodo
                Nodo nuevo = new Nodo();
                nuevo.nombre = nombre;
                nuevo.id = lastNodoID++;
                nuevo.modeloID = modeloID;

                try
                {
                    //agrego al arbol
                    if (leftRight)
                        padre.children.Add(nuevo);
                    else
                        padre.children.Insert(0, nuevo);

                    //fijo nivel
                    nuevo.nivel = getPath(nuevo.id).Count - 1;  //quito la raiz

                    leftRight = !leftRight;

                    //agrego la propuesta
                    ModeloDocumento m = getModelo(modeloID);
                    Propuesta op = new Propuesta();
                    op.titulo = nombre;
                    op.modeloID = modeloID;
                    op.nodoID = nuevo.id;
                    op.seccion = getPath(nuevo.id).Count - 2;
                    foreach (TextoTema tt in tts)
                    {
                        op.textos.Add(tt);
                    }
                    propuestas.Add(op);

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
            foreach (Usuario u in usuarios)
            {
                if (u.floresDisponibles().Count > 0)
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
                foreach (Usuario u2 in usuarios)
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
                comprobarConsenso();
            }
            else
                throw new Exception("El nodo no tiene flores para quitar");
            return u;
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
            comprobarConsenso();

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
                if (usada != null)
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
            comprobarConsenso(n);

            //actualizo negados
            actualizarNegados();
        }

        private void actualizarNegados()
        {
            List<Nodo> nodos = toList();
            foreach (Nodo n in nodos)
            {
                ModeloDocumento m = getModelo(n.modeloID);
                if (m != null && n.nivel == m.secciones.Count())
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
            if (p != null)
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
            foreach (ModeloDocumento m in modelosDocumento)
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

        public Usuario getUsuario(string email, string clave)
        {
            //comparo en minusculas por los moviles y iPad y tablets que ponen la 1ra en mayuscula y confunde
            Usuario ret = getUsuario(email);

            if (ret != null && ret.clave.ToLower() == clave.ToLower())
                return ret;
            else
                return null;
        }

        public Usuario getUsuario(string email)
        {
            Usuario ret = null;

            foreach (Usuario u in usuarios)
            {
                if (u.email.ToLower() == email.ToLower())
                {
                    //login correcto
                    ret = u;
                }
            }
            return ret;
        }

        public ModeloDocumento getModelo(int modeloID)
        {
            ModeloDocumento ret = null;

            foreach (ModeloDocumento m in modelosDocumento)
            {
                if (m.id == modeloID)
                {
                    ret = m;
                }
            }
            return ret;
        }

        public Usuario removeUsuario(string email)
        {
            Usuario u = getUsuario(email);
            if (u == null)
                throw new appException("El usuario no existe");
            else
            {
                //quito sus flores
                foreach (Flor f in u.flores)
                {
                    if (f.id != 0)
                    {
                        //quito la flor
                        List<Nodo> pathn = getPath(f.id);
                        Nodo n = pathn[0];
                        n.flores -= 1;
                        f.id = 0;
                    }
                }

                //borro
                usuarios.Remove(u);
            }
            return u;
        }
    }
}