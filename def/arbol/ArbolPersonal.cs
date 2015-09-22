using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class ArbolPersonal
    {
        public string nombre = "";
        public string objetivo = "";
        public string URLEstatuto = "";
        public Usuario usuario;
        public Nodo raiz;
        public int cantidadFlores = 5;
        public bool simulacion = false;

        //log de consensos alcanzados
        public List<LogDocumento> logDocumentos = new List<LogDocumento>();

        //condicion de consenso
        public int usuarios = 0; //cantidad de usuarios
        public int activos = 0; //cantidad de usuarios activos
        public float minSiPc = 80; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNoPc = 10; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso

        public float minSiValue = 0; 
        public float maxNoValue = 0; 

    }
}