using OpenTK.Graphics.OpenGL4;
using System.Collections;
using System.Drawing;
using UGXP.Core.Render;

namespace UGXP.Assets;

// to remember this is only a structure that will create Texture2Ds according to the data
/// <summary>
/// This object is a data structure that hold all relevant information for creating, using and storing Texture2Ds.<br/>
/// A texture asset can only point to a singular texture file, that then can be used as a whole or split into multiple Texture2D objects.
/// </summary>
public class TextureAsset : Asset
{
    /// <summary>
    /// Generic Filter types to be used when scaling an image in the viewport.
    /// </summary>
    public class TextureFilter {
        public static readonly TextureFilter NEAREST = new(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        public static readonly TextureFilter LINEAR = new(TextureMinFilter.Linear, TextureMagFilter.Linear);
        public static readonly TextureFilter MIPMAP_NEAREST = new(TextureMinFilter.NearestMipmapLinear, TextureMagFilter.Nearest);
        public static readonly TextureFilter MIPMAP_LINEAR = new(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);

        public readonly TextureMinFilter min;
        public readonly TextureMagFilter mag;
        public TextureFilter(TextureMinFilter min, TextureMagFilter mag) {
            this.min = min;
            this.mag = mag;
        }
    }

    /// <summary>
    /// The pixels per game unit of this texture<br/>
    /// <i>Default: 16</i>
    /// </summary>
    public int PixelsPerUnit = 16;
    /// <summary>
    /// The filtering mode of this texture.<br/>
    /// <i>Default: <see cref="TextureFilter.MIPMAP_LINEAR"/></i>
    /// </summary>
    public TextureFilter Filter = TextureFilter.MIPMAP_NEAREST;
    private TextureMode _textureMode = TextureMode.SINGLE;
    /// <summary>
    /// The <see cref="Assets.TextureMode"/> of this image.<br/>
    /// Determines how the image is stored, processed and displayed.<br/>
    /// <i>Default: <see cref="TextureMode.SINGLE"/></i>
    /// </summary>
    public TextureMode TextureMode {
        get => _textureMode;
        set {
            _textureMode = value;
            if (value == TextureMode.MULTIPLE)
                IsSingle = false;
        }
    }
    /// <summary>
    /// The <see cref="TextureMap"/> of this raw image. Delimitates multiple sprites inside a raw image.
    /// </summary>
    public TextureMap Textures = new TextureMap(); // not to be used if TextureMode is not MULTIPLE
    /// <summary>
    /// The pivot point of the sprite.<br/>
    /// This determines where is the sprite's central point in game coordinates.<br/>
    /// This is only used if the <see cref="TextureMode"/> is <see cref="Assets.TextureMode.SINGLE"/>.
    /// For <see cref="Assets.TextureMode.MULTIPLE"/> textures, the <see cref="Textures"/> map contains each of their Pivots.<br/>
    /// </summary>
    public PivotPoint Pivot = PivotPoint.CENTER;

    /// <summary>
    /// This holds the entire texture and is used by the returning Sprites which are AssetInstances.<br/>
    /// This is created when GetAssets() is invoked and loaded/initialized when the sprites (AssetInstances) are loaded/initialized.
    /// </summary>
    private Texture2D texture;

    public TextureAsset() { }

    internal override List<string> GetAssetNames() {
        if (TextureMode == TextureMode.SINGLE || TextureMode == TextureMode.WRAP || TextureMode == TextureMode.SLICED)
            return new List<string> { Name };
        else if (TextureMode == TextureMode.MULTIPLE) {
            List<string> names = new();

            foreach (var texture in Textures)
                names.Add(texture.Name);

            return names;
        }

        return new List<string> { Name };
    }

    internal override Dictionary<string, AssetInstance> GetAssets() {
        CheckForFile();

        if (TextureMode == TextureMode.SINGLE)
            return new Dictionary<string, AssetInstance>() { { this.Name, GetSingleSprite() } };
        else if (TextureMode == TextureMode.WRAP)
            return new Dictionary<string, AssetInstance>() { { this.Name, GetSingleSprite(true) } };
        // TODO SLICED
        else if (TextureMode == TextureMode.MULTIPLE) {
            texture = new Texture2D(this);
            Dictionary<string, AssetInstance> assets = new();

            foreach (var texture in Textures)
                assets.Add(texture.Name, GetMultiSprite(texture.Name, texture.Bounds, texture.Pivot));

            return assets;
        }

        throw new NotImplementedException("TextureMode.SLICED is not yet implemented :(");
    }

    private AssetInstance GetSingleSprite(bool wrap = false) {
        texture = new Texture2D(this) { wrap = wrap };
        return new Sprite(texture) { 
            name = this.Name, 
            asset = this,
            origin = Vector2.zero,
            pivot = Pivot,
        };
    }
    private AssetInstance GetMultiSprite(string name, Rectangle bounds, PivotPoint pivot) {
        return new Sprite(texture) {
            name = this.Name,
            asset = this, 
            size = new Vector2(bounds.Size.Width, bounds.Size.Height),
            origin = new Vector2(bounds.X, bounds.Y),
            pivot = pivot
        };
    }

    private void CheckForFile() {
        if (!File.Exists(Path)) throw new FileNotFoundException("The file referenced in Path could not be found.");
    }
}

/// <summary>
/// Determines how the raw image is stored and displayed
/// </summary>
public enum TextureMode {
    /// <summary>
    /// The whole image is taken as a Texture2D
    /// </summary>
    SINGLE = 0,
    /// <summary>
    /// This acts like a single image, but wraps.
    /// </summary>
    WRAP = 1,
    /// <summary>
    /// The raw image is split in multiple Texture2Ds based on a TextureMap
    /// </summary>
    MULTIPLE = 2,
    /// <summary>
    /// UNIMPLEMENTED YET
    /// The texture's margins are marked and the texture can be resized without the margins being stretched, only the middle.
    /// </summary>
    SLICED = 3
}

public struct ChildTexture {
    public string Name;
    public Rectangle Bounds;
    public PivotPoint Pivot;
}

/// <summary>
/// The TextureMap helps deliminate a raw image's contained textures.<br/>
/// Can be used to separate multiple sprites from an image and be used as separate assets or to create an animation from a spritesheet.<br/>
/// Recommended usage is {} instantiation:<br/>
/// <i>new <see cref="TextureMap"/> { 
/// { "CoolPartOfImage", 0,0, 16,16 }
/// }</i><br/>
/// <see cref="Add(string, int, int, int, int)"/>
/// <see cref="Add(string, int[])"/>
/// </summary>
public class TextureMap : IEnumerable<ChildTexture>
{
    protected Dictionary<string, Rectangle> map = new();
    protected Dictionary<string, PivotPoint> pivots = new();

    public ChildTexture this[string key] { 
        get => new ChildTexture() {
            Name = key,
            Bounds = map[key],
            Pivot = pivots[key]
        };
    }
    public ICollection<string> Keys => map.Keys;
    public ICollection<ChildTexture> Values {
        get { 
            List<ChildTexture> values = new();

            foreach (var pair in map)
                values.Add(new ChildTexture() {
                    Name = pair.Key,
                    Bounds = pair.Value,
                    Pivot = pivots[pair.Key]
                });

            return values;
        }
    }
    public int Count => map.Count;

    /// <summary>
    /// Adds a new entry to the texture map.
    /// </summary>
    /// <param name="name">The name of the sprite</param>
    /// <param name="value">The coordinates of the sprite. X,Y, Width,Height (in pixels)</param>
    /// <param name="pivot">The pivot of the image (in pixels)</param>
    public void Add(string name, Rectangle value, PivotPoint pivot) {
        map.Add(name, value);
        pivots.Add(name, pivot);
    }
    /// <summary>
    /// Adds a new entry to the texture map.
    /// </summary>
    /// <param name="name">The name of the sprite</param>
    /// <param name="value">The coordinates of the sprite. X,Y, Width,Height (in pixels)</param>
    /// <param name="pivot">The pivot of the image (in pixels)</param>
    public void Add(string name, int[] value, PivotPoint pivot) {
        map.Add(name, new Rectangle(value[0], value[1], value[2], value[3]));
        pivots.Add(name, pivot);
    }
    /// <summary>
    /// Adds a new entry to the texture map.
    /// </summary>
    /// <param name="name">The name of the sprite</param>
    /// <param name="x">The x position in the image (in pixels)</param>
    /// <param name="y">The y position in the image (in pixels)</param>
    /// <param name="width">The width of the sprite (in pixels)</param>
    /// <param name="height">The height of the sprite (in pixels)</param>
    /// <param name="pivot">The pivot of the image (in pixels)</param>
    public void Add(string name, int x, int y, int width, int height, PivotPoint pivot) {
        map.Add(name, new Rectangle(x, y, width, height));
        pivots.Add(name, pivot);
    }

