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
            logger.Info("New Allure Lifecycle");
        }
        public void Intercept(IInvocation invocation)
        {
            var message = new StringBuilder();
            message
                .Append($"[{invocation.InvocationTarget.GetHashCode()}] ")
                .Append(invocation.Method.Name)
                .Append(" (")
                .Append(string.Join(", ", invocation.Arguments))
                .Append(")");

            logger.Info(message.ToString());
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
