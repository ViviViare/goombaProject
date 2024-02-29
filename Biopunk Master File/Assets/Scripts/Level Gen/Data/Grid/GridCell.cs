/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 05/02/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  GridCell.cs
//
//  Holds the data for each grid cell produced by Level_Generator.cs
//  All fields are publically avaliable as they are used heavily in the level generation script
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    [Header("Main Cell Data")]
    // Position of the cell in the world space
    [HideInInspector] public Vector3 _position;
    // Position of the cell in the grid
    [ShowOnly] public Vector3Int _positionInGrid;
    [HideInInspector] public bool _setAsRoom;
    [HideInInspector] public bool _roomSetup;
    [HideInInspector] public bool _setAsIrregular;
    [HideInInspector] public bool _setAsAddOn;
    [ShowOnly] public bool _endRoomConnection;

    [Header("Mine Data")]
    [HideInInspector] public CellStatus _cellStatus = CellStatus.isRegular;
    public enum CellStatus
    {
        isRegular,
        isMine,
        isBarrier,
    }

    [HideInInspector] public int _clusterValue;

    [Header("Pathfinding")]
    [HideInInspector] public int _gCost; // Distance between the current node and the start node
    [HideInInspector] public int _hCost; // heuristic cost
    [ShowOnly] public int _fCost; // full cost
    [HideInInspector] public bool _neighbourYCostIncreased;
    [HideInInspector] public bool _increasedYCost;
    [HideInInspector] public GridCell _cameFromCell;

    [Header("Room Data")]
    [HideInInspector] public RoomData _roomData;
    [HideInInspector] public bool _requiredDoorsSet;
    [HideInInspector] public HasDoors _requiredDoors = new HasDoors();
    [HideInInspector] public HasDoors _requiredIrregularDoors = new HasDoors();
    [ShowOnly] public Verticality _verticalityType = Verticality.NONE;
    [HideInInspector] public DoorConfiguration _neededDoorConfiguration;

    public void SetStatus(CellStatus status)
    {
        _cellStatus = status;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    // Note - Leviathan
    // When creating a class, do NOT have a list of that same class as a field in that class.
    // It causes so much recursion that causes Unity to memory leak.
    // This script used to have 4 while it was being written.
    // My pc soft-locked for far too long because of this.
}