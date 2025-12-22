using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal abstract class AllureReqnrollEventHandler
{
    public abstract void Register(ITestThreadExecutionEventPublisher publisher);
}

internal class AllureReqnrollEventHandler<T> : AllureReqnrollEventHandler
    where T : IExecutionEvent
{
    readonly Lazy<CrossBindingContextTransport> stateStorage;

    protected CrossBindingContextTransport StateStorage
    {
        get => this.stateStorage.Value;
    }

    public AllureReqnrollEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    )
    {
        this.stateStorage = new(stateStorageFactory);
    }

    public override sealed void Register(ITestThreadExecutionEventPublisher publisher) =>
        publisher.AddHandler<T>(this.Handle);

    public void Handle(T eventData) =>
        this.StateStorage.PropagateState(
            () => this.HandleInAllureContext(eventData)
        );

    protected virtual void HandleInAllureContext(T eventData) { }
}
