using System;
using Allure.Sdk.Registration;

namespace Allure.Sdk.Internal.Registration;

sealed class LateBoundReference<T> : IReadOnlyLateBoundReference<T>
{
    T? value = default;

    public bool IsBound => this.value is not null;

    public T Value => this.value ?? throw new InvalidOperationException(
        $"{typeof(T).Name} has not been bound yet."
    );

    public void Bind(T value)
    {
        if (this.IsBound)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} is already bound."
            );
        }

        this.value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
