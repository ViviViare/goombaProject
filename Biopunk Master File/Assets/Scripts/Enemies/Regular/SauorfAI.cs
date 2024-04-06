using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauorfAI : enemyBaseAI, IDamageable
{
    [SerializeField] private Coroutine attackCoroutine;

    public void Update()
    {
        if (_spawnData._activatedAi)
        {
            _enemyAgent.destination = _target.GetComponent<Transform>().position;
            float distanceToPlayer = Vector3.Distance(_target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);
            if (distanceToPlayer <= _enemyRange && _canAttack)
            {
                attackCoroutine = StartCoroutine(EnemyAttack());
                _canAttack = false;
                _currentlyAttacking = true;
            }
            else if (distanceToPlayer > _enemyRange && _currentlyAttacking == true)
            {
                _currentlyAttacking = false;
                _canAttack = true;
                StopCoroutine(attackCoroutine);
            }
        }
    }

    IEnumerator EnemyAttack()
    {
        while (_target.GetComponent<playerHealth>()._playerHealth > 0)
        {
            IDamageable damageable = _target.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(_enemyDamage);
                yield return new WaitForSeconds(_attackCooldown);
            }
        }
    }

    public void Damage(int damageAmount)
    {
        _enemyHealth -= damageAmount;
        if (_enemyHealth <= 0 && _spawnData._activatedAi)
        {
            _spawnData?.EnemyDied();
            ObjectPooler.Despawn(this.gameObject);
            
        }
    }
}
