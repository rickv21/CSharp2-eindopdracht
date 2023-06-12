using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Networking
{
    public class TcpConnection
    {
        private const int PORT = 5732;

        private Task listenerTask;
        private TcpListener server;
        private TcpClient client;
        private Socket webSocket;


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
            server = new TcpListener(IPAddress.Any, PORT);
            server.Start();
            Debug.WriteLine("Server started. Waiting for a client to connect...");

            webSocket = server.AcceptSocket();
            Debug.WriteLine("Client connected.");


            Debug.WriteLine(RecieveMessage());
        }
        public void StartClient(string ip)
        {
            Console.WriteLine("Player B is trying to connect");
            try
            {
                client = new TcpClient(ip, PORT);
                Debug.WriteLine("Connected to server.");

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
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                webSocket.Send(data);

                Debug.WriteLine("Message sent: " + msg);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending message: " + ex.Message);
            }

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
            try
            {
                if (server != null)
                {
                    server.Stop();
                }

                if (client != null)
                {
                    client.Close();
                }

                listenerTask.Dispose();
                Debug.WriteLine("Disconnected.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error disconnecting: " + ex.Message);
            }
        }
    }
}
