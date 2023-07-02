using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Game.Manager;
public class SceneManager
{
    private static SceneManager Instance;

    protected List<SceneStructure> sceneData;

    private int currentSceneId;
    private Scene currentScene;

    internal SceneManager() {
        if (Instance != null)
            throw new InvalidOperationException("Cannot re-instantiate the SceneManager!");

        Instance = this;
    }

    internal void SetScenes(SceneStructure[] scenes) {
        sceneData = new List<SceneStructure>(scenes);
    }

    internal void LoadSceneInternal(int id) {
        if (id < 0 || id >= sceneData.Count)
            throw new IndexOutOfRangeException("The scene with the specified id does not exist!");

        if (currentScene != null)
            throw new InvalidOperationException("Cannot load a new scene as one is already loaded! Use LoadScene instead.");

        Scene loadedScene = new(sceneData[id]);
        loadedScene.Subscribe();

        currentScene = loadedScene;
        currentSceneId = id;
    }

    public static Scene GetActiveScene() {
        return Instance.currentScene;
    }
    public static int GetActiveSceneId() {
        return Instance.currentSceneId;
    }
}
