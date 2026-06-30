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
        foreach (var handler in handlers)
        {
            if (!handler.OnMessage(message))
            {
                return false;
            }
        }

        return true;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var handler in handlers)
        {
            await handler.DisposeAsync();
        }
    }
}
