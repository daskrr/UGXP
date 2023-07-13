using System.Reflection.Emit;
using UGXP.Game.Manager;

namespace UGXP.Game;
public struct LayerMask
{
    private int mask;

    public static implicit operator int(LayerMask mask) {
        return mask.mask;
    }

    // implicitly converts an integer to a LayerMask
    public static implicit operator LayerMask(int intVal) {
        return new LayerMask { mask = intVal };
    }

    public int value {
        get { return mask; }
        set { mask = value; }
    }

    public bool HasLayer(int layer) {
        return this == (this | 1 << layer);
    }
    public bool HasLayer(string layer) {
        return this == (this | 1 << NameToId(layer));
    }

    public int[] GetLayerIds() {
        List<int> ids = new();

        for (int i = 0; i < 32; i++)
            if (this == (this | (1 << i)))
                ids.Add(i);

        return ids.ToArray();
    }

    public string[] GetLayerNames() {
        List<string> layers = new();

        for (int i = 0; i < 32; i++)
            if (this == (this | (1 << i)))
                layers.Add(IdToName(i));

        return layers.ToArray();
    }

    public static string IdToName(int layer) {
        return LayerManager.GetLayerName(layer);
    }

    public static int NameToId(string name) {
        return LayerManager.GetLayerId(name);
    }

    public static int Create(params string[] names) {
        if (names == null) throw new ArgumentNullException(nameof(names));

        int mask = 0;
        foreach (string name in names) {
            int layerId = NameToId(name);

            if (layerId != -1)
                mask |= 1 << layerId;
        }

        return mask;
    }
}
