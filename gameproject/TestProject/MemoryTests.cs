using GameProject.Memory;
using GameProject.ViewModels;
using System.Reflection;
using Xunit.Abstractions;

namespace TestProject
{
    public class MemoryTests
    {

        //Use this for loggin in tests (usage: output.WriteLine(<value>);)
        //It needs to be in the constructor to work.
        private readonly ITestOutputHelper output;

        readonly MemoryViewModel viewModel;

        //Is run before each test.
        public MemoryTests(ITestOutputHelper output) {
            this.output = output;
            viewModel = new MemoryViewModel(new Grid());
            viewModel.SetupModelValues();
        }

        /// <summary>
        /// Tests if the default model values are correctly set.
        /// </summary>
        [Fact]
        public void TestModelValues()
        {
            // Check if the default values are set correctly in the model
            Assert.Equal(-1, viewModel.GetModel().Get<int>("lastCreatedCard"));
            Assert.True(viewModel.GetModel().Get<bool>("animationDone"));
            Assert.NotNull(viewModel.GetModel().Get<List<int>>("foundNumbers"));
            Assert.Equal(1, viewModel.GetModel().Get<int>("turns"));
        }

        /// <summary>
        /// Tests if the correct values are generated and that there are always two of each value.
        /// </summary>
        [Fact]
        public void TestBoardValues()
        {
            Card[,] cards = viewModel.GetModel().Get<Card[,]>("cards");
            //36 total cards.
            Assert.Equal(36, cards.Length);

            Dictionary<int, int> cardValues = new Dictionary<int, int>();

            foreach (Card card in cards)
            {
                var value = card.GetValue();
                if (cardValues.ContainsKey(value))
                {
                    var amount = cardValues[value];
                    cardValues[value] = amount + 1;
                } else
                {
                    cardValues[value] = 1;
                }
                output.WriteLine(card.GetValue().ToString());
            }
            
            //Each value has exactly two cards.
            foreach(int i in cardValues.Keys) {
                output.WriteLine("Testing amount for: " + i);
                Assert.Equal(2, cardValues[i]);
            }
        
        }

        /// <summary>
        /// Tests the click card action and if winning the game works correctly.
        /// </summary>
        [Fact]
        public async Task TestWinAsync()
        {
            List<int> list = viewModel.GetModel().Get<List<int>>("foundNumbers");
            for(int i = 0; i < 32; i++)
            {
                list.Add(i);
            }

            viewModel.GetModel().Add("foundNumbers", list);
            viewModel.GetModel().Add("lastClickedCard",new CardButton(new Card(32)));

            await viewModel.ClickCardAsync(new CardButton(new Card(32)));
            Assert.True(viewModel.GetModel().Get<bool>("won"));
        }
    }
}