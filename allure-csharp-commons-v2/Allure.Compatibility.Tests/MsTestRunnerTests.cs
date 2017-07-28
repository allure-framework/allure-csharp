using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allure.Commons;
using System.Collections.Generic;
using System.Threading;

namespace Allure.Compatibility.Tests
{
    [TestClass]
    public class MsTestRunnerTests2
    {
        static AllureLifecycle cycle = AllureLifecycle.Instance;

        [TestMethod]
        public void PassingTest1()
        {
            var name = "PassingTest1";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(500);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }

        [TestMethod]
        public void PassingTest2()
        {
            var name = "PassingTest2";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(500);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }
    }
    [TestClass]
    public class MsTestRunnerTests1
    {
        static AllureLifecycle cycle = AllureLifecycle.Instance;

        [TestMethod]
        public void PassingTest1()
        {
            var name = "PassingTest1";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(2000);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }

        [TestMethod]
        public void PassingTest2()
        {
            var name = "PassingTest2";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new Commons.TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(2000);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }
    }
}
