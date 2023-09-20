using UGXP.Core.Components;
using UGXP.Core.Render;
using UGXP.Game;
using OpenTK.Mathematics;

namespace UGXP.Tools;

public sealed class Gizmos
{
    private static Gizmos Instance;

    public static void Initialize() {
        Instance = new Gizmos();
    }

    private List<uint> indicesQueue = new();
    private List<float> verticesQueue = new();

    private ShaderProgram shader;
    private Gizmos() {
        shader = ShaderProgram.DefaultGizmos;
    }

    internal static void Render() {
        // render user-added line vertices
        int[] buffers = GameProcess.Context.CreateVerts(Instance.verticesQueue.ToArray(), Instance.shader, Instance.indicesQueue.ToArray());

        BlendMode.NORMAL.enable();

        Instance.shader.Use();

        Instance.shader.SetVector4("color", new Vector4(1f,1f,1f,1f));

        Instance.shader.SetMatrix4("view", Camera.Main.GetViewMatrix());
        Instance.shader.SetMatrix4("projection", Camera.Main.GetOrthographicProjection());

        GameProcess.Context.DrawLines(buffers[0], Instance.indicesQueue.ToArray());
        GameProcess.Context.DisposeBuffers(buffers);

        Instance.shader.Unbind();

        Instance.verticesQueue.Clear();
        Instance.indicesQueue.Clear();

        // render collider boxes
        // TODO
    }

    private static void AddToQueue(float[] vertices, uint[] indices) {
        List<uint> indicesList = new();

        uint max = 0;
        foreach (var index in Instance.indicesQueue)
            max = Math.Max(max, index + 1);

        foreach (var index in indices)
            indicesList.Add(index + max);

        Instance.verticesQueue.AddRange(vertices);
        Instance.indicesQueue.AddRange(indicesList.ToArray());
    }

    /// <summary>
    /// Draws a line on top of everything.
    /// </summary>
    /// <param name="color">(optional) the color of the line</param>
    public static void DrawLine(Vector2 a, Vector2 b) {
        // create vertices
        float[] verts = {
             a.x, a.y,
             b.x, b.y
        };

        uint[] indices = {
            0, 1
        };

        AddToQueue(verts, indices);
    }

    /// <summary>
    /// Draws a hollow rectangle on top of everything.<br/>
    /// The corners are: Bottom Left and Top Right
    /// </summary>
    /// <param name="corner1">the bottom left corner</param>
    /// <param name="corner2">the top right corner</param>
    /// <param name="color">(optional) the color of the line</param>
    public static void DrawRect(Vector2 corner1, Vector2 corner2) {
        Vector2 corner1a = new Vector2(corner1.x, corner2.y);
        Vector2 corner2a = new Vector2(corner2.x, corner1.y);

        // create vertices
        float[] verts = {
            corner1.x, corner1.y,
            corner2a.x, corner2a.y,
            corner2.x, corner2.y,
            corner1a.x, corner1a.y,
        };

        uint[] indices = {
            0, 1, 
            1, 2, 
            2, 3, 
            3, 0
        };

        AddToQueue(verts, indices);
    }

    /// <summary>
    /// Draws a hollow circle on top of everything
    /// </summary>
    /// <param name="center">the center of the circle</param>
    /// <param name="radius">the radius of the circle</param>
    /// <param name="fidelity">how many lines to be used in the creation of the circle</param>
    public static void DrawCircle(Vector2 center, float radius, int fidelity = 30) {
        float[] vertices = new float[fidelity * 2];
        uint[] indices = new uint[fidelity * 2];

        int j = 0;
        for (int i = 0; i < fidelity; i++) {
            float angle = Mathf.DegreesToRadians((360f / fidelity) * i);

            vertices[j] = radius * Mathf.Cos(angle) + center.x;
            indices[j] = (uint) i;
            j++;

            vertices[j] = radius * Mathf.Sin(angle) + center.y;
            indices[j] = (uint) (i == fidelity - 1 ? 0 : i + 1);
            j++;
        }

        AddToQueue(vertices, indices);
    }

    ~Gizmos() {
        shader.Delete();
    }
}
