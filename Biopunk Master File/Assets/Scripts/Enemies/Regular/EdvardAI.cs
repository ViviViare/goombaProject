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

    [SerializeField] public AudioClip _clipToPlay;

    
    // Sets up the enemy's speed and cooldowns. The coroutine to spawn the damage trail begins here and runs indefinitely in the background.
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



    // Loops indefinitely as soon as Edvard is enabled. If Edvard's AI isn't enabled, it simply loops, but when Edvard's AI is enabled it will spawn a damaging trail of blood.
    // This is done to ensure that Edvard does not spawn the trail when the player isn't in the same room as him, seeing as that would simply eat resources for no reason.

    // It will spawn a damage trail object on an interval based on whatever _damageTrailSpawnCooldown is set to.
    IEnumerator SpawnDamageTrail()
    {
        while (true)
        {
            if (!_aiActivated) 
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


    // Update() handles Edvard's AI behaviours.
    public void Update()
    {
        // Do not run AI code if the enemy's AI has not been activated
        if (!_aiActivated) return;

        _enemyAgent.destination = _target.GetComponent<Transform>().position;
        

        // If the player is close enough to Edvard, and he is allowed to attack, he will continously execute attacks towards the player.
        // Otherwise, if Edvard is out of range and is currently attacking, he will be prevented from making more attacks until he is close enough to the player again.
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

    // Handles Edvard's attacks.
    IEnumerator EnemyAttack()
    {
        // If the player's health is greater than zero, Edvard will damage the player in intervals based on the _attackCooldown float.
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

    // Handles Edvard taking damage. This is called whenever something needs to deal damage to Edvard.
    // It reduces his health by whatever damageAmount is, and if that amount of damage would be enough to bring his health to or below zero, it will despawn him and play a death sound.
    // This also tries to grab _spawnData (if Edvard is spawned with one, which should always be the case if he is generated naturally) and tells it that an enemy has died.
    // This is used to keep track of if a room is considered "cleared" or not, which is relevant to a lot of different scripts (like checking to see if a room should unlock/lock itself.)
    public void Damage(int damageAmount)
    {
        _enemyHealth -= damageAmount;
        if (_enemyHealth <= 0 && _aiActivated)
        {
            Debug.Log(_spawnData);
            _spawnData?.EnemyDied();
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
            
        }
    }
}
