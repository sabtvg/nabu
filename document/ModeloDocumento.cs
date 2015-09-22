using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class ModeloDocumento
    {
        public List<Seccion> secciones = new List<Seccion>();
        public string titulo = "";
        public int id = 0;
        public bool enUso = false;
        public bool activo = true;

        public ModeloDocumento crear(int seccion, string titulo, string tip, int maxLen)
        {
            if (secciones.Count > seccion)
            {
                secciones[seccion].temas.Add(new Tema(titulo, tip, maxLen));
            }
            else
            {
                secciones.Add(new Seccion(new Tema(titulo, tip, maxLen)));
            }
            return this;
        }
    }
}

