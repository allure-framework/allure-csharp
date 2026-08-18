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

    public Type[] DataTypesProduced => [
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

    public async Task AddAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken _)
    {
        await this.SendExecutionItemUpdateAsync(
            new AllureAttachmentProperty<ExecutableItem>(name, content)
            {
                MediaType = mediaType,
                FileExtension = fileExtension,
            }
        );
    }

    public async Task AddAttachmentFromFileAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken _)
    {
        await this.SendExecutionItemUpdateAsync(
            new AllureAttachmentFileProperty<ExecutableItem>(name, path)
            {
                MediaType = mediaType,
                FileExtension = fileExtension,
            }
        );
    }

    public async Task AddGlobalAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var source = AttachmentSource.CreateName(fileExtension);
        Globals globals = new()
        {
            Attachments = [
                new GlobalAttachment
                {
                    Name = name,
                    Type = mediaType,
                    Source = source,
                    FileExtension = fileExtension,
                    Timestamp = timestamp,
                }
            ],
        };
        await this.ResultsDestination.WriteAttachmentAsync(source, content, cancellationToken);
        await this.ResultsDestination.WriteGlobalsAsync(globals, cancellationToken);
    }

    public async Task AddGlobalAttachmentFromFileAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var source = AttachmentSource.CreateName(fileExtension);
        Globals globals = new()
        {
            Attachments =
            [
                new GlobalAttachment
                {
                    Name = name,
                    Type = mediaType,
                    Source = source,
                    FileExtension = fileExtension,
                    Timestamp = timestamp
                }
            ]
        };
        await this.ResultsDestination.CopyAttachmentAsync(source, path, cancellationToken);
        await this.ResultsDestination.WriteGlobalsAsync(globals, cancellationToken);
    }

    public async Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken)
    {
        await this.ResultsDestination.WriteGlobalsAsync(new()
        {
            Errors = [error],
        }, cancellationToken);
    }

    public async Task AddLabelAsync(Label label, CancellationToken _)
    {
        await this.SendTestUpdateAsync(
            new AllureLabelsProperty([label])
        );
    }

    public async Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken _)
    {
        await this.SendTestUpdateAsync(
            new AllureLabelsProperty(labels)
        );
    }

    public async Task AddLinkAsync(Link link, CancellationToken _)
    {
        await this.SendTestUpdateAsync(
            new AllureLinksProperty([link])
        );
    }

    public async Task AddLinksAsync(IEnumerable<Link> links, CancellationToken _)
    {
        await this.SendTestUpdateAsync(
            new AllureLinksProperty(links)
        );
    }

    public async Task AddScreenDiffAsync(Stream expected, Stream actual, Stream diff, CancellationToken _)
    {
        await this.SendExecutionItemUpdateAsync(
            new AllureScreenDiffProperty<ExecutableItem>(expected, actual, diff)
        );
    }

    public async Task AddScreenDiffFromFilesAsync(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken)
    {
        await this.SendExecutionItemUpdateAsync(
            new AllureScreenDiffFileProperty<ExecutableItem>(expectedPath, actualPath, diffPath)
        );
    }

    public async Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken)
    {
        await this.SendTestUpdateAsync(
            new AllureParametersProperty<TestResult>([parameter])
        );
    }

    public async Task SetDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        await this.SendTestUpdateAsync(
            new AllureDescriptionProperty<TestResult>(description)
        );
    }

    public async Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken)
    {
        await this.SendTestUpdateAsync(
            new AllureDescriptionHtmlProperty<TestResult>(descriptionHtml)
        );
    }

    public async Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken)
    {
        await this.SendFixtureUpdateAsync(
            new AllureNameProperty<FixtureResult>(newName)
        );
    }

    public async Task SetLabelAsync(string name, string value, CancellationToken cancellationToken)
    {
        await this.SendTestUpdateAsync(
            new AllureSetLabelProperty(name, value)
        );
    }

    public async Task SetNameAsync(string newName, CancellationToken cancellationToken)
    {
        await this.SendExecutionItemUpdateAsync(
            new AllureNameProperty<ExecutableItem>(newName)
        );
    }

    public async Task SetTestNameAsync(string newName, CancellationToken cancellationToken)
    {
        await this.SendTestUpdateAsync(
            new AllureNameProperty<TestResult>(newName)
        );
    }

    public async Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            await body(context, cancellationToken);
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
            );
        }
    }

    public async Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            return await body(context, cancellationToken);
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
            );
        }
    }

    public async Task StepAsync(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken)
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
        );
        await this.channel.PublishAsync(
            this,
            new AllureStepStopMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                stepUid
            )
        );
    }

    public async Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncStepContext, CancellationToken, Task> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncStepContext context = new(registration, stepUid);
            await body(context, cancellationToken);
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
            );
        }
    }

    public async Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncStepContext context = new(registration, stepUid);
            return await body(context, cancellationToken);
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
            );
        }
    }

    public async Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            await body(context, cancellationToken);
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
            );
        }
    }

    public async Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken)
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
        );

        try
        {
            using AllureTestingPlatformAsyncFixtureContext context = new(registration, fixtureUid);
            return await body(context, cancellationToken);
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
            );
        }
    }

    async Task SendExecutionItemUpdateAsync(params IEnumerable<IAllureProperty<ExecutableItem>> properties)
    {
        await this.channel.PublishAsync(
            this,
            new AllureExecutableItemUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentExecutableItemUid
            )
            {
                Properties = [.. properties],
            }
        );
    }

    async Task SendTestUpdateAsync(params IEnumerable<IAllureProperty<TestResult>> properties)
    {
        await this.channel.PublishAsync(
            this,
            new AllureTestUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentTestUid
            )
            {
                Properties = [.. properties],
            }
        );
    }

    async Task SendFixtureUpdateAsync(params IEnumerable<IAllureProperty<FixtureResult>> properties)
    {
        await this.channel.PublishAsync(
            this,
            new AllureFixtureUpdateMessage(
                this.CorrelationContext.CurrentCorrelationUid,
                this.CurrentFixtureUid
            )
            {
                Properties = [.. properties],
            }
        );
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
