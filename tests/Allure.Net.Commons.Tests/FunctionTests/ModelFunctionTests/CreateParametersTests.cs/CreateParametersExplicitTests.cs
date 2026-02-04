using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests.CreateParametersTests.cs;

class CreateParametersExplicitTests
{
    static readonly Dictionary<Type, ITypeFormatter> emptyFormatters = [];

    [Test]
    public void EmptyNameSeqGivesEmptySeq()
    {
        var parameters = ModelFunctions.CreateParameters([], [new()], ["foo"], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void EmptyAttrSeqGivesEmptySeq()
    {
        var parameters = ModelFunctions.CreateParameters(["p1"], [], ["foo"], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void EmptyValueSeqGivesEmptySeq()
    {
        var parameters = ModelFunctions.CreateParameters(["p1"], [new()], [], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void NoAttribute()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["noAttribute"], [null], ["foo"], emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "noAttribute", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void EmptyAttribute()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["noEffect"],
            [new()],
            ["foo"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "noEffect", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void Ignored()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["ignored"],
            [new(){ Ignore = true }],
            ["foo"],
            emptyFormatters
        );

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void RenamedParameter()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["original"],
            [new(){ Name = "New name" }],
            ["foo"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "New name", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void MaskedParameter()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["masked"],
            [new(){ Mode = ParameterMode.Masked }],
            ["foo"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter
                {
                    name = "masked",
                    value = "\"foo\"",
                    mode = ParameterMode.Masked
                }
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void HiddenParameter()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["hidden"],
            [new(){ Mode = ParameterMode.Hidden }],
            ["foo"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter
                {
                    name = "hidden",
                    value = "\"foo\"",
                    mode = ParameterMode.Hidden
                }
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ExcludedParameter()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["excluded"],
            [new(){ Excluded = true }],
            ["foo"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter
                {
                    name = "excluded",
                    value = "\"foo\"",
                    excluded = true
                }
            ]).UsingPropertiesComparer()
        );
    }

    class StringFormatterStub : TypeFormatter<string>
    {
        public override string Format(string value) => "bar";
    }

    [Test]
    public void FormatterUsedIfMatched()
    {
        var formatters = new Dictionary<Type, ITypeFormatter>
        {
            { typeof(string), new StringFormatterStub() },
        };

        var parameters = ModelFunctions.CreateParameters(["foo"], [null], ["bar"], formatters);

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter { name = "foo", value = "bar" }
            ]).UsingPropertiesComparer()
        );
    }

    class StringFormatterDummy : TypeFormatter<string>
    {
        public override string Format(string value) => throw new NotImplementedException();
    }

    [Test]
    public void FormattingSkippedForIgnoredParameter()
    {
        var formatters = new Dictionary<Type, ITypeFormatter>
        {
            { typeof(string), new StringFormatterDummy() },
        };

        var parameters = ModelFunctions.CreateParameters(
            ["foo"],
            [new(){ Ignore = true }],
            ["bar"],
            formatters
        );

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void MultipleParameters()
    {
        var parameters = ModelFunctions.CreateParameters(
            ["noAttribute", "ignored", "masked", "excluded"],
            [
                null,
                new(){ Ignore = true },
                new(){ Mode = ParameterMode.Masked },
                new(){ Excluded = true },
            ],
            ["foo", "bar", "baz", "qux"],
            emptyFormatters
        );

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter
                {
                    name = "noAttribute",
                    value = "\"foo\"",
                },
                new Parameter
                {
                    name = "masked",
                    value = "\"baz\"",
                    mode = ParameterMode.Masked,
                },
                new Parameter
                {
                    name = "excluded",
                    value = "\"qux\"",
                    excluded = true,
                }
            ]).UsingPropertiesComparer()
        );
    }
}
