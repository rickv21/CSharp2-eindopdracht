using System.Diagnostics;

using GameProject.Networking;
using GameProject.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameProject;

public partial class MainPage : ContentPage
{
    private TcpConnection network;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCreateHostClicked(object sender, EventArgs e)
    {
        ToggleNetworkButtons();
        Title.Text = "Wachten op verbinding";
        Info.Text = $"IP: {NetworkingUtils.GetLocalIPAddress()}";
        Info.IsVisible = true;

        this.network = new TcpConnection();

        await network.StartServer();
    }

    private void OnConnectClientClicked(object sender, EventArgs e)
    {
        ToggleNetworkButtons();
        Title.Text = "Voer het IP address in";
        IpInput.IsVisible = true;
    }

    private void IpInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        string userInput = e.NewTextValue;
        if (!String.IsNullOrEmpty(userInput))
        {
            foreach (char c in userInput)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    IpInput.Text = e.OldTextValue;
                    break;
                }
            }
        }

    }

    private async void IpInput_Completed(object sender, EventArgs e)
    {
        string ipInput = ((Entry)sender).Text;

        if (NetworkingUtils.CheckIfInputIsValidIp(ipInput))
        {
            Title.Text = "Verbinding proberen te maken...";
            IpInput.IsEnabled = false;
            try
            {
                network = new TcpConnection();
                await network.StartClient(ipInput);
                Title.Text = "Connected!";
            }
            catch (Exception ex)
            {
                Title.Text = ex.Message;
                IpInput.IsEnabled = true;
            }

            Debug.WriteLine(network.IsConnected());
            if(network.IsConnected() )
            {
            
                    JObject jObject = new JObject();
                    jObject["message"] = "test";
                    network.SendMessage(jObject.ToString(Formatting.None));
            }
        }
    }

    private void ToggleNetworkButtons()
    {
        CreateHostBtn.IsVisible = !CreateHostBtn.IsVisible;
        ConnectClientBtn.IsVisible = !ConnectClientBtn.IsVisible;
    }
}


