using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageBrick : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(_damage);
        }
    }
}
