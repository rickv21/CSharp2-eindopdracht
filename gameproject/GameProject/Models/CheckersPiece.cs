using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject.Models
{
    public class CheckersPiece : INotifyPropertyChanged
    {
        private Ellipse _piece;
        private bool _isKing;
        private Color _color;
        public event PropertyChangedEventHandler PropertyChanged;


        public CheckersPiece(Color color)
        {
            this._color = color;
            this._isKing = false;
            this._piece = new Ellipse()
            {
                WidthRequest = 60,
                HeightRequest = 60,
                Fill = new SolidColorBrush(_color),
                Stroke = Colors.Transparent,
                StrokeThickness = 0
            };
        }


        public Ellipse Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                OnPropertyChanged();
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
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
