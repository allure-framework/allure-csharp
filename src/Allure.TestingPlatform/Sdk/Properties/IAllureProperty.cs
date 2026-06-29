using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Properties;

public interface IAllureProperty : IProperty;

public interface IAllureProperty<TModel> : IAllureProperty
{
    public abstract void Apply(LiveAllureTestingPlatformRuntime allureState, TModel target);
}
