using System;
using Xunit;
using Xunit.Sdk;

namespace Allure.Xunit.Attributes
{
    [Obsolete("Use [Xunit.Theory] instead")]
    public class AllureXunitTheoryAttribute : TheoryAttribute
    {
    }
}