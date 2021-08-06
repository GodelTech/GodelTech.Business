using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        public static IEnumerable<object[]> GetListMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    new Collection<FakeDto<Guid>>(),
                    new Collection<FakeDto<Guid>>()
                },
                new object[]
                {
                    default(Guid),
                    new Collection<FakeDto<Guid>>
                    {
                        new FakeDto<Guid>()
                    },
                    new Collection<FakeDto<Guid>>
                    {
                        new FakeDto<Guid>()
                    }
                },
                new object[]
                {
                    default(Guid),
                    new Collection<FakeDto<Guid>>
                    {
                        new FakeDto<Guid>
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            Name = "Test Name"
                        }
                    },
                    new Collection<FakeDto<Guid>>
                    {
                        new FakeDto<Guid>
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            Name = "Test Name"
                        }
                    }
                },
                // int
                new object[]
                {
                    default(int),
                    new Collection<FakeDto<int>>(),
                    new Collection<FakeDto<int>>()
                },
                new object[]
                {
                    default(int),
                    new Collection<FakeDto<int>>
                    {
                        new FakeDto<int>()
                    },
                    new Collection<FakeDto<int>>
                    {
                        new FakeDto<int>()
                    }
                },
                new object[]
                {
                    default(int),
                    new Collection<FakeDto<int>>
                    {
                        new FakeDto<int>
                        {
                            Id = 1,
                            Name = "Test Name"
                        }
                    },
                    new Collection<FakeDto<int>>
                    {
                        new FakeDto<int>
                        {
                            Id = 1,
                            Name = "Test Name"
                        }
                    }
                },
                // string
                new object[]
                {
                    string.Empty,
                    new Collection<FakeDto<string>>(),
                    new Collection<FakeDto<string>>()
                },
                new object[]
                {
                    string.Empty,
                    new Collection<FakeDto<string>>
                    {
                        new FakeDto<string>
                        {
                            Id = string.Empty
                        }
                    },
                    new Collection<FakeDto<string>>
                    {
                        new FakeDto<string>
                        {
                            Id = string.Empty
                        }
                    }
                },
                new object[]
                {
                    string.Empty,
                    new Collection<FakeDto<string>>
                    {
                        new FakeDto<string>
                        {
                            Id = "Test Id",
                            Name = "Test Name"
                        }
                    },
                    new Collection<FakeDto<string>>
                    {
                        new FakeDto<string>
                        {
                            Id = "Test Id",
                            Name = "Test Name"
                        }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(GetListMemberData))]
        public async Task GetListAsync_Success<TKey>(
            TKey defaultKey,
            Collection<FakeDto<TKey>> list,
            Collection<FakeDto<TKey>> expectedResult)
        {
            // Arrange
            var mockRepository = new Mock<IRepository<FakeEntity<TKey>, TKey>>(MockBehavior.Strict);
            mockRepository
                .Setup(
                    x => x.GetListAsync<FakeDto<TKey>>(null)
                )
                .ReturnsAsync(list);

            var businessService = new FakeBusinessService<TKey>(
                _mockUnitOfWork.Object,
                _ => mockRepository.Object,
                _mockBusinessMapper.Object,
                _mockLogger.Object
            );

            // Act
            var result = await businessService.GetListAsync();

            // Assert
            Assert.NotNull(defaultKey);

            mockRepository
                .Verify(
                    x => x.GetListAsync<FakeDto<TKey>>(null),
                    Times.Once
                );

            Assert.Equal(expectedResult, result, new FakeDtoEqualityComparer<TKey>());
        }
    }
}