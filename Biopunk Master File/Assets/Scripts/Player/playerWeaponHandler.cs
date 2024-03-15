/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// A script that handles the player's weapon attacks. Serves as a middleman between the player's inputs and the weapons, so as to both work with the
// new input system and avoid overuse of if statements in every weapon's update function to check for attack inputs.

// Edits since script completion:
// 05/03/24: Cut down script bloat by a lot, also making the script more modular.
*/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerWeaponHandler : MonoBehaviour
{
    [SerializeField] public GameObject _leftWeapon;
    [SerializeField] public GameObject _rightWeapon;

    [SerializeField] public LeftOrRight _leftOrRight;

    public static event Action<LeftOrRight> _triggerAction;

    // The below methods are called by the Player Input component attached to the player, with "LeftWeapon" being called on a "left attack" input and vice versa.

    // The aforementioned method has become vastly more efficient due to the introduction of actions; now, instead of having to reference each weapon's 
    // "attacking" scripts and components to actually call the corresponding weapon, each weapon's scripts now individually subscribe to a "trigger action"
    // which then locally references and runs the weapon's "attacking" method.

    // This is a lot prettier, modular and easier to work with than having to individually check to see if a weapon had a certain component before calling the method;
    // no more do we have to suffer through thirty different "if" statements checking to see if the left weapon is a quadra, fist, normal gun, or normal melee weapon.

    // Thank you Levi :)
    public void LeftWeapon()
    {
        _leftOrRight = LeftOrRight.Left;
        _triggerAction.Invoke(LeftOrRight.Left);
    }

    public void RightWeapon()
    {
        _leftOrRight = LeftOrRight.Right;
        _triggerAction.Invoke(LeftOrRight.Right);
    }
}
