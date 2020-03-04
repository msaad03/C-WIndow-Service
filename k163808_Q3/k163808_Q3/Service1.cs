using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using System.Configuration;

namespace k163808_Q3
{

    public partial class Service1 : ServiceBase
    {
        Timer CheckMail = new Timer();

        public Service1()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            this.CheckMail.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);
            this.CheckMail.Elapsed += new ElapsedEventHandler(this.checkingMail);
            this.CheckMail.Enabled = true;

            Log.writeLog("Service Started");
        }

        private void checkingMail(object sender, ElapsedEventArgs e)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(EmailMessage));

            //Getting all the email files in a specified folder
            string[] SourceFolderFiles = System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["Path"]);

            foreach (string file in SourceFolderFiles)
            {
                if(!Path.GetFileName(file).Equals("count.txt"))
                {
                    //---------For Checking if email had not already been sent--------
                    DateTime ModifyDateTime = File.GetCreationTime(file);
                    Log.writeLog(Path.GetFileName(file) + "  " + ModifyDateTime);

                    DateTime CurrentDate = DateTime.Now;
                    Log.writeLog("Current Time : " + CurrentDate);

                    TimeSpan value = CurrentDate.Subtract(ModifyDateTime);  //Subtracting current time to the file modify time
                    double valueToMs = value.TotalMilliseconds;         //Converting to MilliSeconds

                    Log.writeLog("Value: " + valueToMs);
                    Log.writeLog("Timer Interval : " + CheckMail.Interval);

                    if (valueToMs <= CheckMail.Interval)    //Means a file was changed and email has to be send
                    {
                        //Read a file
                        TextReader reader = new StreamReader(ConfigurationManager.AppSettings["Path"] + "/" + Path.GetFileName(file));
                        object obj = deserializer.Deserialize(reader);
                        EmailMessage XmlData = (EmailMessage)obj;
                        reader.Close();

                        try
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);

                            mail.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                            mail.To.Add(XmlData.To);
                            mail.Subject = XmlData.Subject;
                            mail.Body = XmlData.Message;

                            SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
                            SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SenderEmail"], ConfigurationManager.AppSettings["SenderPassword"]);
                            SmtpServer.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["Enablessl"]);

                            SmtpServer.Send(mail);
                        }

                        catch (Exception ex)
                        {

                        }
                    }
                }
                
               
            }
        
        }

        protected override void OnStop()
        {
            Log.writeLog("Attemp to shut down a service");
            CheckMail.Stop();
            CheckMail = null;
            Log.writeLog("Service shut down by user");
        }
    }

    public class EmailMessage
    {

        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public EmailMessage()
        {

        }
    }
}


