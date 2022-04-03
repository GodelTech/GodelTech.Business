﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using GodelTech.Data.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using Xunit;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        public static IEnumerable<object[]> DeleteMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    0,
                    false
                },
                new object[]
                {
                    default(Guid),
                    2,
                    false
                },
                new object[]
                {
                    default(Guid),
                    1,
                    true
                },
                // int
                new object[]
                {
                    default(int),
                    0,
                    false
                },
                new object[]
                {
                    default(int),
                    2,
                    false
                },
                new object[]
                {
                    default(int),
                    1,
                    true
                },
                // string
                new object[]
                {
                    string.Empty,
                    0,
                    false
                },
                new object[]
                {
                    string.Empty,
                    2,
                    false
                },
                new object[]
                {
                    string.Empty,
                    1,
                    true
                }
            };

        [Theory]
        [MemberData(nameof(DeleteMemberData))]
        public async Task DeleteAsync_Success<TKey>(
            TKey defaultKey,
            int numberOfRowsAffected,
            bool expectedResult)
        {
            // Arrange
            Expression<Action<ILogger>> loggerExpressionDelete = x => x.Log(
                LogLevel.Information,
                0,
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() ==
                    $"Delete item: {defaultKey}"
                ),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
            );
            _mockLogger.Setup(loggerExpressionDelete);

            var entity = new FakeEntity<TKey>();

            var filterExpression = FilterExpressionExtensions.CreateIdFilterExpression<FakeEntity<TKey>, TKey>(defaultKey);

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.Get(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        )
                    )
                )
                .Returns(entity);

            mockRepository
                .Setup(
                    x => x.Delete(entity)
                );

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
            _mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true);
            _mockLogger.Setup(loggerExpressionSave);

            _mockUnitOfWork
                .Setup(
                    x => x.CommitAsync()
                )
                .ReturnsAsync(numberOfRowsAffected);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act
            var result = await businessService.DeleteAsync(defaultKey);

            // Assert
            Assert.NotNull(defaultKey);

            _mockLogger.Verify(loggerExpressionDelete, Times.Once);

            mockRepository
                .Verify(
                    x => x.Get(
                        It.Is<QueryParameters<FakeEntity<TKey>, TKey>>(
                            y => Lambda.Eq(
                                     y.Filter.Expression,
                                     filterExpression
                                 )
                                 && y.Sort == null
                                 && y.Page == null
                        )
                    ),
                    Times.Once
                );

            mockRepository
                .Verify(
                    x => x.Delete(entity),
                    Times.Once
                );

            _mockLogger.Verify(loggerExpressionSave, Times.Once);

            _mockUnitOfWork
                .Verify(
                    x => x.CommitAsync(),
                    Times.Once
                );

            Assert.Equal(expectedResult, result);
        }
    }
}