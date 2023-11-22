using System;
using System.Collections.Generic;
using Allure.Net.Commons.Steps;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;
class StepAspectHelperTests
{
    class TargetClass
    {
#pragma warning disable CA1822, IDE0060
        public void TargetMethod(int p1, string p2) { }
        public void TargetMethod2(int p1, string p2, DateTime p3) { }
#pragma warning restore CA1822, IDE0060
    }

    class MyIntFormatter : TypeFormatter<int>
    {
        public override string Format(int value) => "int-value";
    }

    class MyStringFormatter : TypeFormatter<string>
    {
        public override string Format(string value) => "string-value";
    }

    class MyDateTimeFormatter : TypeFormatter<DateTime>
    {
        public override string Format(DateTime value) => "datetime-value";
    }

    [Test]
    public void StepParametersAreSerialized()
    {
        var method = typeof(TargetClass).GetMethod(nameof(TargetClass.TargetMethod));
        var parameters = AllureStepParameterHelper.GetStepParameters(
            method,
            new object[] { 1, "a" },
            new Dictionary<Type, ITypeFormatter>()
        );

        Assert.That(parameters, Has.Count.EqualTo(2));
        Assert.That(parameters[0].name, Is.EqualTo("p1"));
        Assert.That(parameters[0].value, Is.EqualTo("1"));
        Assert.That(parameters[1].name, Is.EqualTo("p2"));
        Assert.That(parameters[1].value, Is.EqualTo("\"a\""));
    }

    [Test]
    public void LifecycleTypeFormattersAreUsedWhenSerializingStepParameterValues()
    {
        var method = typeof(TargetClass).GetMethod(nameof(TargetClass.TargetMethod));
        var parameters = AllureStepParameterHelper.GetStepParameters(
            method,
            new object[] { 1, "a" },
            new Dictionary<Type, ITypeFormatter>()
            {
                { typeof(int), new MyIntFormatter() },
                { typeof(string), new MyStringFormatter() }
            }
        );

        Assert.That(parameters, Has.Count.EqualTo(2));
        Assert.That(parameters[0].name, Is.EqualTo("p1"));
        Assert.That(parameters[0].value, Is.EqualTo("int-value"));
        Assert.That(parameters[1].name, Is.EqualTo("p2"));
        Assert.That(parameters[1].value, Is.EqualTo("string-value"));
    }

    [TestCase("{0}", "1")]
    [TestCase("{1}", "\"a\"")]
    [TestCase("{2}", "\"2023-01-31T10:30:45.25\"")]
    [TestCase("{p1}", "1")]
    [TestCase("{p2}", "\"a\"")]
    [TestCase("{p3}", "\"2023-01-31T10:30:45.25\"")]
    [TestCase("{p3.Year}", "2023")]
    public void TestGetStepNameWithIndexOrNameInterpolation(string nameFormat, string expectedValue)
    {
        var method = typeof(TargetClass).GetMethod(nameof(TargetClass.TargetMethod2));
        Assert.That(
            AllureStepParameterHelper.GetStepName(
                nameFormat,
                method,
                new object[]
                {
                    1,
                    "a",
                    new DateTime(2023, 01, 31, 10, 30, 45, 250)
                },
                new Dictionary<Type, ITypeFormatter>()
            ),
            Is.EqualTo(expectedValue)
        );
    }

    [TestCase("{0}", "int-value")]
    [TestCase("{1}", "string-value")]
    [TestCase("{2}", "datetime-value")]
    [TestCase("{p1}", "int-value")]
    [TestCase("{p2}", "string-value")]
    [TestCase("{p3}", "datetime-value")]
    [TestCase("{p3.Year}", "int-value")]
    public void TestGetStepNameUsesProvidedFormatters(string nameFormat, string expectedValue)
    {
        var method = typeof(TargetClass).GetMethod(nameof(TargetClass.TargetMethod2));
        Assert.That(
            AllureStepParameterHelper.GetStepName(
                nameFormat,
                method,
                new object[]
                {
                    1,
                    "a",
                    new DateTime(2023, 01, 31, 10, 30, 45, 250)
                },
                new Dictionary<Type, ITypeFormatter>
                {
                    { typeof(int), new MyIntFormatter() },
                    { typeof(string), new MyStringFormatter() },
                    { typeof(DateTime), new MyDateTimeFormatter() }
                }
            ),
            Is.EqualTo(expectedValue)
        );
    }
}
