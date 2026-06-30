using System;
using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using Allure.Xunit;
using Xunit;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.XunitV3.Examples;


[AllureName("Lorem")]
public class MyTests
{
    [Fact]
    static void MyTest2()
    {
    }
}
