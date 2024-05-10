using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    #region Variables

    [Header("Enemies Generated")]
    [ShowOnly] public int _generatedPickups;
    [ShowOnly] public int _pickupsLeft;
    [ShowOnly] public bool _hasGenerated;

    [Header("Generation Parameters")]
    [SerializeField] private List<floorPickup> _pickupPool = new List<floorPickup>();
    [SerializeField] private Transform _spawnPointsParent;
    private List<Transform> _totalSpawnPoints = new List<Transform>();
    private List<Transform> _unusedSpawnPoints;
    
    [Header("Constraints")]
    [SerializeField] private int _maxWeight = 15;
    private int _currentWeight;

    [Tooltip("The lower this value, the weaker the preference will be towards pickups spawned.")]
    [SerializeField] [Range(0,1)] private float _weightPreference;
 

    [Header("Room References")]
    private RoomStatus _status;
    
    [Header("Debug")]
    [SerializeField] private int _maxSpawnAttempts = 40;

    #endregion
    
    private void Start()
    {
        _status = GetComponent<RoomStatus>();
    }
    
    public void GeneratePickups()
    {
        SetSpawnPoints();
        _hasGenerated = true;

        // A recursion handler variable is good when making a while loop.
        int attempts = 0;

        if (_pickupPool.Count == 0)
        {
            Debug.LogError($"There is no pickups to generate for {gameObject.name}");
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
            GeneratePickup();
        }

        _pickupsLeft = _generatedPickups;
        
    }   

    private void SetSpawnPoints()
    {
        foreach (Transform child in _spawnPointsParent)
        {
            _totalSpawnPoints.Add(child);
        }
        _unusedSpawnPoints = _totalSpawnPoints;
    }
    
    private void GeneratePickup()
    {
        List<floorPickup> filteredPickups = _pickupPool.FindAll(valid => !IsOverdraft(valid._pickupWeight, _currentWeight, _maxWeight));
        // If the filtered enemies list comes back as 0 then bomb out of this code
        if (filteredPickups.Count == 0) return;

        // Get a new list of enemies adjusting for weight preference
        List<floorPickup> newFilteredList = new List<floorPickup>();
        int attempts = 0;
        // Make sure that the new filtered list has at least 20% of the amount of possible enemies that the original filtered list has
        do
        {
            attempts++;
            newFilteredList = GetAdjustedPickupList(filteredPickups);

        } while (newFilteredList.Count < Mathf.CeilToInt(filteredPickups.Count * 0.2f) || attempts >= _maxSpawnAttempts);

        if (newFilteredList.Count == 0) return;
        
        // Get a random enemy out of the enemy pool
        int chosenPickup = Random.Range(0, newFilteredList.Count);
        floorPickup newPickup = newFilteredList[chosenPickup];
        GameObject newPickupGo = newPickup.gameObject;

        // Get a random spawn point out of valid spawn points
        int chosenSpawn = Random.Range(0, _unusedSpawnPoints.Count);
        Transform newSpawn = _unusedSpawnPoints[chosenSpawn];
        
        // Remove this spawn point from the unused spawn points list
        _unusedSpawnPoints.Remove(newSpawn);

        // Update the enemy handlers current weight 
        _currentWeight += newPickup._pickupWeight;

        // Spawn the enemy
        ObjectPooler.Spawn(newPickupGo, newSpawn.position, Quaternion.identity);

        _generatedPickups++;
    }

    private List<floorPickup> GetAdjustedPickupList(List<floorPickup> originalFilter)
    {
        List<floorPickup> newFilteredList = new List<floorPickup>();

        foreach (floorPickup pickupData in originalFilter)
        {
            // Get the absolute value of the weight difference between the difficulty preference and the possible enemy
            float weightDifference = Mathf.Abs(pickupData._pickupWeight - _weightPreference);
            float inclusionChance = 1f - Mathf.Clamp01(weightDifference);
            
            // Random chance between 0 and 1
            float randChance = Random.value;

            if (inclusionChance <= randChance) newFilteredList.Add(pickupData);
        }

        return newFilteredList;
    }

    private bool IsOverdraft(int addition, int currentVal, int maxVal)
    {
        int newVal = currentVal + addition;

        if (newVal > maxVal) return true;
        return false;
    }
}
