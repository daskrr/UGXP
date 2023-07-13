namespace UGXP.Reference;

/// <summary>
/// IReferenceable makes any object able to be referenced using the <see cref="ReferenceManager"/> anywhere in the game at any time.<br/>
/// It is the duty of the object that implements this interface to create a reference (ONCE) using <see cref="ReferenceManager.Create"/> and
/// make it available through <see cref="GetReference"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReferenceable<T> : IReferenceable where T : IReferenceable
{
    public ObjectReference<T> GetReference();
}

public interface IReferenceable {  }
