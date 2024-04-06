using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBaseAI : MonoBehaviour
{
    [SerializeField] public int _enemyHealth;
    [SerializeField] public int _enemyDamage;

    [SerializeField] public float _enemySpeed;
    [SerializeField] public float _enemySpawnWeight;
    [SerializeField] public float _enemyRange;
    [SerializeField] public float _attackCooldown;

    [SerializeField] public int _initialHealth;
    [SerializeField] public int _initialDamage;

    [SerializeField] public float _initialSpeed;
    [SerializeField] public float _initialSpawnWeight;
    [SerializeField] public float _initialRange;
    [SerializeField] public float _initialAttackCooldown;


    [SerializeField] public GameObject _target;

    protected EnemySpawnData _spawnData;

    [SerializeField] public bool _canAttack = true;
    [SerializeField] public bool _currentlyAttacking;

    [SerializeField] public NavMeshAgent _enemyAgent;

    public void SetupEnemy()
    {
        _spawnData = GetComponent<EnemySpawnData>();
        _target = GlobalVariables._player;
        _enemyAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _enemyAgent.stoppingDistance = _enemyRange;
        _canAttack = true;

        // Cache all of its stats
        _initialDamage = _enemyDamage;
        _initialAttackCooldown = _attackCooldown;
        _initialHealth = _enemyHealth;
        _initialRange = _enemyRange;
        _initialSpawnWeight = _enemySpawnWeight;
        _initialSpeed = _enemySpeed;
    }

    void OnDisable()
    {
        _enemyDamage = _initialDamage;
        _attackCooldown = _initialAttackCooldown;
        _enemyHealth = _initialHealth;
        _enemyRange = _initialRange;
        _enemySpawnWeight = _initialSpawnWeight;
        _enemySpeed = _initialSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _enemyRange);
    }
}
