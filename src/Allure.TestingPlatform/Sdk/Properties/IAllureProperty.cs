using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Properties;

public interface IAllureProperty : IProperty;

public interface IAllureProperty<TObject> : IAllureProperty
{
    public abstract void Apply(ReadyAllureTestingPlatform allureState, TObject obj);
}
