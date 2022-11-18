using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPeer2Peer
{
    public class TcpClientPeer {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static TcpClient? client;
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;
        public static int myPort;
        public const int MainServerPort = 8888;


        public static string IpAddressEndPoint = String.Empty;
        public static int PortEndPoint = 0;


        public static void Start()
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            var ipLocalEndPoint = new IPEndPoint(IPAddress.Any, 8888);
            client = new TcpClient(ipLocalEndPoint);

            ConnectToMainServer();
            /*
            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), EndPort);

            Console.WriteLine("Starting Peer ...");
            HolePunching();*/
        }

        public static void ConnectToMainServer()
        {
            client.Connect(IPAddress.Parse("20.13.17.73"), MainServerPort);
            if (client.Connected) {
                Console.WriteLine("Connected to Main Server");
            }
            client.Client.Send(Encoding.ASCII.GetBytes("hello i'm client"));
            NetworkStream stream;
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes;


            while (true)
            {
                // Get a client stream for reading and writing. 
                stream = client.GetStream();
                // Read the first batch of the TcpServer response bytes.
                bytes = stream.Read(data, 0, data.Length); //(**This receives the data using the byte method**)
                // Buffer to store the response bytes.
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes); //(**This converts it to string**)
                Console.WriteLine("Received: " + responseData);
                if (responseData.Contains(":")) {
                    break;
                }
                Thread.Sleep(250);

            }

            client.Close();

            IpAddressEndPoint = responseData.Split(":")[0];
            string Ports = responseData.Split(":")[1];
            PortEndPoint = Int32.Parse(Ports.Split("\n")[0]);
            myPort = Int32.Parse(responseData.Split("\n")[1]);
            peerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
            client = new TcpClient(ipLocalEndPoint);
            HolePunching();


            // new Thread(() => 
            // {
            //     Thread.CurrentThread.IsBackground = true; 
            //     while (true)
            //     {
            //         ListenMessage();
            //     }
            // }).Start();

        }

        public static void ListenMessage() {

            NetworkStream stream;
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes;

            while (true)
            {
                // Get a client stream for reading and writing. 
                stream = client.GetStream();
                // Read the first batch of the TcpServer response bytes.
                bytes = stream.Read(data, 0, data.Length); //(**This receives the data using the byte method**)
                // Buffer to store the response bytes.
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes); //(**This converts it to string**)
                Console.WriteLine("Received: " + responseData);
                Thread.Sleep(250);

            }
        }

        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }

        public static void HolePunching()
        {

            if (peerEndPoint == null)
            {
                peerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
            }
            
            Console.WriteLine("Trying to connect to: " + IpAddressEndPoint);
            
            if (!client.Connected) {
                try 
                {
                    client.Connect(peerEndPoint);
                }
                catch (Exception e)
                {
                }
            } else {
                Console.WriteLine("Already connected");
            }

            if (client.Connected)
            {
                Console.WriteLine("Connected");
                while (true)
                {
                    
                }
            } else
            {
                HolePunching();
            }
            //
        }
    }
}