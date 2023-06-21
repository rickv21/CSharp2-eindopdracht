using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace GameProject.Memory
{
    public class CardButton : Button
    {
        private readonly Card card;

        public CardButton(Card card)
        {
            this.card = card;
            BackgroundColor = new Color(174, 83, 212);
            IsEnabled = false;
        }


        private void OnPointerEntered(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Hover");
        }

        private void OnPointerExited(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal");
        }

        public Card GetCard()
        {
            return card;
        }

        public void ShowIcon()
        {
            Text = card.GetValue().ToString();
           // BackgroundImageSource = card.GetImage();
        }

        public void MarkSelection()
        {
            BackgroundColor = Colors.Blue;
            BorderWidth = 2;
        }

        public void MarkWrong()
        {
            BackgroundColor = Colors.Red;
            BorderWidth = 2;
        }

        public void ResetMarking()
        {
            Text = "??";
            //Text = card.GetValue().ToString();
            BackgroundColor = Colors.MediumPurple;
            BorderWidth = 1;
           // BackgroundImageSource = null;
        }

        public void MarkCorrect()
        {
            BackgroundColor = null;
            BorderColor = Colors.Green;
            BorderWidth = 2;
        }

  
    }
}
