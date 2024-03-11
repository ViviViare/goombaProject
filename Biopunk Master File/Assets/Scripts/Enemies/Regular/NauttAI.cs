using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NauttAI : enemyBaseAI, IDamageable
{
    [SerializeField] private Coroutine attackCoroutine;

    [SerializeField] public GameObject _suanonSpawnPoint;
    [SerializeField] public GameObject _sauorfSpawnPoint;
    [SerializeField] public GameObject _edvardSpawnPoint;


    [SerializeField] public GameObject _suanonPrefab;
    [SerializeField] public GameObject _sauorfPrefab;
    [SerializeField] public GameObject _edvardPrefab;


    public void Update()
    {
        if (_isActive)
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
        if (_enemyHealth <= 0)
        {
            _isActive = false;
            ObjectPooler.Spawn(_edvardPrefab, _edvardSpawnPoint.transform.position, Quaternion.identity);
            ObjectPooler.Spawn(_suanonPrefab, _suanonSpawnPoint.transform.position, Quaternion.identity);
            ObjectPooler.Spawn(_sauorfPrefab, _sauorfSpawnPoint.transform.position, Quaternion.identity);
            ObjectPooler.Despawn(this.gameObject);
        }
    }
}
