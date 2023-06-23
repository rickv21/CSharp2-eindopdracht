using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace GameProject.Memory
{
    public class Card
    {
        private int value;

        public Card(int value)
        {
            this.value = value;
        }

        /**
        * Returnes the number associated with this Card.
         * Used to check if 2 times the same card is selected.
         *
         * @return The number value.
         */
        public int GetValue()
        {
            return value;
        }
    }
}
