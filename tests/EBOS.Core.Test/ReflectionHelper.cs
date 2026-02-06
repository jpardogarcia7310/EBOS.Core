using System.Reflection;

namespace EBOS.Core.Test;

internal static class ReflectionHelper
{
    public static object? InvokeStaticNonPublicMethod(Type type, string methodName, params object?[] parameters)
    {
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static) ?? throw new MissingMethodException(type.FullName, methodName);
        return method.Invoke(null, parameters);
    }
}