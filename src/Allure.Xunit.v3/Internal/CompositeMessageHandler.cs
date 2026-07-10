using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.Xunit.Internal;

sealed class CompositeMessageHandler(params IEnumerable<IRunnerReporterMessageHandler> handlers) :
    IRunnerReporterMessageHandler
{
    public bool OnMessage(IMessageSinkMessage message)
    {
        bool result = true;
        foreach (var handler in handlers)
        {
            result &= handler.OnMessage(message);
        }
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var handler in handlers)
        {
            await handler.DisposeAsync();
        }
    }
}
