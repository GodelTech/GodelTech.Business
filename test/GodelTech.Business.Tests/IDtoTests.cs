using System.Collections.Generic;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    // ReSharper disable once InconsistentNaming
    public class IDtoTests
    {
        private readonly Mock<IDto<int>> _mockDto;

        public IDtoTests()
        {
            _mockDto = new Mock<IDto<int>>(MockBehavior.Strict);
        }

        [Fact]
        public void Id_Get_Success()
        {
            // Arrange
            const int id = 1;

            _mockDto.SetupGet(m => m.Id).Returns(id);

            // Act & Assert
            Assert.Equal(id, _mockDto.Object.Id);
        }

        [Fact]
        public void Id_Set_Success()
        {
            // Arrange
            var result = 0;

            const int id = 1;

            _mockDto.SetupSet(m => m.Id = It.IsAny<int>()).Callback((int value) => { result = value; });

            // Act
            _mockDto.Object.Id = id;

            // Assert
            Assert.Equal(id, result);
        }
    }
}