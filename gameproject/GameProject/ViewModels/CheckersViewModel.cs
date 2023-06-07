using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject.Models;

namespace GameProject.ViewModels
{
    public class CheckersViewModel : INotifyPropertyChanged
    {
        private CheckersModel model;
        private Grid checkersGrid;
        private string turnText;
        public event PropertyChangedEventHandler PropertyChanged;

        public string TurnText
        {
            get { return turnText; }
            set
            {
                if (turnText != value)
                {
                    turnText = value;
                    OnPropertyChanged(nameof(TurnText));
                }
            }
        }

        public Grid MemoryGrid
        {
            get { return checkersGrid; }
            set
            {
                if (checkersGrid!= value)
                {
                    checkersGrid = value;
                    OnPropertyChanged(nameof(CheckersGrid));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CheckersViewModel(Grid checkersGrid)
        {
            model = new CheckersModel();
            this.checkersGrid = checkersGrid;
            StartGame();
        }

        internal void ResetCheckersGame()
        {
            TurnText = "Player 1";
            SetupModelValues();
        }

        private void SetupModelValues() { }

        private void GenerateBoard()
        {

        }
    }
}
