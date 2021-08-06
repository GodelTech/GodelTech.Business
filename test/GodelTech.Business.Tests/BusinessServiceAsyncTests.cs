using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GodelTech.Data;
using Microsoft.Extensions.Logging;
using Moq;

namespace GodelTech.Business.Tests
{
    public partial class BusinessServiceAsyncTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBusinessMapper> _mockBusinessMapper;
        private readonly Mock<ILogger> _mockLogger;

        public BusinessServiceAsyncTests()
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
    }
}