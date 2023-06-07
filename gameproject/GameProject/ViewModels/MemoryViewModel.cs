using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using GameProject.Memory;
using GameProject.Models;
using Plugin.Maui.Audio;
using SharpHook;
using SharpHook.Reactive;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace GameProject.ViewModels
{
    public class MemoryViewModel : INotifyPropertyChanged
    {
        private MemoryModel model;
        private Grid memoryGrid;
        private string turnText;
        private Dictionary<int, int> usedNumbers = new Dictionary<int, int>();
        public event PropertyChangedEventHandler PropertyChanged;
        private bool debug = false;

        //Audio
        Stream normalBeep;
        Stream goodBeep;
        Stream badBeep;

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

            Assembly assembly = Assembly.GetExecutingAssembly();
            normalBeep = assembly.GetManifestResourceStream("GameProject.Resources.Audio.beep.wav");
            badBeep = assembly.GetManifestResourceStream("GameProject.Resources.Audio.beep-bad.wav");
            goodBeep = assembly.GetManifestResourceStream("GameProject.Resources.Audio.beep-good.wav");

            var hook = new SimpleGlobalHook();

            hook.KeyPressed += (s, e) =>
            {
                var key = e.Data.KeyCode.ToString().Substring(2);
                if(key == "Insert")
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        debug = !debug;
                        if (debug)
                        {
                            foreach (CardButton button in memoryGrid.Children)
                            {
                                button.ShowIcon();
                            }
                        }
                    });
            
                    }
            };

            hook.RunAsync();
            StartGame();
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
            TurnText = "Turn: 1";
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
            model.Add("turns", 1);
        }




        private void GenerateCards()
        {
            GameProject.Memory.Card[,] cards = new GameProject.Memory.Card[6, 6];
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    int randomValue = GetRandomValue();
                    GameProject.Memory.Card card = new GameProject.Memory.Card(randomValue);
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
            memoryGrid.Padding = new Thickness(50, 50, 50, 50);

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

        private void OnKey(KeyboardHookEventArgs e, IReactiveGlobalHook hook)
        {
            Debug.WriteLine("test");
            Debug.WriteLine(e.Data.KeyChar);
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
                playSound(normalBeep);
                model.Add("lastClickedCard", button);
                button.MarkSelection();
                button.IsEnabled = false;
                return;
            }

            //Wanneer er 2 buttons met dezelfde afbeelding geselecteerd zijn. (Goed)
            if (button.GetCard().GetValue() == lastButton.GetCard().GetValue())
            {
                playSound(goodBeep);

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
                playSound(badBeep);
              
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

            int turn = model.Get<int>("turns") + 1;
            Debug.WriteLine(turn);  
            TurnText = "Turn: " + turn;
            model.Set("turns", turn);
        }

        private void playSound(Stream stream)
        {
            if (stream != null)
            {
                var audioPlayer = AudioManager.Current.CreatePlayer(stream);
                audioPlayer.Play();
            }
        }

        protected void SaveHighScore()
        {
            int tries = model.Get<int>("turns");
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