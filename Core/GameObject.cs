using OpenTK.Mathematics;
using System.Collections;
using System.Reflection;
using UGXP.Core.Components;
using UGXP.Game;
using UGXP.Game.Manager;
using UGXP.Reference;
using UGXP.Util;

namespace UGXP.Core;

/// <summary>
/// The game object is the building block of a game. This object can be used as many times as needed inside a scene.<br/>
/// The game object can contain components which give it functionality and make it behave in any desired way.
/// </summary>
public class GameObject : Object, IComponentHolder, IReferenceable<GameObject>, IList<GameObject>
{
    public string name;
    public string tag = null;

    public int layerId = -1;
    public string layer {
        get => LayerMask.IdToName(layerId);
    }

    private string? referenceName;
    private ObjectReference<GameObject> reference;

    private Transform _transform;
    public Transform transform {
        get {
            return _transform;
        }
        set {
            value.gameObject = this;
            _transform = value;
        }
    }

    internal GameObject parent = null;
    protected List<GameObject> children = new();

    protected List<Component> components = new();

    internal Renderer renderer;
    internal List<Collider> colliders = new();

    private readonly GameObjectStructure structure;

    /// <summary>
    /// Dynamically creates a game object with no properties
    /// </summary>
    /// <param name="name"></param>
    internal GameObject(string name) { 
        this.name = name;
        this.transform = new();

        // create a reference for the first time when object is initialized.
        _ = GetReference();

        Awake();
    }

    /// <summary>
    /// Creates a game object from a structure
    /// </summary>
    /// <param name="structure">the data structure</param>
    internal GameObject(GameObjectStructure structure) {
        this.structure = structure;

        // set name before making the reference
        this.referenceName = structure.ReferenceName;
        // create a reference for the first time when object is initialized.
        _ = GetReference();

        this.name = structure.Name;
        this.layerId = LayerMask.NameToId(structure.LayerName);
        this.tag = structure.Tag;
        this.transform = structure.Transform.Clone() ?? new Transform();
        InitialAddComponents(structure.Components);

        if (structure.Children.Length > 0)
            Array.ForEach(structure.Children, childStruct => this.children.Add(new(childStruct) { parent = this }) );

        Awake();
    }

    private void Awake() {
        if (tag != null)
            RegisterGameObjectTag(this);

        AwakeComponents();
    }

    /// <summary>
    /// Gets the scene of this game object.
    /// Recursively goes back through the hierarchy to find the scene.
    /// This method can be expensive the first time it is ran. Use with caution!
    /// </summary>
    /// <returns>The scene containing this game object or null</returns>
    public Scene? GetScene() {
        if (parent == null && this is Scene scene)
            return scene;
        else if (parent == null)
            return null;

        return parent.GetScene();
    }

    public ObjectReference<GameObject> GetReference() {
        // if referenceName is null, the reference will be created only internally (just id)
        reference ??= ReferenceManager.Create<GameObject>(referenceName, this);
        return reference;
    }

    public GameObject Clone() {
        if (structure != null)
            return new GameObject(structure);
        else
            return new GameObject(name);
    }

    public override void SetActive(bool active) {
        base.SetActive(active);

        if (active)
            GameObjectManager.Subscribe(this);
        else
            GameObjectManager.Unsubscribe(this);
    }

    /// <summary>
    /// Compares this game object's tag against another.
    /// </summary>
    /// <param name="otherTag">The tag to compare against</param>
    /// <returns>A bool representing if the tags are equal or not</returns>
    public bool CompareTag(string otherTag) {
        return tag == otherTag;
    }

    public override void OnDestroy() {
        if (tag != null)
            RemoveGameObjectTag(this);

        // destroy the components of this object
        foreach (var comp in GetComponents())
            Destroy(comp);

        // destroy all children and their components
        foreach (var child in this) {
            foreach (var comp in child.GetComponents())
                Destroy(comp);

            Destroy(child);
        }

        ReferenceManager.RemoveReference(reference.id);
    }

    #region GameObject fields
    //public GameObject parent {
    //    get {
    //        return _parent;
    //    }
    //    set {
    //        // wait until end of frame
    //        // GameProcess.Main.OnAfterStep +=

    //        if (_parent == null) {
    //            if (value == null)
    //                // removed from the game process
    //                Unsubscribe();
    //        }
    //        else {
    //            if (value == null) {
    //                // removed from the game process
    //                _parent.Remove(this);
    //                Unsubscribe();
    //            }
    //        }

    //        if (value != null) {
    //            // added to the game process
    //            _parent = value;
    //            _parent.Add(this);
    //            Subscribe();
    //        }
    //    }
    //}

    public virtual int Index {
        get {
            if (parent == null) return -1;
            return parent.IndexOf(this);
        }
    }
    #endregion

    #region GameObject internal functionality
    // this is handled by the GameObjectManager
    internal bool isSubscribed = false;
    internal void Subscribe() {
        // unsubscribe game object if it's already subscribed
        // removed: there is no reason to unsubscribe the object and re-subscribe it
        //if (isSubscribed) Unsubscribe();

        GameObjectManager.Subscribe(this);
        // the game object manager already subscribes the children
        //foreach (var child in this)
        //    child.Subscribe();
    }

    internal void Unsubscribe() {
        GameObjectManager.Unsubscribe(this);
        // the game object manager already unsubscribes the children
        //foreach (var child in this)
        //    child.Unsubscribe();
    }
    #endregion

    #region static methods & fields

    private static HashSet<string> tags = new();
    private static Dictionary<string, List<GameObject>> linkTags = new();

    /// <summary>
    /// Register tags from the game settings to the game object for easier accessibility.
    /// </summary>
    /// <param name="tags">The tags to register</param>
    internal static void RegisterTags(HashSet<string> tags) {
        GameObject.tags = tags;
        foreach (var tag in tags)
            linkTags.Add(tag, new List<GameObject>());
    }

    private static bool CheckTag(string tag) {
        return tags.Contains(tag);
    }

    private static void RegisterGameObjectTag(GameObject obj) {
        if (obj.tag == null) return;
        if (!CheckTag(obj.tag)) return;
        if (linkTags[obj.tag].Contains(obj)) return;

        linkTags[obj.tag].Add(obj);
    }

    private static void RemoveGameObjectTag(GameObject obj) {
        if (obj.tag == null) return;
        if (!CheckTag(obj.tag)) return;

        linkTags[obj.tag].Remove(obj);
    }

    /// <summary>
    /// Looks for and gives the first game object with the specified tag.<br/>
    /// </summary>
    /// <param name="tag">The tag of the object to look for</param>
    /// <returns>The first object with <i>tag</i> or null.</returns>
    public static GameObject FindGameObjectWithTag(string tag) {
        if (linkTags[tag].Count == 0) return null;

        GameObject[] objs = FindGameObjectsWithTag(tag);
        if (objs.Length == 0) return null;

        return objs[0];
    }

    public static GameObject[] FindGameObjectsWithTag(string tag) {
        if (!CheckTag(tag))
            throw new ArgumentOutOfRangeException(nameof(tag), "The provided tag does not exist. Make sure it exists in the Tags from GameSettings!");

        return linkTags[tag].ToArray();
    }

    #endregion

    #region component holder
    private bool addedInitialComponents = false;
    public virtual void InitialAddComponents(LazyComponent[] components) {
        if (addedInitialComponents)
            throw new InvalidOperationException("Components have already been added at scene load. Cannot add other initial components!");

        foreach (var lazyComponent in components) {
            Component component = lazyComponent.Create();

            component.gameObject = this;
            // add renderers and colliders for ease of use
            if (component is Renderer renderer)
                this.renderer = renderer;
            if (component is Collider collider)
                this.colliders.Add(collider);

            this.components.Add(component);
        }

        // not updating since this registers the object before it's supposed to be subscribed
        // GameObjectManager.Update(this);

        addedInitialComponents = true;
    }

    public virtual Component[] AddComponents(Component[] components) {
        Component[] returnComponents = new Component[components.Length];
        foreach (var component in components)
            _ = returnComponents.Append(AddComponent(component));

        // CHANGED TO only Update, since DoNextFrame is incorporated into Update now
        //ExecutionManager.DoNextFrame(() => {
        //    GameObjectManager.Update(this);
        //});
        GameObjectManager.Update(this);

        return returnComponents;
    }

    public virtual Component AddComponent(Component component) {
        component.gameObject = this;
        components.Add(component);

        if (component is Renderer renderer)
            this.renderer = renderer;
        if (component is Collider collider)
            this.colliders.Add(collider);

        GameObjectManager.Update(this);

        return component;
    }

    public virtual Component AddComponent(Type component) {
        object? compInst = Activator.CreateInstance(component);
        if (compInst == null || compInst is not Component)
            throw new Exception("Could not create/add component to game object.");

        Component newComponent = (Component) compInst;
        newComponent.gameObject = this;

        components.Add(newComponent);

        if (newComponent is Renderer renderer)
            this.renderer = renderer;
        if (newComponent is Collider collider)
            this.colliders.Add(collider);

        GameObjectManager.Update(this);

        return newComponent;
    }

