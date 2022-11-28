using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPeer2Peer
{
    public class TcpClientPeer
    {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static Socket? client;
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;
        public static int myPort;
        public const int MainServerPort = 8888;


        public static string IpAddressEndPoint = String.Empty;
        public static int PortEndPoint = 0;
        static Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static IPEndPoint MainServerEndPoint = new IPEndPoint(IPAddress.Parse("20.13.17.73"), MainServerPort);

        public static void Start()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            ConnectToMainServer();

        }

        public static void ConnectToMainServer()
        {
            //client.Bind(new IPEndPoint(IPAddress.Any, 8888));
            client.Connect(MainServerEndPoint);
            if (client.Connected)
            {
                Console.WriteLine("Connected to Main Server");
            }
            client.Send(Encoding.ASCII.GetBytes("hello i'm client"));

            RequestLoop();

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
            if (responseData == "TCPNEW")
            {
                tcpClient.Connect(MainServerEndPoint);
                if (tcpClient.Connected)
                {
                    Console.WriteLine("TCPNEW connected");
                    PeerRequestLoop();
                }
            }

            if (responseData.StartsWith("ConnectTo:"))
            {
                string InfoToConnect = responseData.Replace("ConnectTo:", "");
                IpAddressEndPoint = InfoToConnect.Split(":")[0];
                string Ports = InfoToConnect.Split(":")[1];
                PortEndPoint = Int32.Parse(Ports.Split("\n")[0]);
                myPort = Int32.Parse(Ports.Split("\n")[1]);

                peerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
                ipLocalEndPoint = new IPEndPoint(IPAddress.Any, myPort);


                tcpClient.Bind(ipLocalEndPoint);
                Console.WriteLine("Try to connect to : " + peerEndPoint + " with port : " + myPort);
                try
                {
                    tcpClient.Connect(peerEndPoint);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("2nd try");
                    try
                    {
                        tcpClient.Connect(peerEndPoint);

                    }
                    catch (System.Exception)
                    {

                    }
                }
                if (tcpClient.Connected)
                {
                    Console.WriteLine("Connected to other peer");
                    tcpClient.Send(Encoding.ASCII.GetBytes("hello i'm client"));

                }
                else
                {
                    Console.WriteLine("Not Connected to other peer");
                }


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
                received = tcpClient.Receive(buffer, SocketFlags.None);
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

            /*foreach (var bytou in data)
            {
                Console.Write(bytou + " ");
            }*/
            string responseData = Encoding.ASCII.GetString(data);
            Console.WriteLine("received : " + responseData);

            if (responseData.StartsWith("TRYCONNECT:"))
            {
                string InfoToConnect = responseData.Replace("TRYCONNECT:", "");
                IpAddressEndPoint = InfoToConnect.Split(":")[0];
                string Ports = InfoToConnect.Split(":")[1];
                PortEndPoint = Int32.Parse(Ports.Split("\n")[0]);
                myPort = Int32.Parse(Ports.Split("\n")[1]);
                Console.WriteLine("Ip : " + IpAddressEndPoint + " Port : " + PortEndPoint + " MyPort : " + myPort);
                peerEndPoint = new IPEndPoint(IPAddress.Parse(IpAddressEndPoint), PortEndPoint);
                ipLocalEndPoint = new IPEndPoint(IPAddress.Any, myPort);

                tcpClient.Close();
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpClient.Bind(ipLocalEndPoint);

                if (!tcpClient.ConnectAsync(peerEndPoint).Wait(2000))
                {
                    Console.WriteLine("TRYCONNECT teminated");
                    client.Send(Encoding.ASCII.GetBytes("TRYTOCONNECTEND"));
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        StartServer();
                    }).Start();
                }

            }

        }

        public static async void StartServer()
        {
            listeningSocket.Close();
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Console.WriteLine("ipLocalEndPoint = " + ipLocalEndPoint);
            listeningSocket.Bind(ipLocalEndPoint);
            listeningSocket.Listen(5);
            Console.WriteLine("Begin accepting");
            var handler = await listeningSocket.AcceptAsync();
            Console.WriteLine("Client ACCEPTED");
        }

        public static void ConnectToOtherPeer()
        {
            try
            {
                Console.WriteLine("Trying to connect to peer:" + peerEndPoint.ToString());
                tcpClient.Bind(ipLocalEndPoint);
                tcpClient.Connect(peerEndPoint);
                Console.WriteLine("Connected to other peer : " + peerEndPoint.ToString());
                tcpClient.Send(Encoding.ASCII.GetBytes("hello i'm client"));
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error on connect to other peer : " + peerEndPoint.ToString());
                Thread.Sleep(1000);
                ConnectToOtherPeer();
            }

        }

        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }

    }
}