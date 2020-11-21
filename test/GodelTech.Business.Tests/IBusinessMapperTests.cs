using GodelTech.Business.Tests.Fakes;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    // ReSharper disable once InconsistentNaming
    public class IBusinessMapperTests
    {
        private readonly Mock<IBusinessMapper> _mockBusinessMapper;

        public IBusinessMapperTests()
        {
            _mockBusinessMapper = new Mock<IBusinessMapper>(MockBehavior.Strict);
        }

        [Fact]
        public void Map_ReturnDestination()
        {
            // Arrange
            var entity = new FakeEntity();
            var dto = new FakeDto();

            _mockBusinessMapper.Setup(m => m.Map<FakeDto>(entity)).Returns(dto);

            // Act & Assert
            Assert.Equal(dto, _mockBusinessMapper.Object.Map<FakeDto>(entity));
        }

        [Fact]
        public void MapSourceToDestination_ReturnDestination()
        {
            // Arrange
            var entity = new FakeEntity();
            var dto = new FakeDto();

            _mockBusinessMapper.Setup(m => m.Map(entity, dto)).Returns(dto);

            // Act & Assert
            Assert.Equal(dto, _mockBusinessMapper.Object.Map(entity, dto));
        }
    }
}
