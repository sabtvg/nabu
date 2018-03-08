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

//representa un grupo de trabajo

namespace nabu.plataforma
{
    public abstract class Seguimiento
    {
        public class Estado
        {
            public int EID = 0;
            public string estado = "";
            public DateTime estadoTs = DateTime.Now;
            public string email = "";
            public string nombreUsuario = "";
            public string descrip = "";
            public DateTime inicio = DateTime.Now.AddDays(15);
            public DateTime fin = DateTime.Now.AddMonths(2);
            public int avance = 0;
            public string luz = "verde.png";
        }

        public int EID = 0;
        public string nombre = "";
        public string docURL = "";
        public DateTime docTs = Tools.minValue;
        public string objetivo = "";
        public string icono = "";
        public DateTime born = DateTime.Now;
        public string estadoEmail = "";
        public string responsable = "";
        public string modeloNombre = "";
        public string modeloID = "";
        public DateTime inicio = DateTime.Now.AddDays(15);
        public DateTime fin = DateTime.Now.AddMonths(2);

        public List<Estado> estados = new List<Estado>();

        public Estado estado
        {
            set
            {
            }
            get
            {
                if (estados.Count > 0)
                    return estados[estados.Count - 1];
                else
                    return new Estado();
            }
        }
    }
}