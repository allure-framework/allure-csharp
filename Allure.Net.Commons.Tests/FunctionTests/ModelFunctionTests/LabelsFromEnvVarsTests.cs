using NUnit.Framework;
using Allure.Net.Commons.Functions;
using System.Collections.Generic;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class LabelsFromEnvVarsTests
{
    Dictionary<string, string> env;

    [SetUp]
    public void SetUpEnvSource()
    {
        this.env = [];
        ModelFunctions.SetGetEnvironmentVariables(() => this.env);
    }

    [TearDown]
    public void RemoveEnvSource()
    {
        ModelFunctions.SetGetEnvironmentVariables(null);
    }

    [Test]
    public void ShouldBeEmtyIfNoVarMatches()
    {
        this.env["foo"] = "bar";
        this.env["bazbazbazbazbazbaz"] = "qux";

        Assert.That(ModelFunctions.EnumerateEnvironmentLabels(), Is.Empty);
    }

    [Test]
    public void ShouldIncludeLabelFromMatchingVar()
    {
        this.env["ALLURE_LABEL_foo"] = "bar";

        Assert.That(
            ModelFunctions.EnumerateEnvironmentLabels(),
            Is.EquivalentTo(new Label[]
            {
                new() { name = "foo", value = "bar" },
            }).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ShouldIncludeMultipleMatchingLabels()
    {
        this.env["ALLURE_LABEL_foo"] = "bar";
        this.env["ALLURE_LABEL_baz"] = "qux";
        this.env["ALLURE_LABEL_qut"] = "qtu";

        Assert.That(
            ModelFunctions.EnumerateEnvironmentLabels(),
            Is.EquivalentTo(new Label[]
            {
                new() { name = "foo", value = "bar" },
                new() { name = "baz", value = "qux" },
                new() { name = "qut", value = "qtu" },
            }).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ShouldPreserveCase()
    {
        this.env["ALLURE_LABEL_Foo"] = "bar";

        Assert.That(
            ModelFunctions.EnumerateEnvironmentLabels(),
            Is.EquivalentTo(new Label[]
            {
                new() { name = "Foo", value = "bar" },
            }).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ShouldIgnoreNullValues()
    {
        this.env["ALLURE_LABEL_foo"] = null;

        Assert.That(ModelFunctions.EnumerateEnvironmentLabels(), Is.Empty);
    }

    [Test]
    public void ShouldIgnoreEmptyValues()
    {
        this.env["ALLURE_LABEL_foo"] = "";

        Assert.That(ModelFunctions.EnumerateEnvironmentLabels(), Is.Empty);
    }

    [Test]
    public void ShouldIgnoreEmptyNames()
    {
        this.env["ALLURE_LABEL_"] = "bar";

        Assert.That(ModelFunctions.EnumerateEnvironmentLabels(), Is.Empty);
    }
}