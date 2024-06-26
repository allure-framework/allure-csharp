﻿using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureNameAttribute : AllureTestCaseAttribute
    {
        public AllureNameAttribute(string name)
        {
            TestName = name;
        }

        private string TestName { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.name = TestName;
        }
    }
}