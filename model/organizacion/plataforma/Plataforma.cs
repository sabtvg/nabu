using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu.organizaciones
{
    public class Plataforma: Organizacion
    {
        public List<nabu.plataforma.GrupoTrabajo> gruposTrabajo = new List<nabu.plataforma.GrupoTrabajo>();
        public string URLEstatuto = "";
        public string objetivo = "";

        public override string getEstructura(Grupo grupo)
        {
            string ret = "{\"objetivo\":\"" + objetivo + "\",";
            ret += "\"URLEstatuto\":\"" + URLEstatuto + "\",";
            ret += "\"gruposTrabajo\":[";

            foreach (nabu.plataforma.GrupoTrabajo gt in gruposTrabajo)
            {
                ret += "{\"nombre\":\"" + gt.nombre + "\",";
                ret += "\"born\":\"" + gt.born.ToShortDateString() + " " + gt.born.ToShortTimeString() + "\",";
                ret += "\"docURL\":\"" + gt.docURL + "\",";
                ret += "\"docTs\":\"" + gt.docTs.ToShortDateString() + " " + gt.docTs.ToShortTimeString() + "\",";
                ret += "\"revision\":\"" + gt.revision + "\",";
                ret += "\"objetivo\":\"" + gt.objetivo + "\",";

                //usuarios
                ret += "\"usuarios\":[";
                foreach (string email in gt.integrantes)
                {
                    ret += "{\"email\":\"" + email + "\",";
                    Usuario u = grupo.getUsuario(email);
                    if (u == null)
                        ret += "\"estado\":\"NoExiste\"";
                    else
                    {
                        ret += "\"nombre\":\"" + u.nombre + "\"";
                    }
                    ret += "},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "],";

                //procesos
                ret += "\"procesos\":[";
                foreach (nabu.plataforma.Proceso pr in gt.procesos)
                {
                    ret += "{\"nombre\":\"" + pr.nombre + "\",";
                    ret += "\"born\":\"" + pr.born.ToShortDateString() + " " + pr.born.ToShortTimeString() + "\",";
                    ret += "\"docURL\":\"" + pr.docURL + "\",";
                    ret += "\"docTs\":\"" + pr.docTs.ToShortDateString() + " " + pr.docTs.ToShortTimeString() + "\",";
                    ret += "\"revision\":\"" + pr.revision + "\",";
                    ret += "\"entradas\":\"" + pr.entradas + "\",";
                    ret += "\"definicion\":\"" + pr.definicion + "\",";
                    ret += "\"objetivo\":\"" + pr.objetivo + "\"";
                    ret += "},";
                }
                if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
                ret += "]";
                ret += "},";
            }
            if (ret.EndsWith(",")) ret = ret.Substring(0, ret.Length - 1);
            ret += "]}";

            return ret;
        }

        public override List<Modelo> getModelos()
        {
            List<Modelo> ret = new List<Modelo>();
            ret.Add(new plataforma.modelos.Manifiesto());
            ret.Add(new plataforma.modelos.GrupoTrabajo());
            ret.Add(new plataforma.modelos.Proceso());
            ret.Add(new plataforma.modelos.Accion());
            ret.Add(new plataforma.modelos.Evento());
            return ret;
        }

    }
}