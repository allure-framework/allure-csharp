using System;
using Allure.Net.Commons;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureFeatureAttribute : AllureTestCaseAttribute
    {
        public AllureFeatureAttribute(params string[] feature)
        {
            Features = feature;
        }

        private string[] Features { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            foreach (var feature in Features)
                testResult.labels.Add(Label.Feature(feature));
        }
    }
}