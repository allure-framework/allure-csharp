using System.Collections.Generic;
using System.Linq;

namespace Allure.Build.SourceGenerators.Samples;

sealed class RegistrySourceComparer : IEqualityComparer<RegistrySource>
{
    public bool Equals(RegistrySource x, RegistrySource y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Namespace == y.Namespace && x.Entries.SequenceEqual(y.Entries);
    }

    public int GetHashCode(RegistrySource obj)
    {
        unchecked
        {
            int hash = 17;

            var @namespace = obj.Namespace;
            hash = hash * 31 + (@namespace?.GetHashCode() ?? 0);

            foreach (var entry in obj.Entries)
            {
                hash = hash * 31 + (entry?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    public static RegistrySourceComparer Instance { get; } = new();
}
