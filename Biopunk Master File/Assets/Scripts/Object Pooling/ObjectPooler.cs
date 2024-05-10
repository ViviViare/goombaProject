/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 11/02/24

// This script works in tandem with the ObjectPool script to handle creation and maintenance of "object pools"; essentially, a pool of commonly used objects that get instantiated but never destroyed.
// They are then simply moved and re-enabled when necessary, to avoid having an excessive amount of instantiation and destruction; both of which have a relative high impact on performance.

// Edits since script completion:
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    private static Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();

    /*
    // The below method replaces instantiation in favour of spawning an object from an object pool. It takes in three variables; what object to spawn, where to spawn it and what rotation to spawn it with.
    // If an object is spawned and already has a corresponding object pool, the last inactive object in the pool gets re-enabled and its position/rotation set to the input values.
    // The result is indistinguishable from instantiating a "fresh" object.

    // If an object is spawned, but there are no currently inactive objects in the pool, it will instantiate a new object and add it to the pool.
    // This is initially the same as regular instantiation in terms of performance hit, but once enough objects are in the pool they will not have to be instantiated again.
    // For example; a "gun" object has to instantiate a new bullet every time it is fired, whereas an object pool will only have to instantiate 10-20 bullets that are, in essence, "re-used".
    // This, as mentioned, is a lot more performance friendly.

    // Should there not be a corresponding object pool for an object in the first place, the method below will create a new object pool for the object and begin to fill it when required.
    */
    public static GameObject Spawn(GameObject gameObject, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject obj;
        string key = gameObject.name.Replace("(Clone)", "");
        GameObject objectUsed;

        if (_pools.ContainsKey(key))
        {
            if (_pools[key]._inactiveObjects.Count == 0)
            {
                objectUsed = Object.Instantiate(gameObject, spawnPosition, spawnRotation, _pools[key]._parentObject.transform);

            }
            else
            {
                objectUsed = obj = _pools[key]._inactiveObjects.Pop();
                obj.transform.position = spawnPosition;
                obj.transform.rotation = spawnRotation;
                obj.SetActive(true);
            }
        }
        else
        {
            GameObject newParent = new GameObject($"{key}_POOL");
            objectUsed = Object.Instantiate(gameObject, spawnPosition, spawnRotation, newParent.transform);
            ObjectPool newPool = new ObjectPool(newParent);
            _pools.Add(key, newPool);
        }

        return(objectUsed);
    }

    // The below method handles despawning of objects in an object pool.
    public static void Despawn(GameObject gameObject)
    {
        string key = gameObject.name.Replace("(Clone)", "");

        if(_pools.ContainsKey(key))
        {
            _pools[key]._inactiveObjects.Push(gameObject);
            gameObject.transform.position = _pools[key]._parentObject.transform.position;
            gameObject.SetActive(false);
        }
        else
        {
            GameObject newParent = new GameObject($"{key}_POOL");
            ObjectPool newPool = new ObjectPool(newParent);

            gameObject.transform.SetParent(newParent.transform);

            _pools.Add(key, newPool);
            _pools[key]._inactiveObjects.Push(gameObject);
            gameObject.SetActive(false);
        }
    }
}
