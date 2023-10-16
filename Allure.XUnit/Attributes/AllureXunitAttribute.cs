using System;
using System.ComponentModel;
using Xunit;

namespace Allure.Xunit.Attributes
{
    [Obsolete("Use [Xunit.Fact] instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AllureXunitAttribute : FactAttribute
    {
    }
}