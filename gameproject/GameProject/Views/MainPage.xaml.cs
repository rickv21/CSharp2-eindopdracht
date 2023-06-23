using Microsoft.Maui.Controls;
using System.Reflection;

namespace GameProject.Views
{
    public partial class MainPage : ContentPage
    {
        private string username;

        public MainPage()
        {
            InitializeComponent();
            username = LoadUserName();
        }

        private async void OnMemoryButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Memory");
        }

        private async void OnCheckersButtonClicked(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//Checkers");

        }

        private async void OnConnectFourButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//ConnectFour?userName={username}");
        }

        private async void OnChangeUsernameButtonClicked(object sender, EventArgs e)
        {
            var inputPromptPage = new InputPromptPage(username);

            inputPromptPage.InputAccepted += nameChange;

            await Shell.Current.Navigation.PushModalAsync(inputPromptPage);
        }

        private void nameChange(object sender, string inputValue)
        {
            username = inputValue;
            SaveUserName();
        }

        private string LoadUserName()
        {
            string username = "Player";
            try
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "GameProject.txt";
                string filePath = Path.Combine(documentPath, fileName);

                List<string> fileContent = new List<string>(File.ReadAllLines(filePath));
                foreach (string line in fileContent)
                {
                    // If the current line starts with "username".
                    if (line.StartsWith("username"))
                    {
                        username = line.Split(": ")[1];
                        break;
                        // The score exists in the file.
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return username;
        }

    
        private void SaveUserName()
        {
            try
            {
                string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "GameProject.txt";
                string filePath = Path.Combine(documentPath, fileName);


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
                        if (fileContent[i].StartsWith("username"))
                        {
                            exists = true;
                            fileContent[i] = "username: " + username;
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    // Add it.
                    fileContent.Add("username: " + username);
                }

                // Write the changes to score.txt
                File.WriteAllLines(filePath, fileContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
