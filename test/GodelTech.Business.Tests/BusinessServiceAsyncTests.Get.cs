using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using GodelTech.Data.Extensions;
using Moq;
using Neleus.LambdaCompare;
using Xunit;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        public static IEnumerable<object[]> GetMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    new FakeDto<Guid>(),
                    new FakeDto<Guid>()
                },
                new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    new FakeDto<Guid>
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
                    new FakeDto<int>(),
                    new FakeDto<int>()
                },
                new object[]
                {
                    1,
                    new FakeDto<int>
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
                    new FakeDto<string>
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
                    new FakeDto<string>
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
        [MemberData(nameof(GetMemberData))]
        public async Task GetAsync_Success<TKey>(
            TKey defaultKey,
            FakeDto<TKey> item,
            FakeDto<TKey> expectedResult)
        {
            // Arrange
            var filterExpression = FilterExpressionExtensions.CreateIdFilterExpression<FakeEntity<TKey>, TKey>(defaultKey);

            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.GetAsync<FakeDto<TKey>>(
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
                .ReturnsAsync(item);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act
            var result = await businessService.GetAsync(defaultKey);

            // Assert
            mockRepository
                .Verify(
                    x => x.GetAsync<FakeDto<TKey>>(
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

            Assert.Equal(expectedResult, result, new FakeDtoEqualityComparer<TKey>());
        }
    }
}
