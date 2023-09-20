using OpenTK.Mathematics;
using UGXP.Core;
using UGXP.Core.Render;
using UGXP.Game.Manager;
using UGXP.Tools;

namespace UGXP.Game;

/// <summary>
/// The main Game process. From this class all the systems and the game itself is ran.
/// </summary>
public class GameProcess
{
    private static GameProcess _main;
    public static GameProcess Main { 
        get {
            return _main;
        }
        private set { 
            if (_main != null)
                throw new InvalidOperationException("Cannot reset Game instance.");

            _main = value;
        }
    }

    private readonly GameSettings _settings;
    public GameSettings settings {
        get {
            return (GameSettings) _settings.Clone();
        }
    }
    public readonly DeveloperSettings devSettings;

    internal static GLContext Context;
    public Vector2i ScreenSize;
    
    internal ExecutionManager execManager;

    internal GameObjectManager gameObjectManager;

    internal UpdateManager updateManager;
    internal RenderManager renderManager;
    internal CollisionManager collisionManager;
    internal SceneManager sceneManager;

    public GameProcess(GameSettings settings, DeveloperSettings devSettings = null) {
        Main = this;
        this._settings = settings;
        this.devSettings = devSettings ?? new DeveloperSettings();

        // initialize managers not dependent on gl or game processes
        LayerManager.Initialize(settings.Layers, settings.SortingLayers);
        GameObject.RegisterTags(settings.Tags);

        // initialize context, create window and managers
        Context = new GLContext (this);
        // temporarily casting to int until I make a Vector2Int
		Context.CreateWindow(
            settings.Name,
            settings.FullScreen,
            settings.VSync,
            settings.WindowSize
        );

        Gizmos.Initialize();

        CursorSetup();

        execManager = new ExecutionManager();

        gameObjectManager = new GameObjectManager();
        updateManager = new UpdateManager();
        renderManager = new RenderManager();
        collisionManager = new CollisionManager();
        sceneManager = new SceneManager();
    }

    // TODO check if there is a camera present. If such a component was not added manually, add a default camera automatically!

    /// <summary>
    /// Add all the scenes available to this game. The order of the scenes determines their id.
    /// The scene at index 0 will be loaded by default. If you want a specific scene to load as the first one, place it at the top of this list.
    /// </summary>
    /// <param name="scenes">All the scenes of the game</param>
    public void SetScenes(SceneStructure[] scenes) {
        if (scenes == null || scenes.Length == 0) 
            throw new Exception("SceneStructure cannot be empty!");

        sceneManager.SetScenes(scenes);
    }

    private void CursorSetup() {
        switch (settings.CursorVisibility) {
            case GameSettings.CursorShowMode.SHOW:
                Context.ShowCursor();
                break;
            case GameSettings.CursorShowMode.HIDE:
                Context.HideCursor();
                break;
            case GameSettings.CursorShowMode.DISABLE:
                Context.HideCursor(true);
                break;
        }

        switch (settings.CursorLocking) {
            case GameSettings.CursorLockMode.UNLOCKED:
                Context.UnlockCursor();
                break;
            //case GameSettings.CursorLockMode.WINDOW_LOCKED:
            //    Context.LockCursor(true);
            //    break;
            case GameSettings.CursorLockMode.LOCKED:
                Context.LockCursor();
                break;
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void Start() {
        sceneManager.Load();
        Context.Run();
    }

    // this looks weird now, but once more managers are introduced it will make sense
    internal void Step() {
        // execute the previous next frame queue
        execManager.InvokeQueue();

        updateManager.BeforeStep();

        updateManager.Step();
        collisionManager.Step();
        execManager.Step();

        updateManager.AfterStep();

        // end of frame
        execManager.AfterStep();
    }

    internal void Render() {
        renderManager.Render();
    }

    internal void Close() { }
}

