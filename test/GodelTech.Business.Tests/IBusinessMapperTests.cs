using GodelTech.Business.Tests.Fakes;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    public class IBusinessMapperTests
    {
        private readonly Mock<IBusinessMapper> _mockBusinessMapper;

        public IBusinessMapperTests()
        {
            _mockBusinessMapper = new Mock<IBusinessMapper>(MockBehavior.Strict);
        }

        [Fact]
        public void MapSource_ReturnsDestination()
        {
            // Arrange
            var entity = new FakeEntity();
            var dto = new FakeDto();

            _mockBusinessMapper
                .Setup(x => x.Map<FakeEntity, FakeDto>(entity))
                .Returns(dto);

            // Act
            var result = _mockBusinessMapper.Object.Map<FakeEntity, FakeDto>(entity);

            // Assert
            Assert.Equal(dto, result);
        }

        [Fact]
        public void MapSourceToDestination_ReturnsDestination()
        {
            // Arrange
            var entity = new FakeEntity();
            var dto = new FakeDto();

            _mockBusinessMapper
                .Setup(x => x.Map(entity, dto))
                .Returns(dto);

            // Act
            var result = _mockBusinessMapper.Object.Map(entity, dto);

            // Assert
            Assert.Equal(dto, result);
        }
    }
}
