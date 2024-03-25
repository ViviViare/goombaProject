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
    private PickupHandler _pickupHandler;
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
        _pickupHandler = GetComponent<PickupHandler>();

        _validDoors = _data._roomDoors.Values.ToList().FindAll(valid => valid._isValid && valid._doorGo != null);

        // Doors get disabled by default
        foreach (Door door in _validDoors)
        {
            DoorHandler doorTeleport = door._doorGo.GetComponent<DoorHandler>();

            doorTeleport._roomData = _data;
            doorTeleport._doorData = door;

            doorTeleport._isOpen = false;
        }

        // Override so that the player is considered to have entered the room if the room is the start
        if (_cell == Level_Generator._instance._startRoom) PlayerEntered();

        // If the room has verticality, then set the ladder's data correctly
        _data._ladderObject?.SetRoomData(_data);
    }

    public void PlayerEntered()
    {
        // Check if this room is part of an irregular connection network
        if (_data is IrregularRoomData)
        {
            IrregularRoomData trueData = (IrregularRoomData)_data;

            trueData._connectedCells._originCell._roomData?.GetComponent<RoomStatus>().ValidateRoom();
            trueData._connectedCells._doubleCell._roomData?.GetComponent<RoomStatus>().ValidateRoom();
            trueData._connectedCells._LCell._roomData?.GetComponent<RoomStatus>().ValidateRoom();
            trueData._connectedCells._TCell._roomData?.GetComponent<RoomStatus>().ValidateRoom();

        }
        else
        {
            ValidateRoom();
        }
    }

    private void ValidateRoom()
    {
        _isActive = true;
        _beenVisited = true;

        _enemyHandler?.EnableEnemies();
        PrepareNeighbours();
        
        if (_enemyHandler == null || _enemyHandler.enabled == false) NoEnemiesRemaining();
    }

    public void NoEnemiesRemaining()
    {
        ToggleDoors();
        GlobalVariables._roomsCleared++;

        if (GlobalVariables._inCombat)
        {
            GlobalVariables._musicManager.GetComponent<MusicManager>().FadeToSecondary();
        }

        GlobalVariables._inCombat = false;

        _pickupHandler?.GeneratePickups();
    }
    
    public void PlayerLeft()
    {
        // Check if this room is part of an irregular connection network
        if (_data is IrregularRoomData)
        {
            IrregularRoomData trueData = (IrregularRoomData)_data;

            trueData._connectedCells._originCell._roomData?.GetComponent<RoomStatus>().DeactivateRoom();
            trueData._connectedCells._doubleCell._roomData?.GetComponent<RoomStatus>().DeactivateRoom();
            trueData._connectedCells._LCell._roomData?.GetComponent<RoomStatus>().DeactivateRoom();
            trueData._connectedCells._TCell._roomData?.GetComponent<RoomStatus>().DeactivateRoom();

        }
        else
        {
            DeactivateRoom();
        }
    }

    private void DeactivateRoom()
    {
        _isActive = false;
        ToggleDoors();
    }

    private void ToggleDoors()
    {
        // Disable each door in this room when a player enters
        foreach (Door door in _validDoors)
        {
            door._doorGo.GetComponent<DoorHandler>().ToggleDoor();
        }
        
        // If the room has verticality, then toggle that rooms ladder
        _data._ladderObject?.ToggleLadder();
    }

    private void PrepareNeighbours()
    {
        List<GridCell> neighbours = Level_Generator._instance.GetNeighbouringRooms(_cell);

        // Enable enemies in neighbouring rooms if that room has an enemy handler
        foreach (GridCell neighbour in neighbours)
        {
            neighbour._roomData.GetComponent<RoomStatus>()._adjacentVisited = true;

            // If there is an EnemyHandler script on the adjacent rooms then generate their enemies.
            EnemyHandler enemyHandler = GetComponent<EnemyHandler>();
            if (enemyHandler != null && !enemyHandler._hasGenerated) enemyHandler.GenerateEnemies();
        }

    }

}
