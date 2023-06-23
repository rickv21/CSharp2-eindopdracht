using Microsoft.Maui.Controls;

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

        private void OnConnectFourButtonClicked(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//ConnectFour");
        }

        private void OnChangeUsernameButtonClicked(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//ChangeUsername");
        }
    }
}
