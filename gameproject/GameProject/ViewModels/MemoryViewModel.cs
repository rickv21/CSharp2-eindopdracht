using System.ComponentModel;
using System.Diagnostics;
using GameProject.Memory;
using GameProject.Models;

namespace GameProject.ViewModels
{
    public class MemoryViewModel : INotifyPropertyChanged
    {
        private MemoryModel model;
        private Grid memoryGrid;
        private string turnText;
        private Dictionary<int, int> usedNumbers = new Dictionary<int, int>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string TurnText
        {
            get { return turnText; }
            set
            {
                if (turnText != value)
                {
                    turnText = value;
                    OnPropertyChanged(nameof(TurnText));
                }
            }
        }

        public Grid MemoryGrid
        {
            get { return memoryGrid; }
            set
            {
                if (memoryGrid != value)
                {
                    memoryGrid = value;
                    OnPropertyChanged(nameof(MemoryGrid));
                }
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MemoryViewModel(Grid memoryGrid)
        {
            model = new MemoryModel();
            this.memoryGrid = memoryGrid;
            StartGame();
            //AddHoverEvents();
            //IncrementCommand = new Command(Increment);
            //DecrementCommand = new Command(Decrement);
        }

        internal void ResetMemoryGame()
        {
            model.RemoveAll();
            usedNumbers.Clear();
            memoryGrid.Clear();
            StartGame();
        }

        private void StartGame()
        {
            turnText = "Turn text";
            Debug.WriteLine("Starting game!!");
            SetupModelValues();
            GenerateCards();
        }

        private void SetupModelValues()
        {
            model.Add("lastCreatedCard", -1);
            model.Add("animationDone", true);
            //model.Add("isPaused", false);
            model.Add("foundNumbers", new List<int>());
            model.Add("tries", 0);
        }




        private void GenerateCards()
        {
            GameProject.Memory.Card[,] cards = new GameProject.Memory.Card[6, 6];
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    int randomValue = GetRandomValue();
                    GameProject.Memory.Card card = new GameProject.Memory.Card(randomValue, GetImageFromValue(randomValue));
                    cards[row, col] = card;
                    Debug.WriteLine("Generated card: " + row + "-" + col);
                }
            }
            model.Add("cards", cards);
            UpdateScreen(cards);
            //Update screen??
        }

        public void UpdateScreen(GameProject.Memory.Card[,] cards)
        {
            // Clear existing buttons from the grid
            memoryGrid.Children.Clear();

            memoryGrid.ColumnSpacing = 25;
            memoryGrid.RowSpacing = 25;

            for (int row = 0; row < cards.GetLength(0); row++)
            {
                for (int col = 0; col < cards.GetLength(1); col++)
                {
                    GameProject.Memory.Card card = cards[row, col];
                    Debug.WriteLine("Loading card button: " + row + "-" + col);
                    GameProject.Memory.CardButton button = new(card)
                    {
                        // Customize button properties
                        Margin = new Thickness(0),
                        BorderColor = Colors.Gray,
                        BackgroundColor = Colors.MediumPurple,
                        BorderWidth = 1,
                        Text = "Memory",
                        //Text = card.GetValue().ToString(),
                        TextColor = Colors.White,
                        HeightRequest = 100,
                    };

                    button.Command = new Command(() => ClickCardAsync(button));
                    // Add button to the grid
                    memoryGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }

        }


        /* private void AddHoverEvents()
         {
             foreach (var child in memoryGrid.Children)
             {
                 if (child is CardButton button)
                 {
                     button.PointerEntered += (sender, e) => button.BackgroundColor = new Color(201, 160, 224);
                     button.PointerExited += (sender, e) => button.BackgroundColor = new Color(174, 83, 212);
                 }
             }
         }*/


        private int GetRandomValue()
        {
            int maxSize = 6 * 6 / 2;
            Random random = new Random();

            int randomNumber = random.Next(1, 32 + 1);

            if (!usedNumbers.ContainsKey(randomNumber))
            {
                if (usedNumbers.Count == maxSize)
                {
                    return GetRandomValue();
                }

                // if (App.IsDebug)
                // {
                Debug.WriteLine("Contains: 0 - " + randomNumber);
                //  }

                usedNumbers[randomNumber] = 1;
                model.Add("lastCreatedCard", randomNumber);
                return randomNumber;
            }

            if (randomNumber == model.Get<int>("lastCreatedCard"))
            {
                //  if (App.IsDebug)
                // {
                Debug.WriteLine("Previous match - " + randomNumber);
                //  }

                int tries = 0;
                if (model.Contains("tries" + randomNumber))
                {
                    tries = model.Get<int>("tries" + randomNumber);
                }

                model.Add("tries" + randomNumber, tries + 1);

                if (tries > 5)
                {
                    if (usedNumbers[randomNumber] < 2)
                    {
                        return randomNumber;
                    }
                    else
                    {
                        Debug.WriteLine("Tries for " + randomNumber + " is more than 5 and number has been used more than 2 times!!!");
                    }
                }

                return GetRandomValue();
            }

            if (usedNumbers[randomNumber] == 1)
            {
                // if (App.IsDebug)
                // {
                Debug.WriteLine("Contains: 1 - " + randomNumber);
                // }

                usedNumbers[randomNumber] = 2;
                model.Add("lastCreatedCard", randomNumber);
                return randomNumber;
            }

            return GetRandomValue();
        }

        private ImageSource GetImageFromResource(string resourceName)
        {
            var assembly = GetType().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return ImageSource.FromStream(() => memoryStream);
            }
            return null;
        }

