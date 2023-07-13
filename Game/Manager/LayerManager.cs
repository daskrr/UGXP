using System.Collections.Immutable;

namespace UGXP.Game.Manager;
internal class LayerManager
{
    internal static ImmutableList<string> layers;
    internal static ImmutableList<string> sortingLayers;

    public static void Initialize(string[] layersSet, string[] sortingLayersSet) {
        // check if layers' size is bigger than 32 (meaning that it exceeds the bitmask)
        if (layersSet.Length > 32)
            throw new ArgumentOutOfRangeException(nameof(layersSet), "Maximum amount of layers exceeded. Up to 32 layers are possible.");

        layers = ImmutableList.CreateRange(layersSet);
        sortingLayers = ImmutableList.CreateRange(sortingLayersSet);

        // check if "Default" exists for both sets of layers
        if (!layers.Contains("Default"))
            layers = layers.Insert(0, "Default");
        if (!sortingLayers.Contains("Default"))
            sortingLayers = sortingLayers.Insert(0, "Default");
    }

    public static int GetLayerId(string name) {
        return layers.IndexOf(name);
    }

    public static string GetLayerName(int id) {
        if (layers.Count >= id)
            throw new ArgumentOutOfRangeException(nameof(id));

        return layers[id];
    }

    public static int GetSortingLayerId(string name) {
        return sortingLayers.IndexOf(name);
    }

    public static string GetSortingLayerName(int id) {
        if (sortingLayers.Count >= id)
            throw new ArgumentOutOfRangeException(nameof(id));

        return sortingLayers[id];
    }
}
