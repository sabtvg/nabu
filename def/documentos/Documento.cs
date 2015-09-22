using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Documento
    {
        public DateTime fecha;
        public string nombre = "";
        public string titulo = "";
        public List<Propuesta> propuestas = new List<Propuesta>();
        public Nodo raiz;


    }
}