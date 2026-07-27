using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Sdk.Internal;

namespace Allure.Sdk.Functions;

public static class Exceptions
{
    public static IEnumerable<string> GetTypeNameClosure(Exception e) =>
        TypeFunctions.GetTypeClosure(e.GetType())
            .Select(static (t) => t.FullName);
}
