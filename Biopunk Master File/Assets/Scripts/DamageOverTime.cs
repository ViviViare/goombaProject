using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Handles damage over time mechanics.

// Edits since script completion:
// 05/03/24: Cut down script bloat by a lot, also making the script more modular.
*/
public class DamageOverTime : MonoBehaviour
{
    [SerializeField] public float _burnTickTime = 1f;
    [SerializeField] public float _poisonTickTime = 3f;

    [SerializeField] public bool _isBurning;
    [SerializeField] public bool _isPoisoned;

    // If executed, will continueously damage whatever object this script is attached to by whatever "damagePerTick" is, and for however long "duration" is.
    public IEnumerator BurnDamage(int duration, int damagePerTick)
    {
        _isBurning = true;
        for (int i = 0; i < duration; i++)
        {
            print(i);
            yield return new WaitForSeconds(1);
            this.gameObject.GetComponent<IDamageable>().Damage(damagePerTick);
        }
        _isBurning = false;
    }

    // Does the same as the above, except it ticks slower.
    public IEnumerator PoisonDamage(int duration, int damagePerTick)
    {
        _isPoisoned = true;
        for (int i = 0; i < duration; i++)
        {
            print(i);
            yield return new WaitForSeconds(3);
            this.gameObject.GetComponent<IDamageable>().Damage(damagePerTick);
        }
        _isPoisoned = false;
    }
}
