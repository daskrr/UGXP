using UGXP.Assets;
using UGXP.Core;
using UGXP.Core.Components;
using UGXP.Reference;

namespace UGXP.Game.Manager;
public class SceneManager
{
    private static SceneManager Instance;

    protected List<SceneStructure> sceneData;

    private readonly Scene dontDestroyOnLoad;

    private int currentSceneIndex = -1;
    private Scene currentScene;

    internal SceneManager() {
        if (Instance != null)
            throw new InvalidOperationException("Cannot re-instantiate the SceneManager!");

        Instance = this;
        dontDestroyOnLoad = new Scene(new SceneStructure {
            Name = "DontDestroyOnLoad",
            Objects = Array.Empty<GameObjectStructure>()
        });
    }

    /// <summary>
    /// Establishes the scenes at the game start
    /// </summary>
    /// <param name="scenes"></param>
    internal void SetScenes(SceneStructure[] scenes) {
        if (sceneData != null)
            throw new InvalidOperationException("Cannot set the scenes once the game has loaded!");

        sceneData = new List<SceneStructure>(scenes);
    }

    /// <summary>
    /// Loads the first scene internally inside the engine.
    /// Only to be used for the first scene.
    /// </summary>
    internal void Load() {
        if (currentScene != null)
            throw new InvalidOperationException("Cannot load a new scene as one is already loaded! Use LoadScene instead.");

        dontDestroyOnLoad.Subscribe();
        LoadScene(0);
    }

    internal bool SceneExists(int index) {
        return index < 0 || index >= sceneData.Count;
    }

    /// <summary>
    /// Gets the active scene
    /// </summary>
    /// <returns>The active scene</returns>
    public static Scene GetActiveScene() {
        return Instance.currentScene;
    }
    /// <summary>
    /// Gets the active scene's index.
    /// </summary>
    /// <returns>The active scene index</returns>
    public static int GetActiveSceneIndex() {
        return Instance.currentSceneIndex;
    }

    // TODO make option to load scene in background (additively) (no rendering?)
    /// <summary>
    /// Loads a scene based on its index, making it the currently active scene and unloading the previously active scene
    /// </summary>
    /// <param name="sceneIndex">the scene to be loaded's index</param>
    public static void LoadScene(int sceneIndex) {
        // unsubscribe the current scene
        if (Instance.currentScene != null) {
            GameObject.Destroy(Instance.currentScene);
        }

        Scene loadedScene = new(Instance.sceneData[sceneIndex]);

        // removed due to the change in time of awake to the game object creation
        //// awake components
        //allObjects.ForEach(gameObject => {
        //    gameObject.AwakeComponents();
        //});

        // initialize all components of the scene with properties (so that all references have been created)
        LazyComponent.InitializeAll();


        // load all assets in the asset manager once the scene loads (after the components are initialized duuh)
        // the assets are taken from game objects' and components' fields. If any assets are used outside of fields,
        // they need to be manually added if they are to be pre loaded with the scene.
        List<string> assetNames = new();

        List<GameObject> allObjects = loadedScene.GetAll();
        foreach (GameObject obj in allObjects) {
            assetNames.AddRange(AssetManager.GatherAssetNamesFromObject(obj));

            foreach (Component comp in obj.GetComponents())
                assetNames.AddRange(AssetManager.GatherAssetNamesFromObject(comp));
        }
        // as well as the assets added manually
        assetNames.AddRange(loadedScene.sceneStructure.LoadAssetsByName);
        AssetManager.Load(assetNames.ToArray());


        // subscribe the scene for updates and all that
        loadedScene.Subscribe();

        // start components
        allObjects.ForEach(gameObject => {
            gameObject.StartComponents();
        });

        Instance.currentScene = loadedScene;
        Instance.currentSceneIndex = sceneIndex;

        // move top level DontDestroyOnLoad objects to their own scene
        foreach (var gameObject in Instance.currentScene)
            if (gameObject.dontDestroyOnLoad) {
                Instance.currentScene.Remove(gameObject);
                Instance.dontDestroyOnLoad.Add(gameObject);
            }
    }

    internal Scene[] GetActiveScenes() {
        return new Scene[] {
            dontDestroyOnLoad,
            GetActiveScene()
        };
    }
}
