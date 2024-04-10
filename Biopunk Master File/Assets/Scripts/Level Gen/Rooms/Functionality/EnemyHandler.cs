using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    #region Variables

    [Header("Enemies Generated")]
    [ShowOnly] public bool _hasGenerated;
    [ShowOnly] public bool _enemiesEnabled;
    [Space]
    [ShowOnly] public int _generatedEnemies;
    [ShowOnly] public int _livingEnemies;
    [ShowOnly] public bool _allEnemiesDead = false;
    [Space]
    public List<GameObject> _enemies = new List<GameObject>();

    [Header("Generation Parameters")]
    [SerializeField] private List<EnemySpawnData> _enemyPool = new List<EnemySpawnData>();
    [SerializeField] private Transform _spawnPointsParent;
    private List<Transform> _totalSpawnPoints = new List<Transform>();
    private List<Transform> _unusedSpawnPoints;
    
    [Header("Constraints")]
    [SerializeField] private int _maxWeight = 15;
    private int _currentWeight;
    [Tooltip("The lower this value, the weaker the preference will be towards enemies spawned.")]
    [SerializeField] [Range(0.02f, 1)] private float _weightPreference;
 
    [Header("Room References")]
    private RoomStatus _status;
    
    [Header("Debug")]
    [SerializeField] private int _maxSpawnAttempts = 40;

    #endregion
    
    private void Start()
    {
        _status = GetComponent<RoomStatus>();
        
    }
    
    public void GenerateEnemies()
    {
        SetSpawnPoints();

        _unusedSpawnPoints = _totalSpawnPoints;

        _hasGenerated = true;
        float randRemoval = Random.Range(0, 1);
        
        int maxEnemies = _totalSpawnPoints.Count - Mathf.CeilToInt(_totalSpawnPoints.Count * randRemoval);
        
        // A recursion handler variable is good when making a while loop.
        int attempts = 0;

        if (_enemyPool.Count == 0)
        {
            Debug.LogError($"There is no enemies to generate for {gameObject.name}");
            return;
        }

        if (_totalSpawnPoints.Count == 0)
        {
            Debug.LogError($"There are no valid spawn points for {gameObject.name}");
            return;
        }

        // Only attempt to spawn enough enemies to fill the total spawn points
        for (int i = 0; i < _totalSpawnPoints.Count; i++)
        {
            attempts++;
            GenerateAnEnemy();
        }

        _livingEnemies = _generatedEnemies;
        
    }   

    private void SetSpawnPoints()
    {
        foreach (Transform child in _spawnPointsParent)
        {
            _totalSpawnPoints.Add(child);
        }
    }
    
    private void GenerateAnEnemy()
    {
        List<EnemySpawnData> filteredEnemies = _enemyPool.FindAll(valid => !IsOverdraft(valid._enemyWeight, _currentWeight, _maxWeight));
        // If the filtered enemies list comes back as 0 then bomb out of this code
        if (filteredEnemies.Count == 0) return;

        // Get a new list of enemies adjusting for weight preference
        List<EnemySpawnData> newFilteredList = new List<EnemySpawnData>();

        // Make sure that the new filtered list has at least 20% of the amount of possible enemies that the original filtered list has
        
        newFilteredList = GetAdjustedEnemyList(filteredEnemies);
        
        if (newFilteredList.Count == 0) return;
        // Get a random enemy out of the enemy pool
        int chosenEnemy = Random.Range(0, newFilteredList.Count);
        EnemySpawnData chosenEnemyData = newFilteredList[chosenEnemy];
        GameObject chosenEnemyGo = chosenEnemyData.gameObject;

        // Get a random spawn point out of valid spawn points
        int chosenSpawn = Random.Range(0, _unusedSpawnPoints.Count);
        Transform newSpawn = _unusedSpawnPoints[chosenSpawn];
        // Remove this spawn point from the unused spawn points list
        _unusedSpawnPoints.Remove(newSpawn);

        // Update the enemy handlers current weight 
        _currentWeight += chosenEnemyData._enemyWeight;

        // Spawn the enemy
        GameObject newEnemySpawned = ObjectPooler.Spawn(chosenEnemyGo, newSpawn.position + (Vector3.up * 4f), Quaternion.identity);
        EnemySpawnData newEnemyData = newEnemySpawned.GetComponent<EnemySpawnData>();

        // Configure the new enemy to be able to communicate correctly
        newEnemyData._connectedRoom = _status;
        newEnemyData._enemyHandler = this;

        // Add this enemy to the enemy list
        _enemies.Add(newEnemySpawned);

        _generatedEnemies += newEnemyData._enemyWorth;
    }

    public void DegenerateEnemies()
    {
        _hasGenerated = false;
        _generatedEnemies = 0;
        _livingEnemies = 0;
        _currentWeight = 0;
        _totalSpawnPoints.Clear();
        _unusedSpawnPoints.Clear();

        // Disable all enemy gameobjects
        foreach (GameObject enemy in _enemies)
        {
            ObjectPooler.Despawn(enemy);
        }

        _enemies.Clear();
        Debug.Log($"Cleared enemies at: {gameObject.name}");
        
    }

    private List<EnemySpawnData> GetAdjustedEnemyList(List<EnemySpawnData> originalFilter)
    {
        List<EnemySpawnData> newFilteredList = new List<EnemySpawnData>();

        foreach (EnemySpawnData spawnData in originalFilter)
        {
            // Get the absolute value of the weight difference between the difficulty preference and the possible enemy
            float weightDifference = Mathf.Abs(spawnData._enemyWeight - _weightPreference);
            float inclusionChance = 1f - Mathf.Clamp01(weightDifference);

            // Random chance between 0 and 1
            float randChance = Random.value;

            if (inclusionChance <= randChance) newFilteredList.Add(spawnData);
        }

        return newFilteredList;
    }

    private bool IsOverdraft(int addition, int currentVal, int maxVal)
    {
        int newVal = currentVal + addition;

        if (newVal > maxVal) return true;
        return false;
    }

    public void EnableEnemies()
    {
        if (_allEnemiesDead) return;

        GlobalVariables._inCombat = true;
        GlobalVariables._musicManager.GetComponent<MusicManager>().FadeToSecondary();

        _enemiesEnabled = true;
        foreach (GameObject enemy in _enemies)
        {
            EnemySpawnData thisEnemy = enemy.GetComponent<EnemySpawnData>();
            thisEnemy.EnableEnemy();
        }
    }

    public void EnemyDied(GameObject deadEnemy)
    {
        _enemies.Remove(deadEnemy);
        _livingEnemies--;

        if (_livingEnemies <= 0 && !_allEnemiesDead)
        {
            _allEnemiesDead = true;
            Debug.Log($"All enemies in {gameObject.name} have died");
            _status.NoEnemiesRemaining();
        }
    }

}
