using GodelTech.Data;
using System;

namespace GodelTech.Business.Tests.Fakes
{
    public class FakeEntity : IEntity<int>
    {
        public int Id { get; set; }

        public bool Equals(IEntity<int> x, IEntity<int> y)
        {
            throw new Exception("Equals is fake method!");
        }

        public int GetHashCode(IEntity<int> obj)
        {
            throw new Exception("GetHashCode is fake method!");
        }
    }
}
