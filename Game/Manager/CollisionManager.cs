using Differ.Data;
using System.Reflection;
using UGXP.Core;
using UGXP.Core.Components;
using UGXP.Util;

namespace UGXP.Game.Manager;

/// <summary>
/// The collision manager calculates the overlap of every active game object with (a) collider(s) 
/// and moves physics-affected objects apart, as well as calling appropriate on collision/trigger methods (enter/exit).<br/>
/// When a collision happens between <b>one</b> of the colliders of a game object, the appropriate method is called <b>for every collider component</b>.<br/>
/// The on collision methods are called when a collider comes into contact with <b>the first collider</b>. If this collider is no longer in contact with
/// the previous collider, but another one starts/is still touching this collider, an exit callback will be called, followed by an immediate enter callback.
/// </summary>
internal class CollisionManager
{
    private delegate void CollisionCallback(Collision2D collision);
    private delegate void TriggerCallback(Collider collider);
    private class OnCollision {
        /// <summary>
        /// Called once when a trigger collision happens
        /// </summary>
        public TriggerCallback? triggerEnter = null;
        /// <summary>
        /// Called every frame a trigger collision is still happening
        /// </summary>
        public TriggerCallback? triggerStay = null;
        /// <summary>
        /// Called once when a trigger collision stops
        /// </summary>
        public TriggerCallback? triggerExit = null;

        /// <summary>
        /// Called once when a collision happens
        /// </summary>
        public CollisionCallback? collisionEnter = null;
        /// <summary>
        /// Called every frame a collision is still happening
        /// </summary>
        public CollisionCallback? collisionStay = null;
        /// <summary>
        /// Called once when a collision stops
        /// </summary>
        public CollisionCallback? collisionExit = null;

        public void CallEnter(Collision2D collision, Collider collider) {
            triggerEnter?.Invoke(collider);
            collisionEnter?.Invoke(collision);
        }

        public void CallStay(Collision2D collision, Collider collider) {
            triggerStay?.Invoke(collider);
            collisionStay?.Invoke(collision);
        }

        public void CallExit(Collision2D collision, Collider collider) {
            triggerExit?.Invoke(collider);
            collisionExit?.Invoke(collision);
        }
    }

    private bool lockEdit = false;

    private readonly Dictionary<GameObject, OnCollision> registeredGameObjects = new();

    // we can do shit like this cuz this is a high level programming language \o/
    // stores the first collision objects that are currently in a collision
    private readonly Dictionary<Collider, Collider> currentCollisions = new();

    public void Add(GameObject obj) {
        if (lockEdit) throw new InvalidOperationException("Cannot modify the objects whilst the manager is locked (step is being executed)");

        if (!obj || !obj.active) return;
        if (obj.colliders.Count == 0) return;

        Remove(obj);

        bool hasTrigger = false;
        bool hasNonTrigger = false;

        foreach (var collider in obj.colliders) {
            if (collider.IsTrigger) hasTrigger = true;
            if (!collider.IsTrigger) hasNonTrigger = true;
        }

        OnCollision onCol = new();
        // go through all components' methods
        Array.ForEach(obj.GetComponents(), comp => {
            if (!comp.active) return;

            if (hasTrigger) {
                onCol.triggerEnter ??= delegate(Collider _) { };
                onCol.triggerStay ??= delegate(Collider _) { };
                onCol.triggerExit ??= delegate(Collider _) { };

                onCol.triggerEnter += MethodToTriggerCallback(comp, "OnTriggerEnter");
                onCol.triggerStay += MethodToTriggerCallback(comp, "OnTriggerStay");
                onCol.triggerExit += MethodToTriggerCallback(comp, "OnTriggerExit");
            }
            if (hasNonTrigger) { 
                onCol.collisionEnter ??= delegate(Collision2D _) { };
                onCol.collisionStay ??= delegate(Collision2D _) { };
                onCol.collisionExit ??= delegate(Collision2D _) { };

                onCol.collisionEnter += MethodToCollisionCallback(comp, "OnCollisionEnter");
                onCol.collisionStay += MethodToCollisionCallback(comp, "OnCollisionStay");
                onCol.collisionExit += MethodToCollisionCallback(comp, "OnCollisionExit");
            }
        });

        registeredGameObjects.Add(obj, onCol);
    }

    public void Update(GameObject obj) {
        if (lockEdit) throw new InvalidOperationException("Cannot modify the objects whilst the manager is locked (step is being executed)");

        Add(obj);
    }

    public void Remove(GameObject obj) {
        if (lockEdit) throw new InvalidOperationException("Cannot modify the objects whilst the manager is locked (step is being executed)");

        registeredGameObjects.Remove(obj);
    }

    public void Step() {
        lockEdit = true;

        // we loop thorugh the registered game objects' colliders
        foreach (var obj1 in registeredGameObjects) {
            foreach (var collider1 in obj1.Key.colliders) {
                // this represents the first collider it collides with
                List<Collider> collidedWith = new();
                // the differ shape collisions that happened (which will be used to a final UGXP Collision object)
                List<ShapeCollision> collisions = new();

                // we loop through all of the registered game objects' colliders again for each collider to check all of them against one another
                foreach (var obj2 in registeredGameObjects) { 
                    if (obj1.Equals(obj2)) continue;

                    foreach (var collider2 in obj2.Key.colliders) {
                        if (collider1.Equals(collider2)) continue;

                        // hit test
                        List<ShapeCollision> _collisions = new();

                        foreach (var shape in collider1.diffShapes)
                            _collisions.AddRange(Differ.Collision.shapeWithShapes(shape, collider2.diffShapes));

                        // no collision happened, dont account for this collider2
                        if (_collisions.Count == 0) continue;

                        // move objects apart (if neither is trigger) (but leave a small amount in, so that the algo doesn't see them not colliding yet)
                        // TODO using physics (only on physics enabled objects with non-trigger colliders)

                        // add the collider to the collided with
                        collidedWith.Add(collider2);
                        // add the shape collisions to the total
                        collisions.AddRange(_collisions);
                    }
                }

                // THIS SHIT IS SO FUCKING COMPLICATED OH. MY. GOD.
                // basically makes sure that at any point if an object is still colliding it doesnt spam the oncollision/ontrigger method(s)
                // and fires the method(s) accordingly, including the stay & exit ones
                if (collidedWith.Count > 0) {
                    // check if this game object collider is already colliding
                    if (!currentCollisions.ContainsKey(collider1)) {
                        // it's not, delegate the first collided with as the collision this game object is in
                        currentCollisions.Add(collider1, collidedWith[0]);

                        // callback the appropriate enter method for both game objects
                        Collision2D collision = new(collider1, collidedWith[0]);
                        //Collision2D collisionOther = new(collidedWith[0], collider1);

                        obj1.Value.CallEnter(collision, collidedWith[0]);
                        //registeredGameObjects[collidedWith[0].gameObject]?.CallEnter(collisionOther, collider1); // might need to remove this, cuz it also executes from the pov of the other collider already
                    }
                    else {
                        // check if the collision the game object collider was in still exists
                        Collider collidingWith = currentCollisions[collider1];
                        if (!collidedWith.Contains(collidingWith)) {
                            // doesn't, call appropriate exit method for both game objects
                            Collision2D collision = new(collider1, collidedWith[0]);
                            //Collision2D collisionOther = new(collidedWith[0], collider1);

                            obj1.Value.CallExit(collision, collidedWith[0]);
                            //registeredGameObjects[collidedWith[0].gameObject]?.CallExit(collisionOther, collider1);

                            // remove it from the current collisions
                            currentCollisions.Remove(collider1);
                        }
                        else {
                            // the colliders are still touching (hopefully not inappropriately)
                            // call the stay callback for both game objects
                            Collision2D collision = new(collider1, collidingWith);
                            //Collision2D collisionOther = new(collidingWith, collider1);

                            obj1.Value.CallStay(collision, collidingWith);
                            //registeredGameObjects[collidingWith.gameObject]?.CallStay(collisionOther, collider1);
                        }

                        // generate new collision to the next object (if any)
                        if (collidedWith.Count > 1) {
                            currentCollisions.Add(collider1, collidedWith[1]);

                            // callback the appropriate enter method for both game objects
                            Collision2D collision = new(collider1, collidedWith[1]);
                            //Collision2D collisionOther = new(collidedWith[1], collider1);

                            obj1.Value.CallEnter(collision, collidedWith[1]);
                            //registeredGameObjects[collidedWith[1].gameObject]?.CallEnter(collisionOther, collider1);
                        }
                    }
                }
                else {
                    // the object isn't colliding with anything
                    // check if the currentCollisions contains the object collider
                    if (currentCollisions.ContainsKey(collider1)) {
                        // call the appropriate exit method for both game objects
                        Collision2D collision = new(collider1, currentCollisions[collider1]);
                        //Collision2D collisionOther = new(currentCollisions[collider1], collider1);

                        obj1.Value.CallExit(collision, currentCollisions[collider1]);
                        //registeredGameObjects[currentCollisions[collider1].gameObject]?.CallExit(collisionOther, collider1);

                        // remove it from the current collisions
                        currentCollisions.Remove(collider1);
                    }
                }
            }
        }

        lockEdit = false;
    }

    private static CollisionCallback? MethodToCollisionCallback(object obj, string methodName) {
        MethodInfo info = Reflection.GetMethod(obj, methodName);
        if (info != null) {
			CollisionCallback methodDelegate = (CollisionCallback) Delegate.CreateDelegate(typeof(CollisionCallback), obj, info);
			if (methodDelegate != null)
				return methodDelegate;
		}

        return null;
    }
    private static TriggerCallback? MethodToTriggerCallback(object obj, string methodName) {
        MethodInfo info = Reflection.GetMethod(obj, methodName);
        if (info != null) {
			TriggerCallback methodDelegate = (TriggerCallback) Delegate.CreateDelegate(typeof(TriggerCallback), obj, info, false);
			if (methodDelegate != null)
				return methodDelegate;
		}

        return null;
    }
}
