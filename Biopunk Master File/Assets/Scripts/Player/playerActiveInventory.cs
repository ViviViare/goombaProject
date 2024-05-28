using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Stores the player's active items (as they are always attached to the player, and are simply enabled/disabled as neccessary.
// Also keeps track of the player's currently active active item and its index in this "active item" list.

//
*/
public class playerActiveInventory : MonoBehaviour
{
    [SerializeField] public List<GameObject> _playerActives;

    [SerializeField] public GameObject _currentActive;
    [SerializeField] public int _currentActiveIndex;
}
