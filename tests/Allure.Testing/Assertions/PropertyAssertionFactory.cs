using Allure.Testing.Assertions.Model;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class PropertyAssertionFactory<T> where T : IAllureModelObject<T>
{
    internal PropertyAssertionFactory(AssertionContext<T> context)
    {
        this.Context = context;
    }

    internal AssertionContext<T> Context { get; }
}
