using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class ParameterTests : AllureApiTestFixture
{
    class TypeFormatterTarget { }
    class TypeFormatterStub : TypeFormatter<TypeFormatterTarget>
    {
        public override string Format(TypeFormatterTarget value) =>
            "serialized target";
    }

    [Test]
    public void NameValueOnly()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", "value");

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"" }
        );
    }

    [Test]
    public void TypeFormattersAreUsedForSerialization()
    {
        this.lifecycle.AddTypeFormatter(new TypeFormatterStub());
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new TypeFormatterTarget());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized target" }
        );
    }

    [TestCase(ParameterMode.Default)]
    [TestCase(ParameterMode.Masked)]
    [TestCase(ParameterMode.Hidden)]
    public void NameValueMode(ParameterMode mode)
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", "value", mode);

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"", mode = mode }
        );
    }

    [TestCase(true)]
    [TestCase(false)]
    public void NameValueExcluded(bool excluded)
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", "value", excluded: excluded);

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"", excluded = excluded }
        );
    }

    [Test]
    public void NameValueModeExcluded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter(
            "name",
            "value",
            mode: ParameterMode.Masked,
            excluded: true
        );

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value\"",
                mode = ParameterMode.Masked,
                excluded = true
            }
        );
    }

    [Test]
    public void ParameterInstance()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter(new()
        {
            name = "name",
            value = "value",
            mode = ParameterMode.Hidden,
            excluded = true
        });

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "value",
                mode = ParameterMode.Hidden,
                excluded = true
            }
        );
    }
}
