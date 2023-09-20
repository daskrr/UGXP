using Differ.Shapes;
using UGXP.Assets;
using UGXP.Game.Manager;

namespace UGXP.Core.Components;

public class Collider : Component
{
    // removed due to specific bounds needed depending on type of collider
    //internal Bounds Bounds;
    private bool isTrigger = false;
    public bool IsTrigger {
        get {
            return isTrigger;
        }
        set {
            isTrigger = value;
            // when a collider changes from or to trigger, the method callbacks need to be updated for the collision manager
            if (gameObject != null)
                GameObjectManager.Update(gameObject);
        }
    }
    public PhysicsMaterial Material;

    private Vector2 offset = Vector2.zero;
    /// <summary>
    /// A collider is centered around the pivot point of the game object.<br/>
    /// This offset is related to that pivot point.
    /// </summary>
    public Vector2 Offset {
        get {
            return offset;
        }
        set {
            offset = value;

            // update shape
            if (diffShapes.Length > 0 && transform != null)
                foreach (var shape in diffShapes) {
                    shape.x = transform.position.x + offset.x;
                    shape.y = transform.position.y + offset.y;
                }
        }
    }

    /// <summary>
    /// The Differ shapes for the SAT collision detection algorithm
    /// </summary>
    internal Shape[] diffShapes;
}
