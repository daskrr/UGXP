namespace UGXP.Util;

public class FRectangle
{
    public Vector2 position;
    public Vector2 size;
    public bool inverseY = false;

    public FRectangle(Vector2 position, Vector2 size, bool inverseY = false) {
        this.position = position;
        this.size = size;
        this.inverseY = inverseY;
    }

    public float left { 
        get { return position.x; }
    }
	public float right {
        get { return position.x + size.x; }
    }
	public float top {
        get {
            if (inverseY)
                return position.y + size.y;
            else
                return position.y;
        }
    }
	public float bottom { 
        get { 
            if (inverseY)
                return position.y;
            else
            return position.y + size.y;
        }
    }

    public override string ToString() {
        return $"FRectangle[{position.x},{position.y}][{size.x},{size.y}]";
    }
}
