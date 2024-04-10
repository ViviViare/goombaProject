using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NauttAI : enemyBaseAI, IDamageable
{
    [SerializeField] private Coroutine attackCoroutine;

    [SerializeField] public Transform _suanonSpawnPoint;
    [SerializeField] public Transform _sauorfSpawnPoint;
    [SerializeField] public Transform _edvardSpawnPoint;

    [SerializeField] public GameObject _suanonPrefab;
    [SerializeField] public GameObject _sauorfPrefab;
    [SerializeField] public GameObject _edvardPrefab;

    [SerializeField] public bool _died = false;


    void OnDisable()
    {
        _died = false;
    }

    public void Update()
    {
        if (_aiActivated) 
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
        if (_enemyHealth <= 0 && _aiActivated)
        {
            if (_died) return;
            _died = true;
            _spawnData?.EnemyDied();
            
            GameObject newEdvard = ObjectPooler.Spawn(_edvardPrefab, _edvardSpawnPoint.position, Quaternion.identity);
            EnemySpawnData edvardData = newEdvard.GetComponent<EnemySpawnData>();
            edvardData.EnableEnemy();
            edvardData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newEdvard);
            
            GameObject newSuanon = ObjectPooler.Spawn(_suanonPrefab, _suanonSpawnPoint.position, Quaternion.identity);
            EnemySpawnData suanonData = newSuanon.GetComponent<EnemySpawnData>();
            suanonData.EnableEnemy();
            suanonData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newSuanon);
            
            GameObject newSuaorf = ObjectPooler.Spawn(_sauorfPrefab, _sauorfSpawnPoint.position, Quaternion.identity);
            EnemySpawnData suaorfData = newSuaorf.GetComponent<EnemySpawnData>();
            suaorfData.EnableEnemy();
            suaorfData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newSuaorf);

            ObjectPooler.Despawn(this.gameObject);
        }
    }
}
