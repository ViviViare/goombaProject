using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuanonAI : enemyBaseAI, IDamageable
{
    [SerializeField] public float _enemyFleeDistance = 12f;
    [SerializeField] public Coroutine _rangedCoroutine;

    [SerializeField] public GameObject _enemyGunBarrel;
    [SerializeField] public GameObject _originalBarrel;
    [SerializeField] public GameObject _enemyBullet;

    [SerializeField] public int _shotsFired;
    [SerializeField] public int _shotsBeforeReload;
    [SerializeField] public int _reloadTime;

    [SerializeField] public float _burstReloadTime;
    [SerializeField] public float _burstCooldown;

    [SerializeField] public bool _canBurst;

    [SerializeField] private List<GameObject> _suanonBarrels = new List<GameObject>();
    [SerializeField] private int _suanonBarrelsCount = 4;
    [SerializeField] public GameObject _burstBullet;

    [SerializeField] public int _initialReloadTime;
    [SerializeField] public float _initialFleeDistance;

    [SerializeField] public bool _canMove;

    [SerializeField] public AudioClip _clipToPlay;


    // Caches variables
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
        // Suanon constantly checks and compares the player's distance to his two range variables, _enemyRange and _fleeDistance.
        // If the player's distance is less than his _enemyRange variable and he can attack, he will start his ranged attack coroutine and fire at the player continously
        // (or instead fire his burst attack, if it is off cooldown.)

        // If the player is outside of his range, he will move towards them until he is able to fire at them.

        // However, if the player is *too* close to Suanon (dictated by the distance between the two compared to his _enemyFleeDistance variable, Suanon will stop firing and move backwards
        // away from the player until they are outside of his flee distance.

        if (!_aiActivated) return;

        transform.LookAt(_target.transform);

        _enemyAgent.destination = _target.GetComponent<Transform>().position;

        float distanceToPlayer = Vector3.Distance(_target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);

        if(distanceToPlayer <= _enemyRange && _canBurst)
        {
            StartCoroutine(BurstAttack());
        }
        else if (distanceToPlayer <= _enemyRange && _canAttack)
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

        if (distanceToPlayer <= _enemyFleeDistance && _canMove)
        {
            transform.Translate(UnityEngine.Vector3.back * _enemySpeed * 1.5f * Time.deltaTime);
        }

        
    }

    // Below coroutine handles Suanon's basic ranged attack.
    // If the player's health is greater than zero, it will make Suanon fire a bullet towards the player's current location (with a small delay between shots.)
    // If Suanon's "magazine" is empty, dictated by whether or not his _shotsFired is equal to his _shotsBeforeReload, a longer "reload" cooldown will play instead of the usual,
    // shorter "between-shots" cooldown.
    IEnumerator RangedAttack()
    {
        while (_target.GetComponent<playerHealth>()._playerHealth > 0)
        {
            ObjectPooler.Spawn(_enemyBullet, _enemyGunBarrel.transform.position, Quaternion.identity);
            _shotsFired++;
            if(_shotsFired < _shotsBeforeReload)
            {
                _canMove = false;
                yield return new WaitForSeconds(_attackCooldown);
            }
            else if(_shotsFired >= _shotsBeforeReload)
            {
                _canMove = true;
                yield return new WaitForSeconds(_reloadTime);
                _shotsFired = 0;
            }
        }
    }

    // Suanon's special "spread burst" attack is contained within this coroutine.
    // Essentially, it fires a non-tracking bullet from Suanon's gun barrels, which are rotated in such a fashion to create a "spread" pattern when the bullets are fired.
    // Also starts a cooldown to prevent Suanon from continously firing this special attack.

    IEnumerator BurstAttack()
    {
        _canBurst = false;
        _canAttack = false;
        _originalBarrel = _enemyGunBarrel;
        for(int i = 0; i < _suanonBarrelsCount; i++)
        {
            _enemyGunBarrel = _suanonBarrels[i];
            ObjectPooler.Spawn(_burstBullet, _enemyGunBarrel.transform.position, _enemyGunBarrel.transform.rotation);
            yield return new WaitForSeconds(_burstReloadTime);
        }
        _enemyGunBarrel = _originalBarrel;
        StartCoroutine(BurstCooldown());
    }


    // Serves as a cooldown timer for Suanon's special "spread burst" attack.
    IEnumerator BurstCooldown()
    {
        _canAttack = true;
        yield return new WaitForSeconds(_burstCooldown);
        _canBurst = true;

    }

    // Handles Suanon taking damage. This is called whenever something needs to deal damage to Suanon.
    // It reduces his health by whatever damageAmount is, and if that amount of damage would be enough to bring his health to or below zero, it will despawn him and play a death sound.
    // This also tries to grab _spawnData (if Suanon is spawned with one, which should always be the case if he is generated naturally) and tells it that an enemy has died.
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
}
