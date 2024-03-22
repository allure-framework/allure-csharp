using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.ReqnrollPlugin.Configuration;
using CsvHelper;
using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Bindings.Reflection;

namespace Allure.ReqnrollPlugin.Functions;

static class MappingFunctions
{
    const string TEST_PENDING_MESSAGE = "One or more steps are not implemented.";

    internal static TestResultContainer CreateContainer() => new()
    {
        uuid = IdFunctions.CreateUUID()
    };

    internal static FixtureResult ToFixtureResult(IHookBinding binding) => new()
    {
        name = binding.Method.GetShortDisplayText()
    };

    internal static TestResult CreateTestResult(
        Assembly featureAssembly,
        FeatureInfo featureInfo,
        ScenarioInfo scenarioInfo,
        (List<Label>, List<Link>) scenarioMetadata
    )
    {
        var (labels, links) = scenarioMetadata;
        return CreateTestResultInternal(
            featureAssembly,
            featureInfo,
            scenarioInfo.Title,
            scenarioInfo.Description,
            GetParameters(scenarioInfo),
            labels,
            links
        );
    }

    internal static TestResult CreateTestResult(
        Assembly featureAssembly,
        FeatureInfo featureInfo,
        string testTitle,
        string? testDescription
    ) =>
        CreateTestResultInternal(
            featureAssembly,
            featureInfo,
            testTitle,
            testDescription
        );

    internal static StepResult ToStepResult(StepInfo stepInfo) => new()
    {
        name = $"{stepInfo.StepDefinitionType} {stepInfo.Text}",
    };

    internal static (Status, StatusDetails?) ResolveTestStatus(
        IEnumerable<string> failExceptions,
        IScenarioContext scenarioContext
    ) =>
        scenarioContext.ScenarioExecutionStatus switch
        {
            ScenarioExecutionStatus.OK => (Status.passed, null),
            ScenarioExecutionStatus.Skipped => (Status.skipped, null),
            ScenarioExecutionStatus.TestError => (
                ModelFunctions.IsKnownError(
                    AllureReqnrollConfiguration.CurrentConfig.IgnoreExceptions,
                    scenarioContext.TestError
                )
                    ? Status.skipped
                    : ModelFunctions.ResolveErrorStatus(
                        failExceptions,
                        scenarioContext.TestError
                    ),
                ModelFunctions.ToStatusDetails(scenarioContext.TestError)
            ),
            ScenarioExecutionStatus.BindingError
                or ScenarioExecutionStatus.UndefinedStep => (
                    Status.broken,
                    ModelFunctions.ToStatusDetails(scenarioContext.TestError)
                ),
            ScenarioExecutionStatus.StepDefinitionPending => (
                Status.broken,
                new() { message = TEST_PENDING_MESSAGE }
            ),
            _ => (Status.broken, null)
        };

    internal static (
        List<(
            string title,
            string mediaType,
            byte[] content,
            string fileExtension
        )> attachmentsData,
        List<Parameter> parameters
    ) ResolveStepAttachmentsAndParameters(
        DataTableToArgsConversionOptions stepArgOpts,
        StepInfo stepInfo
    )
    {
        List<(string, string, byte[], string)> attachments = new();
        List<Parameter>? parameters = null;

        var docString = stepInfo.MultilineText;
        if (!string.IsNullOrEmpty(docString))
        {
            attachments.Add((
                "Doc String",
                "text/plain",
                Encoding.UTF8.GetBytes(docString),
                ".txt"
            ));
        }
        
        var dataTable = stepInfo.Table;
        if (dataTable is not null)
        {
            parameters = GenerateParametersFromTable(
                stepArgOpts,
                dataTable
            ).ToList();

            if (!parameters.Any())
            {
                attachments.Add((
                    "Data Table",
                    "text/csv",
                    GetCsvFileContent(dataTable),
                    ".csv"
                ));
            }
        }

        return (attachments, parameters ?? new());
    }

    internal static (List<Label> labels, List<Link> links) GetLabelsAndLinks(
        GherkinPatterns patterns,
        FeatureInfo featureInfo,
        ScenarioInfo scenarioInfo
    )
    {
        var labels = new List<Label>();
        var links = new List<Link>();

        var tags = scenarioInfo.Tags
            .Union(featureInfo.Tags)
            .Distinct(StringComparer.CurrentCultureIgnoreCase);

        foreach (var tag in tags)
        {
            var tagValue = tag;
            // link
            if (TryUpdateValueByMatch(patterns.Links.Link, ref tagValue))
            {
                links.Add(new()
                {
                    name = tagValue,
                    url = tagValue
                });
                continue;
            }

            // issue
            if (TryUpdateValueByMatch(patterns.Links.Issue, ref tagValue))
            {
                links.Add(
                    Link.Issue(tagValue, tagValue)
                );
                continue;
            }

            // tms
            if (TryUpdateValueByMatch(patterns.Links.Tms, ref tagValue))
            {
                links.Add(
                    Link.Tms(tagValue, tagValue)
                );
                continue;
            }

            // parent suite
            if (TryUpdateValueByMatch(patterns.Grouping.Suites.ParentSuite, ref tagValue))
            {
                labels.Add(
                    Label.ParentSuite(tagValue)
                );
                continue;
            }

            // suite
            if (TryUpdateValueByMatch(patterns.Grouping.Suites.Suite, ref tagValue))
            {
                labels.Add(
                    Label.Suite(tagValue)
                );
                continue;
            }

            // sub suite
            if (TryUpdateValueByMatch(patterns.Grouping.Suites.SubSuite, ref tagValue))
            {
                labels.Add(
                    Label.SubSuite(tagValue)
                );
                continue;
            }

            // epic
            if (TryUpdateValueByMatch(patterns.Grouping.Behaviors.Epic, ref tagValue))
            {
                labels.Add(
                    Label.Epic(tagValue)
                );
                continue;
            }

            // story
            if (TryUpdateValueByMatch(patterns.Grouping.Behaviors.Story, ref tagValue))
            {
                labels.Add(
                    Label.Story(tagValue)
                );
                continue;
            }

            // owner
            if (TryUpdateValueByMatch(patterns.Metadata.Owner, ref tagValue))
            {
                labels.Add(
                    Label.Owner(tagValue)
                );
                continue;
            }

            // severity
            if (TryUpdateValueByMatch(patterns.Metadata.Severity, ref tagValue) &&
                Enum.TryParse(tagValue, out SeverityLevel level))
            {
                labels.Add(
                    Label.Severity(level)
                );
                continue;
            }

            // label
            if (GetLabelProps(patterns.Metadata.Label, tagValue, out var label))
            {
                labels.Add(new()
                {
                    name = label.name,
                    value = label.value
                });
                continue;
            }

            // tag
            labels.Add(
                Label.Tag(tagValue)
            );
        }

        return (labels, links);
    }

    internal static string CreateFullName(
        Assembly featureAssembly,
        FeatureInfo featureInfo,
        string scenarioTitle
    ) =>
        string.Join(
            "/",
            new[]
            {
                    featureAssembly.GetName().Name,
                    featureInfo.FolderPath,
                    EscapeFullNamePart(featureInfo.Title),
                    EscapeFullNamePart(scenarioTitle)
            }
        );

    static TestResult CreateTestResultInternal(
        Assembly featureAssembly,
        FeatureInfo featureInfo,
        string scenarioTitle,
        string? scenarioDescription,
        List<Parameter>? scenarioParameters = null,
        List<Label>? extraLabels = null,
        List<Link>? links = null
    ) =>
        new()
        {
            uuid = IdFunctions.CreateUUID(),
            fullName = CreateFullName(
                featureAssembly,
                featureInfo,
                scenarioTitle
            ),
            name = scenarioTitle,
            description = scenarioDescription,
            labels = InitializeTestLabels(featureInfo, extraLabels),
            links = links ?? new(),
            parameters = scenarioParameters ?? new()
        };

    static IEnumerable<Parameter> GenerateParametersFromTable(
        DataTableToArgsConversionOptions stepArgOpts,
        Table dataTable
    )
    {
        if (stepArgOpts.CreateFromDataTables)
        {
            var nameRegex = stepArgOpts.NameColumn;
            var valueRegex = stepArgOpts.ValueColumn;
            var header = dataTable.Header.ToList();
            if (TreatAsVerticalArgumentTable(header, nameRegex, valueRegex))
            {
                if (IsVerticalArgumentTableMatch(header, nameRegex!, valueRegex!))
                {
                    foreach (var row in dataTable.Rows)
                    {
                        yield return new()
                        {
                            name = row[0],
                            value = row[1]
                        };
                    }
                }
            }
            else if (TreatAsHorizontalArgumentTable(dataTable))
            {
                foreach (var row in dataTable.Rows[0])
                {
                    yield return new()
                    {
                        name = row.Key,
                        value = row.Value
                    };
                };
            }
        }
    }

