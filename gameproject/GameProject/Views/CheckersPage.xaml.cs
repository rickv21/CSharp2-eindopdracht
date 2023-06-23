using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Threading.Channels;
using GameProject.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.Views
{
        public partial class CheckersPage : ContentPage
        {

            private CheckersViewModel viewModel;
            public CheckersPage()
            {
                InitializeComponent();
                viewModel = new CheckersViewModel(CheckersGrid);
                BindingContext = viewModel;

                MessagingCenter.Subscribe<CheckersViewModel, PopupMessage>(this, "CheckersPopup", async (sender, message) =>
                {
                    await DisplayAlert(message.Title, message.Message, "OK");
                    viewModel.ResetCheckersGame();
                    await Shell.Current.GoToAsync("//MainPage");
                });
             }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            viewModel.ResetGame();
            await Shell.Current.GoToAsync("//MainPage");
        }

    }

}