using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;

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

        public Grid CheckersGrid
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

        private void StartGame()
        {
            Debug.WriteLine("Starting game!!");
            TurnText = "Turn: 1";
            SetupModelValues();
            GenerateBoard();
        }

        private void GenerateBoard()
        {
            CheckersSquare[,] squares = new CheckersSquare[6, 6];
            for (var row = 1; row < 9; row++)
            {
                for (var col = 1; col < 9; col++)
                {
                    Debug.WriteLine("Generated square: " + row + "-" + col);

                    CheckersSquare square = new CheckersSquare();


                    if ((row + col) % 2 == 0)
                    {
                        square.TileColor = Colors.White;
                    }
                    else
                    {
                        square.TileColor = Colors.Black;
                    }

                    if (row < 3 && square.PieceColor.Equals(Colors.Black))
                    {
                        square.PieceColor = Colors.Red;
                    }
                    else if (row > 4 && square.PieceColor.Equals(Colors.Black))
                    {
                        square.PieceColor = Colors.Blue;
                    }
                    squares[row, col] = square;
                }
            }
            model.Add("squares", squares);
            UpdateScreen(squares);
        }

        public void UpdateScreen(CheckersSquare[,] squares)
        {
            // Clear existing buttons from the grid
            checkersGrid.Children.Clear();
            checkersGrid.Padding = new Thickness(50, 50, 50, 50);

            for (int row = 0; row < squares.GetLength(0); row++)
            {
                for (int col = 0; col < squares.GetLength(1); col++)
                {
                    CheckersSquare square = squares[row, col];
                    Debug.WriteLine("Loading card button: " + row + "-" + col);

                    // Add square to the grid
                    checkersGrid.Children.Add(square);
                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, col);
                }
            }

        }

        internal void ResetCheckersGame()
        {
            TurnText = "Player 1";
            SetupModelValues();
        }

        private void SetupModelValues() { }
    }
}