    public IEnumerator<ChildTexture> GetEnumerator() {
        foreach (var childTex in map)
            yield return new ChildTexture() {
                Name = childTex.Key,
                Bounds = childTex.Value,
                Pivot = pivots[childTex.Key]
            };
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

public class SpriteSheet : TextureMap
{
    private Vector2 textureSize;

    private readonly int columnSize;
    private readonly int rowSize;

    private readonly int columns; // x-axis
    private readonly int rows; // y-axis
    public SpriteSheet(int columns, int rows, int columnSize, int rowSize) {
        this.columns = columns;
        this.rows = rows;
        this.columnSize = columnSize;
        this.rowSize = rowSize;
        this.textureSize = new Vector2(columnSize * columns, rowSize * rows);
    }

    /// <summary>
    /// Adds a new entry to the sprite sheet
    /// </summary>
    /// <param name="name">The name of the sprite</param>
    /// <param name="row">The row this sprite is at</param>
    /// <param name="column">The column this sprite is at</param>
    /// <param name="pivot">The pivot of the image (in pixels)</param>
    public void Add(string name, int column, int row, PivotPoint pivot) {
        if (column >= columns)
            throw new ArgumentOutOfRangeException(nameof(column), $"Out of range! The column is treated as an index! [0-{columns - 1}]");
        if (row >= rows)
            throw new ArgumentOutOfRangeException(nameof(column), $"Out of range! The row is treated as an index! [0-{rows - 1}]");

        base.Add(name, columnSize * column, rowSize * row, columnSize, rowSize, pivot);
    }
}

public class TexCoords {
    public float Right;
    public float Left;
    public float Top;
    public float Bottom;
}

public class PivotPoint {
    public static readonly PivotPoint CENTER = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x / 2;
        pivot.y = -size.y / 2;

        return pivot;
    });
    public static readonly PivotPoint TOP_LEFT = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.y = -size.y;

        return pivot;
    });
    public static readonly PivotPoint TOP_CENTER = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x / 2;
        pivot.y = -size.y;

        return pivot;
    });
    public static readonly PivotPoint TOP_RIGHT = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x;
        pivot.y = -size.y;

        return pivot;
    });
    public static readonly PivotPoint BOTTOM_LEFT = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;

        return pivot;
    });
    public static readonly PivotPoint BOTTOM_CENTER = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x / 2;

        return pivot;
    });
    public static readonly PivotPoint BOTTOM_RIGHT = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x;

        return pivot;
    });
    public static readonly PivotPoint LEFT_BOTTOM = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;

        return pivot;
    });
    public static readonly PivotPoint LEFT_CENTER = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.y = -size.y / 2;

        return pivot;
    });
    public static readonly PivotPoint LEFT_TOP = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.y = -size.y;

        return pivot;
    });
    public static readonly PivotPoint RIGHT_BOTTOM = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x;

        return pivot;
    });
    public static readonly PivotPoint RIGHT_CENTER = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x;
        pivot.y = -size.y / 2;

        return pivot;
    });
    public static readonly PivotPoint RIGHT_TOP = new PivotPoint(size => {
        Vector2 pivot = Vector2.zero;
        pivot.x = -size.x;
        pivot.y = -size.y;

        return pivot;
    });

    public readonly Func<Vector2, Vector2> calculate;

    private PivotPoint(Func<Vector2, Vector2> calc) {
        calculate = calc;
    }

    public PivotPoint(Vector2 position) {
        calculate = _ => position;
    }
}