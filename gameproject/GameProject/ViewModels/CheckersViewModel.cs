using System;
using System.Collections;
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
                        square.Piece = new CheckersPiece(Colors.Black);
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
                    CheckersSquareButton button = new(square, row, col)
                    {
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        CornerRadius = 0,
                    };
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

        public bool IsValidMove(CheckersSquareButton start, CheckersSquareButton end)
        {
            var sourceRow = start.GetRow();
            var sourceCol = start.GetCol();
            var targetRow = end.GetRow();
            var targetCol = end.GetCol();

            if (_squareButtons[targetRow, targetCol].HasPiece())
            {
                return false;
            }

            if (targetRow > sourceRow && start.GetSquare().Piece.Color.Equals(Colors.Black))
            {
                return false;
            }

            if (targetRow < sourceRow && start.GetSquare().Piece.Color.Equals(Colors.Red))
            {
                return false;
            }


            var rowDelta = Math.Abs(targetRow - sourceRow);
            var columnDelta = Math.Abs(targetCol - sourceCol); 
            if (rowDelta != columnDelta)
            {
                return false;
            }

            var maxDistance = 1;
            if (rowDelta > maxDistance || columnDelta > maxDistance)
            {
                return false;
            }

            return true;
        }

        public bool MovePiece(CheckersSquareButton startSquare, CheckersSquareButton endSquare)
        {
            IsValidMove(startSquare, endSquare);
            if (endSquare.BackgroundColor.Equals(Color.FromRgb(152, 143, 143)))
            {
                endSquare.ShowPiece(startSquare.GetSquare().Piece.Color);
                startSquare.HidePiece();
                UpdateSquareGestureRecognizers();
                return true;
            }
            return false;
        }

        public void ShowPossibleMoves(CheckersSquareButton squareButton)
        {
            foreach (var sqButton in _squareButtons)
            {
                if (IsValidMove(squareButton, sqButton))
                {
                    sqButton.SelectPiece();
                }
            }

            if (squareButton.GetSquare().Piece.Color.Equals(Colors.Black))
            {
                CheckForRedNeighborPieces(squareButton);
            }
            else
            {
                CheckForBlackNeighborPieces(squareButton);
            }
        }

        public void CheckForRedNeighborPieces(CheckersSquareButton squareButton)
        {
            var sourceRow = squareButton.GetRow();
            var sourceCol = squareButton.GetCol();
            var opponentColor = squareButton.GetSquare().Piece.Color == Colors.Red ? Colors.Black : Colors.Red;
            var targetRow = sourceRow - 1;
            var targetColLeft = sourceCol - 1; 
            var targetColRight = sourceCol + 1; 

            if (targetRow >= 0 && targetRow < 8 && targetColLeft >= 0 && targetColLeft < 8)
            {
                var leftNeighbor = _squareButtons[targetRow, targetColLeft];
                if (leftNeighbor.HasPiece() && leftNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow - 1;
                    var jumpCol = targetColLeft - 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                    }
                }
            }

            if (targetRow >= 0 && targetRow < 8 && targetColRight >= 0 && targetColRight < 8)
            {
                var rightNeighbor = _squareButtons[targetRow, targetColRight];
                if (rightNeighbor.HasPiece() && rightNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow - 1;
                    var jumpCol = targetColRight + 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                    }
                }
            }
        }

        public void CheckForBlackNeighborPieces(CheckersSquareButton squareButton)
        {
            var sourceRow = squareButton.GetRow();
            var sourceCol = squareButton.GetCol();
            var opponentColor = squareButton.GetSquare().Piece.Color == Colors.Red ? Colors.Black : Colors.Red;
            var targetRow = sourceRow + 1; // Up direction
            var targetColLeft = sourceCol - 1; // Left diagonal
            var targetColRight = sourceCol + 1; // Right diagonal

            if (targetRow >= 0 && targetRow < 8 && targetColLeft >= 0 && targetColLeft < 8)
            {
                var leftNeighbor = _squareButtons[targetRow, targetColLeft];
                if (leftNeighbor.HasPiece() && leftNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow + 1;
                    var jumpCol = targetColLeft - 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                    }
                }
            }

            if (targetRow >= 0 && targetRow < 8 && targetColRight >= 0 && targetColRight < 8)
            {
                var rightNeighbor = _squareButtons[targetRow, targetColRight];
                if (rightNeighbor.HasPiece() && rightNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow + 1;
                    var jumpCol = targetColRight + 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                    }
                }
            }
        }


        private void ResetSelected()
        {
            foreach (var squareButton in _squareButtons)
            {
                squareButton.ResetSelected();
            }
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
                            ResetSelected();
                        }
                        //if(this._selectedSquare
                        this._selectedSquare = squareButton;
                        squareButton.SelectPiece();
                        ShowPossibleMoves(squareButton);
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
                            //MovePiece(this._selectedSquare, tappedButton);
                            if (!MovePiece(this._selectedSquare, tappedButton))
                            {
                                return;
                            }

                            ResetSelected();
                            //this._selectedSquare.HidePiece();
                            //tappedButton.ShowPiece(this._selectedSquare.GetSquare().Piece.Color);
                        }
                        this._selectedSquare = null;

                    };

                    squareButton.AddGestureRecognizer(emptyTapGestureRecognizer);
                }
            }
        }
    }
}
