using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace TcpPeer2Peer
{
    class Peer
    {
        IPEndPoint localEndPoint;
        IPEndPoint remoteEndPoint;
        bool useParallelAlgorithm;
        //public Socket? listeningSocket;
        public Socket? punchSocket;
        public Peer(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, bool useParallelAlgorithm)
        {
            this.localEndPoint = localEndPoint;
            this.remoteEndPoint = remoteEndPoint;
            this.useParallelAlgorithm = useParallelAlgorithm;
        }
        public void Run()
        {
            RunImpl();
        }

        void RunImpl()
        {
            if (useParallelAlgorithm)
            {
                Parallel.Invoke(() =>
                {
                    while (true)
                    {
                        PunchHole();
                    }
                },
                () => RunServer());
            }
            else
            {

                PunchHole();

                RunServer();
            }
        }

        void PunchHole()
        {
            Console.WriteLine("Punching hole...");

            punchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            EnableReuseAddress(punchSocket);

            punchSocket.Bind(localEndPoint);
            try
            {
                punchSocket.Connect(remoteEndPoint);
                new Thread(() => 
                {
                    Thread.CurrentThread.IsBackground = true; 
                    while (true)
                    {
                        punchSocket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, punchSocket);
                    }
                }).Start();


            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Punching hole: " + socketException.SocketErrorCode);
            }
            

            Console.WriteLine("Hole punch completed.");
        }

        void ProcessConnection(Socket connectionSocket)
        {
            Console.WriteLine("Socket accepted.");

            using (connectionSocket)
            {
                connectionSocket.Shutdown(SocketShutdown.Both);
            }

            Console.WriteLine("Socket shut down.");
        }

        void EnableReuseAddress(Socket socket)
        {
            if (useParallelAlgorithm)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }




        public const int BufferSize = 2048;
        public readonly byte[] buffer = new byte[BufferSize];
        public Socket current;

        void RunServer()
        {
            using(var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                
                EnableReuseAddress(listeningSocket);

                listeningSocket.Bind(localEndPoint);
                listeningSocket.Listen(0);

                Task.Run(() => 
                {
                    while (true)
                    {
                        var connectionSocket = listeningSocket.Accept();
                        ProcessConnection(connectionSocket);
                    }
                }
                );
            }

            

        }



        public void ReceiveCallback(IAsyncResult AR)
        {
            current = (Socket)AR.AsyncState;
            int received = 0;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected " + current.RemoteEndPoint);
                
                current.Close();
                return;
            }

            byte[] Data = new byte[received];
            Array.Copy(buffer, Data, received);

            Console.Write("Received Data ");
            foreach (var someData in Data)
            {
                Console.Write(someData + " ");
            }
            Console.Write("\n");


            string text = Encoding.ASCII.GetString(Data);
            Console.WriteLine("Text received : " + text);

            if (text.StartsWith("NewUserConnected"))
            {
                
            }

            current.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }


        public void SendString(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                punchSocket.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected!" + ex.Message);
            }
        }


    }
}