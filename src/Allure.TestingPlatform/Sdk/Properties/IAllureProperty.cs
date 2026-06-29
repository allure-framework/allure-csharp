using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Properties;

public interface IAllureProperty : IProperty;

public interface IAllureProperty<TObject> : IAllureProperty
{
    public abstract void Apply(ReadyAllureTestingPlatformRuntime allureState, TObject obj);
}
