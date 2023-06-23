using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.ViewModels
{
    public class ConnectFourViewModel : MultiplayerGame
    {
        public ConnectFourViewModel() {
            network.SetCallback(OnNetworkRecieve);
        }


        private void OnNetworkRecieve(JObject jObject)
        {
            Debug.WriteLine(jObject["message"]);
        }
    }
}
