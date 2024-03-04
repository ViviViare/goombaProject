/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 09/02/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Door.cs
//
//  Holds the data for doors
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/

using System;
using UnityEngine;

[Serializable]
public class Door
{
    // _direction is used for the direction that this door is facing (Can be edited by a script to rotate rooms as nessissary)
    [ShowOnly] public Compass _direction;
    [ShowOnly] public GameObject _doorGo;

    // _isSet is used for if there is any doors on this direction
    [ShowOnly] public bool _isValid;

    // _isIrregularConnector is used for if the wall does not exist in this direction
    [ShowOnly] public bool _isIrregularConnector;

    // _isSet is used for if the door is being used to connect to something
    [ShowOnly] public bool _isSetAsUsed;

    public Door(Compass setTo)
    {
        _direction = setTo;
    }
}


