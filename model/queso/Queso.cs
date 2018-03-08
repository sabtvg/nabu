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
    public class Queso
    {
        public List<Tema> temas = new List<Tema>();
        public int caducaDias = 30;
        public int lastEvalID = 1;
        public Grupo grupo;


        public QuesoPersonal getQuesoPersonal(string email)
        {
            return getQuesoPersonal(email, "");
        }

        public QuesoPersonal getQuesoPersonal(string email, string msg)
        {
            limpiarCaducados();

            QuesoPersonal ret = new QuesoPersonal();
            ret.nombre = grupo.nombre;
            ret.temas = temas;
            ret.caducaDias = caducaDias;
            ret.msg = msg;
            ret.colorPromedio = getColorPromedio();

            /////queda pendiente clonar temas, evalauciones y quitar preguntas para que el mensaje sea bastante mas chico

            return ret;
        }

        public Tema getTema(string id)
        {
            foreach (Tema t in temas)
                if (t.id == id)
                    return t;
            return null;
        }

        public float getColorPromedio()
        {
            float total = 0;
            int cant = 0;
            foreach (Tema t in temas)
            {
                foreach (Evaluacion ev in t.evaluaciones)
                {
                    foreach(Pregunta pr in ev.preguntas)
                    {
                        total += pr.respuesta;
                        cant++;
                    }
                }
            }
            if (cant == 0)
                return 0;
            else
                return total / cant;
        }

        public void limpiarCaducados()
        {
            int indext = 0;
            while (indext < temas.Count)
            {
                Tema t = temas[indext];
                int indexe = 0;
                while (indexe < t.evaluaciones.Count)
                {
                    Evaluacion e = t.evaluaciones[indexe];
                    if (DateTime.Now.Subtract(e.born).TotalDays > caducaDias)
                    {
                        Usuario u = grupo.getUsuario(e.email);
                        if (u != null)
                            u.alertas.Add(new Alerta(Tools.tr("Evaluacion caida para el tema [%1]", t.nombre, grupo.idioma)));

                        t.evaluaciones.RemoveAt(indexe);
                        
                        if (t.evaluaciones.Count == 0)
                        {
                            temas.RemoveAt(indext);
                            break;
                        }
                    }
                    else
                        indexe++;
                }
                if (t.evaluaciones.Count == 0 && DateTime.Now.Subtract(t.born).TotalDays > caducaDias)
                    temas.RemoveAt(indext);
                else
                    indext++;
            }
        }

        public void evaluar()
        {
            //calculo respuestas segun las evaluaciones
            foreach (Tema t in temas)
                t.evaluar(grupo);
        }
    }
}