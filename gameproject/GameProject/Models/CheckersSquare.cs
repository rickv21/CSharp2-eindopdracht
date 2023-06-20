using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.Models
{
    public class CheckersSquare : INotifyPropertyChanged
    {
        private Color _tileColor;
        private CheckersPiece _piece;
        public event PropertyChangedEventHandler PropertyChanged;

        public CheckersSquare()
        {
        }

        public CheckersSquare(CheckersPiece piece)
        {
            this._piece = piece;
        }

        public Color TileColor
        {
            get => _tileColor;
            set
            {
                _tileColor = value;
                OnPropertyChanged();
            }
        }

        public CheckersPiece Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                OnPropertyChanged();
            }

        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
