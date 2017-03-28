using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//representa un grupo de trabajo

namespace nabu.plataforma
{
    public class GrupoTrabajo
    {
        public string nombre = "";
        public string docURL = "";
        public DateTime docTs = Tools.minValue;
        public string revision = "";
        public string objetivo = "";
        public List<string> integrantes = new List<string>(); //emails
        public List<nabu.plataforma.Proceso> procesos = new List<nabu.plataforma.Proceso>();
        public DateTime born = DateTime.Now;

        //datos del arbol si es que tiene
        public string grupoURL = "";
        public string grupoNombre = "";
        public string organizacion = "";
        public string idioma = "";

    }
}