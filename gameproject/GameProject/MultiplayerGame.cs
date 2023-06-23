using GameProject.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject 
{
    public class MultiplayerGame : ContentPage
    {
        protected TcpConnection network;

        public string Network
        {
            set
            {
                string networkJson = Uri.UnescapeDataString(value);
                network = JsonConvert.DeserializeObject<TcpConnection>(networkJson);
            }
        }

        public MultiplayerGame()
        {
        }

    }
}
