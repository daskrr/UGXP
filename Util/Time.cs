using GLFW;

namespace UGXP {
    /// <summary>
    /// Contains various time related functions.
    /// </summary>
    public class Time {
        private static float previousTime;

        static Time() {
        }

        /// <summary>
        /// Returns the current system time in milliseconds
        /// </summary>
        public static float now {
            get { return System.Environment.TickCount / 1000f; }
        }

        /// <summary>
        /// Returns this time in milliseconds since the program started		
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public static float time {
            get { return (float) Glfw.Time; }
        }

        /// <summary>
        /// Returns the time in milliseconds that has passed since the previous frame
        /// </summary>
        /// <value>
        /// The delta time.
        /// </value>
        private static float previousFrameTime;
        public static float deltaTime {
            get {
                return previousFrameTime;
            }
        }

        internal static void newFrame() {
            previousFrameTime = time - previousTime;
            previousTime = time;
        }
    }
}

