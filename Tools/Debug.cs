using System.Collections;
using UGXP.Core;

namespace UGXP;
public class Debug
{
    internal static Func<string, string> SimpleFormatter = message => {
        return $"[{ DateTime.Now.ToString("H:mm:ss") }] {message}";
    };

    internal static Func<string, string> StackTraceFormatter = message => {
        return $"{SimpleFormatter(message)}\n{Environment.StackTrace}";
    };

    public static void Log(object message, bool showEnumerables = false, bool stackTrace = false) {
        message ??= "NULL";

        if (showEnumerables && message is IEnumerable oen && message is not GameObject)
            message = LogEnumerable(oen);

        if (stackTrace)
            Console.WriteLine(StackTraceFormatter(message.ToString()));
        else
            Console.WriteLine(SimpleFormatter(message.ToString()));
    }

    private static string LogEnumerable(IEnumerable obj) {
        string output = obj.ToString() + "\n";

        int j = 0;
        foreach (var i in obj)
            output += $"[{j++}] -> " + i.ToString() + "\n";

        return output;
    }

    public static void LogWarning(object message, bool stackTrace = false) {
        message ??= "NULL";

        if (stackTrace)
            Console.WriteLine("[WARN]" + StackTraceFormatter(message.ToString()));
        else
            Console.WriteLine("[WARN]" + SimpleFormatter(message.ToString()));
    }

    public static void LogError(object message, bool stackTrace = false) {
        message ??= "NULL";

        if (stackTrace)
            Console.WriteLine("[ERR]" + StackTraceFormatter(message.ToString()));
        else
            Console.WriteLine("[ERR]" + SimpleFormatter(message.ToString()));
    }
}
