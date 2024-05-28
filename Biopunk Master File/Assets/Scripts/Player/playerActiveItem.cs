using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Handles active item usage

//
*/
public class playerActiveItem : MonoBehaviour
{
    [SerializeField] public bool _hasActiveItem = false;

    [SerializeField] public int _activeItemMaxCharge;
    [SerializeField] public int _activeItemCharge;

    public static event Action _activeAction;

    // If the player presses the active item button, the below runs.
    // If the player's active item is still on cooldown (if its charge is less than its max charge) or if the player does not have an active item, this method stops running.
    // Otherwise, it'll invoke an action that broadcasts to the currently equipped active item and make it perform whatever its function is.
    public void OnActiveUsage()
    {
        if (_activeItemCharge < _activeItemMaxCharge || _hasActiveItem == false) return;
        _activeAction?.Invoke();
        _activeItemCharge = 0;
        ActiveFillUpdate._instance.UpdateChargeAmount(_activeItemCharge);
    }
}
