using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPeer2Peer
{

    public class Server
    {
        public const int BufferSize = 2048;
        public const int Port = 7777;
        public readonly List<Socket> clientSockets = new List<Socket>();
        public readonly byte[] buffer = new byte[BufferSize];
        //public readonly List<Player> players = new List<Player>();
        public Socket serverSocket;
        public Socket current;
        public byte[] actionsCode = new byte[1];
        //public int dataSent;


        public void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Listen(5);
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

        public void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
                if (clientSockets.Count() >= 2){
                    socket.Close();
                    return;
                }
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected: " + socket.RemoteEndPoint);
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

            byte[] Data = new byte[received];
            Array.Copy(buffer, Data, received);

            Console.Write("Received Data " + Data[0] + " from " + current.RemoteEndPoint);

            Console.Write("\n");

            string text = Encoding.ASCII.GetString(Data);

            if (text.StartsWith("NewUserConnected"))
            {
                string newuser = text.Replace("newuser:", string.Empty);
                int nb = 1;
                SendByte(new byte[]{(byte)nb,3,4});

                //SendString("WHAT IS YOUR NAME PLAYER " + (nb == 1 ? "ONE ?" : "TWO ?"));
                Console.WriteLine($"New Client has joined the game ID : {nb}");
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
                // current.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
                //dataSent++;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected!" + ex.Message);
            }
            //Console.WriteLine(dataSent);
        }
    }
    
    public enum ActionCodes {
        ChoosedWarrior = 1,
        ChoosedCleric = 2,
        ChoosedPaladin = 3,
        PlyrMove = 4,
    } 
}
