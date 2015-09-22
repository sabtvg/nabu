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
        public Usuario usuario;
        public Nodo raiz;

        //condicion de consenso
        public int usuarios = 0; //cantidad de usuarios
        public int activos = 0; //cantidad de usuarios activos
        public float minActivosPc = 80; //porcentaje minimo de usuarios activos en el arbol para alcanzar consenso
        public float minImplicadosPc = 80; //porcentaje minimo de usuarios implicados en el debate (en una rama) para alcanzar consenso
        public float maxNegadosPc = 10; //porcentaje maximo de usuarios en otras ramas del mismo debate (en una rama) para alcanzar consenso

    }
}