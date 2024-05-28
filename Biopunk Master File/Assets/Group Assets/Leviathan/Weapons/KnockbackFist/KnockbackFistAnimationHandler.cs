using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackFistAnimationHandler : MonoBehaviour
{
    [SerializeField] private playerKnockbackFist _meleeScript;


    public void ConnectEvent()
    {
        _meleeScript.StopSwing();
    }

    public void ConnectAttack()
    {
        _meleeScript.KnockbackRaycast();
    }
}
