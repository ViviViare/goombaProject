using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EdvardAI : enemyBaseAI, IDamageable
{
    [SerializeField] public GameObject _damageTrail;
    [SerializeField] public float _damageTrailSpawnCooldown = 1.5f;

    [SerializeField] public float _initialTrailSpawnCooldown;

    [SerializeField] private Coroutine attackCoroutine;

    void OnEnable()
    {
        StartCoroutine(SpawnDamageTrail());
        _enemyAgent.speed = _enemySpeed;
        _initialTrailSpawnCooldown = _damageTrailSpawnCooldown;
    }

    void OnDisable()
    {
        _damageTrailSpawnCooldown = _initialTrailSpawnCooldown;
    }


    IEnumerator SpawnDamageTrail()
    {
        while (true)
        {
            if (!_spawnData._activatedAi) 
            {
                yield return new WaitForSeconds(_damageTrailSpawnCooldown);
                continue;
            }
            else
            {
                ObjectPooler.Spawn(_damageTrail, transform.position, transform.rotation);
                yield return new WaitForSeconds(_damageTrailSpawnCooldown);
            }
            
        }
    }

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
