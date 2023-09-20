namespace UGXP.Assets;
public class PhysicsMaterialAsset : Asset
{
    // hide path since this asset doesn't need one
    // TODO not sure whether to add the possibility to store a material as a file and load it from a file..?
    private new string Path;

    public float Friction = 4f;
    public float Bounciness = 1f;

    internal override List<string> GetAssetNames() {
        return new() { Name };
    }

    internal override Dictionary<string, AssetInstance> GetAssets() {
        return new() { { Name, new PhysicsMaterial(Friction, Bounciness) } };
    }
}

public class PhysicsMaterial : AssetInstance {
    public float Friction;
    public float Bounciness;

    public PhysicsMaterial(float friction, float bounciness) {
        Friction = friction;
        Bounciness = bounciness;
    }

    // TODO add calculations?

    internal override void Unload() {  }
}