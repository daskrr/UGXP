using System.Collections;
using UGXP.Game.Manager;

namespace UGXP.Game.Manager
{ 
    // TODO TEST
    internal class ExecutionManager
    {
        internal static ExecutionManager Instance { get; private set; }

        internal ExecutionManager() {
            Instance ??= this;
        }

        private List<Routine> routines = new();

        public delegate void NextFrameDelegate();
        private NextFrameDelegate nextFrameQueue = () => { };

        internal static void DoNextFrame(NextFrameDelegate action) {
            Instance.nextFrameQueue += action;
        }

        internal void InvokeQueue() {
            nextFrameQueue();
        }

        internal static Routine StartRoutine(IEnumerator routine) {
            Routine routineInst = new Routine(routine);
            Instance.routines.Add(routineInst);

            return routineInst;
        }

        internal void Step() {
            routines.ForEach(routine => routine.Step());
        }

        internal void AfterStep() {
            routines.ForEach(routine => routine.EndOfFrame());
        }

        internal static bool Dispose(Routine routine) {
            return Instance.routines.Remove(routine);
        }
    }
}

namespace UGXP.Game
{ 
    /// <summary>
    /// The routine after it's been created and added to the game loop.
    /// </summary>
    public class Routine
    {
        private IEnumerator routine;
        private Wait wait;

        public bool isPaused { get; internal set; }

        internal Routine(IEnumerator routine) {
            this.routine = routine;
            this.isPaused = false;
        }

        internal void Step() {
            if (isPaused) return;

            wait = (Wait) routine.Current;

            if (wait.Step())
                if (!routine.MoveNext())
                    End();
        }

        internal void EndOfFrame() {
            if (isPaused) return;

            wait = (Wait) routine.Current;

            if (wait.EndOfFrame())
                if (!routine.MoveNext())
                    End();
        }

        public void Pause(bool pause = true) {
            isPaused = pause;
        }

        public void End() {
            // disposes itself
            ExecutionManager.Dispose(this);
        }
    }

    public abstract class Wait {
        public int counter;

        public virtual bool Step() { return false; }
        public virtual bool EndOfFrame() { return false; }
    }

    /// <summary>
    /// Waits for an approximate amount of seconds.
    /// </summary>
    public class WaitForSeconds : Wait {
        public WaitForSeconds(int seconds) {
            counter = seconds;
        }

        private int nextUpdate = 1;
        public override bool Step() {
            if (Time.time >= nextUpdate) { 
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;

                if (--counter <= 0)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Wait until the end of all the frame's execution.
    /// </summary>
    public class WaitForEndOfFrame : Wait {
        public WaitForEndOfFrame() {
            counter = 1;
        }

        public override bool EndOfFrame() {
            if (--counter <= 0)
                return true;

            return false;
        }
    }
}