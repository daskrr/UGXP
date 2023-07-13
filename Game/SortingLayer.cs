using System.Threading.Tasks;
using UGXP.Game.Manager;

namespace UGXP.Game;
internal struct SortingLayer
{
    private int id;

    public static implicit operator int(SortingLayer layer) {
        return layer.id;
    }

    public static implicit operator SortingLayer(int intVal) {
        return new SortingLayer { id = intVal };
    }

    public int value {
        get { return id; }
        set { id = value; }
    }

    public static string IdToName(int layer) {
        return LayerManager.GetSortingLayerName(layer);
    }

    public static int NameToId(string name) {
        return LayerManager.GetSortingLayerId(name);
    }
}
