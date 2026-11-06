using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.AllureFacadeTests;

class ParameterTests : AllureApiTestFixture
{
    class TypeFormatterTarget { }
    class TypeFormatterStub : TypeFormatter<TypeFormatterTarget>
    {
        public override string Format(TypeFormatterTarget value) =>
            "serialized target";
    }

    interface ITargetInterface { }

    class ImplementsInterface : ITargetInterface { }

    class ClosedGenericTarget : List<int> { }

    class DerivedFromGenericBase : GenericBase<int> { }

    class GenericBase<T> { }

    class FixedResultFormatter : ITypeFormatter
    {
        private readonly string result;

        public FixedResultFormatter(string result) => this.result = result;

        public string Format(object value) => this.result;
    }

    [Test]
    public void TypeFormatterMatchesImplementedInterface()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(ITargetInterface),
            new FixedResultFormatter("serialized via interface")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new ImplementsInterface());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via interface" }
        );
    }

    [Test]
    public void TypeFormatterMatchesGenericInterfaceDefinition()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(IEnumerable<>),
            new FixedResultFormatter("serialized via generic interface")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new List<int> { 1, 2, 3 });

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via generic interface" }
        );
    }

    [Test]
    public void TypeFormatterMatchesBaseClass()
    {
        this.lifecycle.AddTypeFormatter(
            new TypeFormatterStub2()
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new DerivedFromTarget());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized base" }
        );
    }

    class DerivedFromTarget : TypeFormatterTarget { }
    class TypeFormatterStub2 : TypeFormatter<TypeFormatterTarget>
    {
        public override string Format(TypeFormatterTarget value) => "serialized base";
    }

    [Test]
    public void TypeFormatterMatchesGenericBaseClassDefinition()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(GenericBase<>),
            new FixedResultFormatter("serialized via generic base")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new DerivedFromGenericBase());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via generic base" }
        );
    }

    [Test]
    public void TypeFormatterMatchesOpenGenericTypeDefinition()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(List<>),
            new FixedResultFormatter("serialized via open generic")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new List<int> { 1, 2, 3 });

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via open generic" }
        );
    }

    [Test]
    public void ExactTypeFormatterTakesPrecedenceOverInterfaceFormatter()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(ITargetInterface),
            new FixedResultFormatter("serialized via interface")
        );
        this.lifecycle.AddTypeFormatter(
            typeof(ImplementsInterface),
            new FixedResultFormatter("serialized via exact type")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new ImplementsInterface());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via exact type" }
        );
    }

    [Test]
    public void FormatterResolutionCacheIsInvalidatedByNewRegistrations()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(ITargetInterface),
            new FixedResultFormatter("serialized via interface")
        );
        // Trigger resolution and caching for ImplementsInterface before the
        // more specific formatter is registered.
        _ = this.lifecycle.TypeFormatters.TryGetValue(
            typeof(ImplementsInterface), out _
        );

        this.lifecycle.AddTypeFormatter(
            typeof(ImplementsInterface),
            new FixedResultFormatter("serialized via exact type")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", new ImplementsInterface());

        this.AssertParameters(
            new Parameter() { name = "name", value = "serialized via exact type" }
        );
    }

    [Test]
    public void ContainsKeyAndIndexerAgreeWithTryGetValueForResolvedTypes()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(ITargetInterface),
            new FixedResultFormatter("serialized via interface")
        );

        Assert.Multiple(() =>
        {
            Assert.That(
                this.lifecycle.TypeFormatters.ContainsKey(typeof(ImplementsInterface)),
                Is.True
            );
            Assert.That(
                this.lifecycle.TypeFormatters[typeof(ImplementsInterface)].Format(null),
                Is.EqualTo("serialized via interface")
            );
        });
    }

    [Test]
    public void TypesWithoutMatchingFormatterFallBackToDefaultSerialization()
    {
        this.lifecycle.AddTypeFormatter(
            typeof(ITargetInterface),
            new FixedResultFormatter("serialized via interface")
        );
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTestParameter("name", "plain value");

        this.AssertParameters(
            new Parameter() { name = "name", value = "\"plain value\"" }
        );
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
