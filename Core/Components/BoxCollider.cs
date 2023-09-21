using Differ.Shapes;
using UGXP.Tools;

namespace UGXP.Core.Components;
public class BoxCollider : Collider
{
    private Vector2 size = Vector2.zero;
    /// <summary>
    /// The width and height of the collider
    /// </summary>
    public Vector2 Size {
        get {
            return size;
        }
        set {
            size = value;

            // update or recreate the array
            if (diffShapes.Length > 0)
                diffShapes[0] = Polygon.rectangle(
                    transform.position.x + Offset.x,
                    transform.position.y + Offset.y,
                    size.x,
                    size.y
                );
            else
                diffShapes = new[] {
                    diffShapes[0] = Polygon.rectangle(
                        transform.position.x + Offset.x,
                        transform.position.y + Offset.y,
                        size.x,
                        size.y
                    )
                };
        }
    }

    private void Awake() {
        // this needs the world position, but we give it the local position!
        diffShapes = new[] { 
            Polygon.rectangle(
                transform.position.x + Offset.x,
                transform.position.y + Offset.y,
                size.x,
                size.y
            )
        };
    }

    private void OnGizmosDraw() {
        if (diffShapes.Length == 0)
            return;

        Vector2 corner1 = new((diffShapes[0] as Polygon).transformedVertices[0].x, (diffShapes[0] as Polygon).transformedVertices[0].y);
        Vector2 corner2 = new((diffShapes[0] as Polygon).transformedVertices[2].x, (diffShapes[0] as Polygon).transformedVertices[2].y);

        Gizmos.DrawRect(corner1, corner2);
    }
}
