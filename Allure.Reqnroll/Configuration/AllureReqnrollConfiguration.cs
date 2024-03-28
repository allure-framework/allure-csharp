using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace Allure.ReqnrollPlugin.Configuration;

record class AllureReqnrollConfiguration(
    GherkinPatterns GherkinPatterns,
    Type RunnerType,
    List<string> IgnoreExceptions
)
{
    const string WELL_KNOWN_IGNORE_EXC_TYPES_NUNIT
        = "NUnit.Framework.IgnoreException";
    const string WELL_KNOWN_IGNORE_EXC_TYPES_XUNIT
        = "Xunit.SkipException";

    static readonly Lazy<AllureReqnrollConfiguration> deferedConfig
        = new(ParseCurrentConfig);

    public static AllureReqnrollConfiguration CurrentConfig
    {
        get => deferedConfig.Value;
    }

    public AllureReqnrollConfiguration() : this(
        new GherkinPatterns(),
        typeof(TestRunner),
        new()
        {
            WELL_KNOWN_IGNORE_EXC_TYPES_NUNIT,
            WELL_KNOWN_IGNORE_EXC_TYPES_XUNIT
        }
    )
    {
    }

    internal static AllureReqnrollConfiguration ParseConfig(
        string jsonSerializedConfig
    ) =>
        JObject.Parse(jsonSerializedConfig)["allure"]
            ?.ToObject<AllureReqnrollConfiguration>(
                JsonSerializer.Create(new()
                {
                    Converters = new List<JsonConverter>
                    {
                        new WholeLineRegexConverter()
                    },
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                })
            )
            ?? new AllureReqnrollConfiguration();

    static AllureReqnrollConfiguration ParseCurrentConfig() =>
        ParseConfig(AllureLifecycle.Instance.JsonConfiguration);
}

record class GherkinPatterns(
    DataTableToArgsConversionOptions StepArguments,
    GherkinTagToGroupingLabelConversionOptions Grouping,
    GherkinTagToMetadataLabelConversionOptions Metadata,
    GherkinTagToLinkConversionOptions Links
)
{
    internal const RegexOptions DEFAULT_PATTERN_OPTS =
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    public GherkinPatterns() : this(
        new(),
        new(),
        new(),
        new()
    ) { }
}

record class DataTableToArgsConversionOptions(
    bool CreateFromDataTables,
    Regex? NameColumn,
    Regex? ValueColumn
)
{
    public DataTableToArgsConversionOptions(): this(false, null, null)
    {
    }
}

record class GherkinTagToGroupingLabelConversionOptions(
    GherkinTagToSuiteConversionOptions Suites,
    GherkinTagToBddConversionOptions Behaviors
)
{
    public GherkinTagToGroupingLabelConversionOptions() : this(new(), new())
    {
    }
}

record class GherkinTagToMetadataLabelConversionOptions(
    Regex? Owner,
    Regex? Severity,
    Regex? Label
)
{
    public GherkinTagToMetadataLabelConversionOptions() : this(
        new(DefaultPatterns.OWNER, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.SEVERITY, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.LABEL, GherkinPatterns.DEFAULT_PATTERN_OPTS)
    )
    {
    }
}

record class GherkinTagToLinkConversionOptions(
    Regex? Link,
    Regex? Issue,
    Regex? Tms
)
{
    public GherkinTagToLinkConversionOptions() : this(
        new(DefaultPatterns.LINK, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.ISSUE, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.TMS, GherkinPatterns.DEFAULT_PATTERN_OPTS)
    )
    {
    }
}


record class GherkinTagToSuiteConversionOptions(
    Regex? ParentSuite,
    Regex? Suite,
    Regex? SubSuite
)
{
    public GherkinTagToSuiteConversionOptions() : this(
        new(DefaultPatterns.PARENT_SUITE, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.SUITE, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.SUB_SUITE, GherkinPatterns.DEFAULT_PATTERN_OPTS)
    )
    {
    }
}

record class GherkinTagToBddConversionOptions(
    Regex? Epic,
    Regex? Story
)
{
    public GherkinTagToBddConversionOptions() : this(
        new(DefaultPatterns.EPIC, GherkinPatterns.DEFAULT_PATTERN_OPTS),
        new(DefaultPatterns.STORY, GherkinPatterns.DEFAULT_PATTERN_OPTS)
    )
    {
    }
}
