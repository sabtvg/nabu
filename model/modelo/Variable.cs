using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//representa una variable dentro de un documento

namespace nabu
{
    public class Variable
    {
        public Variable()
        {
            //default para serializar
        }

        public Variable(string id, int len, int nivel)
        {
            this.id = id;
            this.len = len;
            this.nivel = nivel;
        }

        public string id = "";
        public int len = 10;
        public int width = 100;
        public int height = 100;
        public string format = "0.00";
        public int nivel = 0;
        public string className = "texto";
        public string editClassName = "editar";
    }
}