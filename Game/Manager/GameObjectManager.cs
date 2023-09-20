using UGXP.Core;

namespace UGXP.Game.Manager;

internal class GameObjectManager
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

        // do this next frame as modifying anything at the wrong time can break things (specifically the collision manager)
        ExecutionManager.DoNextFrame(() => {
            // check so that we only subscribe the object if it wasn't subscribed already
            if (!obj.isSubscribed) {
                GameProcess.Main.updateManager.Add(obj);
                GameProcess.Main.collisionManager.Add(obj);
                GameProcess.Main.renderManager.Add(obj);

                obj.isSubscribed = true;
            }
        });

        // still try to subscribe the children
        foreach (var child in obj)
            Subscribe(child);
    }
    public static void Update(GameObject obj) {
        ExecutionManager.DoNextFrame(() => {
            GameProcess.Main.updateManager.Update(obj);
            GameProcess.Main.collisionManager.Update(obj);
            GameProcess.Main.renderManager.Update(obj);
        });

        foreach (var child in obj)
            Update(child);
    }
    public static void Unsubscribe(GameObject obj) {
        ExecutionManager.DoNextFrame(() => {
            GameProcess.Main.updateManager.Remove(obj);
            GameProcess.Main.collisionManager.Remove(obj);
            GameProcess.Main.renderManager.Remove(obj);
        });

        obj.isSubscribed = false;

        foreach (var child in obj)
            Unsubscribe(child);
    }
}
