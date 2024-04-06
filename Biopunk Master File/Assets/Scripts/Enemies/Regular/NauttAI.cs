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

    [SerializeField] public bool _died = false;


    void OnDisable()
    {
        _died = false;
    }


    public void Start()
    {
        //_enemyAgent;
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
            if (_died) return;
            _died = true;
            _spawnData?.EnemyDied();

            GameObject newEdvard = ObjectPooler.Spawn(_edvardPrefab, _edvardSpawnPoint.transform.position, Quaternion.identity);
            newEdvard.GetComponent<EnemySpawnData>().EnableEnemy();
            
            GameObject newSuanon = ObjectPooler.Spawn(_suanonPrefab, _suanonSpawnPoint.transform.position, Quaternion.identity);
            newSuanon.GetComponent<EnemySpawnData>().EnableEnemy();
            
            GameObject newSuaorf = ObjectPooler.Spawn(_sauorfPrefab, _sauorfSpawnPoint.transform.position, Quaternion.identity);
            newSuaorf.GetComponent<EnemySpawnData>().EnableEnemy();
            ObjectPooler.Despawn(this.gameObject);
        }
    }
}
