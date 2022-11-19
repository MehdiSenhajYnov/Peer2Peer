using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPeer2PeerServer
{

    public class Server
    {
        public const int BufferSize = 2048;
        public const int Port = 8888;
        public readonly List<Socket> clientSockets = new List<Socket>();
        public readonly byte[] buffer = new byte[BufferSize];
        //public readonly List<Player> players = new List<Player>();
        public Socket serverSocket;
        public Socket current;
        public byte[] actionsCode = new byte[1];
        //public int dataSent;

        public string endpointToGive = String.Empty;
        public void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Listen();
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
            Console.WriteLine("Listening on port: " + Port);
        }

        public void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        string ipWaited = string.Empty;

        public void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
                Console.WriteLine("Client connected: " + socket.RemoteEndPoint);
                if (!String.IsNullOrEmpty(ipWaited)) 
                {
                    Console.WriteLine("IP waited: " + ipWaited + " IP connected: " + socket.RemoteEndPoint.ToString().Split(":")[0]);
                    if (socket.RemoteEndPoint.ToString().Split(":")[0] == ipWaited)
                    {
                        Console.WriteLine("Waited Client Connected");
                        endpointToGive = socket.RemoteEndPoint.ToString();
                        byte[] messageByte = Encoding.ASCII.GetBytes("TRYCONNECT:" + clientSockets[0].RemoteEndPoint.ToString());
                        socket.Send(messageByte);
                        Console.WriteLine("Sending TRYCONNECT message to client");
                        socket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
                        serverSocket.BeginAccept(AcceptCallback, null);
                        return;
                    }
                }
                clientSockets.Add(socket);
                if (clientSockets.Count() == 2){
                    // tell to the client to open a new TCP connection
                    byte[] messageByte = Encoding.ASCII.GetBytes("TCPNEW");
                    clientSockets[1].SendTo(messageByte, 0, messageByte.Length, SocketFlags.None, clientSockets[1].LocalEndPoint);
                    ipWaited = string.Format("{0}", socket.RemoteEndPoint.ToString()).Split(':')[0];
                    Console.WriteLine("Waiting for client : " + ipWaited + "to connect to the new TCP connection" );
                    /*
                    byte[] IPPlyrOne = Encoding.ASCII.GetBytes(clientSockets[0].RemoteEndPoint.ToString() + "\n" + clientSockets[1].RemoteEndPoint.ToString().Split(':')[1]);
                    byte[] IPPlyrTwo = Encoding.ASCII.GetBytes(clientSockets[1].RemoteEndPoint.ToString() + "\n" + clientSockets[0].RemoteEndPoint.ToString().Split(':')[1]);
                    clientSockets[0].SendTo(IPPlyrTwo, 0, IPPlyrTwo.Length, SocketFlags.None, clientSockets[0].LocalEndPoint);
                    clientSockets[1].SendTo(IPPlyrOne, 0, IPPlyrOne.Length, SocketFlags.None, clientSockets[1].LocalEndPoint);
                    Console.WriteLine(Encoding.ASCII.GetString(IPPlyrOne) + " " + Encoding.ASCII.GetString(IPPlyrTwo));*/
                }
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            socket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
            serverSocket.BeginAccept(AcceptCallback, null);
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
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                clientSockets.Remove(current);
                return;
            }
            if (received <= 2)
            {
                return;
            }
            byte[] Data = new byte[received];
            Array.Copy(buffer, Data, received);


            string text = Encoding.ASCII.GetString(Data);
            Console.Write("Received Data : " + text + " from " + current.RemoteEndPoint);

            Console.Write("\n");

            if (text.StartsWith("TRYTOCONNECTEND"))
            {
                var DataToSend = Encoding.ASCII.GetBytes("ConnectTo:" + endpointToGive);
                clientSockets[0].SendTo(DataToSend, 0, DataToSend.Length, SocketFlags.None, clientSockets[0].LocalEndPoint);
                Console.WriteLine("Sending ConnectTo message to client");
            }

            current.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }

        public void SendByte(byte byby) {
            actionsCode[0] = byby;
            current.Send(actionsCode);
            Console.WriteLine($"SENDED : {byby}   " + current.RemoteEndPoint);
        }

        public void SendByte(byte[] byby) {
            current.Send(byby);
            Console.WriteLine($"SENDED : {byby}   " + current.RemoteEndPoint);
        }

        public void SendString(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                current.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected!" + ex.Message);
            }
        }
    }
    
}
