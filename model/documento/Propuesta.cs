///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo sabtvg@gmail.com
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