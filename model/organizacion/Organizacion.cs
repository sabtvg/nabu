using System;
using System.Collections.Generic;
using System.Web;

namespace nabu
{
    public abstract class Organizacion
    {
        //datos de arbol si es que tiene
        public string grupoURL = "";
        public string grupoNombre = "";
        public string grupoOrganizacion = "";
        public string grupoIdioma = "";

        public DateTime born = DateTime.Now;

        public Modelo getModelo(string id)
        {
            foreach (Modelo m in getModelos())
            {
                if (m.id == id)
                    return m;
            }
            throw new Exception("Modelo [" + id + "] no existe");
        }

        public abstract List<Modelo> getModelos();

        public abstract string getOperativo(Grupo g);
        public abstract string doAccion(Grupo g, string email, string accion, HttpRequest req);

        private int lastEID = 0;

        public int getEID()
        {
            int ret = 0;
            lock (this)
                ret = ++lastEID;
            return ret;
        }

        public static List<Organizacion> getOrganizaciones()
        {
            //aqui se dan de alta los modelos existentes
            List<Organizacion> ret = new List<Organizacion>();
            ret.Add(new organizaciones.Plataforma());
            ret.Add(new organizaciones.Cooperativa());
            ret.Add(new organizaciones.Colegio());
            return ret;
        }
    }
}