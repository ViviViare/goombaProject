using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerWeaponInventory : MonoBehaviour
{
    [SerializeField] public List<GameObject> _playerWeapons;

    [SerializeField] public GameObject _currentLeftWeapon;
    [SerializeField] public int _currentLeftIndex;

    [SerializeField] public GameObject _currentRightWeapon;
    [SerializeField] public int _currentRightIndex;
}
