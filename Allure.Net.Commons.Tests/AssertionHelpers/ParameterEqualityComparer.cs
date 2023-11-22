using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Allure.Net.Commons.Tests.AssertionHelpers;

class ParameterEqualityComparer : IEqualityComparer<Parameter>
{
    public bool Equals(Parameter x, Parameter y) =>
        Equals(x.name, y.name)
            && Equals(x.value, y.value)
            && Equals(x.excluded, y.excluded)
            && Equals(x.mode, y.mode);
    public int GetHashCode([DisallowNull] Parameter obj) =>
        HashCode.Combine(obj.name, obj.value, obj.excluded, obj.mode);
}
