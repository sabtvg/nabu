using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{

    public class Tema
    {
        public string titulo = "";
        public string tip = "";
        public int maxLen = 3000;

        public Tema()
        {
        }

        public Tema(string titulo, string tip, int maxLen)
        {
            this.titulo = titulo;
            this.tip = tip;
            this.maxLen = maxLen;
        }
    }
}