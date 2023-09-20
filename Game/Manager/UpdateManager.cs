using UGXP.Core;

namespace UGXP.Game.Manager;
internal class UpdateManager : StepManager
{
    public override void Add(GameObject obj) {
        if (!obj || !obj.active) return;

        StepDelegate beforeStep = () => { };
        StepDelegate step = () => { };
        StepDelegate afterStep = () => { };
        Array.ForEach(obj.GetComponents(), comp => {
            if (!comp.active) return;

            StepDelegate? early = MethodToDelegate(comp, "EarlyUpdate");
            if (early != null)
                beforeStep += () => {
                    if (obj.active && comp.active)
                        early();
                };

            StepDelegate? update = MethodToDelegate(comp, "Update");
            if (update != null)
                step += () => {
                    if (obj.active && comp.active)
                        update();
                };

            StepDelegate? late = MethodToDelegate(comp, "LateUpdate");
            if (late != null)
                afterStep += () => {
                    if (obj.active && comp.active)
                        late();
                };

            // Add gizmos here since it creates its own queue
            StepDelegate? gizmosDraw = MethodToDelegate(comp, "OnGizmosDraw");
            if (gizmosDraw != null)
                afterStep += () => {
                    if (obj.active && comp.active)
                        gizmosDraw();
                };
        });

        // TODO add fixedupdate (fixed rate)

        if (activeGameObjects.ContainsKey(obj))
            throw new InvalidOperationException("The object already exists! Use Update or Remove, then Add!");

        // removed due to "why the heck is this even here?" - memory could get clogged easily if used improperly
        //if (activeGameObjects.ContainsKey(obj)) {
        //    activeGameObjects[obj] += new OnStep {
        //        BeforeStep = beforeStep,
        //        Step = step,
        //        AfterStep = afterStep,
        //    };

        //    return;
        //}

        activeGameObjects.Add(obj, new OnStep {
            BeforeStep = beforeStep,
            Step = step,
            AfterStep = afterStep,
        });
    }

    public override void Update(GameObject obj) {
        // due to the high volume of singular delegates, this method of updating the game object might be faster?
        Remove(obj);
        Add(obj);
    }
}
