/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 01/03/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  RoomStatus.cs
//
//  Holds the current status of a room
//  Used to determin if there are any enemies in this room
//  And to handle teleporation between rooms
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomStatus : MonoBehaviour
{
    #region Variables
    // Room Data
    private RoomData _data;
    private GridCell _cell;
    private bool _isIrregular;

    // References
    private EnemyHandler _enemyHandler;
    private LootHandler _lootHandler;
    private List<Door> _validDoors;

    // Parameters
    [ShowOnly] public bool _isActive;
    [ShowOnly] public bool _beenVisited;
    [ShowOnly] public bool _adjacentVisited;

    #endregion

    
    private void Start()
    {
        _data = GetComponent<RoomData>();
        _cell = Level_Generator._instance._cellDictionary[_data._cellPosition];

        if (_data is IrregularRoomData) _isIrregular = true;

        _enemyHandler = GetComponent<EnemyHandler>();
        _lootHandler = GetComponent<LootHandler>();

        _validDoors = _data._roomDoors.Values.ToList().FindAll(valid => valid._isValid && valid._doorGo != null);

        // Doors get disabled by default
        foreach (Door door in _validDoors)
        {
            DoorHandler doorTeleport = door._doorGo.GetComponent<DoorHandler>();

            doorTeleport._roomData = _data;
            doorTeleport._doorData = door;

            doorTeleport._isOpen = false;
        }
        if (_cell == Level_Generator._instance._startRoom) PlayerEntered();
    }

    public void PlayerEntered()
    {
        _isActive = true;
        _beenVisited = true;

        _enemyHandler?.EnableEnemies();
        PrepareNeighbours();

        if (_enemyHandler == null) NoEnemiesRemaining();
    }

    public void NoEnemiesRemaining()
    {
        ToggleDoors();
    }
    
    public void PlayerLeft()
    {
        _isActive = false;
    }

    private void ToggleDoors()
    {
        // Disable each door in this room when a player enters
        foreach (Door door in _validDoors)
        {
            door._doorGo.GetComponent<DoorHandler>().ToggleDoor();
        }
    }

    private void PrepareNeighbours()
    {
        List<GridCell> neighbours = Level_Generator._instance.GetNeighbouringRooms(_cell);

        // Enable enemies in neighbouring rooms if that room has an enemy handler
        foreach (GridCell neighbour in neighbours)
        {
            Debug.Log("FUck off eat shit");
            neighbour._roomData.GetComponent<RoomStatus>()._adjacentVisited = true;

            // If there is an EnemyHandler script on the adjacent rooms then generate their enemies.
            EnemyHandler enemyHandler = GetComponent<EnemyHandler>();
            if (enemyHandler != null && !enemyHandler._hasGenerated) enemyHandler.GenerateEnemies();
        }

    }

}
