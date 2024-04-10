using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawning : MonoBehaviour
{
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Transform _bossSpawnPoint;
    public bool _spawnedBoss;

    void Start()
    {
        // Temporary call for testing
        SpawnBoss();
    }

    public void SpawnBoss()
    {
        // only Spawn the boss once
        if (_spawnedBoss) return;

        _spawnedBoss = true;
        Instantiate(_bossPrefab, _bossSpawnPoint.position, Quaternion.identity);
    }
}
