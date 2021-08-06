using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace GodelTech.Business.Tests
{
    public class DtoTests
    {
        public static IEnumerable<object[]> DtoMemberData =>
            new Collection<object[]>
            {
                // Guid
                new object[]
                {
                    default(Guid),
                    new Guid("00000000-0000-0000-0000-000000000001")
                },
                // int
                new object[]
                {
                    default(int),
                    1
                },
                // string
                new object[]
                {
                    string.Empty,
                    "Test Id"
                }
            };

        [Theory]
        [MemberData(nameof(DtoMemberData))]
        public void Repositories_Success<TKey>(
            TKey defaultKey,
            TKey id)
        {
            // Arrange
            var dto = new Dto<TKey>();

            // Act
            dto.Id = id;

            // Assert
            Assert.NotNull(defaultKey);
            Assert.Equal(id, dto.Id);
        }
    }
}