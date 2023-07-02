using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGXP.Core;

namespace UGXP.Game.Manager;
public class UpdateManager : StepManager
{
    public override void Add(GameObject obj) {
        StepDelegate beforeStep = () => { };
        StepDelegate step = () => { };
        StepDelegate afterStep = () => { };
        Array.ForEach(obj.GetComponents(), comp => {
            StepDelegate? early = MethodToDelegate(obj, "EarlyUpdate");
            if (early != null)
                beforeStep += early;

            StepDelegate? update = MethodToDelegate(obj, "Update");
            if (update != null)
                step += update;

            StepDelegate? late = MethodToDelegate(obj, "EarlyUpdate");
            if (late != null)
                afterStep += late;
        });

        // TODO add fixedupdate (fixed rate)

        activeGameObjects.Add(obj, new OnStep {
            BeforeStep = beforeStep,
            Step = step,
            AfterStep = afterStep,
        });
    }
}
