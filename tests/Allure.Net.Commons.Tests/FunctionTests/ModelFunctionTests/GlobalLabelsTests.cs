using NUnit.Framework;
using Allure.Net.Commons.Functions;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons.Configuration;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class GlobalLabelsTests
{
    AllureConfiguration config;

    [SetUp]
    public void SetUpEnvSource()
    {
        this.config = new();
        ModelFunctions.Config = this.config;
    }

    [TearDown]
    public void RemoveEnvSource()
    {
        ModelFunctions.Config = null;
    }

    [Test]
    public void ShouldNotThrowIfGlobalLabelsNull()
    {
        this.config.GlobalLabels = null;

        Assert.That(
            () => ModelFunctions.EnumerateGlobalLabels().ToList(),
            Throws.Nothing
        );
    }

    [Test]
    public void ShouldIncludeGlobalLabel()
    {
        this.config.GlobalLabels["foo"] = "bar";
        this.config.GlobalLabels["baz"] = "qux";

        Assert.That(
            ModelFunctions.EnumerateGlobalLabels(),
            Is.EquivalentTo(new Label[]
            {
                new() { name = "foo", value = "bar" },
                new() { name = "baz", value = "qux" },
            }).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ShouldIgnoreNullValues()
    {
        this.config.GlobalLabels["foo"] = null;

        Assert.That(ModelFunctions.EnumerateGlobalLabels(), Is.Empty);
    }

    [Test]
    public void ShouldIgnoreEmptyValues()
    {
        this.config.GlobalLabels["foo"] = "";

        Assert.That(ModelFunctions.EnumerateGlobalLabels(), Is.Empty);
    }

    [Test]
    public void ShouldIgnoreEmptyNames()
    {
        this.config.GlobalLabels[""] = "foo";

        Assert.That(ModelFunctions.EnumerateGlobalLabels(), Is.Empty);
    }
}