using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Models
{
    public class CheckersPlayer
    {
        private readonly bool _isWhitePiece;
        private bool _isTurn;

        public CheckersPlayer(bool isWhitePiece)
        {
            this._isWhitePiece = isWhitePiece;
        }

        public bool IsWhitePiece
        {
            get => _isWhitePiece;
        }

        public bool IsTurn
        {
            get => _isTurn;
            set => _isTurn = value;
        }
    }
}
