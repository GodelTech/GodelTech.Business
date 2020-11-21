using System.Diagnostics.CodeAnalysis;

namespace GodelTech.Business.Tests.Fakes
{
    [ExcludeFromCodeCoverage]
    public class FakeDto : IDto<int>
    {
        public int Id { get; set; }
    }
}
