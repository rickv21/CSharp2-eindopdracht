﻿using GameProject.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Diagnostics;

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

            MessagingCenter.Subscribe<MemoryViewModel, PopupMessage>(this, "MemoryPopup", async (sender, message) =>
            {
                await DisplayAlert(message.Title, message.Message, "OK");
                viewModel.ResetMemoryGame();
                await Shell.Current.GoToAsync("//MainPage");
            });
;
        }


        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            viewModel.ResetMemoryGame();
            await Shell.Current.GoToAsync("//MainPage");
        }

    }

}
