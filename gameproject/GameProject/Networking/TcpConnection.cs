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
    internal class TcpConnection
    {
        private const int PORT = 5732;

        private char PlayerChar;
        private char OpponentChar;
        private Socket sock;
        private TcpListener server = null;
        private TcpClient client;
        private BackgroundWorker MessageReceiver = new BackgroundWorker();

        public TcpConnection(bool isHost, string ip = null)
        {

            if (isHost)
            {
                Debug.WriteLine("Host has been created!");

            }
            else
            {
                try
                {
                    Console.WriteLine("Player B has connected!");
                    client = new TcpClient(ip, PORT);
                    sock = client.Client;
                    MessageReceiver.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
