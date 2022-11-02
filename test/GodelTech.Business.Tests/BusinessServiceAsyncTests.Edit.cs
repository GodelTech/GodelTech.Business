using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using Xunit;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        [Theory]
        [MemberData(nameof(BusinessServiceTests.TypesMemberData), MemberType = typeof(BusinessServiceTests))]
        public async Task EditAsync_ThrowsArgumentNullException<TKey>(
            TKey defaultKey)
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act & Assert
            Assert.NotNull(defaultKey);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => businessService.EditAsync(null, cancellationToken)
            );

            Assert.Equal("item", exception.ParamName);
        }

        [Theory]
        [MemberData(nameof(BusinessServiceTests.TypesMemberData), MemberType = typeof(BusinessServiceTests))]
        public async Task EditAsync_WhenNotFound_ReturnsNull<TKey>(
            TKey defaultKey)
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var item = new FakeEditDto<TKey>
            {
                Id = defaultKey
            };

            Expression<Action<ILogger>> loggerExpressionEdit = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Edit item: {item}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true);
            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Warning))
                .Returns(true);
            _mockLogger.Setup(loggerExpressionEdit);

            var filterExpression = FilterExpressionExtensions.CreateIdFilterExpression<FakeEntity<TKey>, TKey>(defaultKey);

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.GetAsync(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        ),
                        cancellationToken
                    )
                )
                .ReturnsAsync(() => null);

            Expression<Action<ILogger>> loggerExpressionNotFound = x => x.Log(
                LogLevel.Warning,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Item not found: {item.Id}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionNotFound);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act
            var result = await businessService.EditAsync(item, cancellationToken);

            // Assert
            _mockLogger.Verify(loggerExpressionEdit, Times.Once);

            mockRepository
                .Verify(
                    x => x.GetAsync(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        ),
                        cancellationToken
                    ),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionNotFound, Times.Once);

            Assert.Null(result);
        }

        public static IEnumerable<object[]> EditMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    new FakeEditDto<Guid>(),
                    new FakeEntity<Guid>(),
                    new FakeDto<Guid>()
                },
                new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    new FakeEditDto<Guid>
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Test New Name"
                    },
                    new FakeEntity<Guid>
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Test Name"
                    },
                    new FakeDto<Guid>
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Test New Name"
                    }
                },
                // int
                new object[]
                {
                    default(int),
                    new FakeEditDto<int>(),
                    new FakeEntity<int>(),
                    new FakeDto<int>()
                },
                new object[]
                {
                    1,
                    new FakeEditDto<int>
                    {
                        Id = 1,
                        Name = "Test New Name"
                    },
                    new FakeEntity<int>
                    {
                        Id = 1,
                        Name = "Test Name"
                    },
                    new FakeDto<int>
                    {
                        Id = 1,
                        Name = "Test New Name"
                    }
                },
                // string
                new object[]
                {
                    string.Empty,
                    new FakeEditDto<string>
                    {
                        Id = string.Empty
                    },
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
                    "Test Id",
                    new FakeEditDto<string>
                    {
                        Id = "Test Id",
                        Name = "Test New Name"
                    },
                    new FakeEntity<string>
                    {
                        Id = "Test Id",
                        Name = "Test Name"
                    },
                    new FakeDto<string>
                    {
                        Id = "Test Id",
                        Name = "Test New Name"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(EditMemberData))]
        public async Task EditAsync_Success<TKey>(
            TKey defaultKey,
            FakeEditDto<TKey> item,
            FakeEntity<TKey> entity,
            FakeDto<TKey> expectedResult)
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            Expression<Action<ILogger>> loggerExpressionEdit = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Edit item: {item}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true);
            _mockLogger.Setup(loggerExpressionEdit);

            var filterExpression = FilterExpressionExtensions.CreateIdFilterExpression<FakeEntity<TKey>, TKey>(defaultKey);

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.GetAsync(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        ),
                        cancellationToken
                    )
                )
                .ReturnsAsync(entity);

            _mockBusinessMapper
                .Setup(
                    x => x.Map(item, entity)
                )
                .Returns(entity);

            mockRepository
                .Setup(
                    x => x.Update(entity, false)
                )
                .Returns(entity);

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
                    x => x.CommitAsync(cancellationToken)
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
            var result = await businessService.EditAsync(item, cancellationToken);

            // Assert
            _mockLogger.Verify(loggerExpressionEdit, Times.Once);

            mockRepository
                .Verify(
                    x => x.GetAsync(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        ),
                        cancellationToken
                    ),
                    Times.Once
                );

            _mockBusinessMapper
                .Verify(
                    x => x.Map(item, entity),
                    Times.Once
                );

            mockRepository
                .Verify(
                    x => x.Update(entity, false),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionSave, Times.Once);

            _mockUnitOfWork
                .Verify(
                    x => x.CommitAsync(cancellationToken),
                    Times.Once
                );

            _mockBusinessMapper
                .Verify(
                    x => x.Map<FakeEntity<TKey>, FakeDto<TKey>>(entity),
                    Times.Once
                );

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
