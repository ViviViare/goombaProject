using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnData : MonoBehaviour
{
    public int _enemyWeight;
    public int _enemyWorth;
    public float _genreBias;
    [Space]
    [ShowOnly] public RoomStatus _connectedRoom;
    [ShowOnly] public EnemyHandler _enemyHandler;
    public enemyBaseAI _baseAi;

    private void Awake()
    {
        _baseAi = GetComponent<enemyBaseAI>();
    }

    public void EnableEnemy()
    {
        if (!_baseAi._aiSetup) _baseAi.SetupEnemy();
        _baseAi.EnableEnemy();
    }

    public void EnemyDied()
    {
        _baseAi.DeactivateAI();
        _enemyHandler.EnemyDied(gameObject);
    }

}
