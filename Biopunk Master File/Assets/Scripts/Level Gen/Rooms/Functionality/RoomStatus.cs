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
            DoorHandler doorTeleport = door._doorGo.AddComponent<DoorHandler>();

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
        TriggerNeighboursEnemies();

        if (_enemyHandler == null) EnableDoors();
    }

    public void PlayerLeft()
    {
        _isActive = false;
    }

    public class DoorHandler : MonoBehaviour
    {
        [Header("Data")]
        [ShowOnly] public bool _isOpen = false;

        [Header("References")]
        public RoomData _roomData;
        public Door _doorData;

        private GridCell _connectedCell;
        private RoomData _connectedRoom;
        private Door _connectedDoor;
        private Compass _inverseDirection;
        private Vector3Int vectorToConnected  = Vector3Int.zero;

        private void Start()
        {
            // Get the room that is connected to this door
            if (_doorData._direction == Compass.North)
            {
                vectorToConnected = Vector3Int.forward;
                _inverseDirection = Compass.South;
            }
            else if (_doorData._direction == Compass.East)
            {
                vectorToConnected = Vector3Int.right;
                _inverseDirection = Compass.West;
            }
            else if (_doorData._direction == Compass.South)
            {
                vectorToConnected = Vector3Int.back;
                _inverseDirection = Compass.North;
            }
            else if (_doorData._direction == Compass.West)
            {
                vectorToConnected = Vector3Int.left;
                _inverseDirection = Compass.East;
            }

            _connectedCell = Level_Generator._instance._cellDictionary[_roomData._cellPosition + vectorToConnected];
            _connectedRoom = _connectedCell._roomData;
            // Get the opposite door that is connected to this door
            _connectedDoor = _connectedRoom._roomDoors.Values.ToList().Find(valid => valid._direction == _inverseDirection);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Do nothing if the collision is not the player
            // Do nothing if the door is not set as open
            if (!_isOpen || collision.gameObject.tag != "Player") return;

            Transform player = collision.gameObject.transform;

            // teleport the player to the opposite rooms connected door with an offset
            player.position = _connectedDoor._doorGo.transform.position + ((Vector3)vectorToConnected * DoorTeleportOffset._offset);
            _connectedRoom.GetComponent<RoomStatus>()?.PlayerEntered();

            // Let the room this door is attatched to know that the player has left.
            _roomData.GetComponent<RoomStatus>()?.PlayerLeft();
        }
    }

    public void EnableDoors()
    {
        // Disable each door in this room when a player enters
        foreach (Door door in _validDoors)
        {
            door._doorGo.GetComponent<DoorHandler>()._isOpen = true;
        }
    }

    private void TriggerNeighboursEnemies()
    {
        List<GridCell> neighbours = Level_Generator._instance.GetNeighbouringRooms(_cell);

        // Enable enemies in neighbouring rooms if that room has an enemy handler
        foreach (GridCell neighbour in neighbours)
        {
            neighbour._roomData.GetComponent<RoomStatus>()._adjacentVisited = true;

            // If there is an EnemyHandler script on the adjacent rooms then generate their enemies.
            neighbour._roomData.GetComponent<EnemyHandler>()?.GenerateEnemies();
        }

    }

}
