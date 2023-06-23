using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Models
{
    public class CheckersPlayer
    {
        private readonly string _name;
        private readonly Color _color;

        public CheckersPlayer(string name, Color color)
        {
            this._name = name;
            this._color = color;
        }

        public string Name
        {
            get => _name;
        }

        public Color Color
        {
            get => _color;
        }
        
    }
}
