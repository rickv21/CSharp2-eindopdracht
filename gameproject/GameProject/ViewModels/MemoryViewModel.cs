using System.ComponentModel;
using System.Diagnostics;
using GameProject.Memory;
using GameProject.Models;
using SharpHook;
using SharpHook.Reactive;

namespace GameProject.ViewModels
{
    public class MemoryViewModel : INotifyPropertyChanged
    {
        private readonly MemoryModel model;
        private Grid memoryGrid;
        private string turnText;
        private string highScore;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool debug = false;

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

        public string HighScore
        {
            get { return highScore; }
            set
            {
                if (highScore != value)
                {
                    highScore = value;
                    OnPropertyChanged(nameof(HighScore));
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
        }

        /// <summary>
        /// Resets the values of the game.
        /// </summary>
        internal void ResetMemoryGame()
        {
            Debug.WriteLine("reset");
            model.RemoveAll();
            memoryGrid.Clear();
            StartGame();
        }
        
        /// <summary>
        /// Starts the game.
        /// </summary>
        private void StartGame()
        {
            Stream normalBeep = AudioPlayer.LoadAudio("beep.wav");
            Stream badBeep = AudioPlayer.LoadAudio("beep-bad.wav");
            Stream goodBeep = AudioPlayer.LoadAudio("beep-good.wav");

            model.Set("normalBeep", normalBeep);
            model.Set("badBeep", badBeep);
            model.Set("goodBeep", goodBeep);

            var hook = new SimpleGlobalHook();

            hook.KeyPressed += (s, e) =>
            {
                var key = e.Data.KeyCode.ToString().Substring(2);
                if (key == "F7")
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        debug = !debug;

                        CardButton lastButton = model.Get<CardButton>("lastClickedCard");

                        foreach (CardButton button in memoryGrid.Children)
                        {
                            if (debug)
                            {
                                button.ShowIcon();
                            }
                            else
                            {
                                button.ResetMarking();
                            }
                            if (button == lastButton)
                            {
                                button.ShowIcon();
                                button.MarkSelection();
                            }

                        }
                    });

                }
                else if (key == "F8")
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        WinGame();
                    });

                }
            };

            hook.RunAsync();
            Debug.WriteLine("Starting game!!");
            TurnText = "Turn: 1";
            SetupModelValues();
            int score = LoadHighScore();
            Debug.WriteLine(score);
            HighScore = "High score: " + (score > 0 ? score : "not set yet");
            GenerateCards();
        }

        /// <summary>
        /// Sets the default values in the mode.
        /// </summary>
        public void SetupModelValues()
        {
            model.Add("lastCreatedCard", -1);
            model.Add("animationDone", true);
            model.Add("foundNumbers", new List<int>());
            model.Add("turns", 1);
        }

        /// <summary>
        /// Generates the memory cards.
        /// </summary>
        private void GenerateCards()
        {
            Card[,] cards = new Card[6, 6];
            Dictionary<int, int> usedNumbers = new Dictionary<int, int>();
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    int randomValue = GetRandomValue(usedNumbers);
                    Card card = new Card(randomValue);
                    cards[row, col] = card;
                    Debug.WriteLine("Generated card: " + row + "-" + col);
                }
            }
            model.Add("cards", cards);
            UpdateScreen(cards);
        }

        /// <summary>
        /// Updates the screen with the memory cards.
        /// </summary>
        /// <param name="cards">The memory cards to be drawn on screen.</param>
        public void UpdateScreen(Card[,] cards)
        {
            // Clear existing buttons from the grid
            memoryGrid.Children.Clear();
            memoryGrid.ColumnSpacing = 25;
            memoryGrid.RowSpacing = 25;
            memoryGrid.Padding = new Thickness(50, 50, 50, 50);

            for (int row = 0; row < cards.GetLength(0); row++)
            {
                for (int col = 0; col < cards.GetLength(1); col++)
                {
                    Card card = cards[row, col];
                    Debug.WriteLine("Loading card button: " + row + "-" + col);
                    CardButton button = new(card)
                    {
                        // Customize button properties
                        Margin = new Thickness(0),
                        BorderColor = Colors.Gray,
                        BackgroundColor = Colors.MediumPurple,
                        FontSize = 24,  
                        BorderWidth = 1,
                        Text = "??",
                        TextColor = Colors.White,
                    };

                    button.Command = new Command(() => ClickCardAsync(button));
                    // Add button to the grid
                    memoryGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }

        }

        /// <summary>
        /// Calculates a random number value for the memory cards.
        /// A specific number can only be generated twice.
        /// </summary>
        /// <param name="usedNumbers">A Dictionary containing numers that are already used.</param>
        /// <returns></returns>
        private int GetRandomValue(Dictionary<int, int> usedNumbers)
        {
            int maxSize = 6 * 6 / 2;
            Random random = new Random();

            int randomNumber = random.Next(1, 32 + 1);

            if (!usedNumbers.ContainsKey(randomNumber))
            {
                if (usedNumbers.Count == maxSize)
                {
                    return GetRandomValue(usedNumbers);
                }

                Debug.WriteLine("Contains: 0 - " + randomNumber);

                usedNumbers[randomNumber] = 1;
                model.Add("lastCreatedCard", randomNumber);
                return randomNumber;
            }

            if (randomNumber == model.Get<int>("lastCreatedCard"))
            {
                Debug.WriteLine("Previous match - " + randomNumber);

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

                return GetRandomValue(usedNumbers);
            }

            if (usedNumbers[randomNumber] == 1)
            {
                Debug.WriteLine("Contains: 1 - " + randomNumber);

                usedNumbers[randomNumber] = 2;
                model.Add("lastCreatedCard", randomNumber);
                return randomNumber;
            }

            return GetRandomValue(usedNumbers);
        }

        /// <summary>
        /// Handles the clicking of a card.
        /// </summary>
        /// <param name="button">The CardButton that was pressed.</param>
        /// <returns></returns>
        public async Task ClickCardAsync(CardButton button)
        {
            Debug.WriteLine("Clicked button! - " + button.GetCard().GetValue());
            if (!model.Get<bool>("animationDone"))
            {
                //If the wrong card animation is still playing, do nothing..
                return;
            }
            //Show the value of the button.
            button.ShowIcon();

            CardButton lastButton = model.Get<CardButton>("lastClickedCard");

            //When no other card has been selected.
            if (lastButton == null)
            {
                AudioPlayer.PlaySound(model.Get<Stream>("normalBeep"), 0.05);
                model.Add("lastClickedCard", button);
                button.MarkSelection();
                button.IsEnabled = false;
                return;
            }

            //When 2 cards with the same value have been selected. (Good)
            if (button.GetCard().GetValue() == lastButton.GetCard().GetValue())
            {
                AudioPlayer.PlaySound(model.Get<Stream>("goodBeep"), 0.05);
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
                    WinGame();
                    return;
                }
                model.Add("foundNumbers", list);
            }
            else
            {
                AudioPlayer.PlaySound(model.Get<Stream>("badBeep"), 0.05);
                //When the two cards do not match. (Error)
                model.Add("animationDone", false);
                button.MarkWrong();
                button.IsEnabled = false;
                lastButton.MarkWrong();

                //Keep the card red for a bit.
                await Task.Delay(750);

                button.IsEnabled = true;
                button.ResetMarking();
                lastButton.ResetMarking();
                lastButton.IsEnabled = true;
                model.Remove("lastClickedCard");
                model.Add("animationDone", true);
            }

            int turn = model.Get<int>("turns") + 1;
            Debug.WriteLine("Turn: " + turn);  
            TurnText = "Turn: " + turn;
            model.Set("turns", turn);
        }

        /// <summary>
        /// Called when the game is won.
        /// </summary>
        private void WinGame()
        {
            int highScore = model.Get<int>("highScore");
            int turn = model.Get<int>("turns");

            Debug.WriteLine($"{highScore} turn {turn}");

            SaveHighScore();

            //Pop-up
            var popupMessage = new PopupMessage
            {
                Title = "Winner",
                Message = "You have won" + (turn < highScore ? " and got a new High Score!" : "!")
            };

            // Publish the popup message
            MessagingCenter.Send(this, "MemoryPopup", popupMessage);
        }

        /// <summary>
        /// Save the high score to the score.txt file.
        /// </summary>
        protected void SaveHighScore()
        {
            int turn = model.Get<int>("turns");
            try
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "score.txt";
                string filePath = Path.Combine(documentPath, fileName);

                Debug.WriteLine(filePath);


                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                bool exists = false;
                List<string> fileContent = new List<string>();

                // Read the contents of score.txt
                if (new FileInfo(filePath).Length != 0)
                {
                    fileContent = new List<string>(File.ReadAllLines(filePath));

                    // Loop through the text lines.
                    for (int i = 0; i < fileContent.Count; i++)
                    {
                        // If the current line starts with "memoryScore".
                        if (fileContent[i].StartsWith("memoryScore"))
                        {
                            exists = true;
                            // If the new time is less than the previously stored time.
                            if (turn < int.Parse(fileContent[i].Split(": ")[1]))
                            {
                                // Update it with the new time.
                                fileContent[i] = "memoryScore: " + turn;
                            }
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    // Add it.
                    fileContent.Add("memoryScore: " + turn);
                }

                // Write the changes to score.txt
                File.WriteAllLines(filePath, fileContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public MemoryModel GetModel()
        {
            return model;
        }


        /// <summary>
        /// Loads the high score from the score.txt file and adds it to the model.
        /// </summary>
        /// <returns>The high score.</returns>
        public int LoadHighScore()
        {
            int score = 0;
            try
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "score.txt";
                string filePath = Path.Combine(documentPath, fileName);

                List<string> fileContent = new List<string>(File.ReadAllLines(filePath));
                foreach (string line in fileContent)
                {
                    // If the current line starts with "memoryScore".
                    if (line.StartsWith("memoryScore"))
                    {
                        score = int.Parse(line.Split(": ")[1]);
                        break;
                        // The score exists in the file.
                    }
                }
                model.Set("highScore", score);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return score;
        }

        public async void ShowPopup(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }


    }

}