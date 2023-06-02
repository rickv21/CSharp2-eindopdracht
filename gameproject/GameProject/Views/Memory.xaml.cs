using GameProject.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;


namespace GameProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Memory : ContentPage
    {
        private MemoryViewModel viewModel;
        
        public Memory()
        {
            InitializeComponent();
            viewModel = new MemoryViewModel(MemoryGrid);
           // BackButton.Command = new Command(() => );
            BindingContext = viewModel;

            MessagingCenter.Subscribe<MemoryViewModel, PopupMessage>(this, "DisplayPopup", async (sender, message) =>
            {
                await DisplayAlert(message.Title, message.Message, "OK");
                await Shell.Current.GoToAsync("//MainPage");
            });
;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Reset or clean up any necessary data
            viewModel.ResetMemoryGame();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

    }

}
