using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Allure.Net.Commons.Tests.AssertionHelpers;

class LinksEqualityComparer : IEqualityComparer<Link>
{
    public bool Equals(Link x, Link y) =>
        Equals(x.name, y.name) && Equals(x.type, y.type) && Equals(x.url, y.url);
    public int GetHashCode([DisallowNull] Link obj) =>
        HashCode.Combine(obj.name, obj.type, obj.url);
}