    public virtual Component[] AddComponents(Type[] components) {
        List<Component> returnComponents = new();

        foreach (Type type in  components) { 
            object? compInst = Activator.CreateInstance(type);
            if (compInst == null || compInst is not Component)
                throw new Exception("Could not create/add component to game object.");

            Component newComponent = (Component) compInst;
            newComponent.gameObject = this;

            this.components.Add(newComponent);

            if (newComponent is Renderer renderer)
                this.renderer = renderer;
            if (newComponent is Collider collider)
                this.colliders.Add(collider);

            returnComponents.Add(newComponent);
        }

        GameObjectManager.Update(this);

        return returnComponents.ToArray();
    }

    public virtual T AddComponent<T>() where T : Component {
        object? compInst = Activator.CreateInstance<T>();
        if (compInst == null || compInst is not Component)
            throw new Exception("Could not create/add component to game object.");

        T newComponent = (T) compInst;
        newComponent.gameObject = this;

        components.Add(newComponent);

        if (newComponent is Renderer renderer)
            this.renderer = renderer;
        if (newComponent is Collider collider)
            this.colliders.Add(collider);

        GameObjectManager.Update(this);

        return newComponent;
    }

    public virtual Component GetComponent(Type component) {
        return components.Find(comp => 
        comp.GetType().Equals(component)) ?? 
            DevelopmentHandlers.HandleValueNotFound<Component>("Could not find the component of type.");
    }

    public virtual T GetComponent<T>() where T : Component {
        return (T) (components.Find(comp => comp is T) ?? DevelopmentHandlers.HandleValueNotFound<T>("Could not find the component of type."));
    }

    public virtual Component[] GetComponents() {
        return components.ToArray();
    }

    public virtual Component[] GetComponents(Type component) {
        return components.FindAll(comp => comp.GetType().Equals(component)).ToArray();
    }

    public virtual T[] GetComponents<T>() where T : Component {
        return (T[]) components.FindAll(comp => comp is T).ToArray();
    }

    /// <summary>
    /// Checks if the component to be removed is a renderer or a collider and is the same renderer/collider as the active renderer/collider for this game object
    /// </summary>
    /// <param name="removed">The component to be checked</param>
    private void CheckRemoveComponent(Component removed) {
        if (removed is Renderer && renderer.Equals(removed))
            renderer = null;
        if (removed is Collider collider && colliders.Contains(removed))
            colliders.Remove(collider);
    }

    public virtual Component RemoveComponent(Component component) {
        ExecutionManager.DoNextFrame(() => {
            components.Remove(component);

            CheckRemoveComponent(component);
            Destroy(component);
        });
        GameObjectManager.Update(this);

        return component;
    }

    public virtual Component RemoveComponent(Type component) {
        Component comp = GetComponent(component);

        ExecutionManager.DoNextFrame(() => {
            components.Remove(comp);

            CheckRemoveComponent(comp);
            Destroy(comp);
        });
        GameObjectManager.Update(this);

        return comp;
    }

    public virtual T RemoveComponent<T>() where T : Component {
        T comp = GetComponent<T>();

        ExecutionManager.DoNextFrame(() => {
            components.Remove(comp);

            CheckRemoveComponent(comp);
            Destroy(comp);
        });
        GameObjectManager.Update(this);

        return comp;
    }

    public virtual Component[] RemoveComponents(Type component) {
        Component[] comp = GetComponents(component);

        ExecutionManager.DoNextFrame(() => {
            Array.ForEach(comp, c => RemoveComponent(c));

            foreach (var c in comp) { 
                CheckRemoveComponent(c);
                Destroy(c);
            }
        });
        GameObjectManager.Update(this);

        return comp;
    }

    public virtual T[] RemoveComponents<T>() where T : Component {
        T[] comp = GetComponents<T>();

        ExecutionManager.DoNextFrame(() => {
            Array.ForEach(comp, c => RemoveComponent(c));

            foreach (var c in comp) { 
                CheckRemoveComponent(c);
                Destroy(c);
            }
        });
        GameObjectManager.Update(this);

        return comp;
    }
    public void AwakeComponents() {
        foreach (var comp in GetComponents()) { 
            MethodInfo? info = Reflection.GetMethod(comp, "Awake");
            if (info == null)
                continue;

            try { 
                info.Invoke(comp, null);
            } catch (Exception) {
                throw new Exception("Awake Method was found but could not be invoked. Make sure it doesn't have any parameters.");
            }

            comp.initialized = true;
        }
    }

