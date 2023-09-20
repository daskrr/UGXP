using System.Reflection;
using UGXP.Core;
using UGXP.Util;

namespace UGXP.Game.Manager;
internal abstract class StepManager
{
    protected delegate void StepDelegate();
    protected class OnStep {
        public StepDelegate BeforeStep = () => { };
        public StepDelegate Step = () => { };
        public StepDelegate AfterStep = () => { };

        public static OnStep operator +(OnStep step1, OnStep step2) {
            OnStep newOnStep = new() {
                BeforeStep = step1.BeforeStep + step2.BeforeStep,
                Step = step1.Step + step2.Step,
                AfterStep = step1.AfterStep + step2.AfterStep
            };

            return newOnStep;
        }
        public static OnStep operator -(OnStep step1, OnStep step2) {
            OnStep newOnStep = new() {
                BeforeStep = step1.BeforeStep - step2.BeforeStep,
                Step = step1.Step - step2.Step,
                AfterStep = step1.AfterStep - step2.AfterStep
            };

            return newOnStep;
        }
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
    public abstract void Update(GameObject obj);
    public virtual bool Contains(GameObject obj) {
        return activeGameObjects.ContainsKey(obj);
    }
    public virtual void Remove(GameObject obj) {
        activeGameObjects.Remove(obj);
    }

    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    protected static StepDelegate? MethodToDelegate(object obj, string methodName) {
        MethodInfo info = Reflection.GetMethod(obj, methodName);
        if (info != null) {
			StepDelegate onUpdate = (StepDelegate) Delegate.CreateDelegate(typeof(StepDelegate), obj, info, false);
			if (onUpdate != null)
				return onUpdate;
		} else {
			ValidateCase(obj, methodName);
		}

        return null;
    }

	private static void ValidateCase(object obj, string methodName) {
		MethodInfo info = Reflection.GetMethod(obj, methodName.ToLower());
		if (info != null)
			throw new Exception($"'{ methodName }' function was not binded for '{ obj } '. Please check its case.");
	}
}

