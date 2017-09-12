using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Allure.Commons;
using System.Threading;

[assembly: Parallelizable(ParallelScope.All)]

namespace Allure.Compatibility.Tests.NUnit
{
    [TestFixture]
    [SingleThreaded]
    public class NUnitRunnerTests
    {
        static AllureLifecycle cycle = AllureLifecycle.Instance;

        [TestCase]
        public void AllureNUnit_1()
        {
            var name = "PassingTest1";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new TestResult { uuid = name, name = name, labels = labels });
            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }

        [TestCase]
        public void AllureNUnit_2()
        {
            var name = "PassingTest2";
            var labels = new List<Label> { Label.Thread() };
            cycle.StartTestCase(new TestResult { uuid = name, name = name, labels = labels });
            cycle.StopTestCase(x => x.status = Status.passed);
            cycle.WriteTestCase(name);
        }
    }
}
