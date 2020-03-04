using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace k163808_Q4
{
    public partial class Service1 : ServiceBase
    {
        List<NewsItem> values { get; set; }
        string Path { get; set; }
        Timer timer = new Timer();     
        public Service1()
        {
            InitializeComponent();
            values = new List<NewsItem>();
        }

        protected override void OnStart(string[] args)
        {
            this.timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"]);
            this.timer.Elapsed += new ElapsedEventHandler(this.ReadRss);
            this.timer.Enabled = true;

        }

        private void ReadRss(object sender, ElapsedEventArgs e)
        {

            values = ReadFromUrl(ConfigurationManager.AppSettings["TheNews"]);
            values.AddRange(ReadFromUrl(ConfigurationManager.AppSettings["Samaa"]));

            this.Path = ConfigurationManager.AppSettings["Path"];



            List<NewsItem> AfterSorting = values.OrderByDescending(o => o.PublishedDate).ToList();

            XmlSerializer x = new XmlSerializer(typeof(NewsItem));

            using (FileStream f = new FileStream(this.Path, FileMode.Create))
            {
                foreach (NewsItem newsitem in AfterSorting)
                {
                    x.Serialize(f, newsitem);

                }
            }

        }
        
        public static List<NewsItem> ReadFromUrl(string url)
        {
            string Channel="";
            
            var webClient = new WebClient();

            string result = webClient.DownloadString(url);

            XDocument document = XDocument.Parse(result);

            foreach (var desc in document.Descendants("channel"))
            {
                Channel = desc.Element("title").Value;
            }

            return (from x in document.Descendants("item")
                    select new NewsItem()
                    {
                        Title = x.Element("title").Value,
                        Description = x.Element("description").Value,
                        PublishedDate = DateTime.Parse(x.Element("pubDate").Value),
                        NewsChannel = Channel,
                    }).ToList();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
        }
    }

    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string NewsChannel { get; set; }
    }
}
