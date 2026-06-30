using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Tests.Comparers;

public class ParameterComparer : IEqualityComparer<Parameter>
{
    public bool Equals(Parameter x, Parameter y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return Equals(x.name, y.name)
            && Equals(x.value, y.value)
            && Equals(x.mode, y.mode)
            && Equals(x.excluded, y.excluded);
    }

    public int GetHashCode([DisallowNull] Parameter obj)
    {
        return HashCode.Combine(obj.name, obj.value, obj.mode, obj.excluded);
    }

    public static ParameterComparer Instance { get; } = new();
}