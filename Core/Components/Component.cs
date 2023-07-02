using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Core.Components;

public class Component : IComponentHolder
{
    /// <summary>
    /// The game object which owns this component
    /// </summary>
    public GameObject gameObject;

    /// <summary>
    /// The tag of the game object
    /// </summary>
    public string tag;

    /// <summary>
    /// The transform of the game object
    /// </summary>
    public Transform transform;

    // disabled for performance reasons, adding such methods will work, but they will not be overrides
    //public virtual void Awake() { }
    //public virtual void Start() { }

    //public virtual void EarlyUpdate() { }

    //public virtual void Update() { }
    //public virtual void FixedUpdate() { }

    //public virtual void LateUpdate() { }

    //public virtual void Destroy() { }

    //public virtual void OnGizmosDraw() { }

    public Component AddComponent(Component component)
    {
        return gameObject.AddComponent(component);
    }

    public Component AddComponent(Type component)
    {
        return gameObject.AddComponent(component);
    }

    public T AddComponent<T>() where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    public Component GetComponent(Type component)
    {
        return gameObject.GetComponent(component);
    }

    public T GetComponent<T>() where T : Component
    {
        return gameObject.GetComponent<T>();
    }

    public Component[] GetComponents()
    {
        return gameObject.GetComponents();
    }

    public Component[] GetComponents(Type component)
    {
        return gameObject.GetComponents(component);
    }

    public T[] GetComponents<T>() where T : Component
    {
        return gameObject.GetComponents<T>();
    }

    public Component RemoveComponent(Component component)
    {
        return gameObject.RemoveComponent(component);
    }

    public Component RemoveComponent(Type component)
    {
        return gameObject.RemoveComponent(component);
    }

    public T RemoveComponent<T>() where T : Component
    {
        return gameObject.RemoveComponent<T>();
    }

    public Component[] RemoveComponents(Type component)
    {
        return gameObject.RemoveComponents(component);
    }

    public T[] RemoveComponents<T>() where T : Component
    {
        return gameObject.RemoveComponents<T>();
    }
}
