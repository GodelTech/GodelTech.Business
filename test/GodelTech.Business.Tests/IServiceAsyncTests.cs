using GodelTech.Business.Tests.Fakes;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Business.Tests
{
    // ReSharper disable once InconsistentNaming
    public class IServiceAsyncTests
    {
        private readonly Mock<IService<FakeDto, int, IFakeAddDto>> _mockService;

        public IServiceAsyncTests()
        {
            _mockService = new Mock<IService<FakeDto, int, IFakeAddDto>>(MockBehavior.Strict);
        }

        [Fact]
        public async Task GetListAsync_ReturnListOfDto()
        {
            // Arrange
            var dto = new FakeDto();

            _mockService
                .Setup(m => m.GetListAsync())
                .ReturnsAsync(new List<FakeDto> { dto });

            // Act & Assert
            Assert.Contains(dto, await _mockService.Object.GetListAsync());
        }

        [Fact]
        public async Task GetAsync_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();

            _mockService
                .Setup(m => m.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _mockService.Object.GetAsync(1);

            // Assert
            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task AddAsync_AddDto_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();
            var addDto = new FakeAddDto();

            _mockService
                .Setup(m => m.AddAsync(addDto))
                .ReturnsAsync(dto);

            // Act & Assert
            Assert.Equal(dto, await _mockService.Object.AddAsync(addDto));
        }

        [Fact]
        public async Task EditAsync_Dto_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();

            _mockService
                .Setup(m => m.EditAsync(It.IsAny<int>(), dto))
                .ReturnsAsync(dto);

            // Act & Assert
            Assert.Equal(dto, await _mockService.Object.EditAsync(1, dto));
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            // Arrange
            _mockService
                .Setup(m => m.DeleteAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _mockService.Object.DeleteAsync(1);

            // Assert
            _mockService.Verify();
        }
    }
}
