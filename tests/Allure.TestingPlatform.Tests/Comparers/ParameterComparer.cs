using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.TestingPlatform.Tests.Comparers;

public class ParameterComparer : IEqualityComparer<Parameter>
{
    public bool Equals(Parameter x, Parameter y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return Equals(x.Name, y.Name)
            && Equals(x.Value, y.Value)
            && Equals(x.Mode, y.Mode)
            && Equals(x.Excluded, y.Excluded);
    }

    public int GetHashCode([DisallowNull] Parameter obj)
    {
        return HashCode.Combine(obj.Name, obj.Value, obj.Mode, obj.Excluded);
    }

    public static ParameterComparer Instance { get; } = new();
}