///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo nabu@nabu.pt
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
///////////////////////////////////////////////////////////////////////////


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
        public int version = 1;

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
                Modelo m = grupo.organizacion.getModeloDocumento(modeloID);
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
            ret += "<div class='titulo2'>Fecha: " + fecha.ToString("dd/MM/yy") + "</div>";
            ret += "<div class='titulo2'>Modelo" + ": " + nombre + "</div>";
            ret += "<div class='titulo2'><a href='" + URLPath + "' target='_blank'>" + URLPath + "</a></div><br>";
                            
            ret += "<table>";
            foreach (Log l in logs)
            {
                ret += "<tr>";
                ret += "<td>" + l.fecha.ToString("dd/MM/yy") + "</td><td>" + l.msg + "</td>";
                ret += "<tr>";
            }
            ret += "</table>";

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