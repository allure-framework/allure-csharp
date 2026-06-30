using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the status of an Allure test, step, or fixture.
/// </summary>
public sealed class AllureStatusProperty<TModel>(Status status) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the status to set.
    /// </summary>
    public Status Status { get; } = status;

    /// <summary>
    /// Gets or sets whether the status should be set only
    /// when the target has no status assigned yet.
    /// </summary>
    public bool OnlyIfUnset { get; init; } = false;

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        if (!this.OnlyIfUnset || target.status is Status.none)
        {
            target.status = this.Status;
        }
    }
}
