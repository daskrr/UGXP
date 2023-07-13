using UGXP.Assets;
using UGXP.Core;
using UGXP.Core.Components;
using UGXP.Game.Manager;

namespace UGXP.Game;

/// <summary>
/// A scene is the root element for any amount of hierarchic game objects.
/// Multiple scenes can co-exist in the same game process.
/// </summary>
public class Scene : GameObject
{
    private SceneManager manager {
         get { return GameProcess.Main.sceneManager; }
    }
    internal SceneStructure sceneStructure;

    public Scene(SceneStructure structure) : base(new GameObjectStructure { 
        Name = structure.Name,
        Children = structure.Objects
    }) {
        this.sceneStructure = structure;
        this.parent = null;
    }

    public override int Index {
        get { return 0; }
    }

    /// <summary>
    /// !OBSOLETE! Use <see cref="GameObject.ContainsDeep"/>
    /// Checks if this scene contains a <see cref="GameObject"/> child <br/>
    /// Note that this method could be quite expensive, since it recursively iterates through all children and their children etc. Use with caution!
    /// </summary>
    /// <param name="obj">The game object to be found</param>
    /// <returns>Whether this scene contains the given child</returns>
    [Obsolete("Use GameObject/Scene.ContainsDeep()")]
    public bool ContainsObject(GameObject obj) {
        foreach (var child in this) {
            if (child == obj) return true;

            ContainsObject(obj);
        }

        return false;
    }

    public override void OnDestroy() {
        base.OnDestroy();

        // unload all assets used by this scene
        List<string> assetNames = new();

        List<GameObject> allObjects = GetAll();
        foreach (GameObject obj in allObjects) {
            assetNames.AddRange(AssetManager.GatherAssetNamesFromObject(obj));

            foreach(Component comp in obj.GetComponents())
                assetNames.AddRange(AssetManager.GatherAssetNamesFromObject(comp));
        }

        // as well as the assets added manually
        assetNames.AddRange(sceneStructure.LoadAssetsByName);

        AssetManager.Unload(assetNames.ToArray());
    }

    /// <summary>
    /// !DEPRECATED!
    /// Renders every object in this scene hierarchically
    /// </summary>
    // TODO maybe make a render manager that uses delegates to render only the objects that are active and have an active renderer
    [Obsolete("Use RenderManager.Render() instead")]
    internal void Render() {
        _render(this);
    }

    /// <summary>
    /// Recursive rendering of objects
    /// </summary>
    /// <param name="parent">The object to start rendering from</param>
    private void _render(GameObject parent) {
        foreach (var child in parent) {
            // only render an object if it's not been destroyed && is active
            if (child && child.active) {
                if (child.renderer != null && child.renderer && child.renderer.active)
                    child.renderer.Render();

                // still render children even if this child's renderer is [null | destroyed | !active]
                _render(child);
            }
        }
    }

    #region deny component usage
    public override Component AddComponent(Component component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component AddComponent(Type component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override T AddComponent<T>() { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component GetComponent(Type component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override T GetComponent<T>() { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component[] GetComponents() { return Array.Empty<Component>(); }
    public override Component[] GetComponents(Type component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override T[] GetComponents<T>() { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component RemoveComponent(Component component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component RemoveComponent(Type component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override T RemoveComponent<T>() { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override Component[] RemoveComponents(Type component) { throw new InvalidOperationException("A scene cannot operate with components."); }
    public override T[] RemoveComponents<T>() { throw new InvalidOperationException("A scene cannot operate with components."); }
    #endregion;
}

/// <summary>
/// The SceneStructure is used to statically create a scene with game objects and set its properties.
/// </summary>
public class SceneStructure
{
    /// <summary>
    /// The name of the scene.
    /// </summary>
    public string Name;
    /// <summary>
    /// The children game objects of the scene
    /// </summary>
    public GameObjectStructure[] Objects = Array.Empty<GameObjectStructure>();
    ///// <summary>
    ///// Assets to be loaded along with this scene.<br/>
    ///// Note that these are extra assets that might be used dynamically through scripts/custom components.<br/>
    ///// The assets required for a scene are automatically loaded with the scene (based on the scene/game object structures).
    ///// </summary>
    // unused as assets should exclusively be added through the AssetManager and if it is required to manually load an asset,
    // LoadAssetsByName should be used
    //public Asset[] LoadAssets
    /// <summary>
    /// Assets to be loaded (by unqiue name) along with this scene.<br/>
    /// Note that these are extra assets that might be used dynamically through scripts/custom components.<br/>
    /// The assets required for a scene are automatically loaded with the scene (based on the scene/game object structures).
    /// </summary>
    public string[] LoadAssetsByName = new string[0];
}