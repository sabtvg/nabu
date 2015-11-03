using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Aplicacion
    {
        public List<Arbol> arboles = new List<Arbol>();
        public HttpServerUtility server;

        public Aplicacion(HttpServerUtility server){
            this.server = server;
        }

        public void addLog(string accion, string ip, string arbol, string email, string descripcion)
        {
            lock (this)
            {
                string l = "<tr><td>" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "</td>";
                l += "<td>" + accion + "</td>";
                l += "<td>" + arbol + "</td>";
                l += "<td>" + email + "</td>";
                l += "<td>" + descripcion + "</td></tr>";

                string fname = server.MapPath("logs.html");

                if (!System.IO.File.Exists(fname))
                {
                    string hd;
                    hd = "<head>";
                    hd += "<title>Nab&uacute logs</title>";
                    hd += "<link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\">";
                    hd += "</head>";
                    hd += "<h1>Nab&uacute;</h1>";
                    hd += "<h2>Logs</h2>";
                    hd += "<table style='border: 1px solid gray; padding: 5px; border-radius: 10px;font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: small;'>";
                    hd += "<tr>";
                    hd += "<td style='width:120px;'><b>IP</b></td>";
                    hd += "<td style='width:120px;'><b>Email</b></td>";
                    hd += "<td style='width:120px;'><b>Arbol</b></td>";
                    hd += "<td style='width:120px;'><b>TS</b></td>";
                    hd += "<td style='width:800px;'><b>Descripcion</b></td>";
                    hd += "</tr>";
                    System.IO.File.AppendAllText(fname, hd);
                }

                System.IO.File.AppendAllText(fname, l);
            }
        }
    }
}