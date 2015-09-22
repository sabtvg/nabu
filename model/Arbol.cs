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
        public List<Usuario> usuarios = new List<Usuario>();
        public Nodo raiz;
        public bool leftRight = false; //alterna agregado de nuevos nodos de un lado y del otro
        public List<Propuesta> propuestas = new List<Propuesta>();
        public DateTime born = DateTime.Now;

        //modelos de documento para este arbol
        public List<ModeloDocumento> modelosDocumento = new List<ModeloDocumento>();

        //condicion de consenso
        public float minActivosPc = 80; //porcentaje minimo de usuarios activos en el arbol para alcanzar consenso
        public float minImplicadosPc = 80; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNegadosPc = 10; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso


        public void setModelosDocumentoDefault()
        {
            //creo modelos de documentos default

            ModeloDocumento d = new ModeloDocumento();
            d.id = 1;
            d.titulo = "Propuesta";
            d.crear(0, "Resumen de la propuesta y motivo", "El consenso es un proceso cooperativo. Somos constructivos con nuestras propuestas y consideramos el bien comun", 3000);
            d.crear(1, "Objetivo a lograr", "Describe que pretendes que logremos", 2000);
            d.crear(1, "Descripcion", "Describe con mayor detalle como sera", 3500);
            d.crear(1, "A quien va dirigido", "Quienes se beneficiaran", 1000);
            d.crear(2, "Materiales", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(2, "Software", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(2, "RRHH", "Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 1500);
            d.crear(3, "Fases", "Describe las fases que se deben alcanzar para lograr el objetivo", 3000);
            d.crear(4, "Documento adjunto", "Adjunta un documento PDF detallando la propuesta, puedes usar graficos para entendernos mejor", 500);
            modelosDocumento.Add(d);

            d = new ModeloDocumento();
            d.id = 2;
            d.titulo = "Nueva comision";
            d.crear(0, "Resumen y motivacion", "¿porque neceistamos una nueva comision? ¿Que actividades realizara?", 2000);
            d.crear(1, "Objetivo de la comision", "¿Que debe lograr la nueva comision?", 3500);
            d.crear(1, "Descripcion de actividades de la comision", "Detalla las actividades a realizar", 3500);
            d.crear(1, "A quien van dirigidas sus actuaciones", "", 3500);
            d.crear(2, "Materiales", "", 3500);
            d.crear(2, "Software", "", 3500);
            d.crear(2, "RRHH", "", 3500);
            d.crear(3, "Composicion de la comision", "", 3500);
            d.crear(4, "Documento adjunto", "", 3500);
            modelosDocumento.Add(d);

            d = new ModeloDocumento();
            d.id = 3;
            d.titulo = "Evento";
            d.crear(0, "Resumen", "¿Como sera el evento?", 3500);
            d.crear(1, "Objetivo del evento", "", 3500);
            d.crear(1, "Descripcion", "", 3500);
            d.crear(1, "A quien va dirigido el evento", "", 3500);
            d.crear(2, "Lugar", "", 3500);
            d.crear(2, "Materiles", "", 3500);
            d.crear(2, "Transporte", "", 3500);
            d.crear(3, "Organizacion del evento", "", 3500);
            d.crear(4, "Documento adjunto", "", 3500);
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
                ap.nombre = nombre;
                ap.usuarios = usuarios.Count;

                foreach (Usuario u2 in usuarios)
                    if (u2.isActive)
                        ap.activos += 1;

                ap.usuario = u;
                ap.minActivosPc = minActivosPc;
                ap.minImplicadosPc = minImplicadosPc;
                ap.maxNegadosPc = maxNegadosPc;
                return ap;
            }
            else
                throw new appException("El usuario no existe");
        }

        public void addNodo(Nodo padre, string email, string nombre, string textos, int modeloID){
            //verifico que el usuario tiene al menos una flor disponible
            Usuario u = getUsuario(email);
            if (nombre == null)
                throw new appException("Nombre de nodo no puede ser vacio");
            else if (u == null)
                throw new appException("El usuario no existe");
            else if (textos == null)
                throw new appException("Los textos no pueden ser vacios");
            else if (padre == null)
                throw new appException("El nodo no existe");
            else
            {
                //agrego nuevo nodo
                Nodo nuevo = new Nodo();
                nuevo.nombre = nombre;
                nuevo.id = getMaxID() + 1;
                nuevo.modeloID = modeloID;

                //agrego la propuesta
                Propuesta op = new Propuesta();
                op.modeloID = modeloID;
                op.nodoID = nuevo.id;
                op.seccion = 0;
                foreach(string texto in textos.Split('|')){
                    op.textos.Add(texto); 
                }
                propuestas.Add(op);

                try
                {
                    //agrego al arbol
                    if (leftRight)
                        padre.children.Add(nuevo);
                    else
                        padre.children.Insert(0, nuevo);

                    //consumo una flor
                    asignarflor(u, nuevo);

                    leftRight = !leftRight;
                }
                catch (Exception ex)
                {
                    //no se pudo agregar, quito nodo nuevo
                    padre.children.Remove(nuevo);
                    throw ex;
                }
            }
        }

        public void asignarflor(Usuario u, Nodo n)
        {
            //si tiene una flor en nivel anterior la subo, si no uso una de las disponibles
            //si hay flor en pathn entonces la subo
            bool subida = false;

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

        public void removeNodo(int id)
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
                    getPath(hijo, id, ret);
                    if (ret.Count > 0) //encontrado
                    {
                        //agrego padres
                        ret.Add(padre);
                    }
                }
            }
        }

        public int getMaxID()
        {
            return getMaxID(raiz);
        }

        private int getMaxID(Nodo n)
        {
            int ret = n.id;
            int MaxHijo;

            foreach (Nodo hijo in n.children)
            {
                MaxHijo = getMaxID(hijo);
                if (MaxHijo > ret)
                    ret = MaxHijo;
            }
            return ret;
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
            Usuario ret = getUsuario(email);

            if (ret != null && ret.clave == clave)
                return ret;
            else
                return null;
        }

        public Usuario getUsuario(string email)
        {
            Usuario ret = null;

            foreach (Usuario u in usuarios)
            {
                if (u.email == email)
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
                throw new Exception("El usuario no existe");
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