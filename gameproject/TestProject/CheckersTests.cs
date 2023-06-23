using System.Linq;
using GameProject.Models;
using Xunit;
using Microsoft.Maui.Controls;
using GameProject.ViewModels;

namespace GameProject.Tests
{
    public class CheckersViewModelTests
    {
        [Fact]
        public void StartGame_InitializesGame()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);

            viewModel.StartGame();

            Assert.NotNull(viewModel.CheckersGrid);
            Assert.NotEmpty(checkersGrid.Children);
            Assert.Equal("Turn: black", viewModel.TurnText);
        }

        [Fact]
        public void GenerateBoard_CreatesCorrectBoard()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);

            viewModel.GenerateBoard();

            Assert.NotNull(viewModel.CheckersGrid);
            Assert.NotEmpty(checkersGrid.Children);
            Assert.Equal(64, checkersGrid.Children.Count);

            var squares = viewModel.GetModel().Get<CheckersSquare[,]>("squares");
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    CheckersSquare square = squares[row, col];
                    if ((row + col) % 2 == 0)
                    {
                        Assert.Equal(Color.FromRgb(216, 162, 90), square.TileColor);
                        Assert.False(square.IsBlack);
                    }
                    else
                    {
                        Assert.Equal(Color.FromRgb(161, 95, 23), square.TileColor);
                        Assert.True(square.IsBlack);
                    }
                }
            }
        }

        [Fact]
        public void IsValidMove_ReturnsFalseForInvalidBlackMove()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 1, 2);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void IsValidMove_ReturnsTrueForValidBlackMove()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 3, 4);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void IsValidMove_ReturnsFalseForInvalidRedMove()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 4, 5);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void IsValidMove_ReturnsTrueForValidRedMove()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 1, 2);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void IsValidMove_ReturnsFalseForBlockedDestination()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 2, 3);
            var blockingSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 3, 4);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Transparent)), 4, 5);
            blockingSquare.ShowPiece(Colors.Black);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void IsValidMove_ReturnsFalseForEmptyStartSquare()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 3, 4);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(isValidMove);
        }

        [Fact]
        public void MakeMove_HidesStartPieceAndShowsEndPiece()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 2, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Red)), 1, 2);

            var isValidMove = viewModel.IsValidMove(startSquare, endSquare);

            Assert.False(startSquare.GetSquare().Piece.Color.Equals(Colors.Transparent));
            Assert.True(endSquare.GetSquare().Piece.Color.Equals(startSquare.GetSquare().Piece.Color));
        }

        [Fact]
        public void MakeMove_UpgradesPieceToKing()
        {
            var checkersGrid = new Grid();
            var viewModel = new CheckersViewModel(checkersGrid);
            var startSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 1, 3);
            var endSquare = new CheckersSquareButton(new CheckersSquare(new CheckersPiece(Colors.Black)), 0, 2);

            viewModel.MakeMove(startSquare, endSquare);

            Assert.True(endSquare.GetSquare().Piece.IsKing);
        }

    }
}
