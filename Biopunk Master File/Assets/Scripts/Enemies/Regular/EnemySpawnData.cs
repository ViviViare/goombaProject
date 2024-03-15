using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnData : MonoBehaviour
{
    public int _enemyWeight;
    public float _genreBias;

    [Space]
    [ShowOnly] public bool _activatedAi;
    [ShowOnly] public RoomStatus _connectedRoom;
    [ShowOnly] public EnemyHandler _enemyHandler;

    public void EnemyDied()
    {
        _enemyHandler.EnemyDied(gameObject);
    }

}
