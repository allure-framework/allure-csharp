using System;

namespace Allure.Xunit.Attributes
{
    public abstract class AllureAttribute : Attribute
    {
        internal bool Overwrite { get; set; }
    }
}