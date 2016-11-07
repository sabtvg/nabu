using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Usuario
    {
        public string nombre = "";
        public string email = "";
        public string clave = "";
        public List<Flor> flores = new List<Flor>();
        public DateTime lastLogin = DateTime.Now;
        public DateTime born = DateTime.Now;
        public bool isAdmin = false;
        public int apoyos = 0; //cantidad de apoyos por parte de otros usuarios
        public DateTime notificado = DateTime.Now;
        public Prevista prevista = null;

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