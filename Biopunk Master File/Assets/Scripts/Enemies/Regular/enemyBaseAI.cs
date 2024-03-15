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

    [SerializeField] public GameObject _target;

    [SerializeField] public bool _isActive;
    [SerializeField] public bool _canAttack = true;
    [SerializeField] public bool _currentlyAttacking;

    [SerializeField] public NavMeshAgent _enemyAgent;

    void OnEnable()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
        _enemyAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _enemyAgent.stoppingDistance = _enemyRange;
        _canAttack = true;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _enemyRange);
    }
}
