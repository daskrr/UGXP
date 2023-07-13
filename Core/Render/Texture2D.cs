using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using UGXP.Assets;

namespace UGXP.Core.Render;

public unsafe class Texture2D
{
    internal TextureAsset asset;

    internal TextureUnit slot = TextureUnit.Texture0;
    internal uint Handle = 0;

    public Vector2 size { get; private set; }

    // TODO allow this to be set at construction
    private bool _wrap = false;
    public bool wrap
    {
        get { return _wrap; }
        set {
            _wrap = value;

            // cannot change texture as it was not created
            if (Handle == 0) return;

            // modify tex wrap
            Bind();
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                value ? new int[] { (int) All.Repeat } : new int[] { (int) All.ClampToEdge });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                value ? new int[] { (int) All.Repeat } : new int[] { (int) All.ClampToEdge });
            Unbind();
        }
    }

    internal bool initialized = false;

    public Texture2D(TextureAsset asset) {
        this.asset = asset;
    }

    /// <summary>
    /// Creates a GL Texture
    /// </summary>
    internal void CreateTexture() {
        if (Handle != 0)
            DestroyTexture();

        fixed (uint* pointer = &Handle)
            GL.GenTextures(1, pointer);

        GL.ActiveTexture(slot);
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        // OpenGL has it's texture origin in the lower left corner instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading.
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        using (Stream stream = File.OpenRead(asset.Path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            size = new Vector2(image.Width, image.Height);
        }

        // filter
        GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int) asset.Filter.min });
        GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int) asset.Filter.mag });

        // wrap
        GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
            wrap ? new int[] { (int) All.Repeat } : new int[] { (int) All.ClampToEdge });
        GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
            wrap ? new int[] { (int) All.Repeat } : new int[] { (int) All.ClampToEdge });

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        Unbind();
    }

    /// <summary>
    /// Destroys the GL Texture
    /// </summary>
    internal void DestroyTexture() {
        fixed (uint* pointer = &Handle)
            GL.DeleteTextures(1, pointer);

        Handle = 0;
    }

    internal void Bind() {
        GL.ActiveTexture(slot);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    internal void Unbind() {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    internal void Load() {
        initialized = true;
        CreateTexture();
    }

    internal void Unload() {
        DestroyTexture();
    }
}

