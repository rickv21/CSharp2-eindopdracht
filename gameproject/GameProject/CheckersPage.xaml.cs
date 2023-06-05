using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Threading.Channels;
using Microsoft.Maui.Controls.Shapes;

namespace GameProject
    {
        public partial class CheckersPage : ContentPage
        {
            public CheckersPage()
            {
                InitializeComponent();
                AddCheckersPieces();
            }

            private void AddCheckersPieces()
            {
                // Add black pieces
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if ((row + col) % 2 == 1)
                        {
                            Ellipse blackPiece = new Ellipse()
                            {
                                Fill = new SolidColorBrush(Colors.Black),
                                WidthRequest = 75,
                                HeightRequest = 75
                            };

                            board.SetRow(blackPiece, row);
                            board.SetColumn(blackPiece, col);

                            board.Children.Add(blackPiece);
                        }
                    }
                }

                // Add white pieces
                for (int row = 5; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if ((row + col) % 2 == 0)
                        {
                            Ellipse whitePiece = new Ellipse()
                            {
                                Fill = new SolidColorBrush(Colors.White),
                                WidthRequest = 75,
                                HeightRequest = 75
                            };

                            board.SetRow(whitePiece, row);
                            board.SetColumn(whitePiece, col);

                            board.Children.Add(whitePiece);
                        }
                    }
                }
            }
        }

}