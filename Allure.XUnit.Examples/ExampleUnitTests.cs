using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Xunit;
using Allure.Xunit.Attributes;
using Allure.XUnit.Attributes.Steps;
using Xunit;

namespace Allure.XUnit.Examples
{
    [AllureOwner("Tinkoff")]
    [AllureTag("TAG-ALL")]
    [AllureEpic("TestEpic")]
    [AllureParentSuite("AllTests")]
    [AllureSuite("Suite Name")]
    [AllureSubSuite("Subsuite Name")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureLink("Google", "https://google.com")]
    [AllureLink("Tinkoff", "https://tinkoff.ru")]
    public class ExampleUnitTests : IDisposable
    {
        public void Dispose()
        {
        }

        public ExampleUnitTests()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
        }

        [Fact(DisplayName = "Test that 1 is not equals 1")]
        [AllureDescription("My long test description; Lorem ipsum dolor sit amet.")]
        [AllureFeature("qwerty", "123")]
        [AllureTag("TAG-1")]
        [AllureIssue("ISSUE-1")]
        public void Test1()
        {
            Steps.Step("Nested step", () => { });
            Assert.True(1 != 1);
        }


        [Fact(DisplayName = "Test that 1 is equals 1")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task Test2()
        {
            await Task.Delay(0);
            Assert.True(1 == 1);
            Attachments.File("allureConfig", @"./allureConfig.json");
        }

        [Fact(DisplayName = "Another Test")]
        public void Test3()
        {
            Assert.Empty(new List<int>() {1, 2, 3});
        }

        [Fact]
        [AllureTag(new[] {"TAG-8", "TAG-9", "TAG-10"}, true)]
        public void TestMultipleTagsWithOverwrite()
        {
            Assert.True(!false);
        }

        [Fact]
        public static void StaticTest()
        {
            Assert.True(true);
        }

        [Fact(DisplayName = "Test mapped to existing test case #1 in allure")]
        [AllureId("1")]
        public void TestAllureIdMapping()
        {
            Assert.True(true);
        }
    }
}
