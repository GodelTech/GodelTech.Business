using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GodelTech.Business.Tests.Fakes;
using GodelTech.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GodelTech.Business.Tests
{
    public class BusinessServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBusinessMapper> _mockBusinessMapper;
        private readonly Mock<ILogger> _mockLogger;

        public BusinessServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _mockBusinessMapper = new Mock<IBusinessMapper>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger>(MockBehavior.Strict);
        }

        public static IEnumerable<object[]> TypesMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid)
                },
                // int
                new object[]
                {
                    default(int)
                },
                // string
                new object[]
                {
                    string.Empty
                }
            };

        [Theory]
        [MemberData(nameof(TypesMemberData))]
        public void Constructor_ThrowsArgumentNullException<TKey>(
            TKey defaultKey)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FakeBusinessService<TKey>(
                    _mockUnitOfWork.Object,
                    null,
                    _mockBusinessMapper.Object,
                    _mockLogger.Object
                )
            );
            Assert.Equal("repositorySelector", exception.ParamName);

            Assert.NotNull(defaultKey);
        }
    }
}
