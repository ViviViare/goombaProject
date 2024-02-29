using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGen_AddOn : MonoBehaviour
{
    #region variables
    [Tooltip("Name of this level gen add on. This is not used by any method")]
    [SerializeField] private string _name = "New LVLGEN_AddOn Component";
    
    [Header("Generated Rooms")]
    [ShowOnly] public int _roomsMade;
    private List<GridCell> _reservedCells = new List<GridCell>();

    [Header("Generation")]
    [SerializeField] [Range(1, 10)] private int _roomsToMake = 1;
    public List<GameObject> _roomPrefabs = new List<GameObject>();
    [Space]
    [Header("Room Positioning")]
    [Tooltip("Limits the amount of neighbouring rooms that this add on can have. Irregular rooms with multiple parts count as one room")]
    [SerializeField] [Range(1, 4)] private int _maxNeighbours = 4;
    [SerializeField] private AddOnDistance _distanceFrom = new AddOnDistance();

    [Serializable]
    public class AddOnDistance
    {
        [Tooltip("If enabled then use the below parameters to define distances between the Start room")]
        public bool _denyNearStart = true;
        [Tooltip("Values are inclusive")]
        public int _minDistanceFromStart, _maxDistanceFromStart = int.MaxValue;
        [Space]
        [Tooltip("If enabled then use the below parameters to define distances between the End room")]
        public bool _denyNearEnd = true;
        [Tooltip("Values are inclusive")]
        public int _minDistanceFromEnd, _maxDistanceFromEnd = int.MaxValue;
        [Space]
        [Tooltip("If enabled then use the below parameters to define distances between other rooms of this type")]
        public bool _denyNearSelf = true;
        [Tooltip("Values are inclusive")]
        public int _minDistanceFromSelf, _maxDistanceFromSelf = int.MaxValue;
    }

    [Header("Generation Restrictions")]
    [Tooltip("If enabled then the Allowed Floors list is ignored")]
    [SerializeField] private bool _allowAllFloors;
    [Tooltip("If enabled then only one room of this type can spawn on a floor, regardless of _roomsToMake's value")]
    [SerializeField] private bool _allowMultipleOnFloor;
    [SerializeField] private List<int> _allowedFloors = new List<int>();
    
    [Space]
    [Header("Debugging")]
    [SerializeField] [Range(1, 100)] private int _maxAttemptsPerSlot = 40;
    [SerializeField] private Color _debugColour = Color.white;
    #endregion
    
    // Subscribe to the level generator events 
    private void OnEnable()
    {
        Level_Generator._addOnReserveSlots += ReserveGridSlots;
        Level_Generator._addOnGenerateReserves += GenerateRooms;
    }
    
    // Unsubscribe from the level generator events
    private void OnDisable()
    {
        Level_Generator._addOnReserveSlots -= ReserveGridSlots;
        Level_Generator._addOnGenerateReserves -= GenerateRooms; 
    }

    private void ReserveGridSlots()
    {
        // Reserve all the slots
        for (int i = 0; i < _roomsToMake; i++)
        {
            ReserveSlot();
        }
    }
    
    private void ReserveSlot()
    {
        //Find valid slots.
        Level_Generator levelGen = Level_Generator._instance;
        List<GridCell> validNeighbours = Level_Generator._totalPath.ToList();
        
        if (!_allowAllFloors)
        {
            validNeighbours.RemoveAll(valid => !IsOnAllowedFloor(valid));
        }

        if (!_allowMultipleOnFloor)
        {
            validNeighbours.RemoveAll(valid => IsOnLockedFloor(valid));
        }

        if (validNeighbours.Count == 0) return;
        

        GridCell addOnCell = validNeighbours[0];
        
        bool checkAgain = false;
        int madeAttempts = 0;
        // Find a new room position that has not already been set as a room
        do
        {
            // If a cell could not be found in X tries, return.
            if (madeAttempts >= _maxAttemptsPerSlot) 
            {
                Debug.Log("Could not find a valid room to make an add-on");
                continue;
            }

            madeAttempts++;
            checkAgain = false;

            // Get all rooms which have not been set as addons 
            int randNeighbour = UnityEngine.Random.Range(0, validNeighbours.Count);
            GridCell suggestedNeighbour = levelGen._cellDictionary[validNeighbours[randNeighbour]._positionInGrid];

            List<GridCell> possibleAddOnCells = levelGen.GetCellNeighbours2D(suggestedNeighbour).FindAll(valid => !valid._setAsAddOn && !valid._setAsRoom);

            // If there are no valid cells then return
            if (possibleAddOnCells.Count == 0) continue;            
            int randAddOnCell = UnityEngine.Random.Range(0, possibleAddOnCells.Count);
            addOnCell = possibleAddOnCells[randAddOnCell];

            if (!_distanceFrom._denyNearEnd
            && levelGen.GetManhattanDistance(addOnCell, levelGen._endRoom) > _distanceFrom._maxDistanceFromEnd) 
                checkAgain = true;
            
            if (!_distanceFrom._denyNearStart
            && levelGen.GetManhattanDistance(addOnCell, levelGen._startRoom) > _distanceFrom._maxDistanceFromStart) 
                checkAgain = true;

            if (!_distanceFrom._denyNearSelf && _reservedCells.Count > 0  
            && LowestDistanceFromSiblings(addOnCell) > _distanceFrom._maxDistanceFromStart) 
                checkAgain = true;
        } 
        while (checkAgain || addOnCell._setAsRoom
        || _distanceFrom._denyNearEnd && levelGen.GetManhattanDistance(addOnCell, levelGen._endRoom) < _distanceFrom._minDistanceFromEnd
        || _distanceFrom._denyNearStart && levelGen.GetManhattanDistance(addOnCell, levelGen._startRoom) < _distanceFrom._minDistanceFromStart
        || _distanceFrom._denyNearSelf && LowestDistanceFromSiblings(addOnCell) < _distanceFrom._minDistanceFromSelf 
        || levelGen.GetNeighbouringRooms(addOnCell).Count > _maxNeighbours );

        // A valid cell would've been found at this stage.
        _reservedCells.Add(addOnCell);

        addOnCell._setAsAddOn = true;
        addOnCell._setAsRoom = true;
    }

    #region Filter allowed rooms
    private bool IsOnAllowedFloor(GridCell cell)
    {
        // Loop through each allowed floor and check to see if the current cell is on it
        if (_allowedFloors.Contains(cell._positionInGrid.y)) return true;
        
        return false;
    }

    private bool IsOnLockedFloor(GridCell cell)
    {
        foreach (GridCell reservedCell in _reservedCells)
        {
            if (cell._positionInGrid.y == reservedCell._positionInGrid.y) return true;
        }
        
        return false;
    }

    private int LowestDistanceFromSiblings(GridCell checkingCell)
    {
        int lowestInt = int.MaxValue;
        foreach (GridCell cell in _reservedCells)
        {
            int distance = Level_Generator._instance.GetManhattanDistance(checkingCell, cell);
            if (distance < lowestInt) lowestInt = distance; 
        }

        return lowestInt;
    }
    #endregion
    
    private void GenerateRooms()
    {
        // Reserve all the slots
        for (int i = 0; i < _reservedCells.Count; i++)
        {
            GenerateRoom(_reservedCells[i]);
        }
    }

    private void GenerateRoom(GridCell thisCell)
    {    
        Level_Generator lvlGen = Level_Generator._instance;
        List<GridCell> neighbouringRooms = lvlGen.GetNeighbouringRooms(thisCell);

        // Check if connected to the end room
        if (neighbouringRooms.Contains(lvlGen._endRoom))
        {
            neighbouringRooms.Remove(lvlGen._endRoom);   
        }

        foreach (GridCell neighbourCell in neighbouringRooms)
        {
            Vector3Int positionDifference = thisCell._positionInGrid - neighbourCell._positionInGrid;
            lvlGen.EnableDoor(positionDifference, thisCell);
        }

        // Configure the needed door configuration for this cell
        List<Compass> directionsNeeded = lvlGen.GetDoorConfiguration(thisCell);
        if (directionsNeeded.Count == 0) return;

        // Find all rooms with the correct door configuration
        List<GameObject> totalRooms = new List<GameObject>();

        totalRooms = _roomPrefabs.FindAll(valid => valid.GetComponent<RoomData>().GetDoorConfiguration() == thisCell._neededDoorConfiguration);
        
        // Pick one of the rooms out of all the possible rooms.
        int randRoom = UnityEngine.Random.Range(0, totalRooms.Count);
        GameObject chosenRoom = totalRooms[randRoom];
        GameObject newRoom = Instantiate(chosenRoom, thisCell._position, Quaternion.identity, lvlGen._floorFolders[thisCell._positionInGrid.y].transform);

        RoomData chosenRoomData = newRoom.GetComponent<RoomData>();
        chosenRoomData._cellPosition = thisCell._positionInGrid;
        chosenRoomData.UpdateRoomDictionary();

        thisCell._roomData = chosenRoomData;
        thisCell._roomSetup = true;

        if (thisCell._verticalityType == Verticality.UP) chosenRoomData._setAsUp = true;
        if (thisCell._verticalityType == Verticality.DOWN) chosenRoomData._setAsDown = true;
        if (thisCell._verticalityType == Verticality.BOTH) {chosenRoomData._setAsUp = true; chosenRoomData._setAsDown = true;}

        lvlGen.CheckRoomRotation(directionsNeeded, thisCell, chosenRoomData);

        // Finalize the room
        _roomsMade++;
        lvlGen._addOnsGenerated++;

        lvlGen.FinalizeRoom(thisCell, newRoom, chosenRoomData);
    }

}
