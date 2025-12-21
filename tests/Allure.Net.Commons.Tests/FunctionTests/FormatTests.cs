using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests;
class FormatTests
{
    [TestCase(null, "null")]
    [TestCase(0, "0")]
    [TestCase(1, "1")]
    [TestCase(-1, "-1")]
    [TestCase(1.5, "1.5")]
    [TestCase(1.5D, "1.5")]
    [TestCase(1.5f, "1.5")]
    [TestCase(1L, "1")]
    [TestCase("", "\"\"")]
    [TestCase("text", "\"text\"")]
    [TestCase("text with \"quotes\"", "\"text with \\\"quotes\\\"\"")]
    [TestCase(new object[] {}, "[]")]
    [TestCase(new [] { 1, 2 }, "[1,2]")]
    public void TestFormat(object value, string expected)
    {
        Assert.That(
            FormatFunctions.Format(value),
            Is.EqualTo(expected)
        );
    }

    [Test]
    public void TestDateTimeFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new DateTime(2023, 1, 31, 10, 35, 45, 250)
            ),
            Is.EqualTo("\"2023-01-31T10:35:45.25\"")
        );
    }

    [Test]
    public void TestDateOnlyFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new DateOnly(2023, 1, 31)
            ),
            Is.EqualTo("\"2023-01-31\"")
        );
    }

    [Test]
    public void TestTimeOnlyFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new TimeOnly(10, 30, 45, 250)
            ),
            Is.EqualTo("\"10:30:45.25\"")
        );
    }

    [Test]
    public void TestDateTimeOffsetFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new DateTimeOffset(2023, 1, 31, 10, 30, 45, 250, TimeSpan.FromHours(7))
            ),
            Is.EqualTo("\"2023-01-31T10:30:45.25+07:00\"")
        );
    }

    [Test]
    public void TestTimeSpanFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new TimeSpan(2, 10, 30, 45, 250)
            ),
            Is.EqualTo("\"2.10:30:45.2500000\"")
        );
    }

    [Test]
    public void TestUserTypeFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new { name = "my-name", value = "my-value" }
            ),
            Is.EqualTo("{\"name\":\"my-name\",\"value\":\"my-value\"}")
        );
    }

    [Test]
    public void TestTupleFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                ("item 1", "item 2")
            ),
            Is.EqualTo("{\"Item1\":\"item 1\",\"Item2\":\"item 2\"}")
        );
    }

    [Test]
    public void TestSequenceFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                Enumerable.Range(1, 3)
            ),
            Is.EqualTo("[1,2,3]")
        );
    }

    [Test]
    public void TestMappingFormat()
    {
        Assert.That(
            FormatFunctions.Format(
                new Dictionary<int, string> { { 1, "a" }, { 2, "b" } }
            ),
            Is.EqualTo("{\"1\":\"a\",\"2\":\"b\"}")
        );
    }

    class MyClass { }
    class MyFormatter : ITypeFormatter
    {
        public string Format(object value)
            => "my-string";
    }

    [Test]
    public void TestCustomFormatter()
    {
        Assert.That(
            FormatFunctions.Format(
                new MyClass(),
                new Dictionary<Type, ITypeFormatter>() 
                {
                    { typeof(MyClass), new MyFormatter() }
                }
            ),
            Is.EqualTo("my-string")
        );
    }
}
