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

        AllureApi.SetTestParameter("name", "value");

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"" }
        );
    }

    [Test]
    public void TypeFormattersAreUsedForSerialization()
    {
        this.lifecycle.AddTypeFormatter(new TypeFormatterStub());
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetTestParameter("name", new TypeFormatterTarget());

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

        AllureApi.SetTestParameter("name", "value", mode);

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"", mode = mode }
        );
    }

    [TestCase(true)]
    [TestCase(false)]
    public void NameValueExcluded(bool excluded)
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetTestParameter("name", "value", excluded: excluded);

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"value\"", excluded = excluded }
        );
    }

    [Test]
    public void NameValueModeExcluded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetTestParameter(
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

        AllureApi.SetTestParameter(new()
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

    [Test]
    public void ExistingParameterValueOnlyOverwritten()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value1", ParameterMode.Masked, true);

        AllureApi.SetTestParameter("name", "value2");

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value2\"",
                mode = ParameterMode.Masked,
                excluded = true
            }
        );
    }

    [Test]
    public void ExistingParameterValueModeOverwritten()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value1", ParameterMode.Masked, true);

        AllureApi.SetTestParameter("name", "value2", ParameterMode.Hidden);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value2\"",
                mode = ParameterMode.Hidden,
                excluded = true
            }
        );
    }

    [Test]
    public void ExistingParameterValueExcludeOverwritten()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value1", ParameterMode.Masked, false);

        AllureApi.SetTestParameter("name", "value2", true);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value2\"",
                mode = ParameterMode.Masked,
                excluded = true
            }
        );
    }

    [Test]
    public void ExistingParameterFullyOverwritten()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value1", ParameterMode.Masked, false);

        AllureApi.SetTestParameter("name", "value2", ParameterMode.Hidden, true);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value2\"",
                mode = ParameterMode.Hidden,
                excluded = true
            }
        );
    }

    [Test]
    public void ExistingParameterOverwrittenByInstance()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value1", ParameterMode.Masked, true);

        AllureApi.SetTestParameter(new()
        {
            name = "name",
            value = "\"value2\"",
            mode = ParameterMode.Hidden
        });

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value2\"",
                mode = ParameterMode.Hidden
            }
        );
    }

    [Test]
    public void UpdateParameterModeAndExcluded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value");

        AllureApi.UpdateTestParameter("name", ParameterMode.Hidden, true);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value\"",
                mode = ParameterMode.Hidden,
                excluded = true
            }
        );
    }

    [Test]
    public void UpdateParameterModeOnly()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value");

        AllureApi.UpdateTestParameter("name", ParameterMode.Hidden);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value\"",
                mode = ParameterMode.Hidden
            }
        );
    }

    [Test]
    public void UpdateParameterExcludedOnly()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetTestParameter("name", "value");

        AllureApi.UpdateTestParameter("name", excluded: true);

        this.AssertParameters(
            new Parameter()
            {
                name = "name",
                value = "\"value\"",
                excluded = true
            }
        );
    }

    [Test]
    public void UpdateParameterThrowsIfNoParameter()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.UpdateTestParameter("name"),
            Throws.InvalidOperationException.With.Message.EqualTo(
                "The parameter 'name' doesn't exist in the current test."
            )
        );
    }
}
