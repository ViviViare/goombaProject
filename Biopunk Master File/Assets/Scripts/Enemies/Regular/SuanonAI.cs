using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuanonAI : enemyBaseAI, IDamageable
{
    [SerializeField] public float _enemyFleeDistance = 12f;
    [SerializeField] public Coroutine _rangedCoroutine;

    [SerializeField] public GameObject _enemyGunBarrel;
    [SerializeField] public GameObject _enemyBullet;

    [SerializeField] public int _shotsFired;
    [SerializeField] public int _shotsBeforeReload;
    [SerializeField] public int _reloadTime;

    [SerializeField] public int _initialReloadTime;
    [SerializeField] public float _initialFleeDistance;

    void OnEnable()
    {
        _initialReloadTime = _reloadTime;
        _initialFleeDistance = _enemyFleeDistance;
    }

    void OnDisable()
    {
        _reloadTime = _initialReloadTime;
        _enemyFleeDistance = _initialFleeDistance;
    }

    void Update()
    {
        if (_spawnData._activatedAi)
        {
            transform.LookAt(_target.transform);
            _enemyAgent.destination = _target.GetComponent<Transform>().position;
            float distanceToPlayer = Vector3.Distance(_target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);
            if (distanceToPlayer <= _enemyRange && _canAttack)
            {
                _canAttack = false;
                _currentlyAttacking = true;
                _rangedCoroutine = StartCoroutine(RangedAttack());
            }
            else if (distanceToPlayer > _enemyRange && _currentlyAttacking == true)
            {
                _currentlyAttacking = false;
                _canAttack = true;
                StopCoroutine(_rangedCoroutine);
            }
            if (distanceToPlayer <= _enemyFleeDistance)
            {
                transform.Translate(UnityEngine.Vector3.back * _enemySpeed * 1.5f * Time.deltaTime);
            }

        }
    }

    IEnumerator RangedAttack()
    {
        while (_target.GetComponent<playerHealth>()._playerHealth > 0)
        {
            ObjectPooler.Spawn(_enemyBullet, _enemyGunBarrel.transform.position, Quaternion.identity);
            _shotsFired++;
            if(_shotsFired < _shotsBeforeReload)
            {
                yield return new WaitForSeconds(_attackCooldown);
            }
            else if(_shotsFired >= _shotsBeforeReload)
            {
                yield return new WaitForSeconds(_reloadTime);
                _shotsFired = 0;
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
