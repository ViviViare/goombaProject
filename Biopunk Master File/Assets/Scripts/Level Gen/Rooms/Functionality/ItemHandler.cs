using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    [SerializeField] public List<ItemSpawnData> _itemPool = new List<ItemSpawnData>();

    [Header("Room References")]
    private RoomStatus _status;

    [Header("Constraints")]
    [SerializeField] private int _maxWeight = 15;
    private int _currentWeight;
    [Tooltip("The lower this value, the weaker the preference will be towards enemies spawned.")]
    [SerializeField] [Range(0.02f, 1)] private float _weightPreference;

    [SerializeField] private GameObject _itemSpawnPoint;

    [SerializeField] private bool _hasGenerated = false;

    public void GenerateItems()
    {
        if (_hasGenerated) return;
        _hasGenerated = true;
        float randRemoval = Random.Range(0, 1);

        // A recursion handler variable is good when making a while loop.
        int attempts = 0;

        if (_itemPool.Count == 0)
        {
            Debug.LogError($"There is no enemies to generate for {gameObject.name}");
            return;
        }

        if (_itemSpawnPoint == null)
        {
            Debug.LogError($"There are no valid spawn points for {gameObject.name}");
            return;
        }

        GenerateItem();
    }

    private void GenerateItem()
    {
        List<ItemSpawnData> filteredItems = _itemPool.FindAll(valid => !IsOverdraft(valid._itemWeight, _currentWeight, _maxWeight));
        // If the filtered enemies list comes back as 0 then bomb out of this code
        if (filteredItems.Count == 0) return;

        // Get a new list of enemies adjusting for weight preference
        List<ItemSpawnData> newFilteredList = new List<ItemSpawnData>();

        // Make sure that the new filtered list has at least 20% of the amount of possible enemies that the original filtered list has

        newFilteredList = GetAdjustedItemList(filteredItems);

        if (newFilteredList.Count == 0) return;
        // Get a random enemy out of the enemy pool
        int chosenItem = Random.Range(0, newFilteredList.Count);
        ItemSpawnData chosenItemData = newFilteredList[chosenItem];
        GameObject chosenItemGameobject = chosenItemData.gameObject;

        // Update the enemy handlers current weight 
        _currentWeight += chosenItemData._itemWeight;

        // Spawn the enemy
        GameObject newItemSpawned = ObjectPooler.Spawn(chosenItemGameobject, _itemSpawnPoint.transform.position, Quaternion.identity);
        ItemSpawnData newItemData = newItemSpawned.GetComponent<ItemSpawnData>();

        // Configure the new enemy to be able to communicate correctly
        //newItemData._connectedRoom = _status;
        //newItemData._enemyHandler = this;

        // Add this enemy to the enemy list
        //_enemies.Add(newEnemySpawned);

        //_generatedEnemies += newEnemyData._enemyWorth;
    }

    private List<ItemSpawnData> GetAdjustedItemList(List<ItemSpawnData> originalFilter)
    {
        List<ItemSpawnData> newFilteredList = new List<ItemSpawnData>();

        foreach (ItemSpawnData spawnData in originalFilter)
        {
            // Get the absolute value of the weight difference between the difficulty preference and the possible enemy
            float weightDifference = Mathf.Abs(spawnData._itemWeight - _weightPreference);
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
}
