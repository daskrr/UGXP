using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGXP.Util;

namespace UGXP.Core.Components;
public abstract class Renderer : Component
{
    public Color color;
    public BlendMode blendMode;
    public int sortingLayer;
    public int sortingOrder;

    public abstract void Render();
}
