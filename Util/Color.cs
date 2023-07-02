using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Util;
public class Color
{
    public int r;
    public int g;
    public int b;
    public int a;

    /// <summary>
    /// A color class to store colors.
    /// </summary>
    public Color() : this(0,0,0,1) { }

    /// <summary>
    /// A color class to store colors.
    /// Values are ints 0-255
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    /// <param name="a">Alpha value</param>
    public Color(int r, int g, int b, int a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
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
}
