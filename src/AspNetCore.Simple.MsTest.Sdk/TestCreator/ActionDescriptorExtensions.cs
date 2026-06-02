using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class ActionDescriptorExtensions
    {
        internal static Type GetReturnType(this ActionDescriptor actionDescriptor)
        {
            var methodInfo = (MethodInfo)actionDescriptor.GetType().GetProperty("MethodInfo")!.GetValue(actionDescriptor)!;

            return methodInfo.ReturnType;
        }
    }
}