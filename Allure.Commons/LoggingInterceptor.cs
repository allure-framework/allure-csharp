using Castle.DynamicProxy;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allure.Commons
{
    class LoggingInterceptor : IInterceptor
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        internal LoggingInterceptor()
        {
            logger.Info("Starting new Allure Lifecycle...");
        }
        public void Intercept(IInvocation invocation)
        {
            var message = new StringBuilder();
            message.Append(invocation.Method.Name);
            if (logger.IsDebugEnabled)
                message
                    .Append(" (")
                    .Append(string.Join(", ", invocation.Arguments))
                    .Append(")");
            logger.Info(message);
            try
            {
                invocation.Proceed();
                if (!invocation.ReturnValue.ToString().EndsWith("Proxy"))
                    logger.Debug(invocation.ReturnValue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
