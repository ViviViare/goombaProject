using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageable : MonoBehaviour, IDamageable
{
    public void Damage(int damageAmount)
    {
       Destroy(gameObject);
    }
}