        private ImageSource GetImageFromValue(int number)
        {
            var resourceName = $"/YourAssemblyName.memory.{number}.png";
            return GetImageFromResource(resourceName);
        }


        /* private void ResizeImageSource(ImageSource source, int width, int height)
         {
             if (source is not FileImageSource fileSource)
                 return;

             var path = fileSource.File;
             if (!File.Exists(path))
                 return;

             var resizedPath = Path.Combine(Path.GetDirectoryName(path), $"{width}x{height}_{Path.GetFileName(path)}");

             if (File.Exists(resizedPath))
                 return;

             var bitmap = new SKBitmap();
             using var image = SKImage.FromBitmap(bitmap);
             using var data = image.Encode();
             using var stream = File.OpenWrite(resizedPath);
             data.SaveTo(stream);
             fileSource.File = resizedPath;
         }*/



        public async Task ClickCardAsync(GameProject.Memory.CardButton button)
        {
            Debug.WriteLine("Clicked button! - " + button.GetCard().GetValue());
            if (!model.Get<bool>("animationDone"))
            {
                //Als de foute kaart animatie nog bezig is, doe dan niets.
                return;
            }
            //Laat de afbeelding van de knop zien.
            button.ShowIcon();

            GameProject.Memory.CardButton lastButton = model.Get<GameProject.Memory.CardButton>("lastClickedCard");

            //Wanneer er geen andere knop geselecteerd is.
            if (lastButton == null)
            {
                //new AudioPlayer("beep-positief-1.wav").play();
                model.Add("lastClickedCard", button);
                button.MarkSelection();
                button.IsEnabled = false;
                /*var popupMessage = new PopupMessage
                {
                    Title = "Win",
                    Message = "Je hebt gewonnen"
                };

                // Publish the popup message
                MessagingCenter.Send(this, "DisplayPopup", popupMessage);*/
                return;
            }

            //Wanneer er 2 buttons met dezelfde afbeelding geselecteerd zijn. (Goed)
            if (button.GetCard().GetValue() == lastButton.GetCard().GetValue())
            {
                //new AudioPlayer("beep-positief-2.wav").play();
                button.MarkCorrect();
                button.IsEnabled = false;
                lastButton.MarkCorrect();
                model.Remove("lastClickedCard");
                List<int> list = model.Get<List<int>>("foundNumbers");
                list.Add(button.GetCard().GetValue());

                int maxSize = 6 * 6 / 2;
                if (list.Count >= maxSize)
                {
                    //If the game is complete.

                    SaveHighScore();
                    //new AudioPlayer("win.wav").play();

                    //pop up
                    var popupMessage = new PopupMessage
                    {
                        Title = "Win",
                        Message = "Je hebt gewonnen"
                    };

                    // Publish the popup message
                    MessagingCenter.Send(this, "DisplayPopup", popupMessage);

                    //TODO: Return to main menu.
                }
                model.Add("foundNumbers", list);

            }
            else
            {
                //  new AudioPlayer("beep-negatief.wav").play();
                //When the two cards do not match. (Error)
                model.Add("animationDone", false);
                button.MarkWrong();
                button.IsEnabled = false;
                lastButton.MarkWrong();

                GameProject.Memory.CardButton finalButton = lastButton;
                //Keep the card red for a bit.

                await Task.Delay(750);

                button.IsEnabled = true;
                button.ResetMarking();
                finalButton.ResetMarking();
                finalButton.IsEnabled = true;
                model.Remove("lastClickedCard");
                model.Add("animationDone", true);

            }
        }

        protected void SaveHighScore()
        {
            int tries = model.Get<int>("tries");
            try
            {
                string executablePath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(Path.GetDirectoryName(executablePath), "score.txt");
                //if (App.IsDebug())
                // {
                //     Console.WriteLine(filePath);
                // }
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }
                // Whether the score exists in the file.
                bool exists = false;

                // Read the contents of score.txt
                List<string> fileContent = new List<string>(File.ReadAllLines(filePath));
                // Loop through the text lines.
                for (int i = 0; i < fileContent.Count; i++)
                {
                    // If the current line starts with "memoryScore".
                    if (fileContent[i].StartsWith("memoryScore"))
                    {
                        // The score exists in the file.
                        exists = true;
                        // If the new time is less than the previously stored time.
                        if (tries < int.Parse(fileContent[i].Split(": ")[1]))
                        {
                            // Update it with the new time.
                            fileContent[i] = "memoryScore: " + tries;
                        }
                        break;
                    }
                }
                // If the time doesn't exist in the file, for example, when the game is played for the first time.
                if (!exists)
                {
                    // Add it.
                    fileContent.Add("memoryScore: " + tries);
                }
                // Write the changes to score.txt
                File.WriteAllLines(filePath, fileContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void LoadHighScore()
        {
            int score = 0;
            try
            {
                string executablePath = AppDomain.CurrentDomain.BaseDirectory;

                string filePath = Path.Combine(Path.GetDirectoryName(executablePath), "score.txt");
                List<string> fileContent = new List<string>(File.ReadAllLines(filePath));
                int length = 0;
                foreach (string line in fileContent)
                {
                    // If the current line starts with "memoryScore".
                    if (line.StartsWith("memoryScore"))
                    {
                        score = int.Parse(line.Split(": ")[1]);
                        // The score exists in the file.
                        length++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }





        public async void ShowPopup(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }


    }

}