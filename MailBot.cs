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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

//esta clase envia emails de una carepta crean un hilo nuevo sin bloquear el hilo principal

namespace nabu
{
    public class MailBot
    {
        private bool sending = false;
        private List<FileInfo> txts;

        public void send()
        {
            bool actuar = false;
            lock (this)
                if (!sending)
                { sending = true; actuar = true; }

            if (actuar)
            {
                //veo si hay algo en cola
                if (!Directory.Exists(Tools.startupPath + "\\mails\\errores"))
                    Directory.CreateDirectory(Tools.startupPath + "\\mails\\errores");
                string path = Tools.startupPath + "\\mails\\cola";
                DirectoryInfo di = new DirectoryInfo(path);
                txts = di.GetFiles("*.txt").ToList();

                if (txts.Count > 0)
                {
                    //envio todo lo que haya en cola
                    System.Threading.Thread hilo = new System.Threading.Thread(sendCola);
                    hilo.Start();
                }
            }
        }

        private void sendCola()
        {
            while (txts.Count > 0)
            {
                FileInfo fi = txts[0];
                string txt = File.ReadAllText(fi.FullName);
                string para = txt.Substring(0, txt.IndexOf("\r\n"));
                txt = txt.Substring(txt.IndexOf("\r\n") + 2);
                string asunto = txt.Substring(0, txt.IndexOf("\r\n"));
                txt = txt.Substring(txt.IndexOf("\r\n") + 2);
                string body = txt;

                try
                {
                    //Tools.sendMail(para, asunto, body);
                    throw new Exception("error");
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Tools.startupPath + "\\mails\\errores\\log.txt",
                        DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                        + " " + fi.Name
                        + " " + ex.Message + "\r\n");

                    if (!File.Exists(Tools.startupPath + "\\mails\\errores\\" + fi.Name))
                        File.Move(fi.FullName, Tools.startupPath + "\\mails\\errores\\" + fi.Name);
                }
                //System.Threading.Thread.Sleep(1000); //test

                txts.RemoveAt(0);
            }

            //libero estado
            lock (this) { sending = false; }
        }
    }
}