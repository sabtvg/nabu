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
        public string titulo = ""; //el nodo de nivel 1 contiene el titulo del debate
        public List<TextoTema> textos = new List<TextoTema>(); //tantos textos como temas
        public List<string> comentarios = new List<string>();
        public DateTime ts = DateTime.Now;
    }
}