using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GodelTech.Business.Tests
{
    public class ServiceAsyncTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<FakeEntity, int>> _mockRepository;
        private readonly Mock<IBusinessMapper> _mockBusinessMapper;

        public ServiceAsyncTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _mockRepository = new Mock<IRepository<FakeEntity, int>>(MockBehavior.Strict);
            _mockBusinessMapper = new Mock<IBusinessMapper>(MockBehavior.Strict);

            _mockUnitOfWork
                .Setup(x => x.GetRepository<FakeEntity, int>())
                .Returns(_mockRepository.Object)
                .Verifiable();
        }

        [Fact]
        public async Task GetListAsync_ReturnListOfDto()
        {
            // Arrange
            var dto = new FakeDto();

            _mockRepository
                .Setup(m => m.GetListAsync<FakeDto>(It.IsAny<QueryParameters<FakeEntity, int>>()))
                .ReturnsAsync(new List<FakeDto> { dto });

            var service = new Service<IUnitOfWork, FakeEntity, FakeDto, FakeAddDto, int>(_mockUnitOfWork.Object, _mockBusinessMapper.Object);

            // Act
            var result = await service.GetListAsync();

            // Assert
            _mockUnitOfWork.VerifyAll();

            Assert.IsAssignableFrom<IList<FakeDto>>(result);
        }

        [Fact]
        public async Task GetAsync_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();

            _mockRepository
                .Setup(m => m.GetAsync<FakeDto>(It.IsAny<QueryParameters<FakeEntity, int>>()))
                .ReturnsAsync(dto);

            var service = new Service<IUnitOfWork, FakeEntity, FakeDto, FakeAddDto, int>(_mockUnitOfWork.Object, _mockBusinessMapper.Object);

            // Act
            var result = await service.GetAsync(1);

            // Assert
            _mockUnitOfWork.VerifyAll();

            Assert.IsAssignableFrom<FakeDto>(result);
        }

        [Fact]
        public async Task AddAsync_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();
            var addDto = new FakeAddDto();
            var entity = new FakeEntity();

            _mockBusinessMapper
                .Setup(m => m.Map<FakeEntity>(It.IsAny<FakeAddDto>()))
                .Returns(entity);

            _mockBusinessMapper
                .Setup(m => m.Map<FakeDto>(It.IsAny<FakeEntity>()))
                .Returns(dto);

            _mockRepository
                .Setup(m => m.InsertAsync(It.IsAny<FakeEntity>()))
                .ReturnsAsync(entity);

            _mockUnitOfWork
                .Setup(m => m.CommitAsync())
                .ReturnsAsync(1);

            var service = new Service<IUnitOfWork, FakeEntity, FakeDto, FakeAddDto, int>(_mockUnitOfWork.Object, _mockBusinessMapper.Object);

            // Act
            var result = await service.AddAsync(addDto);

            // Assert
            _mockUnitOfWork.VerifyAll();

            Assert.IsAssignableFrom<FakeDto>(result);
        }

        [Fact]
        public async Task EditAsync_ReturnDto()
        {
            // Arrange
            var dto = new FakeDto();
            var entity = new FakeEntity();

            _mockBusinessMapper
                .Setup(m => m.Map(It.IsAny<FakeDto>(), It.IsAny<FakeEntity>()))
                .Returns(entity);

            _mockBusinessMapper
                .Setup(m => m.Map<FakeDto>(It.IsAny<FakeEntity>()))
                .Returns(dto);

            _mockRepository
                .Setup(m => m.GetAsync(It.IsAny<QueryParameters<FakeEntity, int>>()))
                .ReturnsAsync(entity);

            _mockRepository
                .Setup(m => m.Update(It.IsAny<FakeEntity>(), false))
                .Returns(entity);

            _mockUnitOfWork
                .Setup(m => m.CommitAsync())
                .ReturnsAsync(1);

            var service = new Service<IUnitOfWork, FakeEntity, FakeDto, FakeAddDto, int>(_mockUnitOfWork.Object, _mockBusinessMapper.Object);

            // Act
            var result = await service.EditAsync(1, dto);

            // Assert
            _mockUnitOfWork.VerifyAll();

            Assert.IsAssignableFrom<FakeDto>(result);
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            // Arrange
            var entity = new FakeEntity();

            _mockRepository
                .Setup(m => m.Get(It.IsAny<QueryParameters<FakeEntity, int>>()))
                .Returns(entity);

            _mockRepository
                .Setup(m => m.Delete(It.IsAny<FakeEntity>()));

            _mockUnitOfWork
                .Setup(m => m.CommitAsync())
                .ReturnsAsync(1);

            var service = new Service<IUnitOfWork, FakeEntity, FakeDto, FakeAddDto, int>(_mockUnitOfWork.Object, _mockBusinessMapper.Object);

            // Act
            await service.DeleteAsync(1);

            // Assert
            _mockUnitOfWork.VerifyAll();
        }
    }
}
