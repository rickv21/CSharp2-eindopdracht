using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

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
                return;
            }

            currentPlayer = (currentPlayer == Player.Red) ? Player.Yellow : Player.Red;
            button.BackgroundColor = (currentPlayer == Player.Red) ? Color.FromHex("FF0000") : Color.FromHex("#FFFF00");
        }
    }

    // TODO: Currently can place whereever you want, should only be at the botom
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

    // TODO: Currently you cannot win
    private bool CheckForWin(int row, int column)
    {
        return CheckHorizontalWin(row, column) ||
                CheckVerticalWin(row, column) ||
                CheckDiagonalWin(row, column);
    }

    private bool CheckHorizontalWin(int row, int column)
    {
        int count = 0;

        for (int c = Math.Max(0, column - 3); c <= Math.Min(Columns - 1, column + 3); c++)
        {
            if (board[row, c] == currentPlayer)
            {
                count++;
                if (count == 4)
                {
                    return true;
                }
                else
                {
                    count = 0;
                }
            }
        }

        return false;
    }

    private bool CheckVerticalWin(int row, int column)
    {
        int count = 0;

        for (int r = Math.Max(0, row - 3); r < Math.Min(Rows - 1, row + 3); r++)
        {
            if (board[r, column] == currentPlayer)
            {
                count++;
                if (count == 4)
                {
                    return true;
                }
                else
                {
                    count = 0;
                }
            }
        }

        return false;
    }

    private bool CheckDiagonalWin(int row, int column)
    {
        int count = 0;

        // Top right to bottom left
        for (int i = -3; i <= 3; i++)
        {
            int r = row - i;
            int c = column + i;

            if (r >= 0 && r < Rows && c >= 0 && c < Columns)
            {
                if (board[r, c] == currentPlayer)
                {
                    count++;
                    if (count == 4)
                    {
                        return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }
        }

        count = 0;

        // Top left to buttom right
        for (int i = -3; i <= 3; i++)
        {
            int r = row + i;
            int c = column + i;

            if (r >= 0 && r < Rows && c >= 0 && c < Columns)
            {
                if (board[r, c] == currentPlayer)
                {
                    count++;
                    if (count == 4)
                    {
                        return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }
        }


        return false;
    }
}