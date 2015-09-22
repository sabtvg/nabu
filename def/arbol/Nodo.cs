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
        public bool consensoAlcanzado = false;
        public float x = 0; //posicion x en el cliente
        public int negados = 0; //votos en contra
        public int nivel = 0;
    }


    public class NodoComparerMayor : IComparer<Nodo>
    {
        public int Compare(Nodo a, Nodo b)
        {
            return b.flores - a.flores;
        }
    }

    public class NodoComparerMenor : IComparer<Nodo>
    {
        public int Compare(Nodo a, Nodo b)
        {
            return a.flores - b.flores;
        }
    }
}