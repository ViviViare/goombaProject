using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdvardDamageTrail : MonoBehaviour
{
    [SerializeField] public int _trailDamage = 10;
    [SerializeField] public float _trailLifespan = 4f;

    // The moment this damage trail spawns, it immediately runs a coroutine to despawn itself based on its _trailLifespan variable.
    private void OnEnable()
    {
        StartCoroutine(DespawnDamageTrail());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    // If something with a damageable enters the collision zone for the damage trail, and it contains the "playerHealth" component, it will damage it for whatever _trailDamage is
    // and play a sound effect.

    // There is functionality to additionally poison the player upon entering the trail.
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.GetComponent<playerHealth>() != null)
        {
            damageable.Damage(_trailDamage);

            this.GetComponent<AudioSource>().Play();

            if (other.gameObject.GetComponent<DamageOverTime>()._isPoisoned) return;
            StartCoroutine(other.gameObject.GetComponent<DamageOverTime>().PoisonDamage(6, 5));
        }
    }

    // Self explanatory; despawns the trail after an amount of time has passed.
    IEnumerator DespawnDamageTrail()
    {
        yield return new WaitForSeconds(_trailLifespan);
        ObjectPooler.Despawn(this.gameObject);
    }
}
