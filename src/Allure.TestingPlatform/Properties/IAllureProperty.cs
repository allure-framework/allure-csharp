using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Properties;

public interface IAllureProperty : IProperty;

public interface IAllureProperty<TObject> : IAllureProperty
{
    public abstract void Apply(IAllureInfrastructure allure, TObject obj);
}
