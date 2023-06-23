using Microsoft.Maui.Controls;

namespace GameProject.Views;

public partial class InputPromptPage : ContentPage
{
    public string InputValue { get; private set; }
    public event EventHandler<string> InputAccepted;
    public event EventHandler InputCancelled;

    public InputPromptPage(string defaultValue)
    {
        InitializeComponent();
        inputEntry.Text = defaultValue;
    }

    private void OnOKClicked(object sender, EventArgs e)
    {
        InputValue = inputEntry.Text;
        InputAccepted?.Invoke(this, InputValue);
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        InputCancelled?.Invoke(this, EventArgs.Empty);
        Navigation.PopModalAsync();
    }
}
