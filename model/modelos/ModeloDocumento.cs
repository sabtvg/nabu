using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class ModeloDocumento
    {
        public List<Seccion> secciones = new List<Seccion>();
        public string nombre = "";
        public int id = 0;
        public bool enUso = false;
        public bool activo = true;

        public ModeloDocumento clone()
        {
            ModeloDocumento ret = new ModeloDocumento();
            ret.nombre = nombre;
            ret.activo = false;
            foreach (Seccion s in secciones)
            {
                Seccion snueva = new Seccion();
                foreach(Tema t in s.temas)
                {
                    Tema tnueva = new Tema();
                    tnueva.titulo = t.titulo;
                    tnueva.tip = t.tip;
                    tnueva.maxLen = t.maxLen;
                    snueva.temas.Add(tnueva);
                }

                ret.secciones.Add(snueva);
            }
            return ret;
        }

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

