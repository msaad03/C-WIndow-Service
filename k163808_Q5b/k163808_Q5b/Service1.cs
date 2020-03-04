using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using System.Configuration;
using System.Net.Mail;

namespace k163808_Q5b
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            timer = new Timer();
            timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"]);    //Setting initial Timer to 15 minutes
            timer.Elapsed += new ElapsedEventHandler(this.checkingChanges);
            timer.Enabled = true;


        }

        private void checkingChanges(object sender, ElapsedEventArgs e)
        {
            string[] SourceFolderFiles = System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["SourceFolder"]);

            FileInformation info = new FileInformation();
            
            foreach (string file in SourceFolderFiles)
            {
                info.FileName = Path.GetFileName(file);
                FileInfo FileInfo = new FileInfo(ConfigurationManager.AppSettings["SourceFolder"] + "/" + info.FileName);
                info.FileSize = FileInfo.Length;

                DateTime ModifyDate = File.GetLastWriteTime(file);
                
                DateTime CurrentDate = DateTime.Now;

                TimeSpan value = CurrentDate.Subtract(ModifyDate);
                double valueToMs = value.TotalMilliseconds;

                
                if (valueToMs <= timer.Interval) //if this condition met , there is a file change after the last checking 
                {                                   //hence email is sent
                    
                    try
                    {
                        
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);
                        
                        mail.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                        mail.To.Add(ConfigurationManager.AppSettings["ReceiverEmail"]);
                        mail.Subject = "About Changes";
                        mail.Body = "Changed file name : " + info.FileName + " & " + "file size : " + info.FileSize+ "Bytes";

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

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
        }
    }

    public class FileInformation
    {

        public string FileName { get; set; }
        public long FileSize { get; set; }

        public FileInformation()
        {

        }
    }
}
