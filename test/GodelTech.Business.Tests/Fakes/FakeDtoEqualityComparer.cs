using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GodelTech.Business.Tests.Fakes
{
    public class FakeDtoEqualityComparer<TKey> : IEqualityComparer<FakeDto<TKey>>
    {
        public bool Equals(FakeDto<TKey> x, FakeDto<TKey> y)
        {
            // Check whether the compared objects reference the same data
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            // Check whether the objects' properties are equal.
            return x.Id.Equals(y.Id)
                   && x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] FakeDto<TKey> obj)
        {
            // Check whether the object is null
            if (ReferenceEquals(obj, null)) return 0;

            // Calculate the hash code for the object.
            return obj.Id.GetHashCode()
                   ^ obj.Name.GetHashCode(StringComparison.InvariantCulture);
        }
    }
}
