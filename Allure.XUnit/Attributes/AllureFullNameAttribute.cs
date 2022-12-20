using System;
using Allure.Net.Commons;
using Allure.XUnit;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureFullNameAttribute : Attribute, IAllureInfo
    {
        public AllureFullNameAttribute(string fullName)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }
        
        internal string FullName { get; }
        
        internal void UpdateTestResult(TestResult testResult)
        {
            testResult.fullName = AllureNameFormatter.Format(FullName, testResult);
        }
    }
}