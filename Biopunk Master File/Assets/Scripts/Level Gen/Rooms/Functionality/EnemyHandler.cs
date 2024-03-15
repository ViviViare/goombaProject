using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    #region Variables

    [Header("Enemies Generated")]
    [ShowOnly] public int _generatedEnemies;
    [ShowOnly] public int _livingEnemies;
    [ShowOnly] public bool _hasGenerated;
    private List<GameObject> _enemies = new List<GameObject>();

    [Header("Generation Parameters")]
    [SerializeField] private List<EnemySpawnData> _enemyPool = new List<EnemySpawnData>();
    [SerializeField] private Transform _spawnPointsParent;
    private List<Transform> _totalSpawnPoints = new List<Transform>();
    private List<Transform> _unusedSpawnPoints;
    
    [Header("Constraints")]
    [SerializeField] private int _maxWeight = 15;
    private int _currentWeight;
    [Tooltip("The lower this value, the weaker the preference will be towards enemies spawned.")]
    [SerializeField] [Range(0,1)] private float _weightPreference;
 
    [Header("Enemy status")]
    [ShowOnly] public bool _enemiesEnabled;
    [ShowOnly] private bool _allEnemiesDead;

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
        _hasGenerated = true;

        int maxEnemies = (_enemyPool.Count + _totalSpawnPoints.Count) / 2;
        
        // A recursion handler variable is good when making a while loop.
        int attempts = 0;

        Debug.Log("Brother");
        if (_enemyPool.Count == 0)
        {
            Debug.LogError($"There is no enemies to generate for {gameObject.name}");
            return;
        }
        Debug.Log("Sister");
        if (_totalSpawnPoints.Count == 0)
        {
            Debug.LogError($"There are no valid spawn points for {gameObject.name}");
            return;
        }
        Debug.Log("Coffin of andy and leyley");
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
        _unusedSpawnPoints = _totalSpawnPoints;
    }
    
    private void GenerateAnEnemy()
    {
        Debug.Log("Trying");
        List<EnemySpawnData> filteredEnemies = _enemyPool.FindAll(valid => !IsOverdraft(valid._enemyWeight, _currentWeight, _maxWeight));
        // If the filtered enemies list comes back as 0 then bomb out of this code
        if (filteredEnemies.Count == 0) return;
        Debug.Log("his");
        // Get a new list of enemies adjusting for weight preference
        List<EnemySpawnData> newFilteredList = new List<EnemySpawnData>();
        int attempts = 0;
        // Make sure that the new filtered list has at least 20% of the amount of possible enemies that the original filtered list has
        do
        {
            attempts++;
            newFilteredList = GetAdjustedEnemyList(filteredEnemies);

        } while (newFilteredList.Count < Mathf.CeilToInt(filteredEnemies.Count * 0.2f) || attempts >= _maxSpawnAttempts);

        if (newFilteredList.Count == 0) return;
        Debug.Log("best");
        // Get a random enemy out of the enemy pool
        int chosenEnemy = Random.Range(0, newFilteredList.Count);
        EnemySpawnData newEnemy = newFilteredList[chosenEnemy];
        GameObject newEnemyGo = newEnemy.gameObject;

        // Get a random spawn point out of valid spawn points
        int chosenSpawn = Random.Range(0, _unusedSpawnPoints.Count);
        Transform newSpawn = _unusedSpawnPoints[chosenSpawn];
        // Remove this spawn point from the unused spawn points list
        _unusedSpawnPoints.Remove(newSpawn);

        // Add this enemy to the enemy list
        _enemies.Add(newEnemyGo);

        // Update the enemy handlers current weight 
        _currentWeight += newEnemy._enemyWeight;

        // Configure the new enemy to be able to communicate correctly
        newEnemy._connectedRoom = _status;
        newEnemy._enemyHandler = this;

        // Spawn the enemy
        ObjectPooler.Spawn(newEnemyGo, newSpawn.position, Quaternion.identity);

        _generatedEnemies++;
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
        foreach (GameObject enemy in _enemies)
        {
            enemy.GetComponent<EnemySpawnData>()._activatedAi = true;
        }
    }

    public void EnemyDied(GameObject deadEnemy)
    {
        _enemies.Remove(deadEnemy);
        _livingEnemies--;

        if (_livingEnemies <= 0)
        {
            _status.NoEnemiesRemaining();
        }
    }

}
