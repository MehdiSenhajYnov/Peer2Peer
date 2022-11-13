using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static string _ipAddress = String.Empty;
        public static TcpClient client = new TcpClient();

        public static void Main(string[] args)
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            Console.WriteLine("Starting Peer ...");
            
        }

        public static async void HolePunching()
        {
            Console.WriteLine("Trying to connect to: " + _ipAddress);
            
            await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(_ipAddress), 7777));
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
        }
    }
}