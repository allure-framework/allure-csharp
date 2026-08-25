using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Sdk.Functions;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class RuntimeSyncOperations<TConfiguration>(IAllureRuntime<TConfiguration> runtime) :
    IAllureInProcessSyncOperations

    where TConfiguration : AllureConfiguration
{
    readonly ImmutableList<string> failExceptions = runtime.Configuration.FailExceptions;

    readonly ImmutableDictionary<string, AllureLinkTemplate> linkTemplates =
        runtime.Configuration.LinkTemplates;

    AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    public void AddAttachment(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    )
    {
        var source = runtime.ResultsDestination.WriteAttachment(content, fileExtension);
        var attachment = new Attachment
        {
            Name = name,
            Type = mediaType,
            Source = source
        };

        runtime.ModelApi.UpdateCurrentExecutableItem(
            (obj) => obj.Attachments.Add(attachment)
        );
    }

    public void AddGlobalAttachment(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    )
    {
        var source = runtime.ResultsDestination.WriteAttachment(content, fileExtension);
        runtime.ResultsDestination.WriteGlobals(new()
        {
            Attachments = [
                new GlobalAttachment
                {
                    Name = name,
                    Type = mediaType,
                    Source = source,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                }
            ],
        });
    }

    public void AddAttachmentFromFile(
        string name,
        string path,
        string? mediaType,
        string fileExtension
    )
    {
        var source = runtime.ResultsDestination.CopyAttachment(path, fileExtension);

        var attachment = new Attachment
        {
            Name = name,
            Type = mediaType,
            Source = source
        };

        runtime.ModelApi.UpdateCurrentExecutableItem(
            (obj) => obj.Attachments.Add(attachment)
        );
    }

    public void AddGlobalAttachmentFromFile(
        string name,
        string path,
        string? mediaType,
        string fileExtension
    )
    {
        var source = runtime.ResultsDestination.CopyAttachment(path, fileExtension);

        runtime.ResultsDestination.WriteGlobals(new()
        {
            Attachments =
            [
                new GlobalAttachment
                {
                    Name = name,
                    Type = mediaType,
                    Source = source,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                }
            ]
        });
    }

    public void AddLabels(params IEnumerable<Label> labels) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Labels.AddRange(labels)
        );

    public void AddLabel(Label label) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Labels.Add(label)
        );

    public void AddLinks(params IEnumerable<Link> links) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            foreach (var link in links)
            {
                LinkTemplates.Apply(this.linkTemplates, link);
                testResult.Links.Add(link);
            }
        });

    public void AddLink(Link link)
    {
        LinkTemplates.Apply(this.linkTemplates, link);
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Links.Add(link)
        );
    }

    public void SetDescription(string description) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Description = description
        );

    public void SetDescriptionHtml(string descriptionHtml) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.DescriptionHtml = descriptionHtml
        );

    public void SetFixtureName(string newName) =>
        runtime.ModelApi.UpdateFixtureResult((fixtureResult) =>
            fixtureResult.Name = newName
        );

    public void SetLabel(string name, string value) =>
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            testResult.Labels.RemoveAll((l) => l.Name == name);
            testResult.Labels.Add(new(){ Name = name, Value = value });
        });

    public void SetName(string newName)
    {
        runtime.ModelApi.UpdateCurrentExecutableItem(
            (item) => item.Name = newName
        );
    }

    public void SetTestName(string newName) =>
        runtime.ModelApi.UpdateTestResult((fixtureResult) =>
            fixtureResult.Name = newName
        );

    public void Step(
        string name,
        IEnumerable<Parameter> parameters,
        Status status,
        StatusDetails? statusDetails
    )
    {
        runtime.LifecycleApi.StartStep(new()
        {
            Name = name,
            Status = status,
            StatusDetails = statusDetails,
            Parameters = [.. parameters],
        });
        runtime.LifecycleApi.StopStep();
    }

    public void Step(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncStepContext> body)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        int level = this.StartStep(name, parameters);
        try
        {
            using SyncStepOperationContext context = new(runtime, level);
            body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopStep(status, statusDetails);
        }
    }

    public TResult Step<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessSyncStepContext, TResult> body
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        int level = this.StartStep(name, parameters);
        try
        {
            using SyncStepOperationContext context = new(runtime, level);
            return body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopStep(status, statusDetails);
        }
    }

    public bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    )
    {
        if (CurrentState.HasFixture)
        {
            result = runtime.ModelApi.ReadFixtureResult(read);
            return true;
        }

        result = default;
        return false;
    }

    public bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    )
    {
        if (CurrentState.HasStep)
        {
            result = runtime.ModelApi.ReadStepResult(read);
            return true;
        }

        result = default;
        return false;
    }

    public bool TryReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    )
    {
        if (CurrentState.HasTest)
        {
            result = runtime.ModelApi.ReadTestResult(read);
            return true;
        }

        result = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update) =>
        runtime.ModelApi.UpdateFixtureResult(update);

    public void UpdateStepResult(Action<StepResult> update) =>
        runtime.ModelApi.UpdateStepResult(update);

    public void UpdateTestResult(Action<TestResult> update) =>
        runtime.ModelApi.UpdateTestResult(update);

    int StartStep(string name, IEnumerable<Parameter> parameters)
    {
        runtime.LifecycleApi.StartStep(new()
        {
            Name = name,
            Parameters = [.. parameters],
        });
        return CurrentState.StepDepth - 1;
    }

    void StopStep(Status status, StatusDetails? statusDetails)
    {
        var stepResult = runtime.LifecycleApi.StopStep();
        if (stepResult.Status == default)
        {
            stepResult.Status = status;
        }
        stepResult.StatusDetails ??= statusDetails;
    }

    public void AddScreenDiff(Stream expected, Stream actual, Stream diff)
    {
        var currentDiffCount =
            this.CurrentState.CurrentExecutableItem.Attachments.Count(
                static (a) => a.Type == DIFF_MEDIA_TYPE
            );
        var name = string.Format(DIFF_NAME_PATTERN, currentDiffCount + 1);

        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, new
        {
            expected = ToDiffEntry(expected),
            actual = ToDiffEntry(actual),
            diff = ToDiffEntry(diff)
        });

        stream.Position = 0;

        AddAttachment(name, stream, DIFF_MEDIA_TYPE, ".json");
    }

    public void AddScreenDiffFromFiles(string expectedPath, string actualPath, string diffPath)
    {
        using var expected = File.OpenRead(expectedPath);
        using var actual = File.OpenRead(actualPath);
        using var diff = File.OpenRead(diffPath);
        AddScreenDiff(expected, actual, diff);
    }

    public void AddGlobalError(GlobalError error)
    {
        runtime.ResultsDestination.WriteGlobals(new()
        {
            Errors = [error],
        });
    }

    public void AddTestParameter(Parameter parameter)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            testResult.Parameters.Add(parameter);
        });
    }

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncFixtureContext> body)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartSetUp(name, parameters);
        try
        {
            using SyncFixtureOperationContext context = new(runtime);
            body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopFixture(status, statusDetails);
        }
    }

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessSyncFixtureContext, TResult> body)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartSetUp(name, parameters);
        try
        {
            using SyncFixtureOperationContext context = new(runtime);
            return body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopFixture(status, statusDetails);
        }
    }

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessSyncFixtureContext> body)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartTearDown(name, parameters);
        try
        {
            using SyncFixtureOperationContext context = new(runtime);
            body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopFixture(status, statusDetails);
        }
    }

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessSyncFixtureContext, TResult> body)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartTearDown(name, parameters);
        try
        {
            using SyncFixtureOperationContext context = new(runtime);
            return body(context);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.failExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            this.StopFixture(status, statusDetails);
        }
    }

    const int DefaultCopyBufferSize = 81920;
    const string DIFF_NAME_PATTERN = "diff-{0}";
    const string DIFF_MEDIA_TYPE = "application/vnd.allure.image.diff";
    const string DIFF_ENTRY_PREFIX = "data:image/png;base64,";

    static string ToDiffEntry(Stream data)
    {
        using var stream = new MemoryStream();
        data.CopyTo(stream, DefaultCopyBufferSize);
        var base64Part = Convert.ToBase64String(stream.ToArray());
        return $"{DIFF_ENTRY_PREFIX}{base64Part}";
    }

    void StartSetUp(string name, IEnumerable<Parameter> parameters)
    {
        runtime.LifecycleApi.StartSetUpFixture(new()
        {
            Name = name,
            Parameters = [.. parameters],
        });
    }

    void StartTearDown(string name, IEnumerable<Parameter> parameters)
    {
        runtime.LifecycleApi.StartTearDownFixture(new()
        {
            Name = name,
            Parameters = [.. parameters],
        });
    }

    void StopFixture(Status status, StatusDetails? statusDetails)
    {
        var fixtureResult = runtime.LifecycleApi.StopFixture();
        if (fixtureResult.Status == default)
        {
            fixtureResult.Status = status;
        }
        fixtureResult.StatusDetails ??= statusDetails;
    }
}