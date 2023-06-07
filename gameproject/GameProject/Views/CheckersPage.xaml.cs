using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Threading.Channels;
using GameProject.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject
    {
        public partial class CheckersPage : ContentPage
        {

            private CheckersViewModel viewModel;
            public CheckersPage()
            {
                InitializeComponent();
                viewModel = new CheckersViewModel(CheckersGrid);
                BindingContext = viewModel;
                //AddCheckersPieces();
            }
        }

}