using System;
using Allure.Net.Commons;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureSeverityAttribute : Attribute, IAllureInfo
    {
        public AllureSeverityAttribute(SeverityLevel severity = SeverityLevel.normal)
        {
            Severity = severity;
        }

        internal SeverityLevel Severity { get; }
    }
}