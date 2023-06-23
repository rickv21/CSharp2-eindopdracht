using GameProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.ViewModels
{
    public class ConnectFourViewModel : INotifyPropertyChanged
    {
        private readonly GameModel model;
        private Grid gameGrid;
        private const int Rows = 6;
        private const int Columns = 7;
        private string titleLabel;
        public event PropertyChangedEventHandler PropertyChanged;

        public string TitleLabel
        {
            get { return titleLabel; }
            set
            {
                if (titleLabel != value)
                {
                    titleLabel = value;
                    OnPropertyChanged(nameof(TitleLabel));
                }
            }
        }

        public Grid GameGrid
        {
            get { return gameGrid; }
            set
            {
                if (gameGrid != value)
                {
                    gameGrid = value;
                    OnPropertyChanged(nameof(GameGrid));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private enum Player
        {
            None = 0,
            Red,
            Yellow
        }

        public ConnectFourViewModel(Grid gameGrid)
        {
            model = new GameModel();
            this.gameGrid = gameGrid;

            InitializeGame();
        }

        public void InitializeGame()
        {
            SetupModelValues();
            Stream normalBeep = AudioPlayer.LoadAudio("beep-v2.mp3");
            Stream win = AudioPlayer.LoadAudio("win.mp3");
            Stream lose = AudioPlayer.LoadAudio("lose.mp3");

            model.Set("normalBeep", normalBeep);
            model.Set("win", win);
            model.Set("lose", lose);

            TitleLabel = "Current Player: Red";
            DrawBoard();
        }

        internal void ResetGame()
        {
            Debug.WriteLine("reset");
            model.RemoveAll();
            gameGrid.Clear();
            InitializeGame();
        }

        /// <summary>
        /// Sets the default values in the model.
        /// </summary>
        public void SetupModelValues()
        {
            //model.Add("titleLabel", -1);
            model.Add("currentPlayer", Player.Red);
            model.Add("board", new Player[Rows, Columns]);
            model.Add("isGameOver", false);
        }


        private void DrawBoard()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            for (int row = 0; row < Rows; row++)
            {
                gameGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int col = 0; col < Columns; col++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var button = new Button
                    {
                        BackgroundColor = Color.FromHex("#D3D3D3"),
                        CommandParameter = col,
                        CornerRadius = 50,
                        BorderColor = Color.FromHex("000000"),
                        BorderWidth = 2
                    };

                    button.Clicked += DropPiece;
                    gameGrid.Add(button, col, row);
                }
            }
        }

        private void DropPiece(object sender, EventArgs e)
        {
            if (model.Get<bool>("isGameOver"))
            {
                return;
            }

            var button = (Button)sender;
            int column = (int)button.CommandParameter;

            if (IsValidMove(column))
            {
                int row = GetNextEmptyRow(column);
                Player[,] board = model.Get <Player[,]> ("board");
                Player currentPlayer = model.Get<Player>("currentPlayer");
                board[row, column] = currentPlayer;
                model.Set("board", board);

  

                currentPlayer = (currentPlayer == Player.Red) ? Player.Yellow : Player.Red;


                Debug.WriteLine(currentPlayer.ToString());

                Button lowestRowButton = (Button)gameGrid.Children.Cast<View>().First(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);
                lowestRowButton.BackgroundColor = (currentPlayer == Player.Red) ? Color.FromHex("FFFF00") : Color.FromHex("#FF0000");

                if (CheckForWin(row, column))
                {
                    AudioPlayer.PlaySound(model.Get<Stream>("win"), 0.3);
                    currentPlayer = model.Get<Player>("currentPlayer");
                    var popupMessage = new PopupMessage
                    {
                        Title = "Game over",
                        Message = $"{currentPlayer} heeft gewonnen!"
                    };

                    MessagingCenter.Send(this, "ConnectFourPopup", popupMessage);

                    model.Set("isGameOver", true);
                    return;
                }

                if (IsBoardFull())
                {
                    AudioPlayer.PlaySound(model.Get<Stream>("lose"), 0.3);
                    var popupMessage = new PopupMessage
                    {
                        Title = "Game over",
                        Message = "Het is gelijkspel!"
                    };

                    MessagingCenter.Send(this, "ConnectFourPopup", popupMessage);
                    model.Set("isGameOver", true);
                    return;
                }
                AudioPlayer.PlaySound(model.Get<Stream>("normalBeep"), 0.3);
                model.Set("currentPlayer", currentPlayer);
                TitleLabel = $"Current Player: {(currentPlayer == Player.Red ? "Red" : "Yellow")}";
            }
        }


        private bool IsValidMove(int column)
        {
            Player[,] board = model.Get<Player[,]>("board");
            return board[0, column] == 0;
        }

        private int GetNextEmptyRow(int column)
        {
            Player[,] board = model.Get<Player[,]>("board");
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (board[row, column] == 0)
                {
                    return row;
                }
            }
            return -1;
        }

        private bool CheckForWin(int row, int column)
        {
            return CheckHorizontalWin(row, column) ||
                    CheckVerticalWin(row, column) ||
                    CheckDiagonalWin(row, column);
        }

        private bool CheckHorizontalWin(int row, int column)
        {
            Player[,] board = model.Get<Player[,]>("board");
            Player currentPlayer = model.Get<Player>("currentPlayer");
            for (int c = Math.Max(0, column - 3); c <= Math.Min(Columns - 4, column); c++)
            {
                if (board[row, c] == currentPlayer)
                {
                    bool win = true;
                    for (int i = 1; i < 4; i++)
                    {
                       
                        if (board[row, c + i] != currentPlayer)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win) {
                        for (int i = 0; i < 4; i++)
                        {
                            int targetColumn = c + i;
                            Button lowestRowButton = (Button)gameGrid.Children.Cast<View>().First(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == targetColumn);
                            lowestRowButton.BackgroundColor = Colors.Green;
                        }
                        return true;
                    }
                     
                }
            }
            return false;
        }

        private bool CheckVerticalWin(int row, int column)
        {
            Player[,] board = model.Get<Player[,]>("board");
            Player currentPlayer = model.Get<Player>("currentPlayer");
            for (int r = Math.Max(0, row - 3); r <= Math.Min(Rows - 4, row); r++)
            {
                if (board[r, column] == currentPlayer)
                {
                    bool win = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (board[r + i, column] != currentPlayer)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int targetRow = r + i;
                            Button lowestRowButton = (Button)gameGrid.Children.Cast<View>().First(c => Grid.GetRow(c) == targetRow && Grid.GetColumn(c) == column);
                            lowestRowButton.BackgroundColor = Colors.Green;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckDiagonalWin(int row, int column)
        {
            List<Button> winningButtons = new List<Button>(); // Store the winning buttons

            // Check top left to bottom right
            int startRow = row;
            int startColumn = column;
            Player[,] board = model.Get<Player[,]>("board");
            Player currentPlayer = model.Get<Player>("currentPlayer");

            while (startRow > 0 && startColumn > 0)
            {
                startRow--;
                startColumn--;
            }

            while (startRow <= Rows - 4 && startColumn <= Columns - 4)
            {
                if (board[startRow, startColumn] == currentPlayer)
                {
                    bool win = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (board[startRow + i, startColumn + i] != currentPlayer)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int targetRow = startRow + i;
                            int targetColumn = startColumn + i;
                            Button winningButton = (Button)gameGrid.Children.Cast<View>().FirstOrDefault(c => Grid.GetRow(c) == targetRow && Grid.GetColumn(c) == targetColumn);
                            if (winningButton != null)
                                winningButtons.Add(winningButton);
                        }
                        break; // Exit the loop after finding a win
                    }
                }
                startRow++;
                startColumn++;
            }

            // Check top right to bottom left
            startRow = row;
            startColumn = column;

            while (startRow > 0 && startColumn < Columns - 1)
            {
                startRow--;
                startColumn++;
            }

            while (startRow <= Rows - 4 && startColumn >= 3)
            {
                if (board[startRow, startColumn] == currentPlayer)
                {
                    bool win = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (board[startRow + i, startColumn - i] != currentPlayer)
                        {
                            win = false;
                            break;
                        }
                    }
                    if (win)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int targetRow = startRow + i;
                            int targetColumn = startColumn - i;
                            Button winningButton = (Button)gameGrid.Children.Cast<View>().FirstOrDefault(c => Grid.GetRow(c) == targetRow && Grid.GetColumn(c) == targetColumn);
                            if (winningButton != null)
                                winningButtons.Add(winningButton);
                        }
                        break; // Exit the loop after finding a win
                    }
                }
                startRow++;
                startColumn--;
            }

            // Color the winning buttons green
            foreach (Button button in winningButtons)
            {
                button.BackgroundColor = Colors.Green;
            }

            return winningButtons.Count > 0;
        }

        private bool IsBoardFull()
        {
            Player[,] board = model.Get<Player[,]>("board");
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (board[row, col] == Player.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
