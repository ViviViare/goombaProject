/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// A script that handles the player's weapon attacks. Serves as a middleman between the player's inputs and the weapons, so as to both work with the
// new input system and avoid overuse of if statements in every weapon's update function to check for attack inputs.

// IMPORTANT NOTE!!!!!!!!!!!!!!
// READ OR THINGS BREAK!!!!!!!!!!
// Given that we currently do not have weapon swapping implemented, if you want to test out the various weapons follow these steps:
// 1: Enable the weapons you want to use in the editor; only enable one left weapon and one right weapon
// 2: On the playerWeaponHandler script on the player, drag any currently enabled weapons in their corresponding fields (GunL in _leftWeapon for example).
// If you do not do the above, the game will throw an error at you.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerWeaponHandler : MonoBehaviour
{
    [SerializeField] public GameObject _leftWeapon;
    [SerializeField] public GameObject _rightWeapon;

    [SerializeField] public bool _leftOrRight;


    // The below methods are called by the Player Input component attached to the player, with "LeftWeapon" being called on a "left attack" input and vice versa.
    // Essentially, it checks the _leftWeapon or _rightWeapon gameobject to see if it has a ranged component, melee component or special component (like the Quadra
    // script or the Knockback Fist script). If it doesn't, it moves on and checks for another component. If the gameobject does contain the respective component,
    // it calls its "attacking" method.

    // This may be a little clunky, and probably more optimizable, but it works pretty robustly and even if inefficient is leaps and bounds more resource efficient 
    // than having a "if attack.input = true" check in every weapon's respective Update() method to actually execute attacks, as the way this script handles attacking
    // only executes the corresponding attack method once when an attack input is made.
    public void LeftWeapon()
    {
        _leftOrRight = false;
        if (_leftWeapon.GetComponent<playerQuadra>() != null)
        {
            _leftWeapon.GetComponent<playerQuadra>().FireQuarda();
        }
        else if (_leftWeapon.GetComponent<playerRangedAttack>() != null)
        {
            _leftWeapon.GetComponent<playerRangedAttack>().FireRangedWeapon();
        }
        else if (_leftWeapon.GetComponent<playerBaseMelee>() != null)
        {
            _leftWeapon.GetComponent<playerBaseMelee>().SwingMeleeWeapon();
        }
        else if (_leftWeapon.GetComponent<playerKnockbackFist>() != null)
        {
            _leftWeapon.GetComponent<playerKnockbackFist>().SwingMeleeWeapon();
        }
    }

    public void RightWeapon()
    {
        _leftOrRight = true;
        if (_rightWeapon.GetComponent<playerQuadra>() != null)
        {
            _rightWeapon.GetComponent<playerQuadra>().FireQuarda();
        }
        if (_rightWeapon.GetComponent<playerRangedAttack>() != null)
        {
            _rightWeapon.GetComponent<playerRangedAttack>().FireRangedWeapon();
        }
        else if (_rightWeapon.GetComponent<playerBaseMelee>() != null)
        {
            _rightWeapon.GetComponent<playerBaseMelee>().SwingMeleeWeapon();
        }
    }
}
