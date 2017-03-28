using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Propuesta: IComparable
    {
        public DateTime born = DateTime.Now;
        public string titulo = ""; //titulo del documento
        public string etiqueta = "";
        public string email = "";
        public int nodoID = 0;
        public string modeloID = "";
        public int nivel = 0;
        public int niveles = 0; //niveles en el momento de crear la propuesta
        public DateTime ts = DateTime.Now;
        public Dictionary<string, object> bag = new Dictionary<string, object>();
        public List<Comentario> comentarios = new List<Comentario>();
        public bool consensoAlcanzado = false;

        public Propuesta clone()
        {
            Propuesta ret = new Propuesta();
            ret.titulo = titulo;
            ret.etiqueta = etiqueta;
            ret.nodoID = nodoID;
            ret.nivel = nivel;
            ret.email = email;
            ret.modeloID = modeloID;
            ret.niveles = niveles;

            foreach (KeyValuePair<string, object> var in bag)
                ret.bag.Add(var.Key, var.Value);

            //no clono comentarios

            return ret;
        }

        int IComparable.CompareTo(Object x)
        {
            Propuesta dos = (Propuesta)x;
            return this.nivel - dos.nivel;
        }

        public bool esPrevista()
        {
            return nodoID == 0;
        }
    }
}