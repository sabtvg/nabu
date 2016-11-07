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
        public DateTime born = DateTime.Now;
        public DateTime ts = DateTime.Now;
        public List<Usuario> usuarios = new List<Usuario>();
        public DateTime lastBackup = DateTime.Now.AddHours(-1);
        public string path = ""; //ruta fisica en el servidor
        public string URLEstatuto = "";
        public string URL = ""; //url base del arbol

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
                    if (u2.isActive)
                        ret += 1;
                return ret;
            }
        }

        public void save(string folderPath)
        {
            if (!arbol.simulacion)
            {
                List<Type> tipos = new List<Type>();
                foreach (Modelo m in Modelo.getModelos())
                {
                    tipos.Add(m.GetType());
                }
                arbol.grupo = null;
                string json = Tools.toJson(this, tipos);

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
                    fs = System.IO.File.CreateText(filepath);
                    fs.Write(json);
                    fs.Close();

                    lastBackup = DateTime.Now;
                }

                fs = System.IO.File.CreateText(filepath);
                fs.Write(json);
                fs.Close();

                //restauro al padre
                arbol.grupo = this;
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
                    //login correcto
                    ret = u;
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