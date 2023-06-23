using GameProject.ViewModels;

namespace GameProject.Views;

[QueryProperty("UserName", "userName")]
public partial class ConnectFour : ContentPage
{

    private string userName;

    public string UserName
    {
        get => userName;
        set
        {
            userName = value;
            // Use the username as needed in your ConnectFour page
            // For example, set it as the title of the page
            Title = $"Connect Four - {userName}";
        }
    }

    private ConnectFourViewModel viewModel;

    public ConnectFour()
    {
        InitializeComponent();
        viewModel = new ConnectFourViewModel(GameGrid, UserName);

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