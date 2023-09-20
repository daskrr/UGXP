using UGXP.Game;
using static UGXP.Game.DeveloperSettings;

namespace UGXP.Util;
internal class DevelopmentHandlers
{
    public static T HandleValueNotFound<T>(string message) {
        ValueHandle valueHandle;
        if (GameProcess.Main == null)
            valueHandle = ValueHandle.THROW_EXCEPTION;
        else
            valueHandle = GameProcess.Main.devSettings.ValueNotFoundHandling;

        if (valueHandle == ValueHandle.THROW_EXCEPTION)
            throw new ValueNotFoundException(message);
        else if (valueHandle == ValueHandle.SILENT_ERROR)
            Debug.LogError("[Engine] " + message);

        // can't always retun null, in case of primitives
        return default;
    }
}

public class ValueNotFoundException : Exception {
    public ValueNotFoundException(string message) : base(Debug.SimpleFormatter(message)) { }
}
