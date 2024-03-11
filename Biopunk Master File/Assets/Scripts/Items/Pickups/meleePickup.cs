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

    public void SwapLeft(GameObject player)
    {
        if (_clawOrFist == false)
        {
            _meleeIndex = 4;
        }
        else
        {
            _meleeIndex = 6;
        }

        playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
        if (wepInv._currentLeftIndex == _meleeIndex)
        {
            playerBaseMelee leftStats = wepInv._currentLeftWeapon.GetComponent<playerBaseMelee>();

            leftStats._meleeDamage = _meleeDamage;
            leftStats._meleeRange = _meleeRange;
        }
        else
        {
            wepInv._currentLeftWeapon.SetActive(false);
            wepInv._currentLeftWeapon = wepInv._playerWeapons[_meleeIndex];
            wepInv._currentLeftWeapon.SetActive(true);
            wepInv._currentLeftIndex = _meleeIndex;

            playerBaseMelee leftStats = wepInv._currentLeftWeapon.GetComponent<playerBaseMelee>();

            leftStats._meleeDamage = _meleeDamage;
            leftStats._meleeRange = _meleeRange;
        }
    }

    public void SwapRight(GameObject player)
    {
        if (_clawOrFist == false)
        {
            _meleeIndex = 5;
        }
        else
        {
            _meleeIndex = 7;
        }

        playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
        if (wepInv._currentLeftIndex == _meleeIndex)
        {
            playerBaseMelee rightStats = wepInv._currentRightWeapon.GetComponent<playerBaseMelee>();

            rightStats._meleeDamage = _meleeDamage;
            rightStats._meleeRange = _meleeRange;
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
        }
    }
}
