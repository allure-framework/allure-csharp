using System;
using Xunit;
using Xunit.Sdk;

namespace Allure.Xunit.Attributes
{
    [Obsolete("Use [Xunit.Fact] instead")]
    public class AllureXunitAttribute : FactAttribute
    {
    }
}