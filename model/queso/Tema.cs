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
using System.Linq;
using System.Web;

namespace nabu
{
    public class Tema
    {
        public List<Evaluacion> evaluaciones = new List<Evaluacion>();
        public List<int> respuestas = new List<int>(); //el promedio de todas las evaluaciones
        public string modeloEvaluacionID = "";
        public string id = "";
        public string nombre = "";
        public string icono = "";
        public string URL = "";
        public string evaluadoID = ""; //id del documento evaluado
        public string autor = "";
        public DateTime born = DateTime.Now;

        public void evaluar(Grupo g)
        {
            //calculo respuestas segun las evaluaciones
            if (modeloEvaluacionID != "")
            {
                ModeloEvaluacion ev = g.organizacion.getModeloEvaluacion(modeloEvaluacionID);
                respuestas = ev.evaluar(evaluaciones);
            }
        }
    }
}