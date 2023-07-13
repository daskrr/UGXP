using System.Reflection;

namespace UGXP.Util;

public class Reflection
{
    public static MethodInfo? GetMethod(object obj, string methodName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) {
        return obj.GetType().GetMethod(methodName, flags);
    }
}
