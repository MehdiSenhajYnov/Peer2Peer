using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace TcpPeer2PeerServer
{
    class HolePunching
    {
        
        IPEndPoint localEndPoint;
        IPEndPoint remoteEndPoint;
        bool useParallelAlgorithm;
        public HolePunching(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, bool useParallelAlgorithm)
        {
            this.localEndPoint = new IPEndPoint(IPAddress.Parse("LOCAL_IP"), 1234);
            this.remoteEndPoint = remoteEndPoint;
            this.useParallelAlgorithm = useParallelAlgorithm;
        }

        public void Run()
        {
            RunImpl();
        }

        void RunImpl()
        {
            Console.WriteLine("localEndPoint: " + localEndPoint + " remoteEndPoint: " + remoteEndPoint);

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

            using (var punchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                EnableReuseAddress(punchSocket);

                punchSocket.Bind(localEndPoint);
                try
                {
                    punchSocket.Connect(remoteEndPoint);
                    Debug.Assert(false);
                }
                catch (SocketException socketException)
                {
                    Console.WriteLine("Punching hole: " + socketException.SocketErrorCode);
                    Debug.Assert(socketException.SocketErrorCode == SocketError.TimedOut || socketException.SocketErrorCode == SocketError.ConnectionRefused);
                }
            }

            Console.WriteLine("Hole punch completed.");
        }

        void RunServer()
        {
            using (var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                EnableReuseAddress(listeningSocket);

                listeningSocket.Bind(localEndPoint);
                listeningSocket.Listen(0);

                while (true)
                {
                    var connectionSocket = listeningSocket.Accept();
                    Task.Run(() => ProcessConnection(connectionSocket));
                }
            }
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
    }
}