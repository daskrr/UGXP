using UGXP.Core.Render;
using UGXP.Game;
using UGXP.Util;

namespace UGXP.Core.Components;
public abstract class Renderer : Component
{
    public Color32 color = Color32.white;
    public BlendMode blendMode = BlendMode.NORMAL;
    public int sortingLayer = SortingLayer.NameToId("Default");
    public int sortingOrder = 0;

    internal abstract void Render();
}
