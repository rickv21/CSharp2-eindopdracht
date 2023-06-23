using GameProject;
using GameProject.Memory;
using GameProject.ViewModels;
using System.Reflection;
using Xunit.Abstractions;
using static GameProject.ViewModels.ConnectFourViewModel;

namespace TestProject
{
    public class ConnectFourTests
    {

        //Use this for loggin in tests (usage: output.WriteLine(<value>);)
        //It needs to be in the constructor to work.
        private readonly ITestOutputHelper output;

        readonly ConnectFourViewModel viewModel;

        //Is run before each test.
        public ConnectFourTests(ITestOutputHelper output) {
            this.output = output;
            viewModel = new ConnectFourViewModel(new Grid(), "username");
            viewModel.SetupModelValues();
        }

        /// <summary>
        /// Tests if the default model values are correctly set.
        /// </summary>
        [Fact]
        public void TestModelValues()
        {
            // Check if the default values are set correctly in the model
            Assert.Equal(Player.Red, viewModel.GetModel().Get<Player>("currentPlayer"));
            Assert.False(viewModel.GetModel().Get<bool>("isGameOver"));
        }

        /// <summary>
        /// Tests if the connect four logic works correctly..
        /// </summary>
        [Fact]
        public async void TestGameLogic()
        {
            Player[,] board = viewModel.GetModel().Get<Player[,]>("board");

            //Is the board empty.
            foreach (var player in board)
            {
                Assert.Equal(Player.None, player);
            }

            //Trigger a click.
            foreach (Button button in viewModel.GameGrid.Children)
            {
                if (Grid.GetRow(button) == 5 && Grid.GetColumn(button) == 0)
                {
                    button.SendClicked();
                    break;
                }
            }

            //Has the "stone" been set?
            Assert.Equal(Player.Red, board[5, 0]);

            //Trigger four stones next to eachother.
            foreach (Button button in viewModel.GameGrid.Children)
            {
                viewModel.GetModel().Set("currentPlayer", Player.Red);

                if (Grid.GetRow(button) == 5 && Grid.GetColumn(button) < 4)
                {
                    button.SendClicked();
                }
            }
            //Has the game ended?
            Assert.True(viewModel.GetModel().Get<bool>("isGameOver"));
        }
    }
}