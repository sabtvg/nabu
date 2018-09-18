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
    public class Nodo
    {
        public string nombre = "";
        public int id = 1;
        public List<Nodo> children = new List<Nodo>(); //debe mantenerse con este nombre o falla el cliente
        public int flores = 0;
        public string modeloID = "";
        public DateTime born = DateTime.Now;
        public bool consensoAlcanzado = false;
        public float x = 0; //posicion x en el cliente
        public int negados = 0; //votos en contra
        public int nivel = 0;
        public string email = ""; //creador
        public int niveles = 0; //niveles del modelo en el momento de crear el nodo
        public bool objecion = false;
        public bool comentario = false;
        public int lastEtiquetaID = 1;
        
        public int getFloresTotales() {
            int ret=0;
            foreach (Nodo hijo in children) 
            {           
                ret += hijo.getFloresTotales();
            }
            return ret + flores;
        }
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