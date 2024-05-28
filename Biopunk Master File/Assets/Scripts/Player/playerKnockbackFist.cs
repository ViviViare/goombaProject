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

    [SerializeField] public float _specialRange = 8f;

    // This is called by an animation event in the Knockback Fist's attacking animation; it simply fires a sphere raycast and damages an object if it has an IDamageable component
    // but with the added effect of applying a knockback force based on where the enemy was hit relative to the player.

    public void KnockbackRaycast()
    {
        Collider[] HitColliders = Physics.OverlapSphere(_meleeHitSphereCenter.transform.position, _meleeRange);
        foreach (var HitCollider in HitColliders)
        {
            if (HitCollider.gameObject.tag == "Player") continue;
            if (HitCollider.gameObject.GetComponent<IDamageable>() != null)
            {
                int calculatedDamage = (int)(_meleeDamage * _player.GetComponent<playerStats>()._playerDamageMultiplier);
                HitCollider.gameObject.GetComponent<IDamageable>().Damage(calculatedDamage);
                Rigidbody objectRigidbody = HitCollider.GetComponent<Rigidbody>();
                objectRigidbody.AddRelativeForce(_player.transform.forward * _knockbackForce);
            }
        }
        StartCoroutine(MeleeCooldown());
    }

    // If the knockback fist's special is off cooldown, it will execute the special move.
    public override void FireSpecial()
    {
        if (_canUseSpecial == false) return;
        FistAOE();
    }

    // Handles the fist's special attack, which is a simple area of effect attack centered on the player that damages any enemy within a certain range.
    public void FistAOE()
    {
        Collider[] HitColliders = Physics.OverlapSphere(GlobalVariables._player.gameObject.transform.position, _specialRange);
        foreach (var HitCollider in HitColliders)
        {
            if (HitCollider.gameObject.tag == "Player") continue;
            if (HitCollider.gameObject.GetComponent<IDamageable>() != null)
            {
                HitCollider.gameObject.GetComponent<IDamageable>().Damage(_specialDamage);
            }
        }
        StartCoroutine(SpecialCooldown());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_meleeHitSphereCenter.transform.position, _meleeRange);
    }
}
