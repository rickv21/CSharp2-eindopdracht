using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.ViewModels
{
    public class CheckersViewModel : INotifyPropertyChanged
    {
        private CheckersModel model;
        private Grid checkersGrid;
        private string turnText;
        private CheckersSquareButton selectedSquare;
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

        public CheckersSquareButton SelectedSquare
        {
            get => selectedSquare;
            set
            {
                if (selectedSquare != value)
                {
                    selectedSquare = value;
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
            CheckersSquare[,] squares = new CheckersSquare[8, 8];
            for (var row = 0; row < 8; row++)
            {
                for (var col = 0; col < 8; col++)
                {

                    CheckersSquare square = new CheckersSquare();

                    if ((row + col) % 2 == 0)
                    {
                        square.TileColor = Colors.White;
                    }
                    else
                    {
                        square.TileColor = Colors.Black;
                    }

                    if (row < 3 && square.TileColor.Equals(Colors.Black))
                    {
                        square.Piece = new CheckersPiece(Colors.Red);
                    }
                    else if (row > 4 && square.TileColor.Equals(Colors.Black))
                    {
                        square.Piece = new CheckersPiece(Colors.Blue);
                    }
                    else
                    {
                        square.Piece = new CheckersPiece(Colors.Transparent);
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
                    CheckersSquareButton button = new(square)
                    {
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        CornerRadius = 0,
                        //BorderWidth = 0
                    };
                    //button.Command = new Command(async () => await ClickSquareAsync(button));
                    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        if (this.selectedSquare != null)
                        {
                            this.selectedSquare.ResetSelected();
                        }
                        this.selectedSquare = button;
                        button.ShowPiece();
                    };
                    button.addGestureRecognizer(tapGestureRecognizer);
                    checkersGrid.Children.Add(button);
                    Grid.SetRow( button, row);
                    Grid.SetColumn(button, col);
                }
            }

        }

        internal void ResetCheckersGame()
        {
            TurnText = "Player 1";
            SetupModelValues();
        }

        private void SetupModelValues()
        {
            model.Add("playerTurn", 0);
            model.Add("squareSelected", false);
        }

        public void MovePiece()
        {

        }
    }
}
