using System;
using Allure.Net.Commons;
using Allure.XUnit;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureNameAttribute : Attribute, IAllureInfo
    {
        public AllureNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        internal string Name { get; }
        
        internal void UpdateTestResult(TestResult testResult)
        {
            testResult.name = AllureNameFormatter.Format(Name, testResult);
        }
    }
}