using UGXP.Util;

namespace UGXP.Core;

public class RectTransform : Transform
{
    public FRectangle _bounds = new FRectangle(Vector2.zero, Vector2.one);
    public FRectangle bounds { get { return _bounds; } private set { _bounds = value; } }

    public Vector2 _size = Vector2.zero;
    public Vector2 size {
        get {
            return _size;
        }
        set { 
            _size = value;
            bounds.size = value;
        }
    }

    public float width {
        get { return _size.x * scale.x; }
    }
    public float height {
        get { return _size.y * scale.y; }
    }

    public new Vector2 position {
        get {
            return _position;
        }

        set {
            _position = value;
            bounds.position = value;
        }
    }

    public RectTransform() { }
}
