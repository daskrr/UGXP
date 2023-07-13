namespace UGXP.Util;
public class Color32
{
    public static readonly Color32 white = new Color32(255,255,255, 255);
    public static readonly Color32 black = new Color32(0,0,0, 255);

    public int r;
    public int g;
    public int b;
    public int a;

    public int this[int key] { 
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
            uint r = (uint) (this.r & 0xFF);
            uint g = (uint) (this.g & 0xFF);
            uint b = (uint) (this.b & 0xFF);
            uint a = (uint) (this.a & 0xFF);

            return (r << 24) + (g << 16) + (b << 8) + (a);
        }
    }

    /// <summary>
    /// A color class to store colors.
    /// </summary>
    public Color32() : this(0,0,0,255) { }

    /// <summary>
    /// A color class to store colors. Alpha is defaulted to 255
    /// Values are ints 0-255
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    public Color32(int r, int g, int b) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 255;
    }

    /// <summary>
    /// A color class to store colors.
    /// Values are ints 0-255
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    /// <param name="a">Alpha value</param>
    public Color32(int r, int g, int b, int a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public float[] GetColors01() {
        float[] colors = new float[4];
        colors[0] = (float) this.r / 255;
        colors[1] = (float) this.g / 255;
        colors[2] = (float) this.b / 255;
        colors[3] = (float) this.a / 255;

        return colors;
    }

    public static explicit operator Color(Color32 color) {
        return new Color((float) color.r / 255, (float) color.g / 255, (float) color.b / 255, (float) color.a / 255);
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
