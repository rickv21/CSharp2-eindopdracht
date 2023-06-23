using GameProject.ViewModels;

namespace GameProject.Views;

public partial class ConnectFour : ContentPage
{
    private ConnectFourViewModel viewModel;

    public ConnectFour()
    {
        InitializeComponent();
        viewModel = new ConnectFourViewModel(GameGrid);

        BindingContext = viewModel;

        MessagingCenter.Subscribe<ConnectFourViewModel, PopupMessage>(this, "ConnectFourPopup", async (sender, message) =>
        {
            await DisplayAlert(message.Title, message.Message, "OK");
            viewModel.ResetGame();
            await Shell.Current.GoToAsync("//MainPage");
        });
    }


    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        viewModel.ResetGame();
        await Shell.Current.GoToAsync("//MainPage");
    }







}