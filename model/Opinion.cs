using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Propuesta
    {
        public DateTime born = DateTime.Now;
        public int nodoID = 0;
        public int modeloID = 0;
        public int seccion = 0;
        public List<string> textos = new List<string>(); //tantos textos como temas
    }
}