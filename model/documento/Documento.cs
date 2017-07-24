using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class Documento
    {
        public class Log
        {
            public DateTime fecha = DateTime.Now;
            public string msg = "";
        }

        public DateTime fecha;
        public string nombre = "";
        public string titulo = "";
        public string fname = "";
        public string path = "";
        public string URLPath = "";
        public string modeloID = "";
        public Grupo grupo;
        public List<Propuesta> propuestas = new List<Propuesta>();
        public List<Log> logs = new List<Log>();

        //anulacion
        public DateTime anuladoTs = Tools.minValue; //no anulado
        public string anuladoNombre = "";
        public string anuladoTitulo = "";
        public string anuladoFname = "";
        public string anuladoPath = "";
        public string anuladoURLPath = "";

        public static Documento load(string path)
        {
            System.IO.StreamReader fs = System.IO.File.OpenText(path);
            string json = fs.ReadToEnd();
            fs.Close();

            Documento ret = Tools.fromJson<Documento>(json);
            return ret;
        }

        public string getText(string id)
        {
            string ret = "";
            try
            {
                ret = (string)getValor(id);
            }
            catch (Exception ex)
            {
                ret = "";
            }
            return ret;
        }

        public Object getValor(string id)
        {
            foreach(Propuesta p in propuestas)
                if (p.bag.ContainsKey(id))
                        return p.bag[id];
            throw new Exception("Variable [" + id + "] no existe");
        }

        public void addLog(string msg)
        {
            Log l = new Log();
            l.msg = msg;
            logs.Add(l);
        }

        public void EjecutarConsenso()
        {
            try
            {
                Modelo m = grupo.organizacion.getModelo(modeloID);
                m.ejecutarConsenso(this);
                addLog("Consenso procesado");
            }
            catch (Exception ex)
            {
                addLog("EjecutarConsenso(): <font color=red>" + ex.Message + "</font>");
            }
        }

        public string toHTMLSeguimiento()
        {
            string ret = "";
            ret += "<div class='titulo1'>Seguimiento: " + fname + "</div>";
            ret += "<div class='titulo1'>Titulo: " + titulo + "</div>";
            ret += "<div class='titulo2'>Fecha: " + fecha.ToShortDateString() + "</div>";
            ret += "<div class='titulo2'>Modelo" + ": " + nombre + "</div>";
            ret += "<div class='titulo2'><a href='" + URLPath + "' target='_blank'>" + URLPath + "</a></div><br>";
                            
            ret += "<table>";
            foreach (Log l in logs)
            {
                ret += "<tr>";
                ret += "<td>" + l.fecha.ToShortDateString() + "</td><td>" + l.msg + "</td>";
                ret += "<tr>";
            }
            ret += "</table>";

            Modelo m = grupo.organizacion.getModelo(modeloID);
            ret += "<br><br><input type='button' class='btn' value='" + Tools.tr("Cerrar", grupo.idioma) + "' onclick='doCerrarDocumento();' />";
            return ret;
        }

        public void save()
        {
            //guardo
            Grupo g = grupo;
            grupo = null; //referencia ciclica
            System.IO.StreamWriter fs = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8);
            //System.IO.StreamWriter fs = System.IO.File.CreateText(path);
            fs.Write(Tools.toJson(this));
            fs.Close();

            grupo = g;
        }
    }
}