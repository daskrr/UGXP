using OpenTK.Mathematics;

namespace UGXP.Core;

public class Transform
{
	public GameObject gameObject;

    protected Vector2 _position = Vector2.zero;

	// TODO this vector is local and doesnt actually update position when using .x or .y so idk what the hell to do
	// (maybe update both local and global position when either is set and just use the local pos when calculating the matrices - since both are updated)

	// TODO change the setting of this to calculate the global position and properly set the local pos so that this object is
	// properly positioned in the local space to account for the world position given to it
    /// <summary>
    /// The current world vector2 position of this Transform<br/>
	/// Setting this will set the local position of the object.
    /// </summary>
    public Vector2 position {
        get {
			// we default to local position in the case that the game object wasn't set yet
			if (gameObject == null)
				return _position;
			
			// recalculate the matrix to only use the parent's matrix as we already have our own position (and it causes a stack overflow :D)
			Matrix4 mat4 = scaleMatrix * rotationMatrix;
			if (gameObject.parent != null)
				mat4 *= gameObject.parent.transform.translationMatrix;

			Vector4 pos4 = _position.ToTKVec4();
			pos4 *= mat4;

            return Vector2.FromTKVec4(pos4);
        }
        set {
            _position = value;
        }
    }

	/// <summary>
    /// The current world vector2 position of this Transform<br/>
	/// Setting this will set the local position of the object.
    /// </summary>
	public Vector2 localPosition {
		get {
			return _position;
		}
		set {
			_position = value;
		}
	}

    /// <summary>
    /// The current rotation of this Transformable in degrees
    /// </summary>
    public Vector3 rotation = Vector3.Zero;

    /// <summary>
    /// The current vector scale of this Transformable
    /// </summary>
    public Vector2 scale = Vector2.one;

	internal Matrix4 scaleMatrix {
		get {
			Matrix4 matrix = Matrix4.Identity;
			if (gameObject.parent)
				matrix = gameObject.parent.transform.scaleMatrix;

			matrix *= Matrix4.CreateScale(scale.ToTKVec3());

			return matrix;
		}
	}
	internal Matrix4 rotationMatrix {
		get {
			Matrix4 matrix = Matrix4.Identity;
			if (gameObject.parent)
				matrix = gameObject.parent.transform.rotationMatrix;

			matrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
			matrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
			matrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));

			return matrix;
		}
	}
	internal Matrix4 translationMatrix {
		get {
			Matrix4 matrix = Matrix4.Identity;
			if (gameObject.parent)
				matrix = gameObject.parent.transform.translationMatrix;

			matrix *= Matrix4.CreateTranslation(localPosition.x, localPosition.y, localPosition.z);
			// adding z just in case it is ever used internally (should be 0) (does not change sorting layer or order!)

			return matrix;
		}
	}

    internal Matrix4 matrix {
        get {
			return scaleMatrix * rotationMatrix * translationMatrix;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------
	//													InverseTransformPoint()
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Transforms the point from the game's global space to this object's local space.
	/// </summary>
	/// <returns>
	/// The point.
	/// </returns>
	/// <param name='x'>
	/// The x coordinate.
	/// </param>
	/// <param name='y'>
	/// The y coordinate.
	/// </param>
	//public virtual Vector2 InverseTransformPoint (float x, float y)
	//{
	//	Vector2 ret = Vector2.zero;
	//	x -= position.x;
	//	y -= position.y;
	//	if (scale.x != 0) ret.x = ((x * matrix[0] + y * matrix[1]) / scale.x); else ret.x = 0;
	//	if (scale.y != 0) ret.y = ((x * matrix[4] + y * matrix[5]) / scale.y); else ret.y = 0;
	//	return ret;
	//}

	/// <summary>
	/// Transforms the direction vector (x,y) from the game's global space to this object's local space.
	/// This means that rotation and scaling is applied, but translation is not.
	/// </summary>
	//public virtual Vector2 InverseTransformDirection (float x, float y)
	//{
	//	Vector2 ret = Vector2.zero;
	//	if (scale.x != 0) ret.x = ((x * matrix[0] + y * matrix[1]) / scale.x); else ret.x = 0;
	//	if (scale.y != 0) ret.y = ((x * matrix[4] + y * matrix[5]) / scale.y); else ret.y = 0;
	//	return ret;
	//}

	//------------------------------------------------------------------------------------------------------------------------
	//														DistanceTo()
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Returns the distance to another Transformable
	/// </summary>
	public float DistanceTo (GameObject other)
	{
		Vector2 otherPosition = other.transform.position;
		return Vector2.Distance(otherPosition, position);
	}

				
	//------------------------------------------------------------------------------------------------------------------------
	//														TransformPoint()
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Transforms the point from this object's local space to the game's global space.
	/// </summary>
	/// <returns>
	/// The point.
	/// </returns>
	/// <param name='x'>
	/// The x coordinate.
	/// </param>
	/// <param name='y'>
	/// The y coordinate.
	/// </param>
	//public virtual Vector2 TransformPoint(float x, float y) {
	//	Vector2 ret = Vector2.zero;
	//	ret.x = matrix[0] * x * scale.x + matrix[4] * y * scale.y + matrix[12];
	//	ret.y = matrix[1] * x * scale.x + matrix[5] * y * scale.y + matrix[13];
	//	return ret;
	//}

	/// <summary>
	/// Transforms a direction vector (x,y) from this object's local space to the game's global space. 
	/// This means that rotation and scaling is applied, but translation is not.
	/// </summary>
	//public virtual Vector2 TransformDirection(float x, float y) {
	//	Vector2 ret = Vector2.zero;
	//	ret.x = matrix[0] * x * scale.x + matrix[4] * y * scale.y;
	//	ret.y = matrix[1] * x * scale.x + matrix[5] * y * scale.y;
	//	return ret;
	//}

	//------------------------------------------------------------------------------------------------------------------------
	//														Move()
	//------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Move the object, based on its current rotation.
	/// </summary>
	/// <param name='stepX'>
	/// Step x.
	/// </param>
	/// <param name='stepY'>
	/// Step y.
	/// </param>
	//public void Move (float stepX, float stepY) {
	//	float r = rotation * Mathf.PI / 180.0f;
	//	float cs = Mathf.Cos (r);
	//	float sn = Mathf.Sin (r);
	//	position.x = (position.x + cs * stepX - sn * stepY);
	//	position.y = (position.y + sn * stepX + cs * stepY);
	//}

	/// <summary>
	/// Returns the inverse matrix, if it exists.
	/// (Use this e.g. for cameras used by sub windows)
	/// </summary>
	//public float[] Inverse() {
	//	float[] matrix = this.matrix;
	//	if (scale.x == 0 || scale.y == 0)
	//		throw new Exception ("Cannot invert a transform with scale 0");

	//	float cs = matrix [0];
	//	float sn = matrix [1];
	//	matrix [0] = cs / scale.x;
	//	matrix [1] = -sn / scale.y;
	//	matrix [4] = sn / scale.x;
	//	matrix [5] = cs / scale.y;
	//	matrix[12] = (-position.x * cs - position.y * sn) / scale.x;
	//	matrix[13] = (position.x * sn - position.y * cs) / scale.y;

	//	return matrix;
	//}

	public Transform Clone() {
		return new Transform() { gameObject = gameObject, position = localPosition, rotation = rotation, scale = scale };
	}
}
