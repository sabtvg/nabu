using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public DateTime lastBackup = DateTime.Now.AddHours(-1);
        public string path = ""; //ruta fisica en el servidor
        public string URLEstatuto = "";
        public string URL = ""; //url base del arbol
        public string padreURL = "";
        public string padreNombre = "";
        public List<Tuple<string, string>> hijos = new List<Tuple<string, string>>();
        public List<LogDocumento> logDecisiones = new List<LogDocumento>();
        public List<LogDocumento> logResultados = new List<LogDocumento>();

        public static Grupo newGrupo(string grupo, string organizacion, string nombreAdmin, string email, string clave, string idioma, string URL)
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

            //admin
            Usuario u = new Usuario(a.cantidadFlores);
            u.nombre = Tools.HtmlEncode(nombreAdmin);
            u.email = email;
            u.clave = clave;
            u.isAdmin = true;
            g.usuarios.Add(u);

            return g;
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
                    if (u2.habilitado && u2.isActive)
                        ret += 1;
                return ret;
            }
        }

        public string toJsonCompleto()
        {
            arbol.grupo = null; //luego lo recupero

            List<Type> tipos = new List<Type>();
            foreach (Modelo m in organizacion.getModelos()) tipos.Add(m.GetType());
            foreach (Organizacion m in Organizacion.getOrganizaciones()) tipos.Add(m.GetType());
            string json = Tools.toJson(this, tipos);

            //restauro al padre
            arbol.grupo = this;
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
                if (DateTime.Now.Subtract(lastBackup).TotalDays >= 1)
                {
                    string date = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00");
                    string bkpath = folderPath + "\\" + nombre + "_" + date + ".json";
                    if (System.IO.File.Exists(bkpath))
                        System.IO.File.Delete(bkpath);
                
                    //guardo foto con fecha
                    fs = new System.IO.StreamWriter(filepath, false, System.Text.Encoding.UTF8);
                    //fs = System.IO.File.CreateText(filepath);
                    fs.Write(json);
                    fs.Close();

                    lastBackup = DateTime.Now;
                }

                fs = new System.IO.StreamWriter(filepath, false, System.Text.Encoding.UTF8);
                //fs = System.IO.File.CreateText(filepath);
                fs.Write(json);
                fs.Close();
            }
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
                if (u.habilitado && u.isActive)
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
                if (u.email.ToLower() == email.ToLower())
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