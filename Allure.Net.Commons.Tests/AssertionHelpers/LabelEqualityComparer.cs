using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Allure.Net.Commons.Tests.AssertionHelpers;

class LabelEqualityComparer : IEqualityComparer<Label>
{
    public bool Equals(Label x, Label y) => 
        Equals(x.name, y.name) && Equals(x.value, y.value);
    public int GetHashCode([DisallowNull] Label obj) =>
        HashCode.Combine(obj.name, obj.value);
}
