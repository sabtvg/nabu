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
    public class LogDocumento
    {
        public DateTime fecha = DateTime.Now;
        public string modeloNombre;
        public string icono = "res/doc.png";
        public string modeloID = "";
        public string documentoNombre = "";
        public string titulo = "";
        public float x;
        public string fname = ""; //nombre del archivo del documento
        public int docID = 0;
        public string arbol = "";
        public string objetivo = "";
        public string URL = "";
        public int flores = 0;
        public int negados = 0;
        public string carpeta = "";
        public string autor = "";

        public string sFecha
        {
            get
            {
                return fecha.ToShortDateString();
            }
            set
            {
            }
        }

        public float dias
        {
            get
            {
                return (float)DateTime.Now.Subtract(fecha).TotalMinutes / (24 * 60);
            }
            set
            {
            }
        }

        public float minutos
        {
            get
            {
                return (float)DateTime.Now.Subtract(fecha).TotalMinutes;
            }
            set
            {
            }
        }
    }
}