namespace GameProject;

public partial class ConnectFour : ContentPage
{
    private const int Rows = 6;
    private const int Columns = 7;
    private Player currentPlayer;
    private Player[,] board;
    private bool isGameOver;

    public ConnectFour()
    {
        InitializeComponent();
    }

    public void InitializeGame()
    {
        currentPlayer = Player.Red;
        board = new Player[Rows, Columns];
        isGameOver = false;
        DrawBoard();
    }

    private void NewGame_Clicked(object sender, EventArgs e)
    {
        InitializeGame();
        titleLabel.IsVisible = true;
        startButton.IsVisible = false;
    }

    private enum Player
    {
        None = 0,
        Red,
        Yellow
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
        if (isGameOver)
        {
            return;
        }

        var button = (Button)sender;
        int column = (int)button.CommandParameter;

        if (IsValidMove(column))
        {
            int row = GetNextEmptyRow(column);
            board[row, column] = currentPlayer;

            if (CheckForWin(row, column))
            {
                DisplayAlert("Game over", $"{currentPlayer} heeft gewonnen!", "OK");
                isGameOver = true;
                titleLabel.IsVisible = false;
                startButton.IsVisible = true;
                return;
            }

            if (IsBoardFull())
            {
                DisplayAlert("Game over", "Het is gelijkspel!", "OK");
                isGameOver = true;
                titleLabel.IsVisible = false;
                startButton.IsVisible = true;
                return;
            }

            currentPlayer = (currentPlayer == Player.Red) ? Player.Yellow : Player.Red;

            Button lowestRowButton = (Button)gameGrid.Children.Cast<View>().First(c => Grid.GetRow(c) == row && Grid.GetColumn(c) == column);
            lowestRowButton.BackgroundColor = (currentPlayer == Player.Red) ? Color.FromHex("FFFF00") : Color.FromHex("#FF0000");
        }
    }

    private bool IsValidMove(int column)
    {
        return board[0, column] == 0;
    }

    private int GetNextEmptyRow(int column)
    {
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
                if (win)
                    return true;
            }
        }
        return false;
    }

    private bool CheckVerticalWin(int row, int column)
    {
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
                    return true;
            }
        }
        return false;
    }

    private bool CheckDiagonalWin(int row, int column)
    {
        // Check top left to bottom right
        int startRow = row;
        int startColumn = column;

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
                    return true;
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
                    return true;
            }
            startRow++;
            startColumn--;
        }
        return false;
    }

    private bool IsBoardFull()
    {
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