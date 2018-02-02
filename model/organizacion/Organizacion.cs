///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo sabtvg@gmail.com
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
using System.Web;

namespace nabu
{
    public abstract class Organizacion
    {
        //datos de arbol si es que tiene
        public string grupoURL = "";
        public string grupoNombre = "";
        public string grupoOrganizacion = "";
        public string grupoIdioma = "";

        public DateTime born = DateTime.Now;

        public Organizacion()
        {
        }

        public Modelo getModeloDocumento(string id)
        {
            foreach (Modelo m in getModelosDocumento())
            {
                if (m.id == id)
                    return m;
            }
            throw new Exception("Modelo [" + id + "] no existe");
        }

        public ModeloEvaluacion getModeloEvaluacion(string id)
        {
            foreach (ModeloEvaluacion m in getModelosEvaluacion())
            {
                if (m.id == id)
                    return m;
            }
            throw new Exception("Modelo [" + id + "] no existe");
        }

        public abstract List<Modelo> getModelosDocumento();
        public abstract List<ModeloEvaluacion> getModelosEvaluacion();

        public abstract string getOperativo(Grupo g);
        public abstract string doAccion(Grupo g, string email, string accion, HttpRequest req);

        private int lastEID = 0;

        public int getEID()
        {
            int ret = 0;
            lock (this)
                ret = ++lastEID;
            return ret;
        }

        public static List<Organizacion> getOrganizaciones()
        {
            //aqui se dan de alta los modelos existentes
            List<Organizacion> ret = new List<Organizacion>();
            ret.Add(new organizaciones.Plataforma());
            ret.Add(new organizaciones.Cooperativa());
            ret.Add(new organizaciones.Colegio());
            return ret;
        }
    }
}