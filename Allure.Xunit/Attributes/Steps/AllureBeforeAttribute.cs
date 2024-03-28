using System;
using System.ComponentModel;
using Allure.Net.Commons.Steps;
using Allure.Xunit;

namespace Allure.Xunit.Attributes.Steps
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}

namespace Allure.XUnit.Attributes.Steps
{
    [Obsolete(AllureXunitHelper.NS_OBSOLETE_MSG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}