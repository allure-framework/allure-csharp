using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allure.Commons;
using System.Collections.Generic;
using System.Threading;

namespace Allure.Compatibility.Tests
{
    [TestClass]
    public class MsTestRunnerTests
    {
        static AllureLifecycle cycle = AllureLifecycle.Instance;

        [TestMethod]
        public void AllureMsTest_1()
        {
            var name = "PassingTest1";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });
            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }

        [TestMethod]
        public void AllureMsTest_2()
        {
            var name = "PassingTest2";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });
            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }
    }
}
