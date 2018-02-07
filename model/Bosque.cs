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
    public class Bosque
    {
        public class Nodo
        {
            public string URL = "";
            public string nombre = "";
            public string padreURL = "";
            public string padreNombre = ""; 
            public string objetivo = "";
            public string msg = "";
            public string acceso = "";
            public int usuarios;
            public int activos;
            public bool padreVerificado;
            public List<Nodo> hijos = new List<Nodo>();
            public bool descargado = false;
        }

        private Grupo grupo = null;
        private Nodo padre = null;
        public DateTime born = DateTime.Now;

        public Bosque(Grupo g)
        {
            //creo todo el bosque en hilo aparte
            grupo = g;
            padre = new Nodo();
            padre.URL = g.URL;
            padre.nombre = g.nombre;
            padre.objetivo = g.objetivo;
            padre.acceso = "si";
            padre.usuarios = g.getUsuariosHabilitados().Count;
            padre.activos = g.getUsuariosHabilitadosActivos().Count;
            padre.padreVerificado = true;
            padre.descargado = true;

            foreach (Hijo hijo in g.hijos)
            {
                Nodo n = new Nodo();
                n.URL = hijo.URL;
                n.nombre = hijo.nombre;
                padre.hijos.Add(n);
            }

            System.Threading.Thread hilo = new System.Threading.Thread(poblar);
            hilo.Start();
            //poblar();
        }

        private void poblar(){
            poblar(padre);
        }

        private void poblar(Nodo n)
        {
            foreach (Nodo hijo in n.hijos)
            {
                try
                {
                    string ret = Tools.getHttp(hijo.URL + "/doMain.aspx?actn=getNodo&grupo=" + hijo.nombre);
                    if (ret.StartsWith("error="))
                        hijo.msg = ret;
                    else
                    {
                        lock (this)
                        {
                            Nodo nodoRet = Tools.fromJson<Nodo>(ret);
                            hijo.objetivo = nodoRet.objetivo;
                            hijo.padreURL = nodoRet.padreURL;
                            hijo.padreNombre = nodoRet.padreNombre;
                            hijo.hijos = nodoRet.hijos;
                            hijo.usuarios = nodoRet.usuarios;
                            hijo.activos = nodoRet.activos;
                            if (hijo.padreURL == n.URL && hijo.padreNombre == n.nombre)
                                hijo.padreVerificado = true;
                            hijo.descargado = true;
                        }

                        //recurso
                        poblar(hijo);

                        //retardo de pruebas
                        //System.Threading.Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    //timeout
                    hijo.msg = ex.Message;
                }
            }
        }

        public string toJson()
        {
            string ret = "";
            lock (this)
            {
                ret = Tools.toJson(padre);
            }
            return ret;
        }
    }
}