using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Drawing;


namespace nabu
{
    public class point3D
    {
        public float x;
        public float y;
        public float z;

        public point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Polar
    {
        public Polar()
        {
        }

        public Polar(float M, float A)
        {
            this.M = M;
            this.A = A;
        }
        public float M { get; set; }
        public float A { get; set; }
    }

    public static class Tools
    {
        public static List<Type> knowntypes;
        private static char coma = ' ';
        public static string startupPath;
        private static int fileIndex = 0;
        private static StreamWriter logFile;

        public static string dateToString(DateTime d)
        {
            //devuelvo formato texto ordenable
            return d.Year.ToString("0000") + "/" + d.Month.ToString("00") + "/" + d.Day.ToString("00") + " " + d.Hour.ToString("00") + ":" + d.Minute.ToString("00") + ":" + d.Second.ToString("00");
        }

        public static string writeTempImgFile(Image img)
        {
            Bitmap bmp;
            if (img is Bitmap) bmp = (Bitmap)img; else bmp = new Bitmap(img);
            string fn = getNewTempFilename(".gif");
            bmp.Save(startupPath + "\\wwwroot\\temp\\" + fn, System.Drawing.Imaging.ImageFormat.Gif);
            return fn;
        }

        public static string jsonFormat(Single s)
        {
            return s.ToString().Replace(",", ".");
        }

        public static System.Net.IPAddress solveIP(string ip)
        {
            if (ip.ToLower() == "any")
                return System.Net.IPAddress.Any;
            else
                return System.Net.Dns.Resolve(ip).AddressList[0];
        }

        public static void CloseLog()
        {
            if (logFile != null)
                logFile.Close();
        }

        public static void Log(string logMessage, string ip)
        {
            if (logFile == null)
                logFile = File.AppendText(startupPath + "\\log.txt");

            lock (logFile)
            {
                DateTime n = DateTime.Now;
                logFile.Write(n.ToShortDateString() + ";" + n.ToShortTimeString() + ";" + ip + ";" + logMessage + "\r\n");
            }
        }

        public static void Log(string logMessage)
        {
            Log(logMessage, "");
        }

        public static string getNewTempFilename(string extension)
        {
            fileIndex++;
            while (System.IO.File.Exists(startupPath + "\\wwwroot\\temp\\temp" + fileIndex + extension))
            {
                fileIndex++;
            }
            return "temp" + fileIndex + extension;
        }

        public static float ParseF(string n)
        {
            if (coma == ' ') coma = (0.0).ToString("0.0")[1];

            return float.Parse(n.Replace('.', coma));
        }

        public static Polar toPolar(int x, int y){
            Polar ret = new Polar();
            ret.M = (int)Math.Sqrt(x * x + y * y);
            if (x == 0)
                if (y > 0) ret.A = 90; else ret.A = 270;
            else if (y == 0)
                if (x > 0) ret.A = 0; else ret.A = 180;
            else
            {
                if (y >= 0 && x >= 0) ret.A = (int)(Math.Atan(y / x) * 180 / Math.PI);
                if (y >= 0 && x <= 0) ret.A = (int)(Math.Atan(y / -x) * 180 / Math.PI) + 90;
                if (y <= 0 && x <= 0) ret.A = (int)(Math.Atan(y / x) * 180 / Math.PI) + 180;
                if (y <= 0 && x >= 0) ret.A = (int)(Math.Atan(-y / x) * 180 / Math.PI) + 270;
            }

            return ret;
        }

        public static string toJson(object objeto)
        {
            if (objeto == null)
                return "null";
            else
            {
                //http://www.esasp.net/2010/06/c-serializar-json-datacontract.html
                string s = string.Empty;
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(objeto.GetType());
                MemoryStream ms = new MemoryStream();
                jsonSerializer.WriteObject(ms, objeto);
                s = Encoding.Default.GetString(ms.ToArray());
                return s;
            }
        }

        public static T fromJson<T>(this string jsonSerializado, List<Type> knowntypes)
        {
            try
            {
                //deserializo
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonSerializado));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(), knowntypes);
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
            catch (Exception ex) { throw new Exception("fromJson:" + ex.Message); }//return default(T); }
        }

        public static T fromJson<T>(this string jsonSerializado)
        {
            try
            {
                //deserializo
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonSerializado));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(), knowntypes);
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
            catch (Exception ex) { throw new Exception("fromJson:" + ex.Message); }//return default(T); }
        }

        public static PointF toRectDeg(Polar vector)
        {
            return toRectDeg(vector.M, vector.A);
        }

        public static PointF toRectDeg(float mod, float angDeg)
        {
            return new Point((int)(Math.Cos(angDeg * Math.PI / 180) * mod), (int)(Math.Sin(angDeg * Math.PI / 180) * mod));
        }

        public static PointF toRectRad(float mod, float angRad)
        {
            return new Point((int)(Math.Cos(angRad) * mod), (int)(Math.Sin(angRad) * mod));
        }

        public static string sendMailAlta(string arbol, string to, string nombre, string email, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\alta.html");
            msg = msg.Replace("%1", nombre);
            msg = msg.Replace("%2", email);
            msg = msg.Replace("%3", arbol);

            return sendMail(to, "Solicitud de alta en [" + arbol + "]", msg);
        }

        public static string sendMailWelcome(string arbol, string to, string clave, string url, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\welcome.html");
            msg = msg.Replace("%1", url);
            msg = msg.Replace("%2", to);
            msg = msg.Replace("%3", clave);
            msg = msg.Replace("%4", arbol);

            return sendMail(to, "Alta Nabú", msg);
        }

        public static string sendMail(string to, string subject, string body)
        {
            //envio por mail
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            string SMTPURL = System.Configuration.ConfigurationManager.AppSettings["SMTPURL"];
            string from = System.Configuration.ConfigurationManager.AppSettings["SMTPFrom"];
            string user = System.Configuration.ConfigurationManager.AppSettings["SMTPUser"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["SMTPPass"];
            string ret = "Enviado";

            if (SMTPURL != "")
            {
                try
                {
                    msg.From = new System.Net.Mail.MailAddress(from, from);
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    msg.Subject = subject;
                    msg.To.Add(new System.Net.Mail.MailAddress(to, to));

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(SMTPURL);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(user, pass);
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Send(msg);
                }
                catch (Exception ex)
                {
                    ret = "Error=" + ex.Message;
                }
            }
            return ret;
        }
    }
}
