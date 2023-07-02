using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGXP.Core.Components;
using UGXP.Game.Manager;
using UGXP.Reference;
using Component = UGXP.Core.Components.Component;

namespace UGXP.Core;

public class GameObject : Object, IComponentHolder, IReferenceable<GameObject>, IList<GameObject>
{
    public string name;
    public string tag;
    public int layerId = -1;

    public string layer {
        get {
            return "";
            // return LayerMask.IdToName(this.layerId)
        }
    }

    private string? referenceName;
    private ObjectReference<GameObject> reference;

    public Transform transform = new();
    protected GameObject parent = null;
    protected List<GameObject> children = new();

    protected List<Component> components = new();

    /// <summary>
    /// Dynamically creates a game object with no properties
    /// </summary>
    /// <param name="name"></param>
    internal GameObject(string name) { 
        this.name = name;
    }

    /// <summary>
    /// Creates a game object from a structure
    /// </summary>
    /// <param name="structure">the data structure</param>
    internal GameObject(GameObjectStructure structure) {
        this.name = structure.Name;
        this.referenceName = structure.ReferenceName;
        //this.layerId = LayerMask.NameToId(structure.LayerName);
        this.tag = structure.Tag;
        this.transform = structure.Transform;
        this.components.AddRange(structure.Components);

        Array.ForEach(structure.Children, childStruct => this.children.Add(new(childStruct.Name) { parent = this }) );
    }

    public ObjectReference<GameObject> GetReference() {
        throw new NotImplementedException();

        // if referenceName is null, the reference will be created only internally (just id)
        //reference ??= ReferenceManager.Create<GameObject>(referenceName, this);
        // return reference;
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
    internal void Subscribe() {
        GameObjectManager.Subscribe(this);
        foreach (var child in this)
            child.Subscribe();
    }

    internal void Unsubscribe() {
        GameObjectManager.Unsubscribe(this);
        foreach (var child in this)
            child.Unsubscribe();
    }
    #endregion

    #region component holder
    public virtual Component AddComponent(Component component) {
        component.gameObject = this;
        component.tag = tag;
        component.transform = transform;

        components.Add(component);
        return component;
    }

    public virtual Component AddComponent(Type component) {
        object? compInst = Activator.CreateInstance(component);
        if (compInst == null || compInst is not Component)
            throw new Exception("Could not create/add component to game object.");

        Component newComponent = (Component) compInst;
        newComponent.gameObject = this;
        newComponent.tag = tag;
        newComponent.transform = transform;

        components.Add(newComponent);

        return newComponent;
    }

    public virtual T AddComponent<T>() where T : Component {
        object? compInst = Activator.CreateInstance<T>();
        if (compInst == null || compInst is not Component)
            throw new Exception("Could not create/add component to game object.");

        T newComponent = (T) compInst;
        newComponent.gameObject = this;
        newComponent.tag = tag;
        newComponent.transform = transform;

        components.Add(newComponent);

        return newComponent;
    }

    public virtual Component GetComponent(Type component) {
        return components.Find(comp => comp.GetType().Equals(component)) ?? throw new Exception("Could not find the component of type.");
    }

    public virtual T GetComponent<T>() where T : Component {
        return (T) (components.Find(comp => comp is T) ?? throw new Exception("Could not find the component of type."));
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

    public virtual Component RemoveComponent(Component component) {
        components.Remove(component);
        return component;
    }

    public virtual Component RemoveComponent(Type component) {
        Component comp = GetComponent(component);
        components.Remove(comp);

        return comp;
    }

    public virtual T RemoveComponent<T>() where T : Component {
        T comp = GetComponent<T>();
        components.Remove(comp);

        return comp;
    }

    public virtual Component[] RemoveComponents(Type component) {
        Component[] comp = GetComponents(component);
        Array.ForEach(comp, c => RemoveComponent(c));

        return comp;
    }

    public virtual T[] RemoveComponents<T>() where T : Component {
        T[] comp = GetComponents<T>();
        Array.ForEach(comp, c => RemoveComponent(c));

        return comp;
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

    public GameObject this[int index] { 
        get => children[index];
        set {
            Destroy(children[index]);

            // child game object is kept in the array, however it is destroyed so a /== null/ check will return true
            if (value == null)
                return;

            value.parent = this;
            value.Subscribe();
            children[index] = value;
        } 
    }

    public int IndexOf(GameObject item) {
        return children.IndexOf(item);
    }

    public void Insert(int index, GameObject item) {
        item.parent = this;
        item.Subscribe();
        children.Insert(index, item);
    }

    public void RemoveAt(int index) {
        children[index].parent = null;
        children[index].Unsubscribe();
        children.RemoveAt(index);
    }

    public void Add(GameObject item) {
        item.parent = this;
        item.Subscribe();
        children.Add(item);
    }

    public void Clear() {
        children.ForEach(child => {
            child.parent = null;
            child.Unsubscribe();
        });
        children.Clear();
    }

    public bool Contains(GameObject item) {
        return children.Contains(item);
    }

    public void CopyTo(GameObject[] array, int arrayIndex) {
        throw new InvalidOperationException();
    }

    public bool Remove(GameObject item) {
        if (children.Remove(item)) {
            item.parent = null;
            item.Unsubscribe();
            return true;
        }

        return false;
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
    /// The transform of the object (containing position, scale and rotation)
    /// The default is [position = v2(0,0)] [scale = v2(1,1)] [rotation = 0f]
    /// </summary>
    public Transform Transform = new() { position = Vector2.zero, scale = Vector2.one, rotation = 0f };
    /// <summary>
    /// The components of this game object
    /// </summary>
    public Component[] Components = Array.Empty<Component>();
    /// <summary>
    /// The children of this game object (if any)
    /// </summary>
    public GameObjectStructure[] Children = Array.Empty<GameObjectStructure>();
}