using System;
using System.Collections.Generic;
using UnityEngine;

public class IrregularRoomData : RoomData
{
    #region Variables
    [Header("Irregular Room Data")]
    // The other irregular room cells that this one is connected to
    public int _irregularRoomNumber;
    public IrregularRoomConnections _connectedCells;

    [Serializable]
    public class IrregularRoomConnections
    {
        public GridCell _originCell;
        public GridCell _doubleCell;
        public GridCell _LCell;
        public GridCell _TCell;
    }

    public Dictionary<Compass, Door> _roomIrregularDoors = new Dictionary<Compass, Door>() {
        {Compass.North, new Door(Compass.North)},
        {Compass.East, new Door(Compass.East)},
        {Compass.South, new Door(Compass.South)},
        {Compass.West, new Door(Compass.West)},
    };

    public HasDoors _irregularDoorsAvailable = new HasDoors();

    [Serializable]
    public struct exposedTile
    {
        public Compass _key;
        public bool[] _value;
        public bool _isExposed;

        public exposedTile(Compass compassKey) : this()
        {
            _key = compassKey;
        }
    }

    // True/False values for how many end tiles this piece has (Top to Bottom)
    public exposedTile[] _exposedTiles =
    {
        new exposedTile(Compass.North),
        new exposedTile(Compass.East),
        new exposedTile(Compass.South),
        new exposedTile(Compass.West),
    };

    #endregion

    public override void RotateDoorsClockwise()
    {
        // Run the original rotation code from RoomData.cs
        base.RotateDoorsClockwise();
        RotateIrregular();
    }

    public void RotateIrregular()
    {
        List<Door> allIrregularDoors = GetValidIrregularDoors();
        for (int i = 0; i < allIrregularDoors.Count; i++)
        {
            if (allIrregularDoors[i] == null) continue;

            switch (allIrregularDoors[i]._direction)
            {
                case Compass.North:
                    allIrregularDoors[i]._direction = Compass.East;
                    break;
                case Compass.East:
                    allIrregularDoors[i]._direction = Compass.South;
                    break;
                case Compass.South:
                    allIrregularDoors[i]._direction = Compass.West;
                    break;
                case Compass.West:
                    allIrregularDoors[i]._direction = Compass.North;
                    break;
            }
        }
        // Once room data has been rotated to accomidate then rotate the room gameobject as well


    }

    public List<Door> GetValidIrregularDoors()
    {
        List<Door> validIrregulars = new List<Door>();

        for (int i = 0; i < _roomIrregularDoors.Count; i++)
        {
            if (_roomIrregularDoors[(Compass)i]._isValid)
            {
                validIrregulars.Add(_roomIrregularDoors[(Compass)i]);
            }
        }

        return validIrregulars;
    }

    public void UpdateIrregularDictionary()
    {
        if (_irregularDoorsAvailable._hasNorthDoor)
        {
            _roomIrregularDoors[Compass.North]._isValid = _irregularDoorsAvailable._hasNorthDoor ? true : false;
        }
        // Check if east doro has been changed
        if (_irregularDoorsAvailable._hasEastDoor)
        {
            _roomIrregularDoors[Compass.East]._isValid = _irregularDoorsAvailable._hasEastDoor ? true : false;
        }
        // Check if south door has been changed
        if (_irregularDoorsAvailable._hasSouthDoor)
        {
            _roomIrregularDoors[Compass.South]._isValid = _irregularDoorsAvailable._hasSouthDoor ? true : false;
        }
        // Check if west door has been changed
        if (_irregularDoorsAvailable._hasWestDoor)
        {
            _roomIrregularDoors[Compass.West]._isValid = _irregularDoorsAvailable._hasWestDoor ? true : false;
        }
    }

    public bool CheckIfConnectedPart(GridCell cellToCheck)
    {
        if (_connectedCells._originCell == cellToCheck) return true;
        if (_connectedCells._doubleCell == cellToCheck) return true;
        if (_connectedCells._LCell == cellToCheck) return true;
        if (_connectedCells._TCell == cellToCheck) return true;

        return false;
    }
    private void OnValidate()
    {
        if (_irregularDoorsAvailable._hasNorthDoor == _doorsAvailable._hasNorthDoor
        || _irregularDoorsAvailable._hasEastDoor == _doorsAvailable._hasEastDoor
        || _irregularDoorsAvailable._hasSouthDoor == _doorsAvailable._hasSouthDoor
        || _irregularDoorsAvailable._hasWestDoor == _doorsAvailable._hasWestDoor)
        {
            Debug.Log("<b><color=#d40f5e>Irregular room: </color><color=#ed7eaa>" + name
            + "</color><color=#d40f5e> cannot have overlapping irregular and regular doors. Please Fix</color></b>");
        }
    }


}
