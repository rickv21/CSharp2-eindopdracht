﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.Models
{
    public class CheckersSquareButton : Frame
    {
        private readonly CheckersSquare _square;
        private Color defaultColor;
        private TapGestureRecognizer _tapGestureRecognizer;
        private readonly int _row;
        private readonly int _column;

        public CheckersSquareButton(CheckersSquare square, int row, int col)
        {
            this._square = square;
            BackgroundColor = square.TileColor;
            this._row = row;
            this._column = col;
            Content = square.Piece.Piece;
        }

        public CheckersSquare GetSquare()
        {
            return this._square;
        }

        public TapGestureRecognizer TapGestureRecognizer
        {
            get => _tapGestureRecognizer;
            set => _tapGestureRecognizer = value;
        }

        public void AddGestureRecognizer(TapGestureRecognizer tapGesture)
        {
            this.GestureRecognizers.Add(tapGesture);
        }

        public int GetRow()
        {
            return this._row;
        }

        public int GetCol()
        {
            return this._column;
        }

        public void SelectPiece()
        {
            BackgroundColor = Color.FromRgb(152, 143, 143);
        }

        public bool IsSelected()
        {
            if (BackgroundColor.Equals(Color.FromRgb(152, 143, 143)))
            {
                return true;
            }

            return false;
        }

        public void ResetSelected()
        {
            //BorderWidth = 0;
            BackgroundColor = _square.TileColor;
        }

        public bool HasPiece()
        {
            if (this._square.Piece.Color.Equals(Colors.Transparent))
            {
                return false;
            }
            return true;
        }

        public void ShowPiece(Color color)
        {
            this._square.Piece.SetColor(color);
        }

        public void HidePiece()
        {
            this._square.Piece.SetColor(Colors.Transparent);
            this._square.Piece.IsKing = false;
            this._square.Piece.Piece.Stroke = Colors.Transparent;
        }

        public void MakeKing(Color color)
        {
            this._square.Piece.IsKing = true;
            this._square.Piece.Piece.Stroke = Colors.Yellow;
            this._square.Piece.Piece.StrokeThickness = 5;
            this._square.Piece.SetColor(color);
        }


    }
}
