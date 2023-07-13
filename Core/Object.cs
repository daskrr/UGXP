using UGXP.Game.Manager;

namespace UGXP.Core;
public class Object
{
    internal bool initialized = false;

    internal bool active = true;
    internal bool destroyed = false;
    internal bool dontDestroyOnLoad = false;

    /// <summary>
    /// Sets this object's active status.
    /// Inactive objects don't receive updates and aren't rendered (if applicable).
    /// </summary>
    /// <param name="active">active status</param>
    public virtual void SetActive(bool active) {
        this.active = active;
    }

    /// <summary>
    /// Destroys a game object after this frame.
    /// Destroyed objects cannot be added back to the game!
    /// </summary>
    /// <param name="obj">The object to destroy</param>
    public static void Destroy(Object obj) {
        if (obj is GameObject)
            GameObjectManager.Unsubscribe(obj as GameObject);

        obj.destroyed = true;
        obj.OnDestroy();
    }

    /// <summary>
    /// Makes an object part of the DontDestroyOnLoad scene.<br/>
    /// Once a scene is changed it doesn't get destroyed.<br/>
    /// <b>IMPORTANT: (If the object is a <see cref="GameObject"/>) Only top level objects (whose parent is the scene) 
    /// are moved to the DontDestroyOnLoad scene!</b>
    /// </summary>
    /// <param name="obj">The object</param>
    public static void DontDestroyOnLoad(Object obj) {
        obj.dontDestroyOnLoad = true;

        if (obj is GameObject gameObject)
            foreach (var child in gameObject)
                DontDestroyOnLoad(child);
    }

    public virtual void OnDestroy() { }

    public static implicit operator bool(Object obj) {
        return obj is not null && !obj.destroyed;
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

        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null)
            return false;

        return false;
    }

}
