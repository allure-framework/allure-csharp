using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Functions;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Runtime;

using IAllureTestingPlatformRegistration = IAllureTestingPlatformRegistration<
    AllureTestingPlatformConfiguration,
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
>;

sealed class AllureTestingPlatformAsyncOperations(
    IAllureTestingPlatformRegistration registration
) :
    IDataProducer,
    IAllureInProcessAsyncOperations
{
    readonly IAllureTestingPlatformMessageChannel channel = registration.MessageChannel;

    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> Runtime =>
        registration.RuntimeReference.Value;

    ExecutionStateContext ExecutionStateContext => this.Runtime.ExecutionStateContext;

    IAllureResultsDestination ResultsDestination => this.Runtime.ResultsDestination;

    ICorrelationContext CorrelationContext => this.Runtime.CorrelationContext;

    AllureTestingPlatformConfiguration Configuration => this.Runtime.Configuration;

    public Type[] DataTypesProduced { get; } = [
        typeof(AllureFixtureUpdateMessage),
        typeof(AllureTestUpdateMessage),
        typeof(AllureBeforeFixtureStartMessage),
        typeof(AllureAfterFixtureStartMessage),
        typeof(AllureFixtureStopMessage),
        typeof(AllureStepStartMessage),
        typeof(AllureStepStopMessage),
        typeof(AllureExecutableItemUpdateMessage),
    ];

    public IAllureExecutionStateUid CurrentExecutableItemUid =>
        this.ExecutionStateContext.CurrentStepUid
            ?? (IAllureExecutionStateUid?)this.ExecutionStateContext.CurrentFixtureUid
            ?? this.ExecutionStateContext.CurrentTestUid
            ?? throw new InvalidOperationException(
                "Cannot get the current test, step, or fixture."
            );

    public ScopeExecutionStateUid CurrentScopeUid =>
        this.ExecutionStateContext.CurrentScopeUid
            ?? throw new InvalidOperationException(
                "Cannot get the current test scope."
            );

    public FixtureExecutionStateUid CurrentFixtureUid =>
        this.ExecutionStateContext.CurrentFixtureUid
            ?? throw new InvalidOperationException(
                "Cannot get the current fixture."
            );

    public TestExecutionStateUid CurrentTestUid =>
        this.ExecutionStateContext.CurrentTestUid
            ?? throw new InvalidOperationException(
                "Cannot get the current test."
            );

    public string Uid => "9c1b264a-ddd2-4ea3-9cea-d49ec9a638c6";

    public string Version => "1.0.0";

    public string DisplayName => "Allure operations";

    public string Description => "An implementation of Allure operations that publishes messages to the MTP message bus.";

    public async Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await this.ResultsDestination.WriteAttachmentAsync(
            content,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);

        await this.SendExecutionItemUpdateAsync(
            new AllureAttachmentReferenceProperty<ExecutableItem>(
                name,
                source,
                mediaType
            )
        ).ConfigureAwait(false);
    }

    public async Task AddAttachmentFromFileAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await this.ResultsDestination.CopyAttachmentAsync(
            path,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);

        await this.SendExecutionItemUpdateAsync(
            new AllureAttachmentReferenceProperty<ExecutableItem>(
                name,
                source,
                mediaType
            )
        ).ConfigureAwait(false);
    }

    public async Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await this.ResultsDestination.WriteAttachmentAsync(
            content,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);

        await this.ResultsDestination.WriteGlobalsAsync(new()
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
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddGlobalAttachmentFromFileAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var source = await this.ResultsDestination.CopyAttachmentAsync(
            path,
            fileExtension,
            cancellationToken
        ).ConfigureAwait(false);

        await this.ResultsDestination.WriteGlobalsAsync( new()
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

    public Task AddGlobalErrorAsync(
        GlobalError error,
        CancellationToken cancellationToken
    ) =>
        this.ResultsDestination.WriteGlobalsAsync(new()
        {
            Errors = [error],
        }, cancellationToken);

    public Task AddLabelAsync(Label label, CancellationToken _) =>
        this.SendTestUpdateAsync(
            new AllureLabelsProperty([label])
        );

    public Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken _) =>
        this.SendTestUpdateAsync(
            new AllureLabelsProperty(labels)
        );

    public Task AddLinkAsync(Link link, CancellationToken _)
    {
        LinkTemplates.Apply(this.Configuration.LinkTemplates, link);
        return this.SendTestUpdateAsync(
            new AllureLinksProperty([link])
        );
    }

    public Task AddLinksAsync(IEnumerable<Link> links, CancellationToken _)
    {
        List<Link> linksToAdd = [.. links];
        foreach (var link in linksToAdd)
        {
            LinkTemplates.Apply(this.Configuration.LinkTemplates, link);
        }

        return this.SendTestUpdateAsync(
            new AllureLinksProperty(linksToAdd)
        );
    }

    public async Task AddScreenDiffAsync(
        Stream expected,
        Stream actual,
        Stream diff,
        CancellationToken cancellationToken
    )
    {
        var fileName = await ScreenDiffContent.ConsumeAsync(
            expected,
            actual,
            diff,
            ConsumeScreenDiff,
            cancellationToken
        ).ConfigureAwait(false);

        await this.SendExecutionItemUpdateAsync(
            new AllureScreenDiffReferenceProperty<ExecutableItem>(fileName)
        ).ConfigureAwait(false);

        Task<string> ConsumeScreenDiff(Stream content, CancellationToken cancellationToken) =>
            this.ResultsDestination.WriteAttachmentAsync(
                content,
                ".json",
                cancellationToken
            );
    }

    public async Task AddScreenDiffFromFilesAsync(
        string expectedPath,
        string actualPath,
        string diffPath,
        CancellationToken cancellationToken
    )
    {
        using var expected = File.OpenRead(expectedPath);
        using var actual = File.OpenRead(actualPath);
        using var diff = File.OpenRead(diffPath);
        await this.AddScreenDiffAsync(expected, actual, diff, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken) =>
        this.SendTestUpdateAsync(
            new AllureParametersProperty<TestResult>([parameter])
        );

    public Task SetDescriptionAsync(string description, CancellationToken cancellationToken) =>
        this.SendTestUpdateAsync(
            new AllureDescriptionProperty<TestResult>(description)
        );

    public Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken) =>
        this.SendTestUpdateAsync(
            new AllureDescriptionHtmlProperty<TestResult>(descriptionHtml)
        );

    public Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken) =>
        this.SendFixtureUpdateAsync(
            new AllureNameProperty<FixtureResult>(newName)
        );

    public Task SetLabelAsync(string name, string value, CancellationToken cancellationToken) =>
        this.SendTestUpdateAsync(
            new AllureSetLabelProperty(name, value)
        );

    public Task SetNameAsync(string newName, CancellationToken cancellationToken) =>
        this.SendExecutionItemUpdateAsync(
            new AllureNameProperty<ExecutableItem>(newName)
        );

    public Task SetTestNameAsync(string newName, CancellationToken cancellationToken) =>
        this.SendTestUpdateAsync(
            new AllureNameProperty<TestResult>(newName)
        );

    public async Task SetUpAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        var fixtureUid = new FixtureExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureBeforeFixtureStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                fixtureUid,
                this.CurrentScopeUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<FixtureResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<FixtureResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<FixtureResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureFixtureStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    fixtureUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
        }
    }

    public async Task<TResult> SetUpAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        var fixtureUid = new FixtureExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureBeforeFixtureStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                fixtureUid,
                this.CurrentScopeUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<FixtureResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            return await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<FixtureResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<FixtureResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureFixtureStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    fixtureUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
        }
    }

    public async Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Status status,
        StatusDetails? statusDetails,
        CancellationToken cancellationToken
    )
    {
        var stepUid = new StepExecutionStateUid(Ids.NewUuid());
        List<IAllureProperty> properties = [
            new AllureParametersProperty<StepResult>(parameters),
            new AllureStatusProperty<StepResult>(status),
        ];
        if (statusDetails is not null)
        {
            properties.Add(new AllureStatusDetailsProperty<StepResult>(statusDetails));
        }

        await this.channel.PublishAsync(
            this,
            new AllureStepStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentExecutableItemUid,
                stepUid,
                name
            )
            {
                Properties = properties,
            }
        ).ConfigureAwait(false);
        await this.channel.PublishAsync(
            this,
            new AllureStepStopMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                stepUid
            )
        ).ConfigureAwait(false);
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

        var stepUid = new StepExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureStepStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentExecutableItemUid,
                stepUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<StepResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncStepContext context = new(registration, stepUid);
            await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<StepResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<StepResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureStepStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    stepUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
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

        var stepUid = new StepExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureStepStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentExecutableItemUid,
                stepUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<StepResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncStepContext context = new(registration, stepUid);
            return await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<StepResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<StepResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureStepStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    stepUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
        }
    }

    public async Task TearDownAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        var fixtureUid = new FixtureExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureAfterFixtureStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                fixtureUid,
                this.CurrentScopeUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<FixtureResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<FixtureResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<FixtureResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureFixtureStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    fixtureUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
        }
    }

    public async Task<TResult> TearDownAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    )
    {
        Status status = Status.Passed;
        StatusDetails? statusDetails = null;

        var fixtureUid = new FixtureExecutionStateUid(Ids.NewUuid());

        await this.channel.PublishAsync(
            this,
            new AllureAfterFixtureStartMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                fixtureUid,
                this.CurrentScopeUid,
                name
            )
            {
                Properties = [new AllureParametersProperty<FixtureResult>(parameters)],
            }
        ).ConfigureAwait(false);

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            return await body(context, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            status = ErrorStatus.Resolve(this.Configuration.FailExceptions, e);
            statusDetails = StatusDetails.FromException(e);
            throw;
        }
        finally
        {
            List<IAllureProperty> properties = [new AllureStatusProperty<FixtureResult>(status)];
            if (statusDetails is not null)
            {
                properties.Add(new AllureStatusDetailsProperty<FixtureResult>(statusDetails));
            }

            await this.channel.PublishAsync(
                this,
                new AllureFixtureStopMessage(
                    this.CorrelationContext.CurrentCorrelationUid,
                    fixtureUid
                )
                {
                    Properties = properties,
                }
            ).ConfigureAwait(false);
        }
    }

    Task SendExecutionItemUpdateAsync(
        params IEnumerable<IAllureProperty<ExecutableItem>> properties
    ) =>
        this.channel.PublishAsync(
            this,
            new AllureExecutableItemUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentExecutableItemUid
            )
            {
                Properties = [.. properties],
            }
        );

    Task SendTestUpdateAsync(
        params IEnumerable<IAllureProperty<TestResult>> properties
    ) =>
        this.channel.PublishAsync(
            this,
            new AllureTestUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentTestUid
            )
            {
                Properties = [.. properties],
            }
        );

    Task SendFixtureUpdateAsync(
        params IEnumerable<IAllureProperty<FixtureResult>> properties
    ) =>
        this.channel.PublishAsync(
            this,
            new AllureFixtureUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentFixtureUid
            )
            {
                Properties = [.. properties],
            }
        );

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
