using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class ParameterTests
{
    static ParameterTests()
    {
        var parameters = typeof(ParameterTests).GetMethod(nameof(Target)).GetParameters();

        NoAttribute = parameters[0];
        NoEffect = parameters[1];
        Ignored = parameters[2];
        Renamed = parameters[3];
        Masked = parameters[4];
        Hidden = parameters[5];
        Excluded = parameters[6];
    }

    public static void Target(
        int noAttribute,
        [AllureParameter] int noEffect,
        [AllureParameter(Ignore = true)] int ignored,
        [AllureParameter(Name = "New name")] int renamed,
        [AllureParameter(Mode = ParameterMode.Masked)] int masked,
        [AllureParameter(Mode = ParameterMode.Hidden)] int hidden,
        [AllureParameter(Excluded = true)] int excluded
    ) { }

    static ParameterInfo NoAttribute;
    static ParameterInfo NoEffect;
    static ParameterInfo Ignored;
    static ParameterInfo Renamed;
    static ParameterInfo Masked;
    static ParameterInfo Hidden;
    static ParameterInfo Excluded;
    static readonly Dictionary<Type, ITypeFormatter> emptyFormatters = [];

    [Test]
    public void EmptyParameterInfoSeqGivesEmptySeq()
    {
        var parameters = ModelFunctions.CreateParameters([], ["foo"], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void EmptyValueSeqGivesEmptySeq()
    {
        var parameters = ModelFunctions.CreateParameters([], ["foo"], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void PlainParameter()
    {
        var parameters = ModelFunctions.CreateParameters([NoAttribute], ["foo"], emptyFormatters);

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "noAttribute", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void ParameterWithEmptyAttribute()
    {
        var parameters = ModelFunctions.CreateParameters([NoEffect], ["foo"], emptyFormatters);

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "noEffect", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void IgnoredParameter()
    {
        var parameters = ModelFunctions.CreateParameters([Ignored], ["foo"], emptyFormatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void RenamedParameter()
    {
        var parameters = ModelFunctions.CreateParameters([Renamed], ["foo"], emptyFormatters);

        Assert.That(
            parameters,
            Is.EqualTo([new Parameter { name = "New name", value = "\"foo\"" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void MaskedParameter()
    {
        var parameters = ModelFunctions.CreateParameters([Masked], ["foo"], emptyFormatters);

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
        var parameters = ModelFunctions.CreateParameters([Hidden], ["foo"], emptyFormatters);

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
        var parameters = ModelFunctions.CreateParameters([Excluded], ["foo"], emptyFormatters);

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

        var parameters = ModelFunctions.CreateParameters([NoAttribute], ["foo"], formatters);

        Assert.That(
            parameters,
            Is.EqualTo([
                new Parameter
                {
                    name = "noAttribute",
                    value = "bar",
                }
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

        var parameters = ModelFunctions.CreateParameters([Ignored], ["foo"], formatters);

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void MultipleParameters()
    {
        var parameters = ModelFunctions.CreateParameters(
            [NoAttribute, Ignored, Masked, Excluded],
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