    static byte[] GetCsvFileContent(Table dataTable)
    {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms, Encoding.UTF8);
        using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        foreach (var item in dataTable.Header)
        {
            csv.WriteField(item);
        }
        csv.NextRecord();
        foreach (var row in dataTable.Rows)
        {
            foreach (var item in row.Values)
            {
                csv.WriteField(item);
            }

            csv.NextRecord();
        }
        sw.Flush();
        return ms.ToArray();
    }

    static bool TreatAsVerticalArgumentTable(
        IReadOnlyList<string> header,
        Regex? nameRegex,
        Regex? valueRegex
    ) =>
        header.Count is 2
            && nameRegex is not null
            && valueRegex is not null;

    static bool TreatAsHorizontalArgumentTable(Table table) =>
        table.RowCount is 1;

    static bool IsVerticalArgumentTableMatch(
        IReadOnlyList<string> header,
        Regex nameRegex,
        Regex valueRegex
    ) =>
        nameRegex.IsMatch(header[0]) && valueRegex.IsMatch(header[1]);

    static List<Parameter> GetParameters(ScenarioInfo scenarioInfo)
    {
        var parameters = new List<Parameter>(scenarioInfo.Arguments.Count);
        var argumentsEnumerator = scenarioInfo.Arguments.GetEnumerator();
        while (argumentsEnumerator.MoveNext())
        {
            var key = argumentsEnumerator.Key.ToString();
            var value = FormatFunctions.Format(
                argumentsEnumerator.Value,
                AllureLifecycle.Instance.TypeFormatters
            );

            parameters.Add(new()
            {
                name = key.ToString(),
                value = value.ToString()
            });
        }
        return parameters;
    }

    static List<Label> InitializeTestLabels(
        FeatureInfo featureInfo,
        IEnumerable<Label>? scenarioLabels
    ) =>
        CreateDefaultLabels(featureInfo)
            .Concat(scenarioLabels ?? Enumerable.Empty<Label>())
            .ToList();

    static IEnumerable<Label> CreateDefaultLabels(FeatureInfo featureInfo)
    {
        yield return ResolveHostLabel();
        yield return Label.Thread();
        yield return Label.Language();
        yield return Label.Framework("Reqnroll");
        yield return Label.Package(
            featureInfo
                .FolderPath
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.')
                + '.' + featureInfo.Title
        );
        yield return Label.Feature(featureInfo.Title);
    }

    static Label ResolveHostLabel() =>
        string.IsNullOrWhiteSpace(
            AllureLifecycle.Instance.AllureConfiguration.Title
        ) ? Label.Host() : Label.Host(
            AllureLifecycle.Instance.AllureConfiguration.Title
        );

    static string EscapeFullNamePart(string part) =>
        part.Replace("/", "\\/");

    static bool TryUpdateValueByMatch(
        Regex? expression,
        ref string? value
    )
    {
        var matchedGroups = GetMatchedGroups(expression, value);

        if (!matchedGroups.Any())
        {
            return false;
        }

        value = matchedGroups.Count == 1
            ? matchedGroups[0]
            : matchedGroups[1];

        return true;
    }

    static List<string> GetMatchedGroups(
        Regex? pattern,
        string? value
    )
    {
        if (string.IsNullOrWhiteSpace(value) || pattern is null)
        {
            return new();
        }

        var match = pattern.Match(value);

        return match.Success
            ? match.Groups.Cast<Group>().Select(g => g.Value).ToList()
            : new();
    }

    static bool GetLabelProps(
        Regex? pattern,
        string? value,
        out (string name, string value) props
    )
    {
        var matchedGroups = GetMatchedGroups(pattern, value);

        if (matchedGroups.Count != 3)
        {
            props = default;
            return false;
        }

        props = (matchedGroups[1], matchedGroups[2]);
        return true;
    }
}
