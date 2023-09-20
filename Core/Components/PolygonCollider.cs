using Differ.Shapes;

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
}
