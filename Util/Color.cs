namespace UGXP.Util;
public class Color
{
    public static Color white => new Color(1,1,1, 1);
    public static Color black => new Color(0,0,0, 1);

    public float r;
    public float g;
    public float b;
    public float a;

    public float this[int key] { 
        get {
            switch(key) {
                case 0:
                    return this.r;
                case 1:
                    return this.g;
                case 2:
                    return this.b;
                case 3:
                    return this.a;
            }

            throw new IndexOutOfRangeException($"The index { key } does not correspond to a value of rgba. Use indices in the range of [0-3]");
        }
    }

    public uint hex {
        get {
            Color32 c32 = (Color32) this;

            uint r = (uint) (c32.r & 0xFF);
            uint g = (uint) (c32.g & 0xFF);
            uint b = (uint) (c32.b & 0xFF);
            uint a = (uint) (c32.a & 0xFF);

            return (r << 24) + (g << 16) + (b << 8) + (a);
        }
    }

    /// <summary>
    /// A color class to store colors.
    /// </summary>
    public Color() : this(0,0,0,1) { }

    /// <summary>
    /// A color class to store colors. Alpha is defaulted to 255
    /// Values are ints 0-255
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    public Color(float r, float g, float b) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 1;
    }

    /// <summary>
    /// A color class to store colors.
    /// Values are ints 0-255
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    /// <param name="a">Alpha value</param>
    public Color(float r, float g, float b, float a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static explicit operator Color32(Color color) {
        return new Color32((int) (color.r * 255), (int) (color.g * 255), (int) (color.b * 255), (int) (color.a * 255));
    }

    //TODO
    ///// <summary>
    ///// Linearly interpolates between colors a and b by t.
    /////t is clamped between 0 and 1. When t is 0 returns a. When t is 1 returns b.
    ///// </summary>
    ///// <param name="a">Start color</param>
    ///// <param name="b">Target color</param>
    ///// <param name="t">time</param>
    ///// <returns></returns>
    //public static Color Lerp(Color a, Color b, float t) {
    //    Color color = new();
    //    Mathf.Lerp();
    //}

    public static Color32 FromHex(uint hex) {
        return new Color32((int) (hex >> 24 & 0xFF), (int) (hex >> 16 & 0xFF), (int) (hex >> 8 & 0xFF), (int) (hex & 0xFF));
    }

    public override string ToString() {
        return $"Color32[R:{r}, G:{g}, B:{b}, A:{a}]";
    }
}
