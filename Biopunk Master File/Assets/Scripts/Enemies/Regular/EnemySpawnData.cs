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

    public bool _activatedAi = false;

    private void Start()
    {
        _baseAi = GetComponent<enemyBaseAI>();
    }

    private void OnDisable()
    {
        _activatedAi = false;
    }

    public void EnableEnemy()
    {
        _activatedAi = true;
        _baseAi.SetupEnemy();
    }

    public void EnemyDied()
    {
        _enemyHandler.EnemyDied(gameObject);
    }

}
