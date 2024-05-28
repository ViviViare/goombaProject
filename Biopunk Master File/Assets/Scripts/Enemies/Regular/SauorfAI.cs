using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SauorfAI : enemyBaseAI, IDamageable
{
    [SerializeField] private Coroutine attackCoroutine;

    [SerializeField] public bool _canCharge = true;
    [SerializeField] public bool _isCharging;
    [SerializeField] public float _chargeSpeed;
    [SerializeField] public float _chargeDuration;
    [SerializeField] public float _chargeCooldown;
    [SerializeField] public int _chargeDamage;

    [SerializeField] private float _raycastLength = 5f;

    [SerializeField] public float _originalTurnSpeed = 500f;

    [SerializeField] public AudioClip _clipToPlay;

    public void Update()
    {
        // If the player is close enough to Sauorf, and he is allowed to attack, he will continously execute attacks towards the player.
        // Otherwise, if Sauorf is out of range and is currently attacking, he will be prevented from making more attacks until he is close enough to the player again.
        // Sauorf will additionally not make regular attacks if he is currently charging, as his charge attack handles damage in a different way.

        if (!_aiActivated) return;

        _enemyAgent.destination = _target.GetComponent<Transform>().position;
        
        float distanceToPlayer = Vector3.Distance(_target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);
        if (distanceToPlayer <= _enemyRange && _canAttack && _isCharging == false)
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

        // If Sauorf's _canCharge variable is true, and he is out of range to attack normally, he will charge at the player, dealing damage when touching them.

        if (distanceToPlayer > _enemyRange && _canCharge)
        {
            StartCoroutine(ChargeAttack());
        }
    }

    // This void only does anything when Sauorf is doing his charge attack, damaging the player upon contact.
    void OnCollisionEnter(Collision collision)
    {
        if (_isCharging == false) return;
        if (collision.gameObject.GetComponent<IDamageable>() == null) return;
        if (collision.gameObject.tag == "Enemy") return;
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable.Damage(_chargeDamage);
    }

    IEnumerator EnemyAttack()
    {
        // If the player's health is greater than zero, Sauorf will damage the player in intervals based on the _attackCooldown float.
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

    // Handles Sauorf taking damage. This is called whenever something needs to deal damage to Sauorf.
    // It reduces his health by whatever damageAmount is, and if that amount of damage would be enough to bring his health to or below zero, it will despawn him and play a death sound.
    // This also tries to grab _spawnData (if Sauorf is spawned with one, which should always be the case if he is generated naturally) and tells it that an enemy has died.
    // This is used to keep track of if a room is considered "cleared" or not, which is relevant to a lot of different scripts (like checking to see if a room should unlock/lock itself.)

    public void Damage(int damageAmount)
    {
        _enemyHealth -= damageAmount;
        if (_enemyHealth <= 0 && _aiActivated)
        {
            _spawnData?.EnemyDied();
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
        }
    }


    // Below coroutine handles Sauorf's charge attack. Essentially prevents Sauorf from turning and increases his speed until his charge is over.
    // Once his charge is complete, it returns his turning speed and regular movespeed to their normal values.
    IEnumerator ChargeAttack()
    {
        NavMeshAgent enemAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _isCharging = true;
        _canCharge = false;
        enemAgent.speed = _chargeSpeed;
        enemAgent.angularSpeed = 0f;
        enemAgent.acceleration = 10f;
        yield return new WaitForSeconds(_chargeDuration);
        _isCharging = false;
        enemAgent.angularSpeed = 500;
        enemAgent.acceleration = 1000f;
        enemAgent.speed = _enemySpeed;
        yield return new WaitForSeconds(_chargeCooldown);
        _canCharge = true;
    }
}
