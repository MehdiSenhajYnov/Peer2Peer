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
        public const int myPort = 8888;
        public const int EndPort = 8080; 
        public const int MainServerPort = 8888;


        public static string IpAddressEndPoint = String.Empty;
        public static int PortEndPoint = 0;
        public static IPEndPoint? otherPeerEndPoint;


        public static void Start()
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            client = new TcpClient();

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
            NetworkStream stream;
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes;

            //Socket peer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
            
            IpAddressEndPoint = responseData.Split(":")[0];
            PortEndPoint = Int32.Parse(responseData.Split(":")[1]);
            otherPeerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
            client.Client.SendTo(Encoding.ASCII.GetBytes("Hello"), otherPeerEndPoint);

            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                while (true)
                {
                    ListenMessage();
                }
            }).Start();

            string newMessage = String.Empty;
            while (true)
            {
                newMessage = Console.ReadLine();
                if (!String.IsNullOrEmpty(newMessage)) {
                    if (newMessage == "exit") {
                        break;
                    }
                    client.Client.SendTo(Encoding.ASCII.GetBytes(newMessage), otherPeerEndPoint);
                }
            }
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
                peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            }
            
            Console.WriteLine("Trying to connect to: " + _ipAddress);
            
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