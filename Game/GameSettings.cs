namespace UGXP.Game;
/// <summary>
/// All fundamental and niche settings of a game
/// The settings stated here cannot be changed at runtime!
/// </summary>
public class GameSettings : ICloneable {
    public enum FullscreenMode {
        FULLSCREEN,
        BORDERLESS,
        WINDOWED
    }

    public enum CursorShowMode {
        /// <summary>
        /// The cursor is shown
        /// </summary>
        SHOW,
        /// <summary>
        /// The cursor is hidden
        /// </summary>
        HIDE,
        /// <summary>
        /// The cursor is disabled and cannot physically move, its position is virtually translated from movement.
        /// </summary>
        DISABLE
    }
    public enum CursorLockMode {
        /// <summary>
        /// The cursor is free to move in and out of the window
        /// </summary>
        UNLOCKED,
        /// <summary>
        /// The cursor is free to move inside the window, but cannot exit it.
        /// </summary>
        [Obsolete("Doesn't work for now since GLFW can't handle it...", true)]
        WINDOW_LOCKED,
        /// <summary>
        /// The cursor cannot move
        /// </summary>
        LOCKED
    }

    /// <summary>
    /// The name of the game
    /// </summary>
    public string Name;

    /// <summary>
    /// The size of the game in units (used for orthographic projection)<br/>
    /// DEPRECATED due to the Camera calculating the orthographic projection based on an orthographic size that keeps aspect ratio.<br/>
    /// See <see cref="UGXP.Core.Components.Camera"/>
    /// </summary>
    [Obsolete("The game size ", true)]
    public Vector2 GameSize;
    /// <summary>
    /// The size of the window in pixels<br/>
    /// Leave blank for automatically snapping to screen size (TODO)
    /// </summary>
    public Vector2 WindowSize;

    /// <summary>
    /// Determines how the cursor is shown on the screen (while the window is in focus)<br/>
    /// <see cref="CursorShowMode"/>
    /// </summary>
    public CursorShowMode CursorVisibility = CursorShowMode.SHOW;
    /// <summary>
    /// Determines how the cursor is able to move inside the window and whether it can leave the window<br/>
    /// <see cref="CursorLockMode"/>
    /// </summary>
    public CursorLockMode CursorLocking = CursorLockMode.UNLOCKED;

    /// <summary>
    /// The fullscreen display mode
    /// <see cref="FullscreenMode.WINDOWED"/> | <see cref="FullscreenMode.BORDERLESS"/> | <see cref="FullscreenMode.FULLSCREEN"/>
    /// </summary>
    public FullscreenMode FullScreen = FullscreenMode.WINDOWED;
    /// <summary>
    /// Whether to wait for vSync
    /// </summary>
    public bool VSync = false;

    /// <summary>
    /// Whether to show the cursor
    /// </summary>
    public bool CursorVisible = true;
    /// <summary>
    /// The texture of the cursor
    /// </summary>
    //public Texture2D Cursor;

    /// <summary>
    /// The tags that can be assigned to game objects
    /// </summary>
    public HashSet<string> Tags = new();
    /// <summary>
    /// The layers of the game, assignable to game objects<br/>
    /// Used for collisions and physics<br/>
    /// Should contain a "Default" layer (placed anywhere). If one is not present, one will be inserted at the 0th index.<br/>
    /// <b>IMPORTANT</b> The amount of layers cannot exceed 32! (due to usage of bitmasks [<see cref="LayerMask"/>])<br/>
    /// There should not be duplicate layers, however if they are wrongfully created, the first is picked.
    /// </summary>
    public string[] Layers = Array.Empty<string>();
    /// <summary>
    /// The layers of the game, assignable to renderable game objects<br/>
    /// Used to determine in what order textures are rendered to the screen (and how they overlap)<br/>
    /// Should contain a "Default" layer (placed anywhere). If one is not present, one will be inserted at the 0th index.<br/>
    /// There should not be duplicate layers, however if they are wrongfully created, the first is picked.
    /// </summary>
    public string[] SortingLayers = Array.Empty<string>();
    /// <summary>
    /// A miniature collision matrix<br/>
    /// The specified <see cref="Layers"/>' names have their collisions disabled
    /// </summary>
    public List<Tuple<string, string>> IgnoreCollisionLayers = new();

    public object Clone() {
        GameSettings gameSettings = (GameSettings) MemberwiseClone();

        return gameSettings;
    }
}
