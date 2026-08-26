using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class MixedAttributeAndApiSteps
    {
        [Fact]
        public void TestMethod()
        {
            AttributeOuter();
            AllureApi.Step("Runtime outer", AttributeInner);
        }

        [AllureStep("Attribute outer")]
        static void AttributeOuter()
        {
            AllureApi.Step("Runtime inner");
        }

        [AllureStep("Attribute inner")]
        static void AttributeInner() { }
    }
}
