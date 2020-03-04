using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k163808_Q3
{
    public static class Log
    {

        public static void writeLog(String message)
        {
            StreamWriter sw = null;
            try
            {
                string Date = System.DateTime.Now.ToString("dd-MM-yyyy");

                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFolder\\Service " + Date + ".txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + message);
                sw.Flush();
                sw.Close();
            }

            catch (Exception e)
            {

            }
        }
    }
}

