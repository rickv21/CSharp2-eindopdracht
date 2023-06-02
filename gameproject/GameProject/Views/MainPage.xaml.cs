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

        private void OnPage2ButtonClicked(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new Page2());
        }
    }
}
