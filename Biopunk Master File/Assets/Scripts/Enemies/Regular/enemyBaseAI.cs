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
    
    private void Awake()
    {
        _spawnData = GetComponent<EnemySpawnData>();
        _enemyAgent = this.gameObject.GetComponent<NavMeshAgent>();
        _target = GlobalVariables._player;
        _enemyAgent.stoppingDistance = _enemyRange;
        _mainTexture = _enemyModel.GetComponentInChildren<Renderer>(true).material;
    }

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

    private void Shiny()
    {
        // Decide if this enemy should be a shiny
        int shinyChance = Random.Range(0, 10);
        Material textureToUse = _mainTexture;
        if (shinyChance <= 0 && _shinyTexture != null) textureToUse = _shinyTexture;

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
