using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Sdk.Functions;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

sealed class RuntimeAsyncOperations<TConfiguration>(IAllureRuntime<TConfiguration> runtime) :
    IAllureInProcessAsyncOperations

    where TConfiguration : AllureConfiguration
{
    readonly ImmutableList<string> failExceptions = runtime.Configuration.FailExceptions;

    readonly ImmutableDictionary<string, AllureLinkTemplate> linkTemplates =
        runtime.Configuration.LinkTemplates;

    AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    public async Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await runtime.ResultsDestination.WriteAttachmentAsync(
            content,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);
        var attachment = new Attachment
        {
            Name = name,
            Type = mediaType,
            Source = source,
        };
        runtime.ModelApi.UpdateCurrentExecutableItem(
            (obj) => obj.Attachments.Add(attachment)
        );
    }

    public async Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await runtime.ResultsDestination.WriteAttachmentAsync(
            content,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);
        await runtime.ResultsDestination.WriteGlobalsAsync(new()
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
            ],
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAttachmentFromFileAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await runtime.ResultsDestination.CopyAttachmentAsync(
            path,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);
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

    public async Task AddGlobalAttachmentFromFileAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await runtime.ResultsDestination.CopyAttachmentAsync(
            path,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);

        await runtime.ResultsDestination.WriteGlobalsAsync(new()
        {
            Attachments =
            [
                new GlobalAttachment
                {
                    Name = name,
                    Type = mediaType,
                    Source = source,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                }
            ]
        }, cancellationToken).ConfigureAwait(false);
    }

    public Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Labels.AddRange(labels)
        );
        return Task.CompletedTask;
    }

    public Task AddLabelAsync(Label label, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Labels.Add(label)
        );
        return Task.CompletedTask;
    }

    public Task AddLinksAsync(IEnumerable<Link> links, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            foreach (var link in links)
            {
                LinkTemplates.Apply(linkTemplates, link);
                testResult.Links.Add(link);
            }

        });
        return Task.CompletedTask;
    }

    public Task AddLinkAsync(Link link, CancellationToken _)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            LinkTemplates.Apply(linkTemplates, link);
            testResult.Links.Add(link);
        });
        return Task.CompletedTask;
    }

    public Task SetDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.Description = description
        );
        return Task.CompletedTask;
    }

    public Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
            testResult.DescriptionHtml = descriptionHtml
        );
        return Task.CompletedTask;
    }

    public Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateFixtureResult((fixtureResult) =>
            fixtureResult.Name = newName
        );
        return Task.CompletedTask;
    }

    public Task SetLabelAsync(string name, string value, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            testResult.Labels.RemoveAll((l) => l.Name == name);
            testResult.Labels.Add(new() { Name = name, Value = value });
        });
        return Task.CompletedTask;
    }

    public Task SetNameAsync(string newName, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateCurrentExecutableItem(
            (item) => item.Name = newName
        );
        return Task.CompletedTask;
    }

    public Task SetTestNameAsync(string newName, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((fixtureResult) =>
            fixtureResult.Name = newName
        );
        return Task.CompletedTask;
    }

    public Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Status status,
        StatusDetails? statusDetails,
        CancellationToken cancellationToken
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
        return Task.CompletedTask;
    }

    public async Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncStepContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        int level = this.StartStep(name, parameters);
        try
        {
            using AsyncStepOperationContext context = new(runtime, level);
            await body(context, cancellationToken);
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

    public async Task<TResult> StepAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        int level = this.StartStep(name, parameters);
        try
        {
            using AsyncStepOperationContext context = new(runtime, level);
            return await body(context, cancellationToken);
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

    public async Task AddScreenDiffAsync(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken)
    {
        var currentDiffCount =
            this.CurrentState.CurrentExecutableItem.Attachments.Count(
                static (a) => a.Type == DIFF_MEDIA_TYPE
            );
        var name = string.Format(DIFF_NAME_PATTERN, currentDiffCount + 1);

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, new
        {
            expected = await ToDiffEntryAsync(expected, cancellationToken),
            actual = await ToDiffEntryAsync(actual, cancellationToken),
            diff = await ToDiffEntryAsync(diff, cancellationToken)
        });

        stream.Position = 0;

        await AddAttachmentAsync(name, stream, DIFF_MEDIA_TYPE, ".json", cancellationToken);
    }

    public async Task AddScreenDiffFromFilesAsync(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken)
    {
        using var expected = File.OpenRead(expectedPath);
        using var actual = File.OpenRead(actualPath);
        using var diff = File.OpenRead(diffPath);
        await AddScreenDiffAsync(expected, actual, diff, cancellationToken);
    }

    public Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken) =>
        runtime.ResultsDestination.WriteGlobalsAsync(new()
        {
            Errors = [error],
        }, cancellationToken);

    public Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken)
    {
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            testResult.Parameters.Add(parameter);
        });
        return Task.CompletedTask;
    }

    public async Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartSetUp(name, parameters);

        try
        {
            using var context = new AsyncFixtureOperationContext(runtime);
            await body(context, cancellationToken);
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

    public async Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartSetUp(name, parameters);

        try
        {
            using var context = new AsyncFixtureOperationContext(runtime);
            return await body(context, cancellationToken);
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

    public async Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartTearDown(name, parameters);

        try
        {
            using var context = new AsyncFixtureOperationContext(runtime);
            await body(context, cancellationToken);
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

    public async Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken)
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        this.StartTearDown(name, parameters);

        try
        {
            using var context = new AsyncFixtureOperationContext(runtime);
            return await body(context, cancellationToken);
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

    const int DefaultCopyBufferSize = 81920;
    const string DIFF_NAME_PATTERN = "diff-{0}";
    const string DIFF_MEDIA_TYPE = "application/vnd.allure.image.diff";
    const string DIFF_ENTRY_PREFIX = "data:image/png;base64,";

    static async Task<string> ToDiffEntryAsync(Stream data, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await data.CopyToAsync(stream, DefaultCopyBufferSize, cancellationToken);
        var base64Part = Convert.ToBase64String(stream.ToArray());
        return $"{DIFF_ENTRY_PREFIX}{base64Part}";
    }
}