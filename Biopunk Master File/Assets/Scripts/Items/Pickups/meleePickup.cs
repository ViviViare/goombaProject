using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleePickup : MonoBehaviour
{
    [Header("Melee Stats")]
    [SerializeField] public int _meleeDamage;
    [SerializeField] public int _meleeRange;

    [SerializeField] public int _meleeIndex;
    [SerializeField] public bool _clawOrFist;

    [SerializeField] public AudioClip _clipToPlay;


    // Deprecated code; used to swap the player's left weapon. Became obsolete when switching to our new single-weapon system.

    //public void SwapLeft(GameObject player)
    //{
    //    if (_clawOrFist == false)
    //    {
    //        _meleeIndex = 4;
    //    }
    //    else
    //    {
    //        _meleeIndex = 6;
    //    }

    //    playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
    //    if (wepInv._currentLeftIndex == _meleeIndex)
    //    {
    //        playerBaseMelee leftStats = wepInv._currentLeftWeapon.GetComponent<playerBaseMelee>();

    //        leftStats._meleeDamage = _meleeDamage;
    //        leftStats._meleeRange = _meleeRange;
    //        ObjectPooler.Despawn(this.gameObject);
    //    }
    //    else
    //    {
    //        wepInv._currentLeftWeapon.SetActive(false);
    //        wepInv._currentLeftWeapon = wepInv._playerWeapons[_meleeIndex];
    //        wepInv._currentLeftWeapon.SetActive(true);
    //        wepInv._currentLeftIndex = _meleeIndex;

    //        playerBaseMelee leftStats = wepInv._currentLeftWeapon.GetComponent<playerBaseMelee>();

    //        leftStats._meleeDamage = _meleeDamage;
    //        leftStats._meleeRange = _meleeRange;
    //        ObjectPooler.Despawn(this.gameObject);
    //    }
    //}


    // When interacting with a melee pickup, it will replace the player's currently equipped weapon with whatever weapon pickup they just interacted with.

    public void SwapRight(GameObject player)
    {
        // Plays a nice pickup sound effect
        AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);

        // The below code essentially uses indexes to figure out what weapon the player currently has and what to swap.
        if (_clawOrFist == false)
        {
            _meleeIndex = 2;
        }
        else
        {
            _meleeIndex = 3;
        }

        // If the player's currently equipped weapon is different to whatever they're picking up, it will disable the current weapon and enable the new weapon on the player.
        // Otherwise, if the two weapons are the same (so for example, if a player has a Quadra and tries to pick up a Quadra), it will simply update the Quadra's stats to match the 
        // stats cached in the Quadra Pickup prefab.

        playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
        if (wepInv._currentRightIndex == _meleeIndex)
        {
            playerBaseMelee rightStats = wepInv._currentRightWeapon.GetComponent<playerBaseMelee>();

            rightStats._meleeDamage = _meleeDamage;
            rightStats._meleeRange = _meleeRange;
            GlobalVariables._startingItemGrabbed = true;
            ObjectPooler.Despawn(this.gameObject);
        }
        else
        {
            wepInv._currentRightWeapon.SetActive(false);
            wepInv._currentRightWeapon = wepInv._playerWeapons[_meleeIndex];
            wepInv._currentRightWeapon.SetActive(true);
            wepInv._currentRightIndex = _meleeIndex;

            playerBaseMelee rightStats = wepInv._currentRightWeapon.GetComponent<playerBaseMelee>();

            rightStats._meleeDamage = _meleeDamage;
            rightStats._meleeRange = _meleeRange;
            GlobalVariables._startingItemGrabbed = true;
            ObjectPooler.Despawn(this.gameObject);
        }
    }
}
