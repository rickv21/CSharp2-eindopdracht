using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Models
{
    public class CheckersSquare : INotifyPropertyChanged
    {
        private Color _tileColor;
        private Color _pieceColor;
        private bool _isKing;
        public event PropertyChangedEventHandler PropertyChanged;

        public CheckersSquare(Color tileColor, Color pieceColor, bool isKing)
        {
            
            this._tileColor= tileColor;
            this._pieceColor = pieceColor;
            this._isKing = isKing;
        }

        public CheckersSquare()
        {

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

        public Color PieceColor
        {
            get => _pieceColor;
            set
            {
                _pieceColor = value;
                OnPropertyChanged();
            }
        }

        public bool IsKing
        {
            get => _isKing;
            set
            {
                _isKing = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
