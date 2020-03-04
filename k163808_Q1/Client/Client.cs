using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Sockets;
using System.IO;


namespace Client
{
    class Client
    {
        int port { get; set; }
        string Ip { get; set; }

        TcpClient ClientSocket = new TcpClient();
        public Client()
        {
            port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            Ip = ConfigurationManager.AppSettings["Ip"];

        }

        void ClientStart()
        {
            try
            {
                Console.WriteLine("Connecting.....");
                ClientSocket.Connect(Ip,port);                
                Console.WriteLine("Connected");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Connection was unsuccessful");   
            }
        }

        void SendMessage(string message)
        {

            Stream stream = ClientSocket.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] sendData = asen.GetBytes(message);
            stream.Write(sendData, 0, sendData.Length);

            byte[] response = new byte[100];
            int byteCount = stream.Read(response, 0, 100);

            string ReversedString = asen.GetString(response).Substring(0, byteCount);

            if (!message.Equals("exit"))
            Console.WriteLine("Message received From Server : " + ReversedString);

        }
        public void ClientStop()
        {
            ClientSocket.Close();
        }

        static void Main(string[] args)
        {
            string message;
            Client obj = new Client();
            obj.ClientStart();

            while(true)
            {
                Console.WriteLine("Type message to Send : ");
                Console.WriteLine("Type exit to disconnect : ");

                message = Console.ReadLine();

                if (message.Length > 0)
                {
                    Console.WriteLine("Message sent by client : " + message);
                    obj.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("Nothing typed");
                }
                if (message.Equals("exit"))
                {                    
                    break;
                }
            }
            obj.ClientStop();


        }
    }


}
