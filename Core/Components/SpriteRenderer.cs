using OpenTK.Mathematics;
using UGXP.Core.Render;
using UGXP.Game;
using UGXP.Util;

namespace UGXP.Core.Components;
public class SpriteRenderer : Renderer
{
	private static uint[] indices {
		get {
			return new uint[] {
				0, 1, 3,
				1, 2, 3
			};
		}
	}

	private Sprite _sprite;
	/// <summary>
	/// The sprite to be rendered for this game object
	/// </summary>
	public Sprite Sprite { 
		get { return _sprite; }
		set {
			_sprite = value;

			if (started) {
				vertices = null;
				InitSprite();
			}
		}
	}
	/// <summary>
	/// Whether to flip the texture horizontally
	/// </summary>
	public bool FlipX = false;
	/// <summary>
	/// Whether to flip the texture vertically
	/// </summary>
	public bool FlipY = false;

	private float[] vertices; // include tex coords
	private int[] buffers;
	private ShaderProgram shader;

	private void Start() {
		shader = ShaderProgram.DefaultTextureShared; // TODO test extensively to see if using a shared shader is a good idea
		InitSprite();
	}

	private void InitSprite() {
		if (buffers != null)
			GameProcess.Context.DisposeBuffers(buffers);

		buffers = GameProcess.Context.CreateTexVerts(GetVertices(), indices, shader);
	}

	// the vertices tex coords are wrong as well as the size of the sprite i think
	internal float[] GetVertices() {
		if (this.vertices == null) {
			float[][] _vertices = (float[][]) Sprite.vertices.Clone();
			float[][] _texCoords = GetTexCoords();
			var verticesList = new List<float>();

			for (int i = 0; i < _vertices.Length; i++) {
				float[] vertex = new float[5];
				Array.Copy(_vertices[i], vertex, _vertices[i].Length);
				Array.Copy(_texCoords[i], 0, vertex, _vertices[i].Length, _texCoords[i].Length);
				verticesList.AddRange(vertex);
			}

			this.vertices = verticesList.ToArray();
		}

		return this.vertices;
	}

	internal float[][] GetTexCoords() {
		float right = !FlipX ? Sprite.texCoords.Right : Sprite.texCoords.Left;
		float left = !FlipX ? Sprite.texCoords.Left : Sprite.texCoords.Right;
		float top = !FlipY ? Sprite.texCoords.Top : Sprite.texCoords.Bottom;
		float bottom = !FlipY ? Sprite.texCoords.Bottom : Sprite.texCoords.Top;

		float[] cRT = new[] { right, top };
		float[] cRB = new[] { right, bottom };
		float[] cLB = new[] { left, bottom };
		float[] cLT = new[] { left, top };

		return new float[][] { cRT, cRB, cLB, cLT };
	}

    internal override void Render() {
		Sprite.texture.Bind();
        shader.Use();

		blendMode.enable();
		Color color = (Color) this.color;
		shader.SetVector4("color", new Vector4(color[0], color[1], color[2], color[3]));

        shader.SetMatrix4("model", transform.matrix);
        shader.SetMatrix4("view", Camera.Main.GetViewMatrix());
        shader.SetMatrix4("projection", Camera.Main.GetOrthographicProjection());

		GameProcess.Context.DrawTriangles(buffers[0], indices);

		BlendMode.NORMAL.enable();

		shader.Unbind();
		Sprite.texture.Unbind();
	}

    public override void OnDestroy() {
        base.OnDestroy();

		shader.Delete();
		GameProcess.Context.DisposeBuffers(buffers);
    }
}
