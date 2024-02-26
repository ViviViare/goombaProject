/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 26/02/24

// A separate, smaller script to the main melee script that only handles the Knockback Fist's attacks.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKnockbackFist : playerBaseMelee
{
    [SerializeField] private float _knockbackForce = 10f;
    [SerializeField] private GameObject _player;


    // This is called by an animation event in the Knockback Fist's attacking animation; it simply fires a raycast and damages an object if it has an IDamageable component
    // but with the added effect of applying a knockback force based on where the enemy was hit relative to the player.

    public void KnockbackRaycast()
    {
        Vector3 rayOrigin = _playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit wepSwing;
        if (Physics.Raycast(rayOrigin, _playerCam.transform.forward, out wepSwing, _meleeRange))
        {
            IDamageable damageable = wepSwing.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(_meleeDamage);
                Rigidbody objectRigidbody = wepSwing.collider.GetComponent<Rigidbody>();
                objectRigidbody.AddRelativeForce(_player.transform.forward * _knockbackForce);
            }
        }
        StartCoroutine(MeleeCooldown());
    }
}
