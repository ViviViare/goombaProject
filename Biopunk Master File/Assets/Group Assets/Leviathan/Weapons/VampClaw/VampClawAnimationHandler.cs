using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampClawAnimationHandler : MonoBehaviour
{
    [SerializeField] private playerBaseMelee _meleeScript;


    public void ConnectEvent()
    {
        _meleeScript.StopSwing();
    }

    public void ConnectAttack()
    {
        _meleeScript.MeleeRaycast();
    }
}
