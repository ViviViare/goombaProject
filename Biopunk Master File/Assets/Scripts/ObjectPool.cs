/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 11/02/24

// This script works in tandem with the ObjectPooler script to handle creation and maintenance of "object pools"; essentially, a pool of commonly used objects that get instantiated but never destroyed.
// They are then simply moved and re-enabled when necessary, to avoid having an excessive amount of instantiation and destruction; both of which have a relative high impact on performance.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public Stack<GameObject> _inactiveObjects = new Stack<GameObject>();
    public GameObject _parentObject;

    public ObjectPool(GameObject parentObject)
    {
        this._parentObject = parentObject;
    }

}
