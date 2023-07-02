using System;
using System.Collections.Generic;
using System.Drawing;
using UGXP.Core;
using UGXP.Game.Manager;

namespace UGXP.Game;

/// <summary>
/// The main Game process. From this class all the systems and the game itself is ran.
/// </summary>
public class GameProcess
{
    public static GameProcess _main;
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

    internal GLContext glContext;
    public Rectangle RenderRange;
    
    internal UpdateManager updateManager;
    internal GameObjectManager gameObjectManager;
    internal SceneManager sceneManager;

    public GameProcess(GameSettings settings) {
        Main = this;
        this._settings = settings;

        glContext = new GLContext (this);
        // temporarily casting to int until I make a Vector2Int
		glContext.CreateWindow(
            settings.Name,
            (int) settings.GameSize.x,
            (int) settings.GameSize.y,
            settings.FullScreen,
            settings.VSync,
            (int) settings.WindowSize.x,
            (int) settings.WindowSize.y
        );

		RenderRange = new Rectangle (0, 0, (int) settings.GameSize.x, (int) settings.GameSize.y);

        // not really necessary
        gameObjectManager = new GameObjectManager();
        updateManager = new UpdateManager();
        sceneManager = new SceneManager();
    }

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

    /// <summary>
    /// Starts the game, opens a window
    /// </summary>
    public void Start() {
        glContext.Run();
        sceneManager.LoadSceneInternal(0);
    }

    // this looks weird now, but once more managers are introduced it will make sense
    internal void Step() {
        updateManager.BeforeStep();

        updateManager.Step();

        updateManager.AfterStep();
    }

    internal void Render() {
        // TODO
    }
}

