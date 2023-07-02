using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UGXP.Core;

namespace UGXP.Game.Manager;
public abstract class StepManager
{
    protected delegate void StepDelegate();
    protected class OnStep {
        public StepDelegate BeforeStep;
        public StepDelegate Step;
        public StepDelegate AfterStep;
    }

    protected Dictionary<GameObject, OnStep> activeGameObjects = new();

    public virtual void BeforeStep() {
        foreach (var obj in activeGameObjects.Values)
            obj.BeforeStep?.Invoke();
    }
    public virtual void Step() {
        foreach (var obj in activeGameObjects.Values)
            obj.Step?.Invoke();
    }
    public virtual void AfterStep() {
        foreach (var obj in activeGameObjects.Values)
            obj.AfterStep?.Invoke();
    }

    public abstract void Add(GameObject obj);
    public virtual bool Contains(GameObject obj) {
        return activeGameObjects.ContainsKey(obj);
    }
    public virtual void Remove(GameObject obj) {
        activeGameObjects.Remove(obj);
    }

    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    protected static StepDelegate? MethodToDelegate(object obj, string methodName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) {
        MethodInfo info = obj.GetType().GetMethod(methodName, flags);
        if (info != null) {
			StepDelegate onUpdate = (StepDelegate)Delegate.CreateDelegate(typeof(StepDelegate), obj, info, false);
			if (onUpdate != null)
				return onUpdate;
		} else {
			ValidateCase(obj, methodName, flags);
		}

        return null;
    }

    //------------------------------------------------------------------------------------------------------------------------
	//														ValidateCase()
	//------------------------------------------------------------------------------------------------------------------------
	private static void ValidateCase(object obj, string methodName, BindingFlags flags) {
		MethodInfo info = obj.GetType().GetMethod(methodName.ToLower(), flags);
		if (info != null) {
			throw new Exception($"'{ methodName }' function was not binded for '{ obj } '. Please check its case.");
		}
	}
}
