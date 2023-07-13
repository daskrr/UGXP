namespace UGXP.Reference;
public class ObjectReference<T> : ObjectReference where T : IReferenceable
{
    internal ObjectReference(int id, string? name) : base(id, name) { }

    public T GetObject() {
         return ReferenceManager.Get<T>(this.id);
    }
}

public class ObjectReference : IEquatable<ObjectReference>
{
    internal int id;
    internal string? name;

    internal ObjectReference(int id, string? name) {
        this.id = id;
        this.name = name;
    }

    public bool Equals(ObjectReference? other) {
        return other == null ? false : other == this;
    }
    public override bool Equals(object? other) {
        if (other == null) return false;
        if (other is not ObjectReference) return false;
        return (other as ObjectReference) == this;
    }
    public override int GetHashCode() {
        return id;
    }

    public static bool operator ==(ObjectReference left, ObjectReference right) {
        if (left is null && right is null) return true;
        if (left is not null && right is null) return false;
        if (left is null && right is not null) return false;

        // just in case ?
        if (left is null || right is null) return false;

        return left.id == right.id && left.name == right.name;
    }
    public static bool operator !=(ObjectReference left, ObjectReference right) {
        return !(left == right);
    }
}

// not necessary if using generics
//internal enum ReferenceType {
//    GAME_OBJECT = 0,
//    ASSET = 1
//}

