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
using System.Text;
using System.Threading.Tasks;

namespace nabu
{
    class Comparador
    {
        static string[] separadores = { " ", "\r\n", ".", "\t", ",", "(", ")", "=", "{", "}", ":", "<", ">" };

        public static string comparar(string master, List<string> vals)
        {
            //resalto las palabras menos aparecidas de master en vals
            if (vals.Count == 0)
                //return HTMLEncode(master);
                return master;
            else
            {
                //creo listas de palabras
                List<string> masterPals = getPalabras(master);
                List<List<string>> valsPals = new List<List<string>>();
                foreach (string val in vals)
                    valsPals.Add(getPalabras(val));

                //master = HTMLEncode(master);

                //analizo repeticiones y reemplazo en master
                foreach (string pal in masterPals) //para cada palabra de Master
                {
                    float aparicion = getAparicion(pal, valsPals);
                    if (aparicion < 0.5)
                        master = replace(master, pal, HTMLResaltar(pal, aparicion));
                }

                return master;
            }
        }

        static string replace(string master, string pal, string newPal)
        {
            //reemplazo palabras que acaban o empienza con un separador
            foreach (string sep1 in separadores)
                foreach (string sep2 in separadores)
                {
                    master = master.Replace(sep1 + pal + sep2, sep1 + newPal + sep2);
                }

            //casos especiales de inicio y fin
            if (master.StartsWith(pal))
            {
                master = newPal + master.Substring(pal.Length);
            }

            if (master.EndsWith(pal))
            {
                master = master.Substring(0, master.Length - pal.Length) + newPal;
            }

            return master;
        }

        static string HTMLResaltar(string palabra, float veces)
        {
            //menos veces mas resalto
            if (veces == 1)
                return palabra;
            else
            {
                int rojo = 155 + (int)Math.Truncate(100 * veces);
                string hex = rojo.ToString("x");
                if (hex.Length == 1) hex = "0" + hex;
                string color = "#" + hex + "FFFF";
                return "<font style='background:" + color + "'>" + palabra + "</font>";
            }
        }

        static float getAparicion(string palabra, List<List<string>> valsPals)
        {
            //veces=1 -> aparece en todos los vals
            //veces=0 -> no aparece en ninguno
            float veces = 0;
            foreach (List<string> valPals in valsPals)
            {
                if (valPals.Count == 0)
                    veces++; //como si apareciera
                else
                    foreach (string pal in valPals)
                    {
                        if (palabra.ToLower() == pal.ToLower()) veces++;
                    }
            }
            return veces / valsPals.Count;
        }

        static List<string> getPalabras(string val)
        {
            List<string> ret = new List<string>();
            string palabra = "";
            string sep = "";

            while (val != "")
            {
                if (startsWithSep(val, ref sep))
                {
                    //guardo palabra actual
                    if (palabra.Length > 3 && !ret.Contains(palabra))
                        ret.Add(palabra);
                    palabra = "";
                    val = val.Substring(sep.Length); //me como el separador
                }
                else
                {
                    //acumulo palabra
                    palabra += val[0];
                    val = val.Substring(1); //me como el caracter
                }
            }
            //agrego lo que queda
            if (palabra.Length > 3 && !ret.Contains(palabra))
                ret.Add(palabra);

            return ret;
        }

        static bool startsWithSep(string val, ref string sepFound)
        {
            foreach (string sep in separadores)
                if (val.StartsWith(sep))
                {
                    sepFound = sep;
                    return true;
                }
            return false;
        }

        static string HTMLEncode(string s)
        {
            s = s.Replace("\r\n", "<br>");
            //s = s.Replace(" ", "&nbsp;");
            return s;
        }

    }
}
