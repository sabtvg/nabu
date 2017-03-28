using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu.organizaciones
{
    public class Colegio: Organizacion
    {

        public override List<Modelo> getModelos()
        {
            //deberian ser modelos propios!!!!!
            List<Modelo> ret = new List<Modelo>();
            return ret;
        }

        public override string getEstructura(Grupo grupo)
        {
            return "no implementado";
        }

    }
}