using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGXP.Core;

namespace UGXP.Game.Manager;

public class GameObjectManager
{
    private static GameObjectManager Instance = null;

    private List<GameObject> gameObjects = new();

    internal GameObjectManager() {
        if (Instance != null)
            throw new InvalidOperationException("Cannot re-instantiate the GameObjectManager!");

        Instance = this;
    }

    // make these be out of sync so the game doesn t break due to missing objects
    public static void Subscribe(GameObject obj) {
        Instance._subscribe(obj);
    }
    public static void Unsubscribe(GameObject obj) {
        Instance._unsubscribe(obj);
    }

    private void _subscribe(GameObject obj) {
        GameProcess.Main.updateManager.Add(obj); // change to Game.Main.Managers something
        gameObjects.Add(obj);
    }
    private void _unsubscribe(GameObject obj) {
        GameProcess.Main.updateManager.Remove(obj); // change to Game.Main.Managers something
        gameObjects.Add(obj);
    }
}
