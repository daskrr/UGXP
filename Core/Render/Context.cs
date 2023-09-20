using GLFW;
using UGXP.Game;

namespace UGXP.Core.Render;
internal abstract class Context
{
    public static Type defaultSoundSystem = typeof(SoloudSoundSystem);

    public KeyCallback keyCallback;
    public MouseButtonCallback mouseButtonCallback;
    public MouseCallback mouseCallback;
    public SizeCallback sizeCallback;

    protected const int MAXKEYS = 65535;
    protected const int MAXBUTTONS = 255;

    public readonly GameProcess game;
    protected Window window;
    protected ISoundSystem _soundSystem;

    public static Vector2 originalSize;
    public Vector2 windowSize = Vector2.zero;

    protected InputState[] keys = new InputState[MAXKEYS + 1];
    protected InputState[] mouseButtons = new InputState[MAXBUTTONS + 1];

    protected int keyPressedCount = 0;
    protected bool anyKeyDown = false;

    public Vector2 mousePosition = Vector2.zero;

    public bool cursorLocked = false;
    public bool cursorLockedInWindow = false;

    protected int targetFrameRate = 240;
    protected long lastFrameTime = 0;
    protected long lastFPSTime = 0;
    protected int frameCount = 0;
    protected int lastFPS = 0;
    protected bool vsync = false;

    public ISoundSystem soundSystem {
        get {
            if (_soundSystem == null)
                #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                #pragma warning disable CS8604 // Possible null reference argument.
                InitializeSoundSystem((ISoundSystem) Activator.CreateInstance(defaultSoundSystem));
                #pragma warning restore CS8604
                #pragma warning restore CS8600

            return _soundSystem;
        }

        protected set {
            _soundSystem = value;
        }
    }

    /// <summary>
    /// Creates a context for the render pipeline (i.e. OpenGL) to be contained in
    /// </summary>
    /// <param name="game">The game process</param>
    /// <param name="dimensions">The dimensions used by game logic (world space)</param>
    public Context(GameProcess game) {
        this.game = game;
    }

    #region core functionality
    /// <summary>
    /// Creates a window in which the game will be displayed.
    /// </summary>
    /// <param name="name">The title of the window</param>
    /// <param name="fullScreen">Whether the game will run in Fullscreen | Borderless | Windowed (<see cref="GameSettings.FullscreenMode"/></param>
    /// <param name="vSync">Whether to wait for vertical synchronization</param>
    /// <param name="dimensions">The dimensions of the window</param>
    public virtual void CreateWindow(string name, GameSettings.FullscreenMode fullScreen, bool vSync, Vector2 dimensions) {
        this.vsync = vSync;
        this.windowSize = dimensions;
        originalSize = dimensions.Clone();
    }
    /// <summary>
    /// Runs the main execution loop
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Does all the rendering (using <see cref="GameProcess.Render"/>)
    /// </summary>
    public abstract void Display();
    /// <summary>
    /// Closes the window and does cleanup.
    /// </summary>
    public abstract void Close();
    #endregion

    #region generic helper methods

    public virtual void SetVSync(bool enabled = false) {
        vsync = enabled;
    }
    public abstract void ShowCursor();
    public abstract void HideCursor(bool lockInWindow = false);

    public abstract void UnlockCursor();
    public abstract void LockCursor(bool inWindow = false);

    public abstract void SetScissor(Vector2 position, Vector2 size);

    /// <summary>
    /// Checks if the specified key is pressed (<see cref="InputState.Repeat"/> || <see cref="InputState.Press"/>)
    /// </summary>
    /// <param name="key">The id of the key (<see cref="Key"/>)</param>
    /// <param name="strict">Whether to accept <see cref="InputState.Press"/> or not</param>
    /// <returns>true if the key is pressed</returns>
    public bool GetKey(int key, bool strict = false) {
        if (strict)
            return keys[key] == InputState.Repeat;

        return keys[key] == InputState.Repeat || keys[key] == InputState.Press;
    }

    /// <summary>
    /// Checks if the specified key is pressed (down) (<see cref="InputState.Press"/>)
    /// </summary>
    /// <param name="key">The id of the key (<see cref="Key"/>)</param>
    /// <returns>true if the key is pressed</returns>
    public bool GetKeyDown(int key) {
        return keys[key] == InputState.Press;
    }

    /// <summary>
    /// Checks if the specified key is released (<see cref="InputState.Release"/>)
    /// </summary>
    /// <param name="key">The id of the key (<see cref="Keys"/>)</param>
    /// <returns>true if the key is released</returns>
    public bool GetKeyUp(int key) {
        return keys[key] == InputState.Release;
    }

    public bool AnyKey() {
        return keyPressedCount > 0;
    }

    public bool AnyKeyDown() {
        return anyKeyDown;
    }

    /// <summary>
    /// Checks if the specified mouse button is pressed (<see cref="InputState.Repeat"/> || <see cref="InputState.Press"/>)
    /// </summary>
    /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
    /// <param name="strict">Whether to accept <see cref="InputState.Press"/> or not</param>
    /// <returns>true if the key is pressed</returns>
    public bool GetMouseButton(int button, bool strict = false) {
        if (strict)
            return mouseButtons[button] == InputState.Repeat;

        return mouseButtons[button] == InputState.Repeat || mouseButtons[button] == InputState.Press;
    }

    /// <summary>
    /// Checks if the specified mouse button is pressed (down) (<see cref="InputState.Press"/>)
    /// </summary>
    /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
    /// <returns>true if the key is pressed</returns>
    public bool GetMouseButtonDown(int button) {
        return mouseButtons[button] == InputState.Press;
    }

    /// <summary>
    /// Checks if the specified mouse button is released (<see cref="InputState.Release"/>)
    /// </summary>
    /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
    /// <returns>true if the key is released</returns>
    public bool GetMouseButtonUp(int button) {
        return mouseButtons[button] == InputState.Release;
    }

    public void ResetHitCounters() {
        Array.Clear(keys, 0, MAXKEYS);
        Array.Clear(mouseButtons, 0, MAXBUTTONS);

        anyKeyDown = false;
    }

    public int currentFps {
        get { return lastFPS; }
    }

    public int targetFps {
        get { return targetFrameRate; }
        set {
            if (value < 1)
                targetFrameRate = 1;
            else
                targetFrameRate = value;
        }
    }

    #endregion

    public void InitializeSoundSystem(ISoundSystem system) {
        soundSystem = system;
        soundSystem.Init();
    }
}
