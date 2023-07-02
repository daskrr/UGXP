using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGXP.Core;
using UGXP.Core.Components;

namespace UGXP.Game;

public class Scene : GameObject
{
    public Scene(SceneStructure structure) : base(new GameObjectStructure { 
        Name = structure.Name,
        Children = structure.Objects
    }) {
        this.parent = null;
    }

    public override int Index {
        get { return 0; }
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

public class SceneStructure
{
    public string Name;
    public GameObjectStructure[] Objects;
}