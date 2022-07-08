using System.Collections.Generic;
using Allure.XUnit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.Xunit
{
    public class AllureXunitDiscover : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public AllureXunitDiscover(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var testCase = new AllureXunitTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                TestMethodDisplayOptions.None, testMethod);
            return new[] { testCase };
        }
    }
}