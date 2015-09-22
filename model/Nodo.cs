using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Nodo
    {
        public string nombre = "";
        public int id = 1;
        public List<Nodo> children = new List<Nodo>(); //debe mantenerse con este nombre o falla el cliente
        public int flores = 0;
        public int modeloID = 0;
        public DateTime born = DateTime.Now;
    }
}