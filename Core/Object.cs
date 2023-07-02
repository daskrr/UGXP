using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UGXP.Game.Manager;

namespace UGXP.Core;
public class Object
{
    protected bool active = true;
    protected bool destroyed = false;

    public void SetActive(bool active) {
        this.active = active;
    }

    /// <summary>
    /// Destroys a game object after this frame.
    /// Destroyed objects cannot be added back to the game!
    /// </summary>
    /// <param name="obj">The object to destroy</param>
    public static void Destroy(GameObject obj) {
        GameObjectManager.Unsubscribe(obj);
        obj.destroyed = true;
    }

    public static implicit operator bool(Object obj) {
        return obj != null;
    }

    public static bool operator ==(Object? obj1, Object? obj2) {
        if (obj1 is null && obj2 is null)
            return true;

        if (obj1 is null && obj2 is not null)
            if (obj2.destroyed)
                return true;

        if (obj1 is not null && obj2 is null)
            if (obj1.destroyed)
                return true;

        if (obj1 is null || obj2 is null)
            return false;

        return obj1.GetHashCode() == obj2.GetHashCode();
    }

    public static bool operator !=(Object? obj1, Object? obj2) {
        return !(obj1 == obj2);
    }

    public override bool Equals(object? obj) {
        if (obj == null && destroyed) return true;

        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (ReferenceEquals(obj, null)) {
            return false;
        }

        return false;
    }

}
