using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu.organizaciones
{
    public class Cooperativa: Organizacion
    {

        public override List<Modelo> getModelosDocumento()
        {
            //deberian ser modelos propios!!!!!
            List<Modelo> ret = new List<Modelo>();
            return ret;
        }

        public override List<ModeloEvaluacion> getModelosEvaluacion()
        {
            //deberian ser modelos propios!!!!!
            List<ModeloEvaluacion> ret = new List<ModeloEvaluacion>();
            return ret;
        }

        public override string doAccion(Grupo g, string email, string accion, HttpRequest req)
        {
            return "no implementado";
        }

        public override string getOperativo(Grupo grupo)
        {
            return "no implementado";
        }

    }
}