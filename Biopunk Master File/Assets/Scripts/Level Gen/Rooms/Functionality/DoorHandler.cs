/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 01/03/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  DoorHandler.cs
//
//  Deals with the door setup for room teleporation
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/
using System.Linq;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    [Header("Data")]
    [ShowOnly] public bool _isOpen = false;

    [Header("References")]
    public Transform _spawnPoint;
    private BoxCollider _doorExit;

    [Header("Door Data")]
    public RoomData _roomData;
    public Door _doorData;
    [Header("Connection Data")]
    private bool _connectionSet;
    private Vector3Int _connectedCellPosition;
    private GridCell _connectedCell;
    public RoomData _connectedRoom;
    public Door _connectedDoor;
    private Compass _inverseDirection;
    private Vector3Int vectorToConnected  = Vector3Int.zero;
    
    private void Awake()
    {
        _doorExit = GetComponent<BoxCollider>();
        _isOpen = false;
        _doorExit.enabled = false;
    }

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
        
        _connectedCellPosition = _roomData._cellPosition + vectorToConnected;
    
        // Check to see if this door is valid
        if (!IsDoorValid() )
        {
            gameObject.SetActive(false);
            return;
        }

        _connectedCell = Level_Generator._instance._cellDictionary[_connectedCellPosition];
        _connectedRoom = _connectedCell._roomData;
        
        // Get the opposite door that is connected to this door
        _connectedDoor = _connectedRoom._roomDoors.Values.ToList().Find(valid => valid._direction == _inverseDirection && valid._isValid);
    }

    private bool IsDoorValid()
    {
        GridCell cellToCheck = Level_Generator._instance.GetGridCell(_connectedCellPosition.x, _connectedCellPosition.y, _connectedCellPosition.z);

        // Check to see if the cell exists or is set as a room
        if (cellToCheck == null || !cellToCheck._setAsRoom)
        {
            return false;
        }

        // Check to see if this door is an end room connection
        if (cellToCheck == Level_Generator._instance._endRoom && !Level_Generator._instance._cellDictionary[_roomData._cellPosition]._endRoomConnection)
        {
            return false;
        }

        // Check to see if there is a valid door connecting to this one
        if ((cellToCheck._roomData._roomDoors.Values.ToList().Find(valid => valid._direction == _inverseDirection && valid._isValid) == null))
        {
            return false;
        }
        

        return true;
    }
    
    public void ToggleDoor()
    {
        // When called, this method reverses _isOpen and _doorExit.Enabled
        _isOpen = !_isOpen;
        _doorExit.enabled = !_doorExit.enabled;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Do nothing if the collision is not the player
        // Do nothing if the door is not set as open
        if (!_isOpen || collider.gameObject.tag != "Player") return;

        Transform player = collider.gameObject.transform;

        // Teleport the player to the opposite rooms connected door with an offset
        player.position = _connectedDoor._doorGo.GetComponent<DoorHandler>()._spawnPoint.position;
        _connectedRoom.GetComponent<RoomStatus>()?.PlayerEntered();

        // Let the room this door is attatched to know that the player has left.
        _roomData.GetComponent<RoomStatus>()?.PlayerLeft();
    }

    
}


