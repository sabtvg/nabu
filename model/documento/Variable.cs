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

//representa una variable dentro de un documento

namespace nabu
{
    public class Variable
    {
        public Variable()
        {
            //default para serializar
        }

        public Variable(string id, int len)
        {
            this.id = id;
            this.len = len;
            this.nivel = 0;
        }

        public Variable(string id, int len, int nivel)
        {
            this.id = id;
            this.len = len;
            this.nivel = nivel;
        }

        public string id = "";
        public int len = 10;
        public int width = 100;
        public int height = 100;
        public string format = "0.00";
        public int nivel = 0;
        public string className = "texto";
        public string editClassName = "editar";
    }
}