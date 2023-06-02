using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace GameProject.Memory
{
    public class Card
    {
        private int value;
        private ImageSource image;

        public Card(int value, ImageSource image)
        {
            this.value = value;
            this.image = image;
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

        /**
         * Returnes the image associated with this Card.
         *
         * @return the image.
         */
        public ImageSource GetImage()
        {
            return image;
        }
    }
}
