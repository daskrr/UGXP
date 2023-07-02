using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// The name of the game
    /// </summary>
    public string Name;

    /// <summary>
    /// The size of the game in units
    /// </summary>
    public Vector2 GameSize;
    /// <summary>
    /// The size of the window in pixels
    /// Leave blank for automatically snapping to screen size (TODO)
    /// </summary>
    public Vector2 WindowSize;

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
    /// If the textures in the game are pixel art
    /// </summary>
    public bool PixelArt = true;

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
    /// The layers of the game, assignable to game objects
    /// Used for collisions and physics
    /// </summary>
    public Dictionary<string, int> Layers = new();
    /// <summary>
    /// The layers of the game, assignable to renderable game objects
    /// Used to determine in what order textures are rendered to the screen (and how they overlap)
    /// </summary>
    public Dictionary<string, int> SortingLayers = new();
    /// <summary>
    /// A miniature collision matrix
    /// The specified <see cref="Layers"/>' ids have their collisions disabled
    /// </summary>
    public List<Tuple<int, int>> IgnoreCollisionLayers = new();

    public object Clone() {
        GameSettings gameSettings = (GameSettings) MemberwiseClone();

        return gameSettings;
    }
}
