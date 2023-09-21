using Differ.Shapes;
using UGXP.Tools;

namespace UGXP.Core.Components;
public class PolygonCollider : Collider
{
    private Vector2[] points;
    /// <summary>
    /// The width and height of the collider
    /// </summary>
    public Vector2[] Points {
        get {
            return points;
        }
        set {
            points = value;

            // TODO partition polygon and update all differ shapes
            // use the hertel mehlhorn concave -> covex poly partitioning (probably from polypartition c++ -> c# bindings)
        }
    }

    private void Awake() {
        // this needs the world position, but we give it the local position!
        diffShapes = new Shape[] { 
            // TODO
        };
    }

    private void OnGizmosDraw() {
        if (diffShapes.Length == 0)
            return;
        
        foreach (Polygon shape in diffShapes.Cast<Polygon>()) {
            for (int i = 0; i < shape.transformedVertices.Count; i += 2) {
                Vector2 a = new(shape.transformedVertices[i].x, shape.transformedVertices[i].y);
                Vector2 b = new(shape.transformedVertices[i + 1].x, shape.transformedVertices[i + 1].y);

                Gizmos.DrawLine(a, b);
            }
        }
    }
}
