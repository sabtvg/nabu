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
        public class Usuario
        {
            public string nombre = "";
            public string funcion = "";
            public string email = "";
            public DateTime born = DateTime.Now;
            public bool isAdmin = false;
            public bool isSecretaria = false;
            public bool isFacilitador = false;
            public bool isRepresentante = false;
            public int apoyos = 0; //cantidad de apoyos por parte de otros usuarios
            public bool readOnly = false;
            public string grupoDesde = "";
            public bool isActive = false;
        }

        public class Seguimiento
        {
            public string nombre = "";
            public int EID;
            public DateTime born = DateTime.Now;
            public string objetivo = "";
            public string responsable = "";
            public string icono = "";
        }

        public class Nodo
        {
            public string URL = "";
            public string nombre = "";
            public string padreURL = "";
            public string padreNombre = ""; 
            public string objetivo = "";
            public string msg = "";
            public string acceso = "";
            public int activos;
            public bool padreVerificado;
            public List<Nodo> hijos = new List<Nodo>();
            public bool descargado = false;
            public float colorPromedio = 0;
            public double horizontalidad = 0; //cantidad representates / cantidad usuarios
            public List<Usuario> usuarios = new List<Usuario>();
            public List<Seguimiento> seguimientos = new List<Seguimiento>();
        }

        private Grupo grupo = null;
        private Nodo padre = null;
        public DateTime born = DateTime.Now;

        public Bosque(Grupo g)
        {
            //creo todo el bosque en hilo aparte
            grupo = g;
            padre = crearNodo(g);
            padre.padreVerificado = true;
            padre.descargado = true;

            System.Threading.Thread hilo = new System.Threading.Thread(poblar);
            hilo.Start();
            //poblar();
        }

        public static void copiarNodo(Nodo ret, Nodo modelo)
        {
            ret.nombre = modelo.nombre;
            ret.objetivo = modelo.objetivo;
            ret.padreURL = modelo.padreURL;
            ret.padreNombre = modelo.padreNombre;
            ret.hijos = modelo.hijos;
            ret.activos = modelo.activos;
            ret.colorPromedio = modelo.colorPromedio;
            ret.horizontalidad = modelo.horizontalidad;

            ret.usuarios.Clear();
            foreach (Usuario u in modelo.usuarios)
            {
                Usuario p = new Usuario();
                p.nombre = u.nombre;
                p.funcion = u.funcion;
                p.email = u.email;
                p.born = u.born;
                p.isAdmin = u.isAdmin;
                p.isSecretaria = u.isSecretaria;
                p.isFacilitador = u.isFacilitador;
                p.isRepresentante = u.isRepresentante;
                p.isActive = u.isActive;
                p.apoyos = u.apoyos; //cantidad de apoyos por parte de otros usuarios
                p.readOnly = u.readOnly;
                p.grupoDesde = u.grupoDesde;
                ret.usuarios.Add(p);
            }

            ret.seguimientos.Clear();
            foreach (Seguimiento u in modelo.seguimientos)
            {
                Seguimiento p = new Seguimiento();
                p.nombre = u.nombre;
                p.EID = u.EID;
                p.born = u.born;
                p.objetivo = u.objetivo;
                p.responsable = u.responsable;
                p.icono = u.icono;
                ret.seguimientos.Add(p);
            }
        }

        public static Nodo crearNodo(Grupo g)
        {
            Nodo nodo = new Nodo();
            nodo.nombre = g.nombre;
            nodo.URL = g.URL;
            nodo.objetivo = g.objetivo;
            nodo.padreURL = g.padreURL;
            nodo.padreNombre = g.padreNombre;
            nodo.acceso = "si";
            nodo.activos = g.getUsuariosHabilitadosActivos().Count;
            nodo.colorPromedio = g.queso.getColorPromedio();
            nodo.horizontalidad = g.getHorizontalidad();

            List<nabu.Usuario> usus = g.usuarios;
            usus.Sort(new nabu.Usuario.RolComparer());
            foreach (nabu.Usuario u in usus)
            {
                if (u.habilitado)
                {
                    Usuario p = new Usuario();
                    p.nombre = u.nombre;
                    p.funcion = u.funcion;
                    p.email = u.email;
                    p.born = u.born;
                    p.isAdmin = u.isAdmin;
                    p.isSecretaria = u.isSecretaria;
                    p.isFacilitador = u.isFacilitador;
                    p.isRepresentante = u.isRepresentante;
                    p.isActive = u.isActive;
                    p.apoyos = u.apoyos; //cantidad de apoyos por parte de otros usuarios
                    p.readOnly = u.readOnly;
                    p.grupoDesde = u.grupoDesde;
                    nodo.usuarios.Add(p);
                }
            }

            if (g.organizacion.GetType().Name == "Plataforma")
            {
                organizaciones.Plataforma pl = (organizaciones.Plataforma)g.organizacion;

                foreach (plataforma.Seguimiento u in pl.seguimientos)
                {
                    Seguimiento p = new Seguimiento();
                    p.nombre = u.nombre;
                    p.EID = u.EID;
                    p.born = u.born;
                    p.objetivo = u.objetivo;
                    p.responsable = u.responsable;
                    p.icono = u.icono;
                    nodo.seguimientos.Add(p);
                }
            }

            foreach (Hijo hijo in g.hijos)
            {
                Bosque.Nodo n = new Bosque.Nodo();
                n.URL = hijo.URL;
                n.nombre = hijo.nombre;
                nodo.hijos.Add(n);
            }
            return nodo;
        }

        private void poblar(){
            poblar(padre, ";");
        }

        private void poblar(Nodo n, string nombres)
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
                            Bosque.copiarNodo(hijo, nodoRet);

                            //compruebo padre
                            if (hijo.padreURL == n.URL && hijo.padreNombre == n.nombre)
                                hijo.padreVerificado = true;

                            //compruebo recursividad
                            if (nombres.IndexOf(";" + hijo.nombre + ";") >= 0)
                                throw new appException("Recursividad detectada");
                            else
                                nombres += hijo.nombre + ";";

                            hijo.descargado = true;
                        }

                        //recurso
                        poblar(hijo, nombres);

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