namespace Allure.Net.Sdk.Tests.Infrastructure;

sealed class RecordingFactory<TInput, TOutput>(Func<TInput, TOutput> factory)
{
    readonly List<TInput> inputs = [];

    public IReadOnlyList<TInput> Inputs => this.inputs;

    public TOutput Invoke(TInput input)
    {
        this.inputs.Add(input);
        return factory(input);
    }
}
