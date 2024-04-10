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
    private List<GridCell> _roomNeighbours = new List<GridCell>();

    // References
    private EnemyHandler _enemyHandler;
    private PickupHandler _pickupHandler;
    private ItemHandler _itemHandler;
    private List<Door> _validDoors;

    // Parameters
    [ShowOnly] public bool _isActive;
    [ShowOnly] public bool _beenVisited;
    [ShowOnly] public bool _adjacentVisited;
    public bool _isItemRoom;

    #endregion

    
    private void Start()
    {
        _data = GetComponent<RoomData>();
        _cell = Level_Generator._instance._cellDictionary[_data._cellPosition];

        if (_data is IrregularRoomData) _isIrregular = true;

        _enemyHandler = GetComponent<EnemyHandler>();
        _pickupHandler = GetComponent<PickupHandler>();
        _itemHandler = GetComponent<ItemHandler>();

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
        _itemHandler?.GenerateItems();
        PrepareNeighbours();
        
        if (_enemyHandler == null || _enemyHandler.enabled == false || _enemyHandler._allEnemiesDead) NoEnemiesRemaining();
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

        GlobalVariables._player.GetComponent<playerActiveItem>()._activeItemCharge++;
        GlobalVariables._player.GetComponent<playerStatusEffects>()._amplifierDuration--;
        GlobalVariables._player.GetComponent<playerStatusEffects>()._serumDuration--;
        GlobalVariables._stimulant.GetComponent<StimulantScript>()._stimulantTimer++;

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
        UnprepareNeighbours();
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

    private void UnprepareNeighbours()
    {
        // Remove all enemies from rooms which are not chosen.
        foreach (GridCell neighbour in _roomNeighbours)
        {
            RoomStatus neighbourStatus = neighbour._roomData.GetComponent<RoomStatus>();
            
            // Do not affect neighbours which are part of the same irregular room set.
            if (neighbourStatus._data is IrregularRoomData)
            {
                IrregularRoomData convertedNeighbourData = (IrregularRoomData)neighbourStatus._data;
                IrregularRoomData convertedOwnData = (IrregularRoomData)_data;

                if (convertedNeighbourData._irregularRoomNumber == convertedOwnData._irregularRoomNumber) continue;
            }
            
            // Only affect the neighbours that are not the newly activated one
            // Do not affect neighbours which have already been visited
            if (neighbourStatus._isActive || neighbourStatus._beenVisited) continue;

            neighbourStatus.GetComponent<EnemyHandler>()?.DegenerateEnemies();
        }

        if (_data._verticality == Verticality.NONE) return;

        LadderHandler ladderHandler = _data._ladderObject.GetComponent<LadderHandler>();
        RoomData ladderConnection = ladderHandler._connectedRoom;
        RoomStatus ladderStatus = ladderConnection.GetComponent<RoomStatus>();

        EnemyHandler ladderEnemyHandler = ladderConnection.GetComponent<EnemyHandler>();

        if (ladderStatus._isActive || ladderStatus._beenVisited) return;

        ladderEnemyHandler.DegenerateEnemies();


    }

    private void PrepareNeighbours()
    {
        _roomNeighbours = Level_Generator._instance.GetNeighbouringRooms(_cell);

        // Enable enemies in neighbouring rooms if that room has an enemy handler
        foreach (GridCell neighbour in _roomNeighbours)
        {
            RoomStatus neighbourStatus = neighbour._roomData.GetComponent<RoomStatus>();
            
            // Do not affect neighbours which are part of the same irregular room set.
            if (neighbourStatus._data is IrregularRoomData)
            {
                IrregularRoomData convertedNeighbourData = (IrregularRoomData)neighbourStatus._data;
                IrregularRoomData convertedOwnData = (IrregularRoomData)_data;

                if (convertedNeighbourData._irregularRoomNumber == convertedOwnData._irregularRoomNumber) continue;
            }

            neighbourStatus._adjacentVisited = true;

            // If there is an EnemyHandler script on the adjacent rooms then generate their enemies.
            EnemyHandler enemyHandler = neighbourStatus.GetComponent<EnemyHandler>();
            
            if (enemyHandler == null || enemyHandler._hasGenerated || neighbourStatus._beenVisited) continue;

            Debug.Log($"Generating enemies for {neighbourStatus.name}");
            enemyHandler.GenerateEnemies();
        }

        if (_data._verticality == Verticality.NONE) return;

        LadderHandler ladderHandler = _data._ladderObject.GetComponent<LadderHandler>();
        RoomData ladderConnection = ladderHandler._connectedRoom;
        RoomStatus ladderStatus = ladderConnection.GetComponent<RoomStatus>();

        EnemyHandler ladderEnemyHandler = ladderConnection.GetComponent<EnemyHandler>();

        if (ladderEnemyHandler == null || ladderEnemyHandler._hasGenerated || ladderStatus._beenVisited) return;

        Debug.Log($"Generating enemies for {ladderStatus.name}");
        ladderEnemyHandler.GenerateEnemies();


    }

}
