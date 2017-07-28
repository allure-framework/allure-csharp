using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Allure.Commons;
using System.Threading;

[assembly: Parallelizable(ParallelScope.All)]

namespace Allure.Compatibility.Tests
{
    [TestFixture]
    [SingleThreaded]
    public class NUnitRunnerTests
    {
        static AllureLifecycle cycle = AllureLifecycle.Instance;

        [TestCase]
        public void PassingTest1()
        {
            var name = "PassingTest1";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(500);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }

        [TestCase]
        public void PassingTest2()
        {
            var name = "PassingTest2";
            var labels = new List<Label> { Label.Thread() };

            cycle.StartTestCase(new TestResult { uuid = name, name = name, labels = labels });

            Thread.Sleep(500);

            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }
    }
}
