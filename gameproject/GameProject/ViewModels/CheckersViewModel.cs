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
        private CheckersPlayer _red;
        private CheckersPlayer _black;
        private CheckersPlayer _turn;
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

        public CheckersModel GetModel()
        {
            return _model; 
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
            this._red = new CheckersPlayer("red", Colors.Red);
            this._black = new CheckersPlayer("black", Colors.Black);
            this._turn = _black;
            StartGame();
        }

        public void StartGame()
        {
            Debug.WriteLine("Starting game!!");
            TurnText = "Turn: " + this._turn.Name;
            SetupModelValues();
            GenerateBoard();
        }

        public void ResetGame()
        {

        }

        public void GenerateBoard()
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
            TurnText = "Turn: " + this._turn.Name;
            SetupModelValues();
        }

        public void SetupModelValues()
        {
            this._model.Add("playerTurn", this._black);
            this._model.Add("squareSelected", null);
            Stream normalBeep = AudioPlayer.LoadAudio("beep-v2.mp3");
            Stream win = AudioPlayer.LoadAudio("win.mp3");

            _model.Set("normalBeep", normalBeep);
            _model.Set("win", win);
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

            if (!start.GetSquare().Piece.IsKing)
            {
                if (targetRow > sourceRow && start.GetSquare().Piece.Color.Equals(Colors.Black))
                {
                    return false;
                }

                if (targetRow < sourceRow && start.GetSquare().Piece.Color.Equals(Colors.Red))
                {
                    return false;
                }
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
            if (endSquare.IsSelected())
            {
                AudioPlayer.PlaySound(_model.Get<Stream>("normalBeep"), 0.3);
                MakeMove(startSquare, endSquare);
                CheckForWin();
                UpdateSquareGestureRecognizers();
                return true;
            }
            return false;
        }

        public void MakeMove(CheckersSquareButton startSquare, CheckersSquareButton endSquare)
        {
            if (startSquare.GetSquare().Piece.IsKing)
            {
                endSquare.MakeKing(startSquare.GetSquare().Piece.Color);
            }
            else
            {
                endSquare.ShowPiece(startSquare.GetSquare().Piece.Color);
            }

            if (Math.Abs(endSquare.GetRow() - startSquare.GetRow()) > 1)
            {
                CheckersSquareButton squareBtn = _model.Get<CheckersSquareButton>("rightNeighbor");
                if (endSquare.GetCol() < startSquare.GetCol())
                {
                    squareBtn = _model.Get<CheckersSquareButton>("leftNeighbor");
                }

                squareBtn.HidePiece();
            }

            if (endSquare.GetSquare().Piece.Color.Equals(Colors.Black) && endSquare.GetRow() == 0 ||
                endSquare.GetSquare().Piece.Color.Equals(Colors.Red) && endSquare.GetRow() == 7)
            {
                endSquare.MakeKing(startSquare.GetSquare().Piece.Color);
            }
            startSquare.HidePiece();



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

            if (squareButton.GetSquare().Piece.IsKing)
            {
                CheckForNeighborPieces(squareButton, 1);
                CheckForNeighborPieces(squareButton, -1);
                return;
            }

            if (squareButton.GetSquare().Piece.Color.Equals(Colors.Black))
            {
                CheckForNeighborPieces(squareButton, -1);
            }
            else
            {
                CheckForNeighborPieces(squareButton, 1);
            }
        }

        public void CheckForNeighborPieces(CheckersSquareButton squareButton, int rowOffset)
        {
            var sourceRow = squareButton.GetRow();
            var sourceCol = squareButton.GetCol();
            var opponentColor = squareButton.GetSquare().Piece.Color == Colors.Red ? Colors.Black : Colors.Red;
            var targetRow = sourceRow + rowOffset;
            var targetColLeft = sourceCol - 1; 
            var targetColRight = sourceCol + 1; 

            if (targetRow >= 0 && targetRow < 8 && targetColLeft >= 0 && targetColLeft < 8)
            {
                var leftNeighbor = _squareButtons[targetRow, targetColLeft];
                if (leftNeighbor.HasPiece() && leftNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow + rowOffset;
                    var jumpCol = targetColLeft - 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                        _model.Add("leftNeighbor", leftNeighbor); 
                    }
                }
            }

            if (targetRow >= 0 && targetRow < 8 && targetColRight >= 0 && targetColRight < 8)
            {
                var rightNeighbor = _squareButtons[targetRow, targetColRight];
                if (rightNeighbor.HasPiece() && rightNeighbor.GetSquare().Piece.Color == opponentColor)
                {
                    var jumpRow = targetRow + rowOffset;
                    var jumpCol = targetColRight + 1;
                    if (jumpRow >= 0 && jumpRow < 8 && jumpCol >= 0 && jumpCol < 8 && !_squareButtons[jumpRow, jumpCol].HasPiece())
                    {
                        _squareButtons[jumpRow, jumpCol].SelectPiece();
                        _model.Add("rightNeighbor", rightNeighbor);
                    }
                }
            }
        }

        private void CheckForWin()
        {
            int blackCount = 0;
            int redCount = 0;
            foreach (var squareButton in _squareButtons)
            {
                if (squareButton.GetSquare().Piece.Color.Equals(Colors.Black))
                {
                    blackCount++;
                }

                if (squareButton.GetSquare().Piece.Color.Equals(Colors.Red))
                {
                    redCount++;
                }
            }

            if (blackCount == 0)
            {
                showWinMSg("Red");
            }

            if (redCount == 0)
            {
                showWinMSg("Black");
            }
        }

        private void showWinMSg(String winner)
        {
            AudioPlayer.PlaySound(_model.Get<Stream>("win"), 0.3);

            var popupMessage = new PopupMessage()
            {
                Title = "Winner",
                Message = winner + " has won"
            };
            MessagingCenter.Send(this, "CheckersPopup", popupMessage);

        }

        private void ResetSelected()
        {
            foreach (var squareButton in _squareButtons)
            {
                squareButton.ResetSelected();
            }
        }

        private void SwitchTurn()
        {
            if (this._turn == this._red)
            {
                this._turn = this._black;
                TurnText = "Turn: " + this._black.Name;
                return;
            }
            this._turn = this._red;
            TurnText = "Turn: " + this._red.Name;
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
                        if (!squareButton.GetSquare().Piece.Color.Equals(this._turn.Color))
                        {
                            return;
                        }
                        if (this._selectedSquare != null)
                        {
                            ResetSelected();
                        }
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
                            SwitchTurn();
                        }
                        this._selectedSquare = null;
                    };

                    squareButton.AddGestureRecognizer(emptyTapGestureRecognizer);
                }
            }

        }
    }
}
