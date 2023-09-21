using Differ.Shapes;
using UGXP.Tools;

namespace UGXP.Core.Components;
public class CircleCollider : Collider
{
    ///// <summary>
    ///// The bounds of the circle
    ///// </summary>
    //internal CircleBounds Bounds; // MIGHT NOT BE USEFUL SINCE WE HAVE THE DIFFER SHAPE (which acts as bounds)

    private float radius = 1;
    /// <summary>
    /// The radius of the circle
    /// </summary>
    public float Radius {
        get {
            return radius;
        }
        set {
            radius = value;

            if (diffShapes.Length > 0)
                (diffShapes[0] as Circle).radius = radius;
        }
    }

    private void Awake() {
        // TODO get pivot if object is sprite, else use the transform local position
        // REMOVED NO LONGER NEEDED, USING DIFFER SHAPES INSTEAD
        //Bounds = new CircleBounds(Offset + transform./*local*/position, Radius);

        // this needs the world position, but we give it the local position!
        diffShapes = new[] { 
            new Circle(transform.position.x + Offset.x, transform.position.y + Offset.y, Radius)
        };
    }

    private void OnGizmosDraw() {
        //Bounds.DrawBounds();
        Gizmos.DrawCircle(new Vector2(diffShapes[0].x, diffShapes[0].y), Radius);
    }
}
