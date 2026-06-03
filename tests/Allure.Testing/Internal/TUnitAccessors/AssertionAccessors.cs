using System.Runtime.CompilerServices;
using TUnit.Assertions.Core;

namespace Allure.Testing.Internal.TUnitAccessors;

static class AssertionAccessors<TValue>
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "Context")]
    public static extern ref AssertionContext<TValue> GetContext(Assertion<TValue> target);
}
