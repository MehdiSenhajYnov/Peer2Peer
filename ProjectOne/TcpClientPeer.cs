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
        public static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;
        public static int myPort;
        public const int MainServerPort = 8888;


        public static string IpAddressEndPoint = String.Empty;
        public static int PortEndPoint = 0;
        static Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket PunchHoleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static IPEndPoint MainServerEndPoint = new IPEndPoint(IPAddress.Parse("20.13.17.73"), MainServerPort);

        public static void Start()
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            PunchHoleSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            ConnectToMainServer();
            /*
            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), EndPort);

            Console.WriteLine("Starting Peer ...");
            HolePunching();*/
        }

        public static void ConnectToMainServer()
        {
            //client.Bind(new IPEndPoint(IPAddress.Any, 8888));
            client.Connect(MainServerEndPoint);
            if (client.Connected) {
                Console.WriteLine("Connected to Main Server");
            }
            client.Send(Encoding.ASCII.GetBytes("hello i'm client"));


            RequestLoop();

            

            /*client.Bind(new IPEndPoint(IPAddress.Any, myPort));
            client.Listen();
            HolePunching();*/


            // new Thread(() => 
            // {
            //     Thread.CurrentThread.IsBackground = true; 
            //     while (true)
            //     {
            //         ListenMessage();
            //     }
            // }).Start();

        }

        public static void RequestLoop()
        {
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                while (client != null && client.Connected)
                {
                    ReceiveResponse();
                }
            }).Start();


            
        }


        public static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received;
            try
            {
                received = client.Receive(buffer, SocketFlags.None);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error On receive Skipped");
                return;
            }
            if (received == 0)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);

            /*foreach (var bytou in data)
            {
                Console.Write(bytou + " ");
            }*/
            string responseData = Encoding.ASCII.GetString(data);
            Console.WriteLine("received : " + responseData);


            if (responseData.StartsWith("ConnectTo:"))
            {
                string InfoToConnect = responseData.Replace("ConnectTo:", "");
                IpAddressEndPoint = InfoToConnect.Split(":")[0];
                string Ports = InfoToConnect.Split(":")[1];
                PortEndPoint = Int32.Parse(Ports.Split("\n")[0]);
                myPort = Int32.Parse(Ports.Split("\n")[1]);

                peerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
                ipLocalEndPoint = new IPEndPoint(IPAddress.Any, myPort);


                
                Parallel.Invoke(() => {
                    BeginServer();
                    }, () => {
                        ConnectToOtherPeer();
                    });
                
            }

        }

        public static async void BeginServer()
        {
            listeningSocket.Bind(ipLocalEndPoint);
            listeningSocket.Listen();
            var handler = await listeningSocket.AcceptAsync();
            Console.WriteLine("Server Started on " + ipLocalEndPoint.ToString());
        }

        public static void ConnectToOtherPeer()
        {
            try
            {
                Console.WriteLine("Trying to connect to peer:" + peerEndPoint.ToString());
                PunchHoleSocket.Bind(ipLocalEndPoint);
                PunchHoleSocket.Connect(peerEndPoint);
                Console.WriteLine("Connected to other peer : " + peerEndPoint.ToString());
                PunchHoleSocket.Send(Encoding.ASCII.GetBytes("hello i'm client"));
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error on connect to other peer : " + peerEndPoint.ToString());
                Thread.Sleep(1000);
                ConnectToOtherPeer();
            }
            
        }





        public static void PeerRequestLoop()
        {
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                while (true)
                {
                    PeerReceiveResponse();
                }
            }).Start();


            
        }


        public static void PeerReceiveResponse()
        {
            var buffer = new byte[2048];
            int received;
            try
            {
                received = listeningSocket.Receive(buffer, SocketFlags.None);
            }
            catch (SocketException e)
            {
                return;
            }
            if (received == 0)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);

            string responseData = Encoding.ASCII.GetString(data);
            Console.WriteLine("received : " + responseData);

        }


        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }

    }
}