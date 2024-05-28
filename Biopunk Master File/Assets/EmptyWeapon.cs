using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyWeapon : MonoBehaviour
{
    private void OnEnable()
    {
        playerWeaponHandler._triggerAction += NullCatcher;
    }
    private void OnDisable()
    {
        playerWeaponHandler._triggerAction -= NullCatcher;
    }

    public void NullCatcher(LeftOrRight direction)
    {
        return;
    }
}
