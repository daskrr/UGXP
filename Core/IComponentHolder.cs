using UGXP.Core.Components;

namespace UGXP.Core;
public interface IComponentHolder
{

    /// <summary>
    /// Adds the initial components to a game object from its structure.
    /// </summary>
    /// <param name="components">The components to be added</param>
    /// <returns>The added components</returns>
    public void InitialAddComponents(LazyComponent[] components);

    /// <summary>
    /// Adds an already existing array of component instances to this game object.
    /// </summary>
    /// <param name="components">The components to be added</param>
    /// <returns>The added components</returns>
    public Component[] AddComponents(Component[] components);
    /// <summary>
    /// Adds an already existing component instance to this game object.
    /// </summary>
    /// <param name="component">The component to be added</param>
    /// <returns>The added component</returns>
    public Component AddComponent(Component component);
    /// <summary>
    /// Adds a new component to this game object.
    /// </summary>
    /// <param name="component">The component type to be created</param>
    /// <returns>The added component</returns>
    public Component AddComponent(Type component);
    /// <summary>
    /// Adds a list of new components to this game object.
    /// </summary>
    /// <param name="components">The components' types to be created</param>
    /// <returns>The added components</returns>
    public Component[] AddComponents(Type[] components);
    /// <summary>
    /// Adds a new component to this game object.
    /// </summary>
    /// <typeparam name="T">The type of the component to be created</typeparam>
    /// <returns>The added component</returns>
    public T AddComponent<T>() where T : Component;

    /// <summary>
    /// Removes a component from this game object based on an instance reference.
    /// </summary>
    /// <param name="component">The component instance</param>
    /// <returns>The component before being removed</returns>
    public Component RemoveComponent(Component component);
    /// <summary>
    /// Removes a component from this game object.
    /// </summary>
    /// <param name="component">The component type</param>
    /// <returns>The component before being removed</returns>
    public Component RemoveComponent(Type component);
    /// <summary>
    /// Removes a component from this game object.
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <returns>The component before being removed</returns>
    public T RemoveComponent<T>() where T : Component;

    /// <summary>
    /// Removes all components of a type from this game object.
    /// </summary>
    /// <param name="component">The component type</param>
    /// <returns>The components before they are removed</returns>
    public Component[] RemoveComponents(Type component);
    /// <summary>
    /// Removes all components of a type from this game object.
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <returns>The components before they are removed</returns>
    public T[] RemoveComponents<T>() where T : Component;

    /// <summary>
    /// Gets a component of type from this game object.
    /// </summary>
    /// <param name="component">The component type</param>
    /// <returns>The component of type</returns>
    public Component GetComponent(Type component);
    /// <summary>
    /// Gets a component of type from this game object.
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <returns>The component of type</returns>
    public T GetComponent<T>() where T : Component;

    /// <summary>
    /// Gets all components of this object.
    /// </summary>
    /// <returns>The components</returns>
    public Component[] GetComponents();
    /// <summary>
    /// Gets all components of type from this game object.
    /// </summary>
    /// <param name="component">The component type</param>
    /// <returns>The components of type</returns>
    public Component[] GetComponents(Type component);
    /// <summary>
    /// Gets all components of type from this game object.
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <returns>The components of type</returns>
    public T[] GetComponents<T>() where T : Component;

    /// <summary>
    /// Invokes the Awake method (If present) from all components in this holder.
    /// </summary>
    public void AwakeComponents();
    /// <summary>
    /// Invokes the Start method (If present) from all components in this holder.
    /// </summary>
    public void StartComponents();
}
