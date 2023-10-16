using System;
using System.ComponentModel;
using Xunit;

namespace Allure.Xunit.Attributes
{
    [Obsolete("Use [Xunit.Theory] instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AllureXunitTheoryAttribute : TheoryAttribute
    {
    }
}