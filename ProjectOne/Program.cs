using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static TcpClient? client;
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;

        public static void Main(string[] args)
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            ipLocalEndPoint = new IPEndPoint(IPAddress.Any, 7777);

            client = new TcpClient(ipLocalEndPoint);
            /*
            listener = new TcpListener(ipLocalEndPoint);

            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnect, null);
            */
            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            
            client.Client.Bind(ipLocalEndPoint);
            client.Client.Listen(5);
            client.Client.BeginAccept(OnClientConnect, null);
            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }


        public static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = client.Client.Receive(buffer, SocketFlags.None);
            if (received == 0)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);

            Console.WriteLine("Received From Server : " + Encoding.ASCII.GetString(data));

        }

        public static void HolePunching()
        {

            if (peerEndPoint == null)
            {
                peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            }
            
            Console.WriteLine("Trying to connect to: " + _ipAddress);
            
            try 
            {
                client.Connect(peerEndPoint);
                Console.WriteLine("Connected to peer");

                

            }
            catch (Exception ex)
            {
                
            }

            if (client.Connected)
            {
                
                Console.WriteLine("Connected");
                while (true)
                {
                    
                }
            } else
            {
                System.Threading.Thread.Sleep(500);
                HolePunching();
            }
        }
    }
}