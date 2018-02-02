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
    public class ArbolPersonal
    {
        public string nombre = "";
        public string objetivo = "";
        public string URLEstatuto = "";
        public Usuario usuario;
        public Nodo raiz;
        public int cantidadFlores = 5;
        public bool simulacion = false;
        public int nuevoNodoID = 0;
        public DateTime born = DateTime.Now;
        public int documentos;
        public string idioma = "ES";
        public string organizacion = "";
        public string padreURL = "";
        public string padreNombre = "";
        public List<Hijo> hijos = new List<Hijo>();


        //log de consensos alcanzados
        public List<LogDocumento> logDecisiones = new List<LogDocumento>();

        //log de resultados alcanzados
        public List<LogDocumento> logResultados = new List<LogDocumento>();

        //condicion de consenso
        public int usuarios = 0; //cantidad de usuarios
        public int activos = 0; //cantidad de usuarios activos
        public float minSiPc = 80; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNoPc = 10; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso

        public float minSiValue = 0; 
        public float maxNoValue = 0; 

    }
}