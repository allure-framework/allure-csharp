using System;
using System.Diagnostics;
using System.Reflection;
using Reqnroll.Bindings.Reflection;

namespace Allure.ReqnrollPlugin.Functions;

static class ExceptionFunctions
{
    internal static bool IsFromHookMethod(
        Exception exception,
        IBindingMethod hookMethod
    )
    {
        var stackTrace = new StackTrace(exception);
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            if (IsFrameOfHookMethod(frame, hookMethod))
            {
                return true;
            }
        }
        return false;
    }

    static bool IsFrameOfHookMethod(
        StackFrame frame,
        IBindingMethod hookMethod
    ) =>
        frame.HasMethod()
            && frame.GetMethod() is MethodInfo method
            && hookMethod.MethodEquals(new RuntimeBindingMethod(method));
}
