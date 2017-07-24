using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//representa un grupo de trabajo

namespace nabu.plataforma
{
    public class Estrategia
    {
        public int EID = 0;
        public string nombre = "";
        public string docURL = "";
        public DateTime docTs = Tools.minValue;
        public string revision = "";
        public string objetivo = "";
        public DateTime born = DateTime.Now;
        public string definicion = "";

    }
}