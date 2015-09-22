using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{

    public class Seccion
    {
        public List<Tema> temas = new List<Tema>();

        public Seccion()
        {
        }

        public Seccion(Tema t)
        {
            temas.Add(t);
        }
    }
}
