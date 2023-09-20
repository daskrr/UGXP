using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UGXP.Util;

namespace UGXP.Reference;

// TODO refrences are completely destroyed when a scene is reset, even if they were not part of that scene
// also only make refrences from the active scenes accessible
public class ReferenceManager
{
    private static ReferenceManager _instance;
    private static ReferenceManager Instance {
        get {
            _instance ??= new ReferenceManager();

            return _instance;
        }
    }

    /// <summary>
    /// Holds all active references
    /// Those references cannot be removed, unless the scene is unloaded
    /// </summary>
    private Dictionary<ObjectReference, IReferenceable> references = new();

    private Dictionary<int, ObjectReference> referencesById = new();
    private Dictionary<string, ObjectReference> referencesByName = new();

    private int referenceIndex = -1;
    private ReferenceManager() { }

    /// <summary>
    /// Creates a reference of an object that can be used anywhere with <see cref="Get"/>
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IReferenceable"/> object</typeparam>
    /// <param name="referenceName">The name of the reference</param>
    /// <param name="referenceable">The <see cref="IReferenceable"/> object</param>
    /// <returns>The object reference</returns>
    /// <exception cref="Exception">Throws an exception if the <paramref name="referenceName"/> already exists.</exception>
    public static ObjectReference<T> Create<T>(string? referenceName, IReferenceable<T> referenceable) where T : IReferenceable {
        if (referenceName != null && ReferenceExists(referenceName))
            throw new Exception("Reference name already exists! Cannot override a reference; please choose another name.");

        int referenceId = ++Instance.referenceIndex;
        ObjectReference<T> reference = new ObjectReference<T>(referenceId, referenceName);

        Instance.references.Add(reference, referenceable);
        Instance.referencesById.Add(referenceId, reference);
        if (referenceName != null)
            Instance.referencesByName.Add(referenceName, reference);

        return reference;
    }

    /// <summary>
    /// Retrieves the referenceable object that corresponds to the name
    /// </summary>
    /// <typeparam name="T">the type of object required</typeparam>
    /// <param name="referenceName">the name of the reference</param>
    /// <returns>the referenceable object</returns>
    /// <exception cref="Exception">Throws an exception if the reference could not be found</exception>
    public static T Get<T>(string referenceName) where T : IReferenceable {
        foreach (var reference in Instance.references)
            if (reference.Key.name == referenceName)
                return (T) reference.Value;

        return DevelopmentHandlers.HandleValueNotFound<T>("The reference name provided does not exist!");
    }
    /// <summary>
    /// Retrieves the referenceable object that corresponds to the id
    /// </summary>
    /// <typeparam name="T">the type of object required</typeparam>
    /// <param name="referenceId">the id of the reference</param>
    /// <returns>the referenceable object</returns>
    /// <exception cref="Exception">Throws an exception if the reference could not be found</exception>
    public static T Get<T>(int referenceId) where T : IReferenceable {
        foreach (var reference in Instance.references)
            if (reference.Key.id == referenceId)
                return (T) reference.Value;

        return DevelopmentHandlers.HandleValueNotFound<T>("The reference id provided does not exist!");
    }

    /// <summary>
    /// Checks if a reference already exists (by name)
    /// </summary>
    /// <param name="name">The name of the reference</param>
    public static bool ReferenceExists(string name) {
        return Instance.referencesByName.ContainsKey(name);
    }

    /// <summary>
    /// Checks if a reference exists (by id)
    /// </summary>
    /// <param name="name">The name of the reference</param>
    public static bool ReferenceExists(int id) {
        return Instance.referencesById.ContainsKey(id);
    }

    /// <summary>
    /// Removed a reference (used in the case that a game object is destroyed, for example)
    /// </summary>
    /// <param name="id">the id of the reference</param>
    internal static void RemoveReference(int id) {
        if (!ReferenceExists(id)) return;

        ObjectReference reference = Instance.referencesById[id];

        Instance.references.Remove(reference);
        Instance.referencesById.Remove(reference.id);
        if (reference.name != null)
            Instance.referencesByName.Remove(reference.name);
    }

    /// <summary>
    /// Resets all references (excludes Objects that are not destroyed on load)
    /// </summary>
    [Obsolete("The references are supposed to be automatically destroyed just as they are created by their objects!", true)]
    internal static void ResetReferences() {
        // save dontDestroyOnLoad references
        Dictionary<ObjectReference, IReferenceable> savedReferences = new();
        Dictionary<int, ObjectReference> savedReferencesById = new();
        Dictionary<string, ObjectReference> savedReferencesByName = new();
        foreach (var reference in Instance.references)
            if (reference.Value is Core.Object && (reference.Value as Core.Object).dontDestroyOnLoad) {
                savedReferences.Add(reference.Key, reference.Value);
                savedReferencesById.Add(reference.Key.id, reference.Key);
                if (reference.Key.name != null)
                    savedReferencesByName.Add(reference.Key.name, reference.Key);
            }

        Instance.referenceIndex = -1;
        Instance.references = savedReferences;
        Instance.referencesById = savedReferencesById;
        Instance.referencesByName = savedReferencesByName;
        //Instance.references.Clear();
    }
}
