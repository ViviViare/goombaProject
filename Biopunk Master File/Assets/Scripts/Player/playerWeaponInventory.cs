using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Keeps track of the weapons the player can acquire, and what weapon they currently have equipped.

// Edits since script completion:
// 05/03/24: Cut down script bloat by a lot, also making the script more modular.
*/
public class playerWeaponInventory : MonoBehaviour
{
    [SerializeField] public List<GameObject> _playerWeapons;

    //[SerializeField] public GameObject _currentLeftWeapon;
    //[SerializeField] public int _currentLeftIndex;

    [SerializeField] public GameObject _currentRightWeapon;
    [SerializeField] public int _currentRightIndex;
}
