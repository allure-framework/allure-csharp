using System.Collections.Generic;
using System.Linq;
using Allure.XUnit;
using Xunit.Abstractions;
using Xunit.Sdk;
using TestMethodDisplayOptions = Xunit.Sdk.TestMethodDisplayOptions;

namespace Allure.Xunit
{
    public class AllureXunitTheoryDiscover : TheoryDiscoverer
    {
        public AllureXunitTheoryDiscover(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var testCases = base.Discover(discoveryOptions, testMethod, factAttribute);

            foreach (var item in testCases)
            {
               var dataAttribute = item.TestMethod.Method
                   .GetCustomAttributes(typeof(DataAttribute)).FirstOrDefault() as IReflectionAttributeInfo;

               if (dataAttribute?.Attribute is DataAttribute memberDataAttribute && item.TestMethodArguments is null)
               {
                   var argumentSets = memberDataAttribute
                       .GetData(item.TestMethod.Method.ToRuntimeMethod());

                   foreach (var arguments in argumentSets)
                   {
                       var testCase  = new AllureXunitTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                           TestMethodDisplayOptions.None, testMethod, arguments);
                       yield return testCase;
                   }
               }
               else
               {
                   var testCase = new AllureXunitTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                       TestMethodDisplayOptions.None, testMethod, item.TestMethodArguments);
                   yield return testCase;
               }
            }
        }
    }
}