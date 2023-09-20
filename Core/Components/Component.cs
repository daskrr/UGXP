using System.Reflection;
using UGXP.Game.Manager;
using UGXP.Reference;

namespace UGXP.Core.Components;

public class Component : Object, IComponentHolder, IReferenceable<Component>
{
    internal bool started;

    /// <summary>
    /// The game object which owns this component
    /// </summary>
    public GameObject gameObject { get; internal set; }

    /// <summary>
    /// The tag of the game object
    /// </summary>
    public string tag {
        get { return gameObject.tag; }
    }

    /// <summary>
    /// The transform of the game object
    /// </summary>
    public Transform transform {
        get { return gameObject.transform; }
    }

    /// <summary>
    /// The reference name of this component (if any)
    /// </summary>
    public string? referenceName;
    protected ObjectReference<Component> reference;

    /// <summary>
    /// Components cannot be instantiated from outside the engine.<br/>
    /// Use <see cref="Create{T}(Action{T})"/> or <see cref="Create{T}(Action{T}, Action{T})"/>
    /// </summary>
    internal Component() { }

    /// <summary>
    /// This is required as components are constructed, then their constructor function is called, then this is called.<br/>
    /// This imitates how the constructor is called after the fields are set for an object initializer.
    /// </summary>
    internal void Constructor() {
        GetReference(); // create reference for the first time
    }

    // disabled for performance reasons, adding such methods will work, but they will not be overrides
    // they can also be private, protected, internal and still, they will be invoked.
    //public virtual void Awake() { }
    //public virtual void Start() { }

    //public virtual void EarlyUpdate() { }

    //public virtual void Update() { }
    //public virtual void FixedUpdate() { }

    //public virtual void LateUpdate() { }

    //public virtual void OnGizmosDraw() { }

    // components are supposed to be referenceable, however in order for everything to work smoothly,
    // the top level component class needs to implement IReferenceable
    // the base component will still implement IReferenceable, just in case

    public virtual ObjectReference<Component> GetReference() {
        reference ??= ReferenceManager.Create<Component>(referenceName, this);
        return reference;
    }

    public override void SetActive(bool active) {
        base.SetActive(active);
        GameObjectManager.Update(gameObject);
    }

    public override void OnDestroy() {
        base.OnDestroy();

        ReferenceManager.RemoveReference(reference.id);
    }

    #region component holder -> game object component holder
    public void InitialAddComponents(LazyComponent[] components) {
        throw new InvalidOperationException("Components have already been added at scene load. Cannot add other initial components!");
    }
    public Component[] AddComponents(Component[] components) {
        return gameObject.AddComponents(components);
    }
    public Component AddComponent(Component component) {
        return gameObject.AddComponent(component);
    }
    public Component AddComponent(Type component) {
        return gameObject.AddComponent(component);
    }
    public Component[] AddComponents(Type[] components) {
        return gameObject.AddComponents(components);
    }
    public T AddComponent<T>() where T : Component {
        return gameObject.AddComponent<T>();
    }
    public Component GetComponent(Type component) {
        return gameObject.GetComponent(component);
    }
    public T GetComponent<T>() where T : Component {
        return gameObject.GetComponent<T>();
    }
    public Component[] GetComponents() {
        return gameObject.GetComponents();
    }
    public Component[] GetComponents(Type component) {
        return gameObject.GetComponents(component);
    }
    public T[] GetComponents<T>() where T : Component {
        return gameObject.GetComponents<T>();
    }
    public Component RemoveComponent(Component component) {
        return gameObject.RemoveComponent(component);
    }
    public Component RemoveComponent(Type component) {
        return gameObject.RemoveComponent(component);
    }
    public T RemoveComponent<T>() where T : Component {
        return gameObject.RemoveComponent<T>();
    }
    public Component[] RemoveComponents(Type component) {
        return gameObject.RemoveComponents(component);
    }
    public T[] RemoveComponents<T>() where T : Component {
        return gameObject.RemoveComponents<T>();
    }
    public void AwakeComponents() {
        gameObject.AwakeComponents();
    }
    public void StartComponents() {
        gameObject.StartComponents();
    }
    #endregion
    

    /// <summary>
    /// Creates a component. Components <b>MUST</b> be created through this method as this holds desynchronized initializers.<br/>
    /// The initializer is an <see cref="Action"/> that gives the component instance as a parameter. Here, all properties of a
    /// component can be set.<br/>
    /// <i>Note: The initializer is invoked after all the game objects are added, and their references are created.</i><br/>
    /// <i>IMPORTANT: If your component is name-referenceable, use <see cref="Create{T}(Action{T}, Action{T})"/>'s <b>creator</b>
    /// parameter. This action will be executed at instatiation, making it the right time to set a reference name</i>
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <param name="initializer">The initializer function</param>
    /// <returns>The LazyComponent that will be used every time a scene is loaded to generate a new component</returns>
    public static LazyComponent<T> Create<T>(Action<T> initializer) where T : Component {
        return new LazyComponent<T>(_ => { }, initializer);
    }
    /// <summary>
    /// Creates a component. Components <b>MUST</b> be created through this method as this holds desynchronized initializers.<br/>
    /// The creator is an <see cref="Action"/> that gives the component instance as a parameter. Here some properties of a component
    /// can be set.<br/>
    /// <br/>
    /// <i>Note: The creator is invoked at instantiation of the component. This point in time is dependent on the position of the
    /// component in the components of a game object and the game object's position in the scene.</i><br/>
    /// <b>A reference name needs to be set here in order for it to work.</b><br/>
    /// <br/>
    /// The initializer is an <see cref="Action"/> that gives the component instance as a parameter. Here, all properties of a
    /// component can be set.<br/>
    /// <i>Note: The initializer is invoked after all the game objects are added, and their references are created.</i>
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    /// <param name="initializer">The initializer function</param>
    /// <returns>The LazyComponent that will be used every time a scene is loaded to generate a new component</returns>
    public static LazyComponent<T> Create<T>(Action<T> creator, Action<T> initializer) where T : Component {
        return new LazyComponent<T>(creator, initializer);
    }
}

public abstract class LazyComponent
{
    // this keeps all lazy components regardless of scene
    protected static List<LazyComponent> lazyComponents = new();

    internal LazyComponent() {
        lazyComponents.Add(this);
    }

    // due to the fact that this is used every time a game object (/scene) is loaded,
    // the previous component of the exact same lazy component will be disposed;
    // obviously after it's being destroyed (through game object)
    internal abstract Component Create();
    internal abstract Component Initialize();

    internal static void InitializeAll() {
        foreach (var lazyComp in lazyComponents) {
            lazyComp.Initialize();
        }
    }
}

public class LazyComponent<T> : LazyComponent where T : Component
{
    internal Action<T> creator;
    internal Action<T> initializer;
    private T component;

    internal LazyComponent(Action<T> creator, Action<T> initializer) : base() {
        this.creator = creator;
        this.initializer = initializer;
    }

    internal override Component Create() {
        component = (T) Activator.CreateInstance(typeof(T));
        creator(component);
        component.Constructor();

        return component;
    }

    internal override Component Initialize() {
        initializer(component);
        return component;
    }

    internal T InitializeOfType() {
        initializer(component);
        return component;
    }
}