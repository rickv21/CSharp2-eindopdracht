using System.Diagnostics;

using GameProject.Networking;
using GameProject.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameProject.Views;

[QueryProperty("Entry", "entry")]
public partial class ConnectionPage : ContentPage
{

    private string entry;

    public string Entry
    {
        set
        {
            entry = Uri.UnescapeDataString(value);
        }
    }

    private TcpConnection network;

    public ConnectionPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine(entry);
    }

    private async void OnCreateHostClicked(object sender, EventArgs e)
    {
        ToggleNetworkButtons();
        Title.Text = "Wachten op verbinding";
        Info.Text = $"IP: {NetworkingUtils.GetLocalIPAddress()}";
        Info.IsVisible = true;

        this.network = new TcpConnection();

        await network.StartServer();
        Debug.WriteLine("host!!!!!!!!!!!!!!");
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
                string networkJson = JsonConvert.SerializeObject(network);
                if (entry.Equals("connectFour"))
                {
                    await Shell.Current.GoToAsync($"//ConnectFour?network={Uri.EscapeDataString(networkJson)}");
                }
            }
        }
    }

    private void ToggleNetworkButtons()
    {
        CreateHostBtn.IsVisible = !CreateHostBtn.IsVisible;
        ConnectClientBtn.IsVisible = !ConnectClientBtn.IsVisible;
    }
}


