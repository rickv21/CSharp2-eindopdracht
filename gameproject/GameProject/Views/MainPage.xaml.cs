using Microsoft.Maui.Controls;
using System.Xml.Linq;

namespace GameProject.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnMemoryButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Memory");
        }

        private void OnCheckersButtonClicked(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//Checkers");

        }

        private async void OnConnectFourButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ConnectionPage?entry=connectFour");
        }

        private void OnChangeUsernameButtonClicked(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//ChangeUsername");
        }
    }
}
