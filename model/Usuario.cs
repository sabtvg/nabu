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

        public Usuario()
        {
            for (int q = 0; q < doArbol.cantidadFlores; q++)
            {
                flores.Add(new Flor());
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