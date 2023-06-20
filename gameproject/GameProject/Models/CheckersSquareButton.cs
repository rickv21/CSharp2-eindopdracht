using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.Models
{
    public class CheckersSquareButton : Frame
    {
        private readonly CheckersSquare square;
        private readonly CheckersPiece piece;
        private TapGestureRecognizer tapGestureRecognizer;

        public CheckersSquareButton(CheckersSquare square)
        {
            this.square = square;
            BackgroundColor = square.TileColor;
            Content = square.Piece.Piece;
        }

        public CheckersSquare GetSquare()
        {
            return this.square;
        }

        public TapGestureRecognizer TapGestureRecognizer
        {
            get => tapGestureRecognizer;
            set => tapGestureRecognizer = value;
        }

        public void addGestureRecognizer(TapGestureRecognizer tapGesture)
        {
            this.GestureRecognizers.Add(tapGesture);
        }

        public void ShowPiece()
        {
            //BorderWidth = 5;
            //BorderColor = Colors.Red;
            BackgroundColor = Color.FromRgb(152, 143, 143);
        }

        public void MarkSelection()
        {
            BackgroundColor = Colors.Blue;
            //BorderWidth = 2;
        }

        public void ResetSelected()
        {
            //BorderWidth = 0;
            BackgroundColor = square.TileColor;
        }



    }
}
