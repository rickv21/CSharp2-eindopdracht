using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Models
{
    public class CheckersSquareButton : Button
    {
        private readonly CheckersSquare square;

        public CheckersSquareButton(CheckersSquare square)
        {
            this.square = square;
            BackgroundColor = square.TileColor;
        }

        private void OnPointerEntered(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Hover");
        }

        private void OnPointerExited(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal");
        }

        public CheckersSquare GetSquare()
        {
            return this.square;
        }

        public void ShowPiece()
        {
            BorderWidth = 5;
            BorderColor = Colors.Red;
        }

        public void MarkSelection()
        {
            BackgroundColor = Colors.Blue;
            BorderWidth = 2;
        }

        public void ResetSelected()
        {
            BorderWidth = 0;
            BorderColor = null;
        }


    }
}
