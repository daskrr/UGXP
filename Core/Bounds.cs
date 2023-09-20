using UGXP.Tools;

namespace UGXP.Core;
public abstract class Bounds
{
    protected List<Vector2> points = new();

    internal abstract void DrawBounds();
}

public class CircleBounds : Bounds
{
    public Vector2 center {
        get {
            if (points.Count > 0)
                return points[0];

            return Vector2.zero;
        }
        set {
            points.Clear();
            points.Add(value);
        }
    }

    public float radius;

    public CircleBounds(Vector2 center, float radius) {
        points.Add(center);
        this.radius = radius;
    }

    [Obsolete("This method is obsolete, use the center field instead.")]
    public void SetCenter(Vector2 center) {
        points[0] = center;
    }

    internal override void DrawBounds() {
        if (points.Count == 0) return;

        Gizmos.DrawCircle(points[0] /* to world space using transform */, radius);
    }
}
