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
    public class Usuario: IComparable
    {
        public string nombre = "";
        public string email = "";
        public string clave = "";
        public List<Flor> flores = new List<Flor>();
        public DateTime lastLogin = DateTime.Now;
        public DateTime born = DateTime.Now;
        public bool isAdmin = false;
        public bool isSecretaria = false;
        public bool isFacilitador = false;
        public bool isRepresentante = false;
        public int apoyos = 0; //cantidad de apoyos por parte de otros usuarios
        public DateTime notificado = DateTime.Now;
        public Prevista prevista = null;
        public bool readOnly = false;
        public bool habilitado = true;
        public string grupoDesde = "";

        public Usuario()
        {
            //constructor default para el deserializador json
        }

        public Usuario(int cantidadFlores)
        {
            for (int q = 0; q < cantidadFlores; q++)
            {
                flores.Add(new Flor());
            }
        }

        int IComparable.CompareTo(Object x)
        {
            Usuario dos = (Usuario)x;
            return (int)this.lastLogin.Subtract(dos.lastLogin).TotalSeconds;
        }

        public bool isActive
        {
            get
            {
                return DateTime.Now.Subtract(lastLogin).TotalDays < 7;
            }
            set
            {
            }
        }

        public string sLastLogin
        {
            get
            {
                return lastLogin.ToShortDateString();
            }
            set
            {
            }
        }

        public Flor getFlor(int id)
        {
            foreach (Flor f in flores)
                if (f.id == id)
                    return f;
            return null;
        }

        public List<Flor> floresDisponibles()
        {
            List<Flor> ret = new List<Flor>();
            foreach (Flor f in flores)
                if (f.id == 0)
                    ret.Add(f);
            return ret;
        }
    }
}