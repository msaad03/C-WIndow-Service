using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace k163808_Q1
{
    class Server
    {
        int Port { get; set; }
        IPAddress Ip { get; set; }
        static int i = 0;

        public Server()
        {
            this.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            this.Ip = IPAddress.Parse(ConfigurationManager.AppSettings["Ip"]);
        }

        void ServerStart()
        {
            TcpListener serverSocket = null;

            try
            {
                serverSocket = new TcpListener(Ip, Port);

                serverSocket.Start();
    
                Console.WriteLine("Server Started at port " +  Port);

            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            
            while (true)
            {
                Socket clientSocket = serverSocket.AcceptSocket();
                Thread thread = new Thread(() => Reverse(clientSocket));
                thread.Start();
                
            }
        }

        void Reverse(Socket clientSocket)
        {
            i++;
            Console.WriteLine("Client " + i + " joined");

            while (true)
            {
                byte[] ReceivedData = new byte[100];

                int byteCount = clientSocket.Receive(ReceivedData);

                string request = Encoding.ASCII.GetString(ReceivedData).Substring(0, byteCount);

                Console.WriteLine("Message sent my client : " + request);

                

                char[] charArray = request.ToCharArray();
                Array.Reverse(charArray);

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] SendData = asen.GetBytes(new string(charArray));

                Console.WriteLine("Message sent by server : " + asen.GetString(SendData));
                clientSocket.Send(SendData);
                
                if (request.Equals("exit"))
                {
                    Console.WriteLine("Client " + i + "disconnect");
                    break;

                }

            }
            clientSocket.Close();

        }
        static void Main(string[] args)
        {
            Server obj = new Server();
            obj.ServerStart();

        }
    }
}
