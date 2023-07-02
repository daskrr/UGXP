using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Core;

public class Transform
{
    protected static float[] matrixSample {
        get {
            return new float[16] {
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f };
        }
    }

    private Vector2 _position = Vector2.zero;

    /// <summary>
    /// The current vector position of this Transformable in x,y space
    /// </summary>
    public Vector2 position {
        get {
            return _position;
        }
        set {
            _position = value;
        }
    }

    // TODO turn this into a vector3 for x,y,z rotation
    /// <summary>
    /// The current z rotation of this Transformable
    /// </summary>
    public float rotation = 0f;

    /// <summary>
    /// The current vector scale of this Transformable
    /// </summary>
    public Vector2 scale = Vector2.zero;

    public float[] matrix {
        get {
            float[] matrix = matrixSample;

            // set rotation
            float r = rotation * Mathf.PI / 180.0f;
			float cs = Mathf.Cos (r);
			float sn = Mathf.Sin (r);

            matrix[0] = cs;
			matrix[1] = sn;
			matrix[4] = -sn;
			matrix[5] = cs;

            // scale
			matrix[0] *= scale.x;
			matrix[1] *= scale.x;
			matrix[4] *= scale.y;
			matrix[5] *= scale.y;

            // set position
            matrix[12] = position.x;
            matrix[13] = position.y;

			return matrix;
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
	public virtual Vector2 InverseTransformPoint (float x, float y)
	{
		Vector2 ret = Vector2.zero;
		x -= position.x;
		y -= position.y;
		if (scale.x != 0) ret.x = ((x * matrix[0] + y * matrix[1]) / scale.x); else ret.x = 0;
		if (scale.y != 0) ret.y = ((x * matrix[4] + y * matrix[5]) / scale.y); else ret.y = 0;
		return ret;
	}

	/// <summary>
	/// Transforms the direction vector (x,y) from the game's global space to this object's local space.
	/// This means that rotation and scaling is applied, but translation is not.
	/// </summary>
	public virtual Vector2 InverseTransformDirection (float x, float y)
	{
		Vector2 ret = Vector2.zero;
		if (scale.x != 0) ret.x = ((x * matrix[0] + y * matrix[1]) / scale.x); else ret.x = 0;
		if (scale.y != 0) ret.y = ((x * matrix[4] + y * matrix[5]) / scale.y); else ret.y = 0;
		return ret;
	}

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
	public virtual Vector2 TransformPoint(float x, float y) {
		Vector2 ret = Vector2.zero;
		ret.x = matrix[0] * x * scale.x + matrix[4] * y * scale.y + matrix[12];
		ret.y = matrix[1] * x * scale.x + matrix[5] * y * scale.y + matrix[13];
		return ret;
	}

	/// <summary>
	/// Transforms a direction vector (x,y) from this object's local space to the game's global space. 
	/// This means that rotation and scaling is applied, but translation is not.
	/// </summary>
	public virtual Vector2 TransformDirection(float x, float y) {
		Vector2 ret = Vector2.zero;
		ret.x = matrix[0] * x * scale.x + matrix[4] * y * scale.y;
		ret.y = matrix[1] * x * scale.x + matrix[5] * y * scale.y;
		return ret;
	}

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
	public void Move (float stepX, float stepY) {
		float r = rotation * Mathf.PI / 180.0f;
		float cs = Mathf.Cos (r);
		float sn = Mathf.Sin (r);
		position.x = (position.x + cs * stepX - sn * stepY);
		position.y = (position.y + sn * stepX + cs * stepY);
	}

	/// <summary>
	/// Returns the inverse matrix, if it exists.
	/// (Use this e.g. for cameras used by sub windows)
	/// </summary>
	public float[] Inverse() {
		float[] matrix = this.matrix;
		if (scale.x == 0 || scale.y == 0)
			throw new Exception ("Cannot invert a transform with scale 0");

		float cs = matrix [0];
		float sn = matrix [1];
		matrix [0] = cs / scale.x;
		matrix [1] = -sn / scale.y;
		matrix [4] = sn / scale.x;
		matrix [5] = cs / scale.y;
		matrix[12] = (-position.x * cs - position.y * sn) / scale.x;
		matrix[13] = (position.x * sn - position.y * cs) / scale.y;

		return matrix;
	}
}
