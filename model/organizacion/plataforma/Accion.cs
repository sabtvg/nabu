using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//representa un grupo de trabajo

namespace nabu.plataforma
{
    public class Accion
    {
        public class Estado
        {
            public int EID = 0;
            public string estado = "";
            public DateTime estadoTs = DateTime.Now;
            public string email = "";
            public string descrip = "";
        }

        public int EID = 0;
        public string nombre = "";
        public string docURL = "";
        public DateTime docTs = Tools.minValue;
        public string objetivo = "";
        public DateTime born = DateTime.Now;
        public string estadoEmail = "";

        public List<Estado> estados = new List<Estado>();

        public string estado {
            get
            {
                if (estados.Count > 0)
                    return estados[estados.Count - 1].estado;
                else
                    return "";
            }
        }

        public DateTime estadoTs
        {
            get
            {
                if (estados.Count > 0)
                    return estados[estados.Count - 1].estadoTs;
                else
                    return born;
            }
        }

    }
}