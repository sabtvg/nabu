using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class LogDocumento
    {
        public DateTime fecha = DateTime.Now;
        public string modeloNombre;
        public string icono = "res/doc.png";
        public string modeloID = "";
        public string titulo = "";
        public float x;
        public string fname = ""; //nombre del archivo del documento
        public int docID = 0;
        public string arbol = "";
        public string objetivo = "";
        public string URL = "";
        public int flores = 0;
        public int negados = 0;
        public string carpeta = "";

        public string sFecha
        {
            get
            {
                return fecha.ToShortDateString();
            }
            set
            {
            }
        }

        public float dias
        {
            get
            {
                return (float)DateTime.Now.Subtract(fecha).TotalMinutes / (24 * 60);
            }
            set
            {
            }
        }

        public float minutos
        {
            get
            {
                return (float)DateTime.Now.Subtract(fecha).TotalMinutes;
            }
            set
            {
            }
        }
    }
}