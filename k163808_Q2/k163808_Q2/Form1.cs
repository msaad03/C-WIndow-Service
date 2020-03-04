using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Configuration;

namespace k163808_Q2
{
    public partial class Form1 : Form
    {
        public static int Count = 0;
        
        public Form1()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;

            //To check the count number from a file and generate filename as per count
            if(File.Exists(ConfigurationManager.AppSettings["Path"] + "/" + "count.txt"))
            {
                //Retrieve count number From count filr
                Count = Int32.Parse(System.IO.File.ReadAllText(ConfigurationManager.AppSettings["Path"] + "/" + "count.txt"));
            }

            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
            
        private void button1_Click(object sender, EventArgs e)
        {
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;

            EmailMessage email = new EmailMessage();
           
            try
            {
                if(!toTextBox.Text.Equals(""))
                {
                    MailAddress m = new MailAddress(toTextBox.Text);

                    if (!subjectTextBox.Text.Equals("") && !messageTextBox.Text.Equals(""))
                    {
                        email.To = toTextBox.Text;
                        email.Subject = subjectTextBox.Text;
                        email.Message = messageTextBox.Text;

                        email.sendEmail(label4, toTextBox, subjectTextBox, messageTextBox);

                        
                    }

                    else
                    {
                        label6.Visible = true;
                        label6.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        label6.Visible = false;

                    }
                }
                else
                {
                    label6.Visible = true;

                    label6.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    label6.Visible = false;
                }
                
            }
            catch (FormatException)
            {
                label5.Visible = true;

                label5.Refresh();
                System.Threading.Thread.Sleep(1000);
                label5.Visible = false;


            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }

    public class EmailMessage
    {
        private String _To;
        private String _Subject;
        private String _Message;

        public String To
        {
            get
            {
                return _To;
            }
            set
            {
                _To = value;
            }
        }
        public String Subject
        {
            get
            {
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }
        public String Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        public void sendEmail(Label label4, TextBox toTextBox, TextBox subjectTextBox, TextBox messageTextBox)
        {
            Form1.Count++;
            string filename = ConfigurationManager.AppSettings["Path"] + "/" +  "email_" + Form1.Count + ".xml";

            XmlSerializer x = new XmlSerializer(typeof(EmailMessage));
            TextWriter writer = new StreamWriter(filename);
            x.Serialize(writer, this);
            writer.Close();

            File.WriteAllText(ConfigurationManager.AppSettings["Path"] +  "/" + "count.txt", (Form1.Count).ToString());

            toTextBox.Text = "";
            subjectTextBox.Text = "";
            messageTextBox.Text = "";

            label4.Visible = true;
            label4.Refresh();
            System.Threading.Thread.Sleep(1000);
            label4.Visible = false;






        }


}

    


      

}
