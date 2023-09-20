using System.Reflection;
using UGXP.Util;

namespace UGXP.Assets;

public class AssetManager
{
    private static AssetManager _instance = null;
    private static AssetManager Instance {
        get {
            _instance ??= new AssetManager();

            return _instance;
        }
    }

    private Dictionary<string, Asset> assets = new();
    private Dictionary<string, AssetInstance> assetCache = new();

    private AssetManager() { }

    /// <summary>
    /// Creates an asset within the <see cref="AssetManager">'s context.
    /// </summary>
    /// <param name="asset">The asset to be created</param>
    /// <returns>The asset in context</returns>
    public static Asset Create(Asset asset) {
        // check if the asset exists
        if (Instance.assets.ContainsKey(asset.Name))
            throw new InvalidOperationException("This asset name is already in use");

        asset.created = true;
        Instance.assets.Add(asset.Name, asset);

        // add the asset instances to cache without initializing them
        foreach (var assetInst in asset.GetAssets())
            Instance.assetCache.Add(assetInst.Key, assetInst.Value);

        return asset;
    }

    /// <summary>
    /// Retrieves an asset instance from the stored assets or from the cache.<br/>
    /// If the asset is not cached but it exists, the asset is automatically cached until destroyed<br/>
    /// <i>Use <see cref="RemoveFromCache"/> to remove an asset from cache.</i>
    /// </summary>
    /// <typeparam name="T">The return type of the asset.</typeparam>
    /// <param name="name">The name of the asset</param>
    /// <param name="childName">Optional: The name of the sub-asset</param>
    /// <returns>The asset instance</returns>
    public static T Get<T>(string name, string? childName = null) where T : AssetInstance {
        AssetInstance asset = Get(name, childName);
        if (asset is not T)
            return DevelopmentHandlers.HandleValueNotFound<T>("The return type provided does not match the asset instance's type");

        return asset as T;
    }
    /// <summary>
    /// Retrieves an asset instance from the stored assets or from the cache (if asset is cached).<br/>
    /// If the asset is not cached but it exists, the asset is automatically cached until the game is closed.<br/>
    /// <i>Use <see cref="RemoveFromCache"/> to remove an asset from cache.</i>
    /// </summary>
    /// <param name="name">The name of the asset</param>
    /// <param name="childName">Optional: The name of the sub-asset</param>
    /// <returns>The asset instance</returns>
    public static AssetInstance Get(string name, string? childName = null) {
        // check if asset exists
        if (!Instance.assets.ContainsKey(name))
            return DevelopmentHandlers.HandleValueNotFound<AssetInstance>("The specified asset name does not exist. Use Create to make an asset.");

        if (!Instance.assets[name].IsSingle && childName == null)
            throw new Exception($"The asset [{name}] requested is not singular. Please use the second parameter to specify the child of this asset to get.");

        if (Instance.assets[name].IsSingle && childName != null)
            throw new Exception($"The asset [{name}] requested is singular and therefore has no children. Use only its name!");

        // check if child name exists too
        if (childName != null)
            if (!Instance.assets[name].GetAssetNames().Contains(childName))
                return DevelopmentHandlers.HandleValueNotFound<AssetInstance>("The specified sub-asset name does not exist. Use Create to make an asset.");

        childName ??= name;

        // check if this asset was cached (which would mean it was here before the game started)
        // then create and initialize the instance
        // if it wasn't already cached it means this was added while the game is running, therefore instantiation and initialization is done right now.
        if (!Instance.assetCache.ContainsKey(childName)) // we check if the child name is contained as only children can be stored in case there are multiple
            Load(name); // but we load the asset rather than the instance, which in turn loads all children (if any)

        return Instance.assetCache[childName];
    }

    /// <summary>
    /// Removes assets from cache.
    /// </summary>
    /// <param name="names">The names of the assets to unload</param>
    public static void RemoveFromCache(params string[] names) {
        Unload(names);
    }

    /// <summary>
    /// Finds an asset by name
    /// </summary>
    /// <param name="name">The name of the asset</param>
    /// <returns>The asset</returns>
    public static Asset GetAsset(string name) {
        if (!Instance.assets.ContainsKey(name))
            return DevelopmentHandlers.HandleValueNotFound<Asset>("The specified asset could not be found!");

        return Instance.assets[name];
    }


    /// <summary>
    /// Loads and initializes all specified assets based on their name.<br/>
    /// If an asset name doesn't exist as an asset it is ignored.
    /// </summary>
    /// <param name="toLoad">The asset names to load as assets</param>
    internal static void Load(params string[] toLoad) {
        // this does not unload cached assets, as that action needs to be done when an object is removed.
        foreach (string name in toLoad)
            if (Instance.assets.ContainsKey(name))
                foreach (var asset in Instance.assets[name].GetAssets()) {
                    // add uninitialized asset instance to cache
                    if (!Instance.assetCache.ContainsKey(asset.Key))
                        Instance.assetCache.Add(asset.Key, asset.Value);
                    
                    // initialize instance (if needed)
                    // this would be used while the game is running
                    if (!Instance.assetCache[asset.Key].initialized)
                        Instance.assetCache[asset.Key].Load();
                }
    }

    /// <summary>
    /// Unloads all specified assets based on their name.<br/>
    /// If an asset name doesn't exist as an asset it is ignored.
    /// <i>Be mindful that this method can only unload asset instances based on assets.<br/>
    /// An asset instance key (part of a multiple asset instances asset) will be ignored!</i>
    /// </summary>
    /// <param name="toUnload">The asset names to unload</param>
    internal static void Unload(params string[] toUnload) {
        foreach (string name in toUnload)
            if (Instance.assets.ContainsKey(name)) {
                List<string> assetNames = Instance.assets[name].GetAssetNames();

                foreach (var asset in assetNames) {
                    if (!Instance.assetCache.ContainsKey(asset)) continue;

                    AssetInstance assetInst = Instance.assetCache[asset];
                    Instance.assetCache.Remove(asset);
                    
                    assetInst.Unload();
                }
            }
    }

    /// <summary>
    /// Gets all names of any AssetInstance stored in a field in the provided object.
    /// </summary>
    /// <param name="obj">The object to gather asset names from the fields of</param>
    /// <returns>The gathered asset names</returns>
    internal static string[] GatherAssetNamesFromObject(object obj) {
        List<string> names = new();

        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields.Where(f => f.FieldType.IsSubclassOf(typeof(AssetInstance))))
            if (field.GetValue(obj) != null)
                names.Add((field.GetValue(obj) as AssetInstance).name);

        return names.ToArray();
    }
}
