using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActiveInventory : MonoBehaviour
{
    [SerializeField] public List<GameObject> _playerActives;

    [SerializeField] public GameObject _currentActive;
    [SerializeField] public int _currentActiveIndex;
}
