using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBaseAI : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] public int _enemyHealth;
    [SerializeField] public int _enemyDamage;

    [SerializeField] public float _enemySpeed;
    [SerializeField] public float _enemySpawnWeight;
    [SerializeField] public float _enemyRange;
    [SerializeField] public float _attackCooldown;

    [SerializeField] public int _champChance = 16;

    [Header("Cached Variables")]
    protected int _initialHealth;
    protected int _initialDamage;
    protected float _initialSpeed;
    protected float _initialSpawnWeight;
    protected float _initialRange;
    protected float _initialAttackCooldown;

    [Header("AI")]
    protected GameObject _target;
    protected EnemySpawnData _spawnData;

    protected bool _canAttack = true;
    protected bool _currentlyAttacking;

    protected NavMeshAgent _enemyAgent;

    [ShowOnly] public bool _aiActivated = false;
    [ShowOnly] public bool _aiSetup = false;

    [Header("Textures")]
    private Material _mainTexture;
    private bool _isShiny;    
    [SerializeField] private Material _shinyTexture;
    [SerializeField] private GameObject _enemyModel;
    

    // Sets up cached variables, and defines the enemy's target (which will always be the player).
    private void Awake()
    {
        _spawnData = GetComponent<EnemySpawnData>();
        _enemyAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _target = GlobalVariables._player;
        _mainTexture = _enemyModel.GetComponentInChildren<Renderer>(true).material;
    }

    // The below caches all the enemy's stats, to be able to reset them upon spawning them again. This is due to our object pooling script; if an enemy is reused, and we don't reset its stats,
    // there will be problems (such as the enemy's health immediately being zero upon spawning).
    public void SetupEnemy()
    {
        _aiSetup = true;

        _canAttack = true;
        _aiActivated = false;
        
        // Cache all of its stats
        _initialDamage = _enemyDamage;
        _initialAttackCooldown = _attackCooldown;
        _initialHealth = _enemyHealth;
        _initialRange = _enemyRange;
        _initialSpawnWeight = _enemySpawnWeight;
        _initialSpeed = _enemySpeed;
    }

    public void EnableEnemy()
    {
        // Reset all of the enemy's stats
        _enemyDamage = _initialDamage;
        _attackCooldown = _initialAttackCooldown;
        _enemyHealth = _initialHealth;
        _enemyRange = _initialRange;
        _enemySpawnWeight = _initialSpawnWeight;
        _enemySpeed = _initialSpeed;

        _aiActivated = true;
        Shiny();
    }


    // This code runs as soon as the enemy is set up. It essentially just rarely changes the enemy's material to be a different colour, as a fun little piece of flair, but we have also
    // utilised this to implement rarer, "champion" versions of enemies.
    // Essentially, when an enemy spawns as a shiny varient, it will multiply its stats by 1.25 on top of the texture change. This adds a little variety in every encounter and runthrough of the game.
    private void Shiny()
    {
        // Decide if this enemy should be a shiny
        int shinyChance = Random.Range(0, _champChance);
        Material textureToUse = _mainTexture;
        if (shinyChance <= 0 && _shinyTexture != null) textureToUse = _shinyTexture;
        if (shinyChance <= 0)
        {
            _enemyHealth = (int)(_enemyHealth * 1.25);
            _enemyDamage = (int)(_enemyDamage * 1.25);
            _enemySpeed = (int)(_enemySpeed * 1.25);
        }

        // If this enemy is not to be shiny and was not shiny already, stop running the code
        if (textureToUse == _mainTexture && !_isShiny) return;

        // Change the material on the enemy to be either shiny or revert from shiny back to normal
        foreach (Renderer child in _enemyModel.GetComponentsInChildren<Renderer>() )
        {
            child.material = _shinyTexture;
        }
    }
    

    public void DeactivateAI()
    {
        // Disable the enemies AI by default
        _aiActivated = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _enemyRange);
    }
}
