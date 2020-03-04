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

namespace k163808_Q5a
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["InitialTimeInterval"]);    //Setting initial Timer to 2 minutes
            timer.Elapsed += new ElapsedEventHandler(this.checkingChanges);
            timer.Enabled = true;
           

        }

        private void checkingChanges(object sender, ElapsedEventArgs e)
        {
            bool flag = false;
            string[] SourceFolderFiles = System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["SourceFolder"]);
            List<string> list = new List<string>();
            string[] DestinationFolderFiles = System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["DestinationFolder"]);

            foreach(string file in DestinationFolderFiles)
            {
                list.Add(Path.GetFileName(file));
            }
            foreach (string file in SourceFolderFiles)
            {
                
                DateTime ModifyDate = File.GetLastWriteTime(file);
                
                DateTime CurrentDate = DateTime.Now;
               
                TimeSpan value = CurrentDate.Subtract(ModifyDate);  //Subtracting current time to the file modify time
                double valueToMs = value.TotalMilliseconds;
               
                if (valueToMs <= timer.Interval)
                {
                    flag = true;                             //Flag = true means that file is updated or added to a folder 
                    File.Copy(file, ConfigurationManager.AppSettings["DestinationFolder"] +"/" + Path.GetFileName(file), true);
                }

                //If name of a file is changed then move it to destination folder
                else if (!list.Contains(Path.GetFileName(file)))
                {
                    flag = true;
                    File.Copy(file, ConfigurationManager.AppSettings["DestinationFolder"] + "/" + Path.GetFileName(file));
                }

            }

            if(flag == false)       //flag = false means that there is no change in a folder for a particular checking
            {
                if(timer.Interval < Convert.ToInt64(ConfigurationManager.AppSettings["DelayTime"])) //Checking 1 hour delay
                {
                    timer.Interval = timer.Interval + 120000;      //Adding 2 minutes if changed not found
                }     
            }
            
            timer.Start();
            
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
        }
    }
}
