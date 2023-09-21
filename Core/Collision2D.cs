using UGXP.Core.Components;

namespace UGXP.Core;
public class Collision2D
{
    /// <summary>
    /// The incoming collider involved in the collision with <see cref="otherCollider"/>
    /// </summary>
    public Collider collider;
    /// <summary>
    /// The other collider involved in the collision with the <see cref="collider"/>.
    /// </summary>
    public Collider otherCollider;

    internal Collision2D (Collider collider, Collider otherCollider) {
        this.collider = collider;
        this.otherCollider = otherCollider;
    }
}
