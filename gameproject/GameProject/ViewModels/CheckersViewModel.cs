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
        private CheckersModel _model;
        private Grid _checkersGrid;
        private string _turnText;
        private CheckersSquareButton _selectedSquare;
        private CheckersPlayer _player1;
        private CheckersPlayer _player2;
        private bool _isPieceSelected;
        private CheckersSquareButton[,] _squareButtons;
        public event PropertyChangedEventHandler PropertyChanged;

        public string TurnText
        {
            get { return _turnText; }
            set
            {
                if (_turnText != value)
                {
                    _turnText = value;
                    OnPropertyChanged(nameof(TurnText));
                }
            }
        }

        public Grid CheckersGrid
        {
            get { return _checkersGrid; }
            set
            {
                if (_checkersGrid != value)
                {
                    _checkersGrid = value;
                    OnPropertyChanged(nameof(CheckersGrid));
                }
            }
        }

        public CheckersSquareButton SelectedSquare
        {
            get => _selectedSquare;
            set
            {
                if (_selectedSquare != value)
                {
                    _selectedSquare = value;
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
            this._model = new CheckersModel();
            this._checkersGrid = checkersGrid;
            this._squareButtons = new CheckersSquareButton[8, 8];
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
                        square.TileColor = Color.FromRgb(216, 162, 90);
                        square.IsBlack = false;
                    }
                    else
                    {
                        square.TileColor = Color.FromRgb(161, 95, 23);
                        square.IsBlack = true;
                    }

                    if (row < 3 && square.IsBlack)
                    {
                        square.Piece = new CheckersPiece(Colors.Red);
                    }
                    else if (row > 4 && square.IsBlack)
                    {
                        square.Piece = new CheckersPiece(Color.FromRgb(40, 40, 40));
                    }
                    else
                    {
                        square.Piece = new CheckersPiece(Colors.Transparent);
                    }
                    squares[row, col] = square;
                }
            }
            this._model.Add("squares", squares);
            UpdateScreen(squares);
        }

        public void UpdateScreen(CheckersSquare[,] squares)
        {
            // Clear existing buttons from the grid
            this._checkersGrid.Children.Clear();
            this._checkersGrid.Padding = new Thickness(50, 50, 50, 50);

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
                    };

                    //TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                    //tapGestureRecognizer.Tapped += async (s, e) =>
                    //{
                        //if(button.HasPiece())
                        //if (this._selectedSquare != null)
                        //{
                        //    this._selectedSquare.ResetSelected();
                        //}

                        //if (button.HasPiece())
                        //{
                        //    this._selectedSquare = button;
                        //    button.SelectPiece();
                        //}

                        //if (button.HasPiece() && !_isPieceSelected)
                        //{
                        //    _isPieceSelected = true;
                        //    this._selectedSquare = button;
                        //    button.SelectPiece();
                        //}
                        //else if (!button.HasPiece() && _isPieceSelected)
                        //{
                        //    _isPieceSelected = false;
                        //    MovePiece(button);
                        //}
                    //};
                    //button.AddGestureRecognizer(tapGestureRecognizer);
                    _squareButtons[row, col] = button;
                    _checkersGrid.Children.Add(button);
                    Grid.SetRow( button, row);
                    Grid.SetColumn(button, col);
                }
            }
            UpdateSquareGestureRecognizers();

        }

        internal void ResetCheckersGame()
        {
            TurnText = "Player 1";
            SetupModelValues();
        }

        private void SetupModelValues()
        {
            this._model.Add("playerTurn", 0);
            this._model.Add("squareSelected", false);
        }

        public bool IsValidMove(int sourceRow, int sourceCol, int targetRow, int targetCol)
        {
            if (_squareButtons[targetRow, targetCol].HasPiece())
            {
                return false;
            }

            int rowDelta = Math.Abs(targetRow - sourceRow);
            int columnDelta = Math.Abs(targetCol - sourceCol);
            if (rowDelta != columnDelta)
            {
                return false;
            }

            int maxDistance = 2; // Maximum allowed move distance
            if (rowDelta > maxDistance || columnDelta > maxDistance)
            {
                return false;
            }

            return true;
        }

        public bool MovePiece(CheckersSquareButton startSquare, CheckersSquareButton endSquare)
        {
            if (IsValidMove(startSquare.GetRow(), startSquare.GetCol(), endSquare.GetRow(), endSquare.GetCol()))
            {
                UpdateSquareGestureRecognizers();
                return true;
            }
            return false;
        }

        private void UpdateSquareGestureRecognizers()
        {
            foreach (var squareButton in _squareButtons)
            {
                squareButton.GestureRecognizers.Clear();

                if (squareButton.HasPiece())
                {
                    TapGestureRecognizer pieceTapGestureRecognizer = new TapGestureRecognizer();
                    pieceTapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        if (this._selectedSquare != null)
                        {
                            this._selectedSquare.ResetSelected();
                        }

                        this._selectedSquare = squareButton;
                        squareButton.SelectPiece();
                    };

                    squareButton.AddGestureRecognizer(pieceTapGestureRecognizer);
                }
                else
                {
                    TapGestureRecognizer emptyTapGestureRecognizer = new TapGestureRecognizer();
                    emptyTapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        CheckersSquareButton tappedButton = (CheckersSquareButton)s;

                        if (this._selectedSquare != null && tappedButton.GetSquare().IsBlack)
                        {
                            if (!MovePiece(this._selectedSquare, tappedButton))
                            {
                                return;
                            }

                            this._selectedSquare.ResetSelected();
                            this._selectedSquare.HidePiece();
                            tappedButton.ShowPiece(this._selectedSquare.GetSquare().Piece.Color);
                            this._selectedSquare = null;
                        }
                    };

                    squareButton.AddGestureRecognizer(emptyTapGestureRecognizer);
                }
            }
        }
    }
}
