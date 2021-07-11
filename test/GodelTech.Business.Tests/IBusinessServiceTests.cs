using GodelTech.Business.Tests.Fakes;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Business.Tests
{
    // ReSharper disable once InconsistentNaming
    public class IBusinessServiceTests
    {
        private readonly Mock<IBusinessService<FakeDto, FakeAddDto, FakeEditDto, int>> _mockBusinessService;

        public IBusinessServiceTests()
        {
            _mockBusinessService = new Mock<IBusinessService<FakeDto, FakeAddDto, FakeEditDto, int>>(MockBehavior.Strict);
        }

        [Fact]
        public async Task GetListAsync_ReturnsListOfDto()
        {
            // Arrange
            var list = new List<FakeDto>();

            _mockBusinessService
                .Setup(x => x.GetListAsync())
                .ReturnsAsync(list);

            // Act
            var result = await _mockBusinessService.Object.GetListAsync();

            // Assert
            Assert.Equal(list, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsDto()
        {
            // Arrange
            const int id = 1;

            var dto = new FakeDto();

            _mockBusinessService
                .Setup(x => x.GetAsync(id))
                .ReturnsAsync(dto);

            // Act
            var result = await _mockBusinessService.Object.GetAsync(id);

            // Assert
            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task AddAsync_ReturnsDto()
        {
            // Arrange
            var addDto = new FakeAddDto();

            var dto = new FakeDto();

            _mockBusinessService
                .Setup(x => x.AddAsync(addDto))
                .ReturnsAsync(dto);

            // Act
            var result = await _mockBusinessService.Object.AddAsync(addDto);

            // Act & Assert
            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task EditAsync_ReturnsDto()
        {
            // Arrange
            var editDto = new FakeEditDto();

            var dto = new FakeDto();

            _mockBusinessService
                .Setup(x => x.EditAsync(editDto))
                .ReturnsAsync(dto);

            // Act
            var result = await _mockBusinessService.Object.EditAsync(editDto);

            // Assert
            Assert.Equal(dto, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteAsync_ReturnsBoolean(bool isDeleted)
        {
            // Arrange
            const int id = 1;

            _mockBusinessService
                .Setup(m => m.DeleteAsync(id))
                .ReturnsAsync(isDeleted);

            // Act
            var result = await _mockBusinessService.Object.DeleteAsync(1);

            // Assert
            Assert.Equal(isDeleted, result);
        }
    }
}
