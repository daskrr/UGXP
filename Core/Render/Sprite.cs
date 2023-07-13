using UGXP.Assets;
using UGXP.Util;

namespace UGXP.Core.Render;

// NOTE: Sprites do not store their own name, rather the name of the texture asset
public class Sprite : AssetInstance
{
    internal Texture2D texture;

    public Vector2 size { get; internal set; }
    internal Vector2 origin;
    internal PivotPoint pivot;
    internal TexCoords texCoords;
    internal float[][] vertices;

    public Sprite(Texture2D texture) {
        this.texture = texture;
    }

    internal float[][] GetVertices() {
        float[][] vertices = new float[4][];

        // apply pixels per inch
        // pixels per inch mean how many texture pixels fit inside of a game unit.
        // divide width and height by the pixels per inch value
        float accWidth = size.x / (float) (asset as TextureAsset).PixelsPerUnit;
        float accHeight = size.y / (float) (asset as TextureAsset).PixelsPerUnit;

        Vector2 pivotPoint = pivot.calculate(new Vector2(accWidth, accHeight));

        FRectangle bounds = new FRectangle(pivotPoint, new Vector2(accWidth, accHeight));

        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(bounds.right, bounds.bottom);
        corners[1] = new Vector2(bounds.right, bounds.top);
        corners[2] = new Vector2(bounds.left, bounds.top);
        corners[3] = new Vector2(bounds.left, bounds.bottom);

        vertices[0] = new float[] {
            corners[0].x, corners[0].y, 0f,
        };
        vertices[1] = new float[] {
            corners[1].x, corners[1].y, 0f,
        };
        vertices[2] = new float[] {
            corners[2].x, corners[2].y, 0f,
        };
        vertices[3] = new float[] {
            corners[3].x, corners[3].y, 0f,
        };

        return vertices;
    }

    private TexCoords GenTexCoords() {
        FRectangle rect = new FRectangle(origin.Clone(), new Vector2(size.x, size.y));

        // divide the values by the size of the texture
        return new TexCoords {
            Right = (float) rect.right / texture.size.x,
            Left = (float) rect.left / texture.size.x,
            Top = InversePointY((float) rect.top) / texture.size.y,
            Bottom = InversePointY((float) rect.bottom) / texture.size.y,
        };
    }

    private float InversePointY(float point) {
        return (1 - (point / texture.size.y)) * texture.size.y;
    }

    internal override void Load() {
        base.Load();

        // load texture2D
        if (!texture.initialized)
            texture.Load();

        if (size == null)
            size = texture.size;

        texCoords = GenTexCoords();

        // calculate vertices
        vertices = GetVertices();
    }

    internal override void Unload() {
        if (texture.initialized)
            texture.Unload();
    }
}
