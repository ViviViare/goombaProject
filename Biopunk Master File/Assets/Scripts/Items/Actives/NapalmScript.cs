using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NapalmScript : MonoBehaviour
{
    [SerializeField] public int _napalmDamage;
    [SerializeField] public int _napalmDuration;
    [SerializeField] public int _napalmTickDamage;

    [SerializeField] public float _napalmRange;

    private void OnEnable()
    {
        playerActiveItem._activeAction += UseActive;
    }
    private void OnDisable()
    {
        playerActiveItem._activeAction -= UseActive;
    }


    // When the player uses their active item, the Napalm will create a large sphere collider around the player's current position.
    // It will then grab every single object within this overlap sphere that has a Damageable (save for the player) and damage it for _napalmDamage.
    // Will also apply a burning effect to whatever is damaged by the blast.

    private void UseActive()
    {
        Collider[] HitColliders = Physics.OverlapSphere(GlobalVariables._player.gameObject.transform.position, _napalmRange);
        foreach (var HitCollider in HitColliders)
        {
            if (HitCollider.gameObject.tag == "Player") continue;
            if (HitCollider.gameObject.GetComponent<IDamageable>() != null)
            {
                HitCollider.gameObject.GetComponent<IDamageable>().Damage(_napalmDamage);
                if (HitCollider.gameObject.GetComponent<DamageOverTime>()._isBurning) return;
                StartCoroutine(HitCollider.gameObject.GetComponent<DamageOverTime>().BurnDamage(_napalmDuration, _napalmTickDamage));
            }
        }

    }
}
