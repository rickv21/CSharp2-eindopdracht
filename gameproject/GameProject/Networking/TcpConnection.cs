using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private bool isHost;
        private Action<JObject> callback;

        public TcpConnection() { }

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

        public async Task StartServer()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, PORT);
                server.Start();
                Debug.WriteLine("Server started. Waiting for a client to connect...");

                client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                stream = client.GetStream();
                receiveBuffer = new byte[1024];

                await Task.Run(() => {

                    RecieveMessage();
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        public async Task StartClient(string ip)
        {
            Debug.WriteLine("Player B is trying to connect");
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ip, PORT);
                Debug.WriteLine("Connected to server.");

                stream = client.GetStream();
                receiveBuffer = new byte[1024];

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
                byte[] data = Encoding.ASCII.GetBytes(msg);
                stream.Write(data, 0, data.Length);

                Debug.WriteLine("Message sent: " + msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending message: " + ex.Message);
            }

        }

        public bool IsConnected()
        {
            return client != null && client.Connected;
        }

        public void SetCallback(Action<JObject> callback)
        {
            this.callback = callback;
        }


        public string RecieveMessage()
        {
            StringBuilder receivedData = new StringBuilder();

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
                        receivedData.Append(receivedMessage);
                        var jsonObject = JObject.Parse(receivedMessage);
                        callback(jsonObject);
                        var price = (JObject)jsonObject["message"];
                        Debug.WriteLine(price.ToString());
                        // Process or display the received data here as needed
                    }
                    else
                    {
                        Console.WriteLine("Connection closed.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving data: " + ex.Message);
            }

            return receivedData.ToString();
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
