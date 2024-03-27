using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Allure.Xunit
{
    internal class ComposedMessageSink : IMessageSink, IMessageSinkWithTypes
    {
        readonly IMessageSink[] sinks;

        public ComposedMessageSink(params IMessageSink[] sinks)
        {
            this.sinks = sinks;
        }

        public void Dispose() { }

        public bool OnMessage(IMessageSinkMessage message)
        {
            foreach (var sink in sinks)
            {
                if (!sink.OnMessage(message))
                {
                    return false;
                }
            }
            return true;
        }

        public bool OnMessageWithTypes(IMessageSinkMessage message, HashSet<string> messageTypes)
        {
            foreach(var sink in this.sinks)
            {
                if (!DispatchTypedSinkMessage(sink, message, messageTypes))
                {
                    return false;
                }
            }
            return true;
        }

        static bool DispatchTypedSinkMessage(
            IMessageSink sink,
            IMessageSinkMessage message,
            HashSet<string> messageTypes
        ) =>
            sink is IMessageSinkWithTypes typedSink
                ? typedSink.OnMessageWithTypes(message, messageTypes)
                : sink.OnMessage(message);
    }
}
