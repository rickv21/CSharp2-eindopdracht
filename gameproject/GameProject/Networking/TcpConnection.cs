using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Networking
{
    internal class TcpConnection: Exception
    {
        private const int PORT = 5732;
        private Task listenerTask = null;

        private Socket webSocket;
        private TcpListener server = null;
        private TcpClient client;

        public TcpConnection(bool isHost, string ip = null)
        {
            if (isHost)
            {
                listenerTask = new Task(() => StartServer());
            }
            else
            {
                listenerTask = new Task(() => StartClient(ip));
            }

            listenerTask.Start();
        }

        public void StartServer()
        {
            Debug.WriteLine("Host has been created!");
            server = new TcpListener(System.Net.IPAddress.Any, PORT);
            server.Start();
            webSocket = server.AcceptSocket();

        }
        public void StartClient(string ip)
        {
            Console.WriteLine("Player B is trying to connect");
            try
            {
                client = new TcpClient(ip, PORT);
                webSocket = client.Client;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        public void SendMessage(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            webSocket.Send(data);
        }

        public string RecieveMessage()
        {
            // TODO: Change the amount of bytes used for the recieve message
            byte[] bytes = new byte[1024];
            webSocket.Receive(bytes);

            return Encoding.UTF8.GetString(bytes);
        }

        public void StopConnection()
        {
            if (webSocket != null)
            {
                webSocket.Close();
            }
        }
    }
}
