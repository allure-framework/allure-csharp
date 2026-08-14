using System;

namespace Allure.TestingPlatform.Internal.Runtime;

public interface IRequestRuntimeBinding : IDisposable
{
    void Activate();

    void Release();
}
