using System;
using System.Collections.Generic;

namespace Allure.Sdk.Internal;

internal static class TypeFunctions
{
    internal static IEnumerable<Type> GetTypeClosure(Type type)
    {
        for (var t = type; t != null; t = t.BaseType)
        {
            yield return t;
        }

        foreach (var iFace in type.GetInterfaces())
        {
            yield return iFace;
        }
    }
}
