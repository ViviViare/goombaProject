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

    [SerializeField] public AudioClip _clipToPlay;

    // Resets the variable that dictates if Nautt has died, as if the script detects that _died is true, it will split Nautt into the three other enemies.
    // If we didn't reset this variable, he'd split immediately upon re-spawning through object pooling.

    void OnDisable()
    {
        _died = false;
    }


    public void Update()
    {
        // If the player is close enough to Nautt, and he is allowed to attack, he will continously execute attacks towards the player.
        // Otherwise, if Nautt is out of range and is currently attacking, he will be prevented from making more attacks until he is close enough to the player again.

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
        // If the player's health is greater than zero, Nautt will damage the player in intervals based on the _attackCooldown float.
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


    // Handles Nautt taking damage. This is called whenever something needs to deal damage to Nautt.
    // It reduces his health by whatever damageAmount is, and if that amount of damage would be enough to bring his health to or below zero, it will despawn him and play a death sound.
    // This also tries to grab _spawnData (if Nautt is spawned with one, which should always be the case if he is generated naturally) and tells it that an enemy has died.
    // This is used to keep track of if a room is considered "cleared" or not, which is relevant to a lot of different scripts (like checking to see if a room should unlock/lock itself.)

    // Nautt's Damage method is unique, in that it will additionally spawn Edvard, Suanon and Suaorf upon Nautt's death (and initialise them properly)
    public void Damage(int damageAmount)
    {
        _enemyHealth -= damageAmount;
        if (_enemyHealth <= 0 && _aiActivated)
        {
            if (_died) return;
            _died = true;
            _spawnData?.EnemyDied();

            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);

            GameObject newEdvard = ObjectPooler.Spawn(_edvardPrefab, _edvardSpawnPoint.position, gameObject.transform.rotation);
            EnemySpawnData edvardData = newEdvard.GetComponent<EnemySpawnData>();
            edvardData.EnableEnemy();
            edvardData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newEdvard);
            
            GameObject newSuanon = ObjectPooler.Spawn(_suanonPrefab, _suanonSpawnPoint.position, gameObject.transform.rotation);
            EnemySpawnData suanonData = newSuanon.GetComponent<EnemySpawnData>();
            suanonData.EnableEnemy();
            suanonData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newSuanon);
            
            GameObject newSuaorf = ObjectPooler.Spawn(_sauorfPrefab, _sauorfSpawnPoint.position, gameObject.transform.rotation);
            EnemySpawnData suaorfData = newSuaorf.GetComponent<EnemySpawnData>();
            suaorfData.EnableEnemy();
            suaorfData._enemyHandler = _spawnData._enemyHandler;
            _spawnData._enemyHandler._enemies.Add(newSuaorf);

            ObjectPooler.Despawn(this.gameObject);
        }
    }
}
