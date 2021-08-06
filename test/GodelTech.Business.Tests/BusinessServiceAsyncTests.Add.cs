using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        public static IEnumerable<object[]> AddMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    new FakeAddDto(),
                    new FakeEntity<Guid>(),
                    new FakeDto<Guid>()
                },
                new object[]
                {
                    default(Guid),
                    new FakeAddDto
                    {
                        Name = "Test Name"
                    },
                    new FakeEntity<Guid>
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Test Name"
                    },
                    new FakeDto<Guid>
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Test Name"
                    }
                },
                // int
                new object[]
                {
                    default(int),
                    new FakeAddDto(),
                    new FakeEntity<int>(),
                    new FakeDto<int>()
                },
                new object[]
                {
                    default(int),
                    new FakeAddDto
                    {
                        Name = "Test Name"
                    },
                    new FakeEntity<int>
                    {
                        Id = 1,
                        Name = "Test Name"
                    },
                    new FakeDto<int>
                    {
                        Id = 1,
                        Name = "Test Name"
                    }
                },
                // string
                new object[]
                {
                    string.Empty,
                    new FakeAddDto(),
                    new FakeEntity<string>
                    {
                        Id = string.Empty
                    },
                    new FakeDto<string>
                    {
                        Id = string.Empty
                    }
                },
                new object[]
                {
                    string.Empty,
                    new FakeAddDto
                    {
                        Name = "Test Name"
                    },
                    new FakeEntity<string>
                    {
                        Id = "Test Id",
                        Name = "Test Name"
                    },
                    new FakeDto<string>
                    {
                        Id = "Test Id",
                        Name = "Test Name"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(AddMemberData))]
        public async Task AddAsync_Success<TKey>(
            TKey defaultKey,
            FakeAddDto item,
            FakeEntity<TKey> entity,
            FakeDto<TKey> expectedResult)
        {
            // Arrange
            Expression<Action<ILogger>> loggerExpressionAdd = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Add item: {item}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionAdd);

            _mockBusinessMapper
                .Setup(
                    x => x.Map<FakeAddDto, FakeEntity<TKey>>(item)
                )
                .Returns(entity);

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.InsertAsync(entity)
                )
                .ReturnsAsync(entity);

            Expression<Action<ILogger>> loggerExpressionSave = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    "Save changes"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionSave);

            _mockUnitOfWork
                .Setup(
                    x => x.CommitAsync()
                )
                .ReturnsAsync(1);

            _mockBusinessMapper
                .Setup(
                    x => x.Map<FakeEntity<TKey>, FakeDto<TKey>>(entity)
                )
                .Returns(expectedResult);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act
            var result = await businessService.AddAsync(item);

            // Assert
            Assert.NotNull(defaultKey);

            _mockLogger.Verify(loggerExpressionAdd, Times.Once);

            _mockBusinessMapper
                .Verify(
                    x => x.Map<FakeAddDto, FakeEntity<TKey>>(item),
                    Times.Once
                );

            mockRepository
                .Verify(
                    x => x.InsertAsync(entity),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionSave, Times.Once);

            _mockUnitOfWork
                .Verify(
                    x => x.CommitAsync(),
                    Times.Once
                );

            _mockBusinessMapper
                .Verify(
                    x => x.Map<FakeEntity<TKey>, FakeDto<TKey>>(entity),
                    Times.Once
                );

            Assert.Equal(expectedResult, result, new FakeDtoEqualityComparer<TKey>());
        }
    }
}