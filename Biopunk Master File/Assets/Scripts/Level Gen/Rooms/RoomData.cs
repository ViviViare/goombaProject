/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 06/02/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  RoomData.cs
//
//  Holds the data for each room
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    [Header("Room Cell Data")]
    [ShowOnly] public Vector3Int _cellPosition;
    [ShowOnly] public int _roomRotation = 0;
    [ShowOnly] public int _timesRotated;

    [Header("Spawn Constraints")]
    public float _genreBias;
    public bool _isLaboratory;
    [ShowOnly] public bool _setAsUp;
    [ShowOnly] public bool _setAsDown;
    public Verticality _verticality;

    public Dictionary<Compass, Door> _roomDoors = new Dictionary<Compass, Door>() {
        {Compass.North, new Door(Compass.North)},
        {Compass.East, new Door(Compass.East)},
        {Compass.South, new Door(Compass.South)},
        {Compass.West, new Door(Compass.West)},
    };

    public DoorGameObjects _doorGameObjects;

    // Hold Data for what direction each local door is pointing towards (3D directions)
    public DoorConfiguration _doorConfiguration;
    // Hold data for what type of room this is (Regular, Dead End, Corner, Three Way)
    public RoomType _roomType;
    // Four bools that enable the validity of the doors in _roomDoors
    public HasDoors _doorsAvailable = new HasDoors();

    public virtual void RotateDoorsClockwise()
    {
        _timesRotated++;
        _roomRotation += 90;
        if (_roomRotation > 270) _roomRotation = 0;

        List<Door> allDoors = GetValidDoors();

        for (int i = 0; i < allDoors.Count; i++)
        {
            if (allDoors[i] == null) continue;

            switch (allDoors[i]._direction)
            {
                case Compass.North:
                    allDoors[i]._direction = Compass.East;
                    break;
                case Compass.East:
                    allDoors[i]._direction = Compass.South;
                    break;
                case Compass.South:
                    allDoors[i]._direction = Compass.West;
                    break;
                case Compass.West:
                    allDoors[i]._direction = Compass.North;
                    break;
            }
        }
    }

    public DoorConfiguration GetDoorConfiguration()
    {
        return _doorConfiguration;
    }

    public List<Door> GetValidDoors()
    {
        List<Door> validDoors = new List<Door>();

        for (int i = 0; i < _roomDoors.Count; i++)
        {
            if (_roomDoors[(Compass)i]._isValid)
            {
                validDoors.Add(_roomDoors[(Compass)i]);
            }
        }

        return validDoors;
    }

    public void DisableUnusedDoors(GridCell thisCell, List<GridCell> neighbours)
    {
        List<Compass> noRoomNeighbours = new List<Compass>();

        foreach (GridCell neighbour in neighbours)
        {
            Vector3Int posDiff = thisCell._positionInGrid - neighbour._positionInGrid;

            // If the neighbour is a valid room, skip it
            if (neighbour._setAsRoom) continue;

            // Neighbour cell is left of the current
            if (posDiff == Vector3.left) noRoomNeighbours.Add(Compass.West);

            // Neighbour cell is right of the current
            else if (posDiff == Vector3.right) noRoomNeighbours.Add(Compass.East);

            // Neighbour cell is in front of the current
            else if (posDiff == Vector3.forward) noRoomNeighbours.Add(Compass.North);

            // Neighbour cell is behind of the current
            else if (posDiff == Vector3.back) noRoomNeighbours.Add(Compass.South);
        }

        for (int i = 0; i < noRoomNeighbours.Count; i++)
        {
            foreach (Door door in _roomDoors.Values)
            {
                if (door._direction == noRoomNeighbours[i] && door._doorGo != null)
                {
                    door._doorGo.SetActive(false);
                }
            }

        }
    }

    public void UpdateRoomDictionary()
    {
        if (_doorsAvailable._hasNorthDoor)
        {
            _roomDoors[Compass.North]._isValid = _doorsAvailable._hasNorthDoor ? true : false;
        }
        // Check if east doro has been changed
        if (_doorsAvailable._hasEastDoor)
        {
            _roomDoors[Compass.East]._isValid = _doorsAvailable._hasEastDoor ? true : false;
        }
        // Check if south door has been changed
        if (_doorsAvailable._hasSouthDoor)
        {
            _roomDoors[Compass.South]._isValid = _doorsAvailable._hasSouthDoor ? true : false;
        }
        // Check if west door has been changed
        if (_doorsAvailable._hasWestDoor)
        {
            _roomDoors[Compass.West]._isValid = _doorsAvailable._hasWestDoor ? true : false;
        }
    }

    public void UpdateRoomDoorDictionary()
    {
        if (_doorsAvailable._hasNorthDoor)
        {
            _roomDoors[Compass.North]._doorGo = _doorGameObjects._northDoor;
        }
        // Check if east doro has been changed
        if (_doorsAvailable._hasEastDoor)
        {
            _roomDoors[Compass.East]._doorGo = _doorGameObjects._eastDoor;
        }
        // Check if south door has been changed
        if (_doorsAvailable._hasSouthDoor)
        {
            _roomDoors[Compass.South]._doorGo = _doorGameObjects._southDoor;
        }
        // Check if west door has been changed
        if (_doorsAvailable._hasWestDoor)
        {
            _roomDoors[Compass.West]._doorGo = _doorGameObjects._westDoor;
        } 
    }

    public Compass GetCompassDirection(Door door)
    {
        return door._direction;
    }

    private void OnDrawGizmos()
    {
        // Only show connected rooms if this is on
        if (!Level_Generator._instance._showRoomConnections) return;

        foreach (Door door in GetValidDoors())
        {
            Gizmos.color = Color.cyan;
            Vector3 target = transform.position;
            if (door._direction == Compass.North) target = Level_Generator._instance._cellDictionary[_cellPosition + Vector3Int.forward]._roomData.transform.position; 
            if (door._direction == Compass.East) target = Level_Generator._instance._cellDictionary[_cellPosition + Vector3Int.right]._roomData.transform.position; 
            if (door._direction == Compass.South) target = Level_Generator._instance._cellDictionary[_cellPosition + Vector3Int.back]._roomData.transform.position; 
            if (door._direction == Compass.West) target = Level_Generator._instance._cellDictionary[_cellPosition + Vector3Int.left]._roomData.transform.position; 

            if (target != transform.position) Gizmos.DrawLine(transform.position, target);
        }

        Gizmos.color = Color.blue;
        Vector3 verticalTarget = transform.position;
        if (_setAsUp) verticalTarget = Level_Generator._instance._cellDictionary[_cellPosition + Vector3Int.up]._roomData.transform.position; 
        
        if (verticalTarget != transform.position) Gizmos.DrawLine(transform.position, verticalTarget);
    }
}
