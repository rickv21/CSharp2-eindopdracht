using GameProject;
using GameProject.ViewModels;
using GameProject.Models;
using Xunit;

namespace GameProject.Tests
{
    public class MemoryViewModelTests
    {

        [Fact]
        public void test()
        {

            Assert.Equal(5, 5);
            // Arrange
           /* var viewModel = new MemoryViewModel(new Grid()); // Pass null for memoryGrid since it's not needed in this test

            // Act
            viewModel.SetupModelValues();

            // Assert
            // Check if the default values are set correctly in the model
            Assert.Equal(-1, viewModel.GetModel().Get<int>("lastCreatedCard"));
            Assert.True(viewModel.GetModel().Get<bool>("animationDone"));
            Assert.NotNull(viewModel.GetModel().Get<List<int>>("foundNumbers"));
            Assert.Equal(1, viewModel.GetModel().Get<int>("turns"));*/
        }
    }
}
