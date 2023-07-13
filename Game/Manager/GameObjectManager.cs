using UGXP.Core;

namespace UGXP.Game.Manager;

public class GameObjectManager
{
    private static GameObjectManager Instance = null;

    internal GameObjectManager() {
        if (Instance != null)
            throw new InvalidOperationException("Cannot re-instantiate the GameObjectManager!");

        Instance = this;
    }

    // make these be out of sync so the game doesn t break due to missing objects
    public static void Subscribe(GameObject obj) {
        if (!obj || !obj.active) return;

        // check so that we only subscribe the object if it wasn't subscribed already
        if (!obj.isSubscribed) {
            GameProcess.Main.updateManager.Add(obj);
            GameProcess.Main.renderManager.Add(obj);
            obj.isSubscribed = true;
        }

        // still try to subscribe the children
        foreach (var child in obj)
            Subscribe(child);
    }
    public static void Update(GameObject obj) {
        GameProcess.Main.updateManager.Update(obj);
        GameProcess.Main.renderManager.Update(obj);

        foreach (var child in obj)
            Update(child);
    }
    public static void Unsubscribe(GameObject obj) {
        GameProcess.Main.updateManager.Remove(obj);
        GameProcess.Main.renderManager.Remove(obj);

        obj.isSubscribed = false;

        foreach (var child in obj)
            Unsubscribe(child);
    }
}
