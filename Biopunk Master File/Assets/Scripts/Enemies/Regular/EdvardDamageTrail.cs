using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdvardDamageTrail : MonoBehaviour
{
    [SerializeField] public int _trailDamage = 10;
    [SerializeField] public float _trailLifespan = 4f;
    private void Awake()
    {
        StartCoroutine(DespawnDamageTrail());
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.GetComponent<playerHealth>() != null)
        {
            damageable.Damage(_trailDamage);
        }
    }

    IEnumerator DespawnDamageTrail()
    {
        yield return new WaitForSeconds(_trailLifespan);
        ObjectPooler.Despawn(this.gameObject);
    }
}
