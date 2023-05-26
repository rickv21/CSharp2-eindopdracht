using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace GameProject;

public partial class ConnectFour : ContentPage
{
    private const int Rows = 6;
    private const int Columns = 7;

    public ConnectFour()
	{
		InitializeComponent();
		InitializeGame();
    }

	public void InitializeGame()
	{
		DrawBoard();
	}

	private void NewGame_Clicked(object sender, EventArgs e)
	{
		
	}

	private void DrawBoard()
	{
		gameGrid.Children.Clear();
		gameGrid.RowDefinitions.Clear();
		gameGrid.ColumnDefinitions.Clear();
		
		for (int row = 0; row < Rows;  row++)
		{
			gameGrid.RowDefinitions.Add(new RowDefinition());
		}

		for (int col = 0; col < Columns; col++)
		{
			gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
		}

        for (int row = 0; row < Rows; row++)
		{
			for(int col = 0; col < Columns; col++)
			{
				var button = new Button
				{
					BackgroundColor = Color.FromHex("#D3D3D3"),
					CommandParameter = col,
                    CornerRadius = 50,
                    BorderColor = Color.FromHex("000000"),
                    BorderWidth = 2
                };

				gameGrid.Add(button, col, row);
            }
		}
	}
}