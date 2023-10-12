using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.ModelExtensionTests;

class TestResultExtensionTests
{
    [Test]
    public void AddParametersWithDefaultFormatter()
    {
        var testResult = new TestResult();

        testResult.AddParameter("name", "value", new Dictionary<Type, ITypeFormatter>());

        Assert.That(testResult.parameters, Has.Count.EqualTo(1));
        Assert.That(
            testResult.parameters[0].name,
            Is.EqualTo("name")
        );
        Assert.That(
            testResult.parameters[0].value,
            Is.EqualTo("\"value\"")
        );
    }

    class CustomStringFormatter : TypeFormatter<string>
    {
        public override string Format(string value) => "other-value";
    }

    [Test]
    public void AddParametersWithCustomFormatter()
    {
        var testResult = new TestResult();

        testResult.AddParameter(
            "name",
            "value",
            new Dictionary<Type, ITypeFormatter>
            {
                { typeof(string), new CustomStringFormatter() }
            }
        );

        Assert.That(testResult.parameters, Has.Count.EqualTo(1));
        Assert.That(
            testResult.parameters[0].name,
            Is.EqualTo("name")
        );
        Assert.That(
            testResult.parameters[0].value,
            Is.EqualTo("other-value")
        );
    }
}
