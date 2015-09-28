using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Aplicacion
    {
        public List<Arbol> arboles = new List<Arbol>();
        public List<Log> logs = new List<Log>();
        public DateTime lastLogSent = DateTime.Now;

        public void addLog(string accion, string ip, string arbol, string email, string descripcion)
        {
            lock (logs)
            {
                Log l = new Log();
                l.ip = ip;
                l.accion = accion;
                l.arbol = arbol;
                l.email = email;
                l.descripcion = descripcion;
                logs.Add(l);
            }
        }
    }
}