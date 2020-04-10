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
using System.Runtime.Serialization;

namespace nabu
{
    public class Grupo
    {
        public string nombre = "";
        public Arbol arbol;
        public string objetivo = "";
        public string idioma = "ES";
        public Organizacion organizacion;
        public DateTime born = DateTime.Now;
        public DateTime ts = DateTime.Now;
        public List<Usuario> usuarios = new List<Usuario>();
        public string path = ""; //ruta fisica en el servidor
        public string URLEstatuto = "";
        public string URL = ""; //url base del arbol
        public string padreURL = "";
        public string padreNombre = "";
        public List<Hijo> hijos = new List<Hijo>();
        public List<LogDocumento> logDecisiones = new List<LogDocumento>();
        public List<LogDocumento> logResultados = new List<LogDocumento>();
        public Queso queso = new Queso();
        public string tipoGrupo = "cerrado";

        [IgnoreDataMember]
        public Bosque bosque = null;

        public static Grupo newGrupo(string grupo, string organizacion, string nombreAdmin, string email, string clave, string idioma, string tipoGrupo, string URL)
        {
            if (grupo == "")
                throw new appException("Nombre de grupo no puede ser vacio");
            if (email == "")
                throw new appException("Email no puede ser vacio");
            if (clave == "")
                throw new appException("Clave no puede ser vacio");
            if (Tools.HtmlEncode(grupo) != grupo)
                throw new appException("Nombre de grupo inv&aacute;lido. Evite acentos y caracteres especiales");
            if (Tools.HtmlEncode(email) != email)
                throw new appException("Email inv&aacute;lido. Evite acentos y caracteres especiales");
            if (Tools.HtmlEncode(clave) != clave)
                throw new appException("Clave inv&aacute;lida. Evite acentos y caracteres especiales");

            //veo que no exista ya
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Tools.MapPath("grupos"));

            foreach (System.IO.DirectoryInfo fi in di.GetDirectories())
            {
                if (fi.Name == grupo)
                {
                    //ya existe
                    throw new appException("El grupo ya existe");
                }
            }

            //creo
            Grupo g = new Grupo();
            g.nombre = grupo;
            g.path = Tools.MapPath("grupos/" + g.nombre);
            g.URL = URL;
            g.idioma = idioma;
            g.tipoGrupo = tipoGrupo;

            //organizacion
            switch (organizacion)
            {
                case "Plataforma":
                    g.organizacion = new organizaciones.Plataforma();
                    break;

                case "Cooperativa":
                    g.organizacion = new organizaciones.Cooperativa();
                    break;

                case "Colegio":
                    g.organizacion = new organizaciones.Colegio();
                    break;

                default:
                    throw new Exception("Modelo organizativo [" + organizacion + "] no existe");
            }

            Arbol a = new Arbol();
            a.nombre = grupo;
            a.raiz = new Nodo();
            a.raiz.nombre = Tools.HtmlEncode(a.nombre);
            a.grupo = g; //referencia ciclica, no se pude serializar
            g.arbol = a;

            Queso q = new Queso();
            q.grupo = g; //referencia ciclica, no se pude serializar
            g.queso = q;

            //admin
            Usuario u = new Usuario(a.cantidadFlores);
            u.nombre = Tools.HtmlEncode(nombreAdmin);
            u.email = email;
            u.clave = clave;
            u.isAdmin = true;
            g.usuarios.Add(u);

            return g;
        }

        public GrupoPersonal getGrupoPersonal(string email)
        {
            GrupoPersonal ret = new GrupoPersonal();
            ret.nombre = nombre;
            ret.objetivo = objetivo;
            Usuario u = getUsuario(email);
            ret.alertas = u.alertas;
            return ret;
        }

        public double getHorizontalidad()
        {
            //porcentaje de horizontalidad
            int factorRepresentacion = 10; //un presentante por cada 10 personas. Por ahora esta valore s fijo
            float cant = 0;
            float reps = 0;
            foreach (Usuario u in usuarios)
            {
                if (u.habilitado)
                {
                    cant++;
                    if (u.isRepresentante)
                        reps++;
                }
            }
            if (cant > 0)
            {
                double maxRep = Math.Ceiling(cant / factorRepresentacion);
                if (reps >= maxRep)
                    return 100;
                else
                    return reps / maxRep * 100;
            }
            else
                return 0;
        }

        public DateTime lastLogin
        {
            get
            {
                DateTime ret = DateTime.MinValue;
                foreach (Usuario u2 in usuarios)
                    if (u2.lastLogin > ret)
                        ret = u2.lastLogin;
                return ret;
            }
        }

        public int activos
        {
            get
            {
                int ret = 0;
                foreach (Usuario u2 in usuarios)
                    if (u2.habilitado && u2.isActive && !u2.readOnly)
                        ret += 1;
                return ret;
            }
        }

        public string toJsonCompleto()
        {
            arbol.grupo = null; //luego lo recupero
            queso.grupo = null; //luego lo recupero

            List<Type> tipos = new List<Type>();
            foreach (Modelo m in organizacion.getModelosDocumento()) tipos.Add(m.GetType());
            foreach (ModeloEvaluacion m in organizacion.getModelosEvaluacion()) tipos.Add(m.GetType());
            foreach (Organizacion m in Organizacion.getOrganizaciones()) tipos.Add(m.GetType());
            tipos.Add(new plataforma.Accion().GetType());
            tipos.Add(new plataforma.Evento().GetType());

            string json = Tools.toJson(this, tipos);

            //restauro al padre
            arbol.grupo = this;
            queso.grupo = this;
            return json;
        }

        public void save(string folderPath)
        {
            if (!arbol.simulacion)
            {
                string json = toJsonCompleto();
                string filepath = folderPath + "\\" + nombre + ".json";

                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);
            
                //foto del dia
                System.IO.StreamWriter fs;
                DateTime ayer = DateTime.Now.AddDays(-1);
                string date = ayer.Year.ToString("0000") + "-" + ayer.Month.ToString("00") + "-" + ayer.Day.ToString("00");
                string bkpath = folderPath + "\\" + nombre + "_" + date + ".json";
                if (!System.IO.File.Exists(bkpath))
                {              
                    //guardo foto con fecha
                    fs = new System.IO.StreamWriter(bkpath, false, System.Text.Encoding.UTF8);
                    fs.Write(json);
                    fs.Close();
                }

                fs = new System.IO.StreamWriter(filepath, false, System.Text.Encoding.UTF8);
                fs.Write(json);
                fs.Close();
            }
        }

        public Usuario getFacilitador()
        {
            foreach (Usuario u in usuarios)
            {
                if (u.isFacilitador)
                    return u;
            }
            return null;
        }

        public List<Usuario> getRepresentantes()
        {
            List<Usuario> ret = new List<Usuario>();
            foreach (Usuario u in usuarios)
            {
                if (u.isRepresentante)
                    ret.Add(u); 
            }
            return ret;
        }

        public Usuario getSecretaria()
        {
            foreach (Usuario u in usuarios)
            {
                if (u.isSecretaria)
                    return u;
            }
            return null;
        }

        public Usuario getAdmin()
        {
            foreach (Usuario u in usuarios)
            {
                if (u.isAdmin)
                    return u;
            }
            return null;
        }

        public List<Usuario> getUsuariosHabilitadosActivos()
        {
            List<Usuario> ret = new List<Usuario>();
            foreach (Usuario u in usuarios)
                if (u.habilitado && u.isActive && !u.readOnly)
                    ret.Add(u);
            return ret;
        }

        public List<Usuario> getUsuariosHabilitados()
        {
            List<Usuario> ret = new List<Usuario>();
            foreach (Usuario u in usuarios)
                if (u.habilitado)
                    ret.Add(u);
            return ret;
        }

        public List<Usuario> getUsuariosOrdenados()
        {
            //creo una copia ordenada
            List<Usuario> ret = new List<Usuario>();
            foreach (Usuario u in usuarios)
                ret.Add(u);
            ret.Sort();
            return ret;
        }

        public Usuario getUsuarioHabilitado(string email, string clave)
        {
            //comparo en minusculas por los moviles y iPad y tablets que ponen la 1ra en mayuscula y confunde
            Usuario ret = getUsuarioHabilitado(email);

            if (ret != null && ret.clave.ToLower() == clave.ToLower())
                return ret;
            else
                return null;
        }

        public Usuario getUsuarioHabilitado(string email)
        {
            Usuario ret = null;

            foreach (Usuario u in usuarios)
            {
                if (u.habilitado && u.email.ToLower() == email.ToLower())
                {
                    ret = u;
                    //muevo al inicio, cola lifo
                    usuarios.Remove(u);
                    usuarios.Insert(0, u);
                    break;
                }
            }
            return ret;
        }

        public Usuario getUsuario(string email, string clave)
        {
            //comparo en minusculas por los moviles y iPad y tablets que ponen la 1ra en mayuscula y confunde
            Usuario ret = getUsuario(email);

            if (ret != null && ret.clave.ToLower() == clave.ToLower())
                return ret;
            else
                return null;
        }

        public Usuario getUsuario(string email)
        {
            Usuario ret = null;

            foreach (Usuario u in usuarios)
            {
                if (u.email.Trim().ToLower() == email.Trim().ToLower())
                {
                    ret = u;
                    //muevo al inicio, cola lifo
                    usuarios.Remove(u);
                    usuarios.Insert(0, u);
                    break;
                }
            }
            return ret;
        }

        public Usuario removeUsuario(string email)
        {
            Usuario u = getUsuario(email);
            if (u == null)
                throw new appException("El usuario no existe");
            else
            {
                //quito sus flores
                foreach (Flor f in u.flores)
                {
                    if (f.id != 0)
                    {
                        //quito la flor
                        List<Nodo> pathn = arbol.getPath(f.id);
                        Nodo n = pathn[0];
                        n.flores -= 1;
                        f.id = 0;
                    }
                }

                //borro
                usuarios.Remove(u);
            }
            return u;
        }

        public string toJson()
        {
            return "{\"nombre\":\"" + nombre + "\", \"idioma\":\"" + idioma + "\", \"born\":" + Tools.toJson(born) + ", \"ts\":" + Tools.toJson(ts) + "}";
        }
    }
}