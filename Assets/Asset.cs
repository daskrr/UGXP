namespace UGXP.Assets;
public abstract class Asset
{
    /// <summary>
    /// Specifies if this asset has been created using <see cref="AssetManager.Create"/> or not<br/>
    /// This is set to true when the <see cref="AssetManager.Create"/> is used.
    /// </summary>
    internal bool created = false;

    /// <summary>
    /// The path to the asset (must include extension)<br/>
    /// <i>i.e.: Assets/Sprites/my_cool_sprite.png</i>
    /// </summary>
    public string Path;
    /// <summary>
    /// The unique name this asset is recognized by
    /// </summary>
    public string Name;

    /// <summary>
    /// Determines whether the asset will return a single asset instance when using <see cref="GetAssets"/>
    /// </summary>
    internal bool IsSingle = true;

    internal abstract List<string> GetAssetNames();

    /// <summary>
    /// Creates a instantiated assets (<see cref="AssetInstance"/> that can be used by the <see cref="AssetManager"/>.<br/>
    /// Only to be used if an asset structure can create multiple assets!
    /// </summary>
    /// <returns>The instantiated assets</returns>
    internal abstract Dictionary<string, AssetInstance> GetAssets();
}

public abstract class AssetInstance {
    public string name;
    public Asset asset;

    internal bool initialized = false;

    internal virtual void Load() { initialized = true; }
    internal abstract void Unload();
}
