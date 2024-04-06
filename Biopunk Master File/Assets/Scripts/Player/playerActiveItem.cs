using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerActiveItem : MonoBehaviour
{
    [SerializeField] public bool _hasActiveItem = false;

    [SerializeField] public int _activeItemMaxCharge;
    [SerializeField] public int _activeItemCharge;

    public static event Action _activeAction;

    private void Update()
    {
        if (GlobalVariables._roomsCleared == GlobalVariables._roomsCleared++)
        {
            _activeItemCharge++;
        }
    }

    public void OnActiveUsage()
    {
        if (_activeItemCharge < _activeItemMaxCharge || _hasActiveItem == false) return;
        _activeAction?.Invoke();
        _activeItemCharge = 0;
    }
}