    public void StartComponents() {
        foreach (var comp in GetComponents()) { 
            MethodInfo? info = Reflection.GetMethod(comp, "Start");
            if (info == null)
                continue;

            try { 
                info.Invoke(comp, null);
            } catch (Exception) {
                throw new Exception("Start Method was found but could not be invoked. Make sure it doesn't have any parameters.");
            }

            comp.started = true;
        }
    }
    #endregion;

    #region enumerable/list
    public IEnumerator<GameObject> GetEnumerator() {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public int Count => children.Count;

    public bool IsReadOnly => false;
    
    /// <summary>
    /// Replaces a child at index, also destroying the previous child at that index.
    /// </summary>
    /// <param name="index">the index for the child</param>
    public GameObject this[int index] { 
        get => children[index];
        set {
            ExecutionManager.DoNextFrame(() => {
                Destroy(children[index]);

                // child game object is kept in the array, however it is destroyed so a /== null/ check will return true
                if (value == null)
                    return;

                value.parent = this;
                value.Subscribe();
                children[index] = value;
            });
        } 
    }

    public List<GameObject> GetAll() {
        List<GameObject> allChildren = new();

        allChildren.AddRange(children);
        foreach (var child in this) {
            if (child)
                allChildren.AddRange(child.GetAll());
        }

        return allChildren;
    }

    public int IndexOf(GameObject item) {
        return children.IndexOf(item);
    }

    public void Insert(int index, GameObject item) {
        ExecutionManager.DoNextFrame(() => {
            item.parent = this;
            item.Subscribe();
            children.Insert(index, item);
        });
    }

    public void RemoveAt(int index) {
        ExecutionManager.DoNextFrame(() => {
            children[index].parent = null;
            children[index].Unsubscribe();
            children.RemoveAt(index);
        });
    }

    public void Add(GameObject item) {
        ExecutionManager.DoNextFrame(() => {
            item.parent = this;
            item.Subscribe();
            children.Add(item);
        });
    }

    public void Clear() {
        ExecutionManager.DoNextFrame(() => {
            children.ForEach(child => {
                child.parent = null;
                child.Unsubscribe();
            });
            children.Clear();
        });
    }

    /// <summary>
    /// Checks (shallow) if this game object contains another game object.
    /// </summary>
    /// <param name="item">The game object to be found</param>
    public bool Contains(GameObject item) {
        return children.Contains(item);
    }

    /// <summary>
    /// Checks (deep) if this game objects or its children (recursively) contain another game object.
    /// </summary>
    /// <param name="item">The game object to be found</param>
    public bool ContainsDeep(GameObject item, bool performShallow = true) {
        // do a shallow check just in case
        if (performShallow)
            if (Contains(item))
                return true;

        foreach (var child in this) {
            if (child == item) return true;

            ContainsDeep(item);
        }

        return false;
    }

    public void CopyTo(GameObject[] array, int arrayIndex) {
        throw new InvalidOperationException("CopyTo is not a valid operation for GameObject!");
    }

    /// <summary>
    /// Removes a child game object.<br/>
    /// Note: This will return true if the child exists and will try to remove it. The return bool does not represent the success of removal!
    /// </summary>
    /// <param name="item">The game object to remove</param>
    public bool Remove(GameObject item) {
        ExecutionManager.DoNextFrame(() => {
            if (children.Remove(item)) {
                item.parent = null;
                item.Unsubscribe();
            }
        });
        
        return Contains(item);
    }
    #endregion;

    public override string ToString() {
        return $"GameObject [{name}] @[{transform.position.x},{transform.position.y}]";
    }
}


/// <summary>
/// Data structure used to easily create game objects.
/// </summary>
public class GameObjectStructure {
    /// <summary>
    /// The name of the object
    /// </summary>
    public string Name;
    /// <summary>
    /// The unique reference name to be used when called with <see cref="ReferenceManager.Get"/>
    /// </summary>
    public string ReferenceName;
    /// <summary>
    /// The layer this object will live in
    /// </summary>
    public string LayerName = "Default";
    /// <summary>
    /// The tag of the object (can be assigned to multiple objects
    /// </summary>
    public string Tag;
    /// <summary>
    /// The transform of the object (containing position, scale and rotation)<br/>
    /// The default is [position = v2(0,0)] [scale = v2(1,1)] [rotation = v3(0,0,0)]<br/>
    /// A RectTransform can also be used here.
    /// </summary>
    public Transform Transform = new() { position = Vector2.zero, scale = Vector2.one, rotation = Vector3.Zero };
    /// <summary>
    /// The components of this game object
    /// </summary>
    public LazyComponent[] Components = Array.Empty<LazyComponent>();
    /// <summary>
    /// The children of this game object (if any)
    /// </summary>
    public GameObjectStructure[] Children = Array.Empty<GameObjectStructure>();
}