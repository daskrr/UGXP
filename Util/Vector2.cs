// courtesy to Unity docs for clarification of how certain methods, fields work and what they do, as well as their summaries.

namespace UGXP {
    public class Vector2 : IEquatable<Vector2>
    {
        // statics
        public static readonly Vector2 down = new(0, -1);
        public static readonly Vector2 left = new(-1, 0);
        public static readonly Vector2 negativeInfinity = new(float.NegativeInfinity, float.NegativeInfinity);
        public static readonly Vector2 one =  new(1, 1);
        public static readonly Vector2 positiveInfinity = new(float.PositiveInfinity, float.PositiveInfinity);
        public static readonly Vector2 right = new(1, 0);
        public static readonly Vector2 up = new(0, 1);
        public static readonly Vector2 zero = new(0, 0);

        public float x = 0f;
        public float y = 0f;

        /// <summary>
        /// Access the <see cref="x"/> or <see cref="y"/> component using [0] or [1] respectively.
        /// </summary>
        /// <param name="i">index (0 or 1)</param>
        /// <returns>the specified value</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public float this[int i]
        {
            get {
                return i switch
                {
                    0 => x,
                    1 => y,
                    _ => throw new IndexOutOfRangeException("Invalid Vector2 index!"),
                };
            }
            set {
                switch (i) {
                    case 0: 
                        x = value; 
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }

        /// <summary>
        /// Returns the length of this vector (Read Only).
        /// The length of the vector is square root of (x*x+y*y).
        /// If you only need to compare magnitudes of some vectors, you can compare squared magnitudes of them using <see cref="sqrMagnitude"/> (computing squared magnitudes is faster).
        /// </summary>
        public float magnitude {
            get {
                return Mathf.Sqrt(sqrMagnitude);
            }
        }

        /// <summary>
        /// Returns this vector with a magnitude of 1 (Read Only).
        /// When normalized, a vector keeps the same direction but its length is 1.0.
        /// Note that the current vector is unchanged and a new normalized vector is returned. If you want to normalize the current vector, use the <see cref="Normalize"/> function.
        /// </summary>
        public Vector2 normalized {
            get { 
                return new Vector2(x, y).Normalize();
            }
        }

        /// <summary>
        /// Returns the squared length of this vector (Read Only).
        /// Calculating the squared magnitude instead of the magnitude is much faster. Often if you are comparing magnitudes of two vectors you can just compare their squared magnitudes.
        /// </summary>
        public float sqrMagnitude {
            get {
                return x*x + y*y;
            }
        }

        /// <summary>
        /// Initializes a Vector2 with 2 float values
        /// </summary>
        /// <param name="x">value for the x component</param>
        /// <param name="y">value for the y component</param>
        public Vector2 (float x, float y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Makes this vector have a magnitude of 1.
        /// When normalized, a vector keeps the same direction but its length is 1.0.
        /// Note that this function will change the current vector. If you want to keep the current vector unchanged, use <see cref="normalized"/> variable.
        /// </summary>
        /// <returns>The current vector after normalization</returns>
        public Vector2 Normalize() {
            if (magnitude > float.Epsilon) {
                x /= magnitude;
                y /= magnitude;
            }
            else
                x = 0f;
                y = 0f;

            return this;
        }

        // overrides

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (obj is not Vector2) return false;
            Vector2 v = (Vector2)obj;

            return x == v.x && y == v.y;
        }

        public bool Equals(Vector2? other) {
            if (other == null) return false;
            return x == other.x && y == other.y;
        }

        public override string ToString() {
            return "Vector2["+ x +","+ y +"]";
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        // check operators

        public static bool operator ==(Vector2? v1, Vector2? v2) {
            if (v1 is null && v2 is null) return true;
            if (v1 is null || v2 is null) return false;

            return v1.x == v2.x && v1.x == v2.y;
        }

        public static bool operator !=(Vector2? v1, Vector2? v2) {
            return !(v1 == v2);
        }

        public static bool operator >(Vector2 v1, Vector2 v2) {
            return v1.x > v2.x && v1.x > v2.y;
        }

        public static bool operator >=(Vector2 v1, Vector2 v2) {
            return v1.x >= v2.x && v1.x >= v2.y;
        }

        public static bool operator <(Vector2 v1, Vector2 v2) {
            return v1.x < v2.x && v1.x < v2.y;
        }

        public static bool operator <=(Vector2 v1, Vector2 v2) {
            return v1.x <= v2.x && v1.x <= v2.y;
        }

        // modify operators

        public static Vector2 operator +(Vector2 v1, Vector2 v2) {
            return new(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2) {
            return new(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2) {
            return new(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2) {
            return new(v1.x / v2.x, v1.y / v2.y);
        }

        //public static implicit operator Vector2(Vector3 v3) { }
        //public static explicit operator Vector3(Vector2 v2) { }


        // static helper methods

        /// <summary>
        /// Linearly interpolate between 2 vectors by time. Time is clamped between 0 and 1.
        /// </summary>
        /// <param name="initial">starting vector</param>
        /// <param name="target">target vector</param>
        /// <param name="time">time</param>
        /// <returns>the current interpolated vector based on time</returns>
        public static Vector2 Lerp(Vector2 initial, Vector2 target, float time) {
            return new Vector2(Mathf.Lerp(initial.x, target.x, time), Mathf.Lerp(initial.y, target.y, time));
        }

        /// <summary>
        /// Linearly interpolate between 2 vectors by time. Time is not clamped
        /// </summary>
        /// <param name="initial">starting vector</param>
        /// <param name="target">target vector</param>
        /// <param name="time">time</param>
        /// <returns>the current interpolated vector based on time</returns>
        public static Vector2 LerpUnclamped(Vector2 initial, Vector2 target, float time) {
            return new Vector2(Mathf.LerpUnclamped(initial.x, target.x, time), Mathf.LerpUnclamped(initial.y, target.y, time));
        }

        // a very basic implementation of Critically Damped Ease Smoothing
        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        /// <param name="current">the initial position</param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="smoothTime"></param>
        /// <returns></returns>
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 velocity, float smoothTime) {
            return new Vector2(
                Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime, Time.deltaTime),
                Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime, Time.deltaTime)
            );
        }

        public static float Distance(Vector2 a, Vector2 b) {
            return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
        }
    }
}
