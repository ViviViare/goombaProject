/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 05/02/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Level_Generator.cs
//
//  This script is what creates the level.
//  First the script creates a grid of X,Y,Z size.
//  Then it populates this grid with the start and end rooms, then using A* it finds the shortest path from these rooms.
//  Finally it extends the linear path with additional rooms
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level_Generator : MonoBehaviour
{

    #region Variables
    [Header("Rooms Generated")]
    [Tooltip("Total rooms generated in this level")]
    [ShowOnly] public int _roomsGenerated;
    [Tooltip("Every full irregular room set generated in this level")]
    [ShowOnly] public int _irregularRoomsGenerated;
    [Tooltip("Every add-on room generated in this level")]
    [ShowOnly] public int _addOnsGenerated;

    [Header("Grid Parameters")]
    [Tooltip("Max size for the 3D grid.\nIf Y = 0 then a 2D grid layout will be made instead")]
    [SerializeField] private Vector3Int _gridBounds = new Vector3Int(1, 1, 1);
    [SerializeField] private float _cellDistanceHorizontal, _cellDistanceVertical;
    [SerializeField] private float _cellDistanceConnected;
    [HideInInspector] public List<GameObject> _floorFolders = new List<GameObject>();

    [Header("End Room Distance From Start")]
    [SerializeField] private int _endMinDistance;
    [SerializeField] private int _endMaxDistance;

    [Header("Extension Parameters")]
    [Tooltip("Amount of mine clusters to generate")]
    [SerializeField] private int _amountOfMines;
    [Tooltip("The size of one mine cluster")]
    [SerializeField] private int _mineClusterMaxSize;
    [Tooltip("The amount the level layout will extent itself by compared to the base length (Linear Path)")]
    [SerializeField] [Range(0, 4)] private float _extensionRatio;

    [Header("Room Toggles")]
    [SerializeField] private Transform _roomParent;
    [Tooltip("If enabled then rooms close to the start room will be converted into laboratory rooms")]
    [SerializeField] private bool _useLaboratoryRooms;
    [Tooltip("If enabled them regular rooms can be converted into irregular rooms")]
    [SerializeField] private bool _useIrregularRooms;
    [Tooltip("Chance that a regular room will attempt to convert into an irregular room (1 / Denominator)")]
    [SerializeField] private int _irregularDenominator;
    [Tooltip("If enabled then every add on room script that is attatched to this script will spawn")]
    [SerializeField] private bool _useAddOns;
    
    [Header("Regular Room Prefabs")]
    [SerializeField] private List<GameObject> _spawnRoomPrefabs;
    [SerializeField] private List<GameObject>  _bossRoomPrefabs;
    [SerializeField] private List<GameObject> _regularRoomPrefabs;

    [Header("Irregular Room Prefabs")]
    [SerializeField] private List<GameObject> _deadEndRoomPrefabs;
    [SerializeField] private List<GameObject> _cornerRoomPrefabs;
    [SerializeField] private List<GameObject> _threeWayRoomPrefabs;

    [Header("Pathfinding")]
    [HideInInspector] public Dictionary<Vector3Int, GridCell> _cellDictionary = new Dictionary<Vector3Int, GridCell>();
    private List<GridCell> _openList;
    private List<GridCell> _closedList;
    private List<GridCell> _linearPath;
    public static List<GridCell> _totalPath;

    public GridCell _startRoom {get; private set;}
    public GridCell _endRoom {get; private set;}

    [Header("Pathfinding Costs")]
    private const int C_REGULAR_COST = 1;
    private const int C_MINE_COST = 9;
    private const int C_Y_COST = 90;
    
    [Space]
    [Header("Testing Parameters")]
    [SerializeField] private bool _showDebugObjects;
    public bool _showRoomConnections;
    [SerializeField] private LevelGenTestingParams _testingParams;
    private float _levelGenTimeElapsed;

    [Header("Referencing")]
    public static Level_Generator _instance;

    //[Header("Delegates and Events")]
    public static event Action _spawnPlayer;
    public static event Action _addOnReserveSlots;
    public static event Action _addOnGenerateReserves;


    #endregion

    private void Awake()
    {
        if (Level_Generator._instance != null) Destroy(this);
        else
        {
            _instance = this;
        }
    }

    // This may need to be replaced with an event call. Depends on how loading is handled
    private void Start()
    {
        Generate3DGrid();
    }

    // Setup the data for the inital level layout
    #region Setup Level Layout
    private void Generate3DGrid()
    {
        _levelGenTimeElapsed = Time.realtimeSinceStartup;
        for (int gridX = 0; gridX < _gridBounds.x; gridX++)
        {
            for (int gridY = 0; gridY < _gridBounds.y; gridY++)
            {
                for (int gridZ = 0; gridZ < _gridBounds.z; gridZ++)
                {
                    Vector3Int currentPosition = new Vector3Int(gridX, gridY, gridZ);

                    // Generates the room for this cell. DELETE LATER.
                    Vector3 cellPosition = new Vector3(gridX * _cellDistanceHorizontal, gridY * _cellDistanceVertical, gridZ * _cellDistanceHorizontal);

                    // Create a new cell at this position
                    GridCell newCell = new GridCell();
                    newCell._position = cellPosition;
                    newCell._positionInGrid = currentPosition;

                    newCell._gCost = int.MaxValue;
                    newCell.CalculateFCost();
                    newCell._cameFromCell = null;

                    _cellDictionary.Add(currentPosition, newCell);
                }
            }
        } // End of Grid Generation

        for (int i = 0; i < _gridBounds.y; i++)
        {
            GameObject floorFolder = new GameObject();
            floorFolder.transform.parent = _roomParent;

            _floorFolders.Add(floorFolder);
            floorFolder.name = "FLOOR " + _floorFolders.Count;
        }

        if (_showDebugObjects) SetUpGridDots();

        // Create start room then create end room
        if (CreateStartRoom() && CreateEndRoom()) CreateLayoutVariation();

    }

    private bool CreateStartRoom()
    {
        int lowerFloorMax = Mathf.FloorToInt(_gridBounds.y * 0.2f);
        int randLowestFloor = UnityEngine.Random.Range(0, lowerFloorMax);

        Vector3Int startRoomPos = new Vector3Int(RandomGridPosition(_gridBounds.x), randLowestFloor, RandomGridPosition(_gridBounds.z));
        _startRoom = _cellDictionary[startRoomPos];

        Vector3Int belowCell = startRoomPos + Vector3Int.down;
        Vector3Int aboveCell = startRoomPos + Vector3Int.up;

        CreateBarriers(startRoomPos);

        return true;
    }

    private int RandomGridPosition(int dimension)
    {
        return UnityEngine.Random.Range(0, dimension);
    }

    private bool CreateEndRoom()
    {
        int upperFloorMin = Mathf.FloorToInt(_gridBounds.y * 0.8f);
        int randHighestFloor = UnityEngine.Random.Range(upperFloorMin, _gridBounds.y);

        int randX = RandomGridPosition(_gridBounds.x);
        int randZ = RandomGridPosition(_gridBounds.z);
        Vector3Int suggestedEndRoom = new Vector3Int(randX, randHighestFloor, randZ);

        // The manhattan distance check needs to ignore the y value, so a smokescreen variable is made to compensate
        Vector3Int suggestedEndRoom2D = new Vector3Int(randX, _startRoom._positionInGrid.y, randZ);

        int distanceToStart = GetManhattanDistance(_startRoom, _cellDictionary[suggestedEndRoom2D]);
        int distanceClone = distanceToStart;

        // End room cannot be within X grid spaces of the X Z of the start room.
        // End room cannot be in a straight line to the start room

        while (distanceToStart <= _endMinDistance || distanceToStart > _endMaxDistance
        || randX == _startRoom._positionInGrid.x && randZ == _startRoom._positionInGrid.z )
        {
            randX = RandomGridPosition(_gridBounds.x);
            randZ = RandomGridPosition(_gridBounds.z);
            suggestedEndRoom = new Vector3Int(randX, randHighestFloor, randZ);
            suggestedEndRoom2D = new Vector3Int(randX, _startRoom._positionInGrid.y, randZ);

            distanceToStart = GetManhattanDistance(_startRoom, _cellDictionary[suggestedEndRoom2D]);
        }


        Debug.Log("End room distance from start: " + distanceClone + " ==> " + distanceToStart + " || Start room pos is: " + _startRoom._positionInGrid);

        _endRoom = _cellDictionary[suggestedEndRoom];
        CreateBarriers(suggestedEndRoom);

        return true;
    }

    #endregion

    // Add additional costs to random grid cells to create variation in the level layout
    #region Create variation in layout

    private void CreateLayoutVariation()
    {
        CreateMines();

        GenerateLevelLayout();
    }

    private void CreateBarriers(Vector3Int gridPosition)
    {
        // Check if there is a cell above the grid position
        if (_cellDictionary.ContainsKey(gridPosition + Vector3Int.up))
        {
            GridCell foundCell = _cellDictionary[gridPosition + Vector3Int.up];

            foundCell._cellStatus = GridCell.CellStatus.isBarrier;
            SpawnDebugObject(_testingParams._testBarrier, foundCell._position, gridPosition, " BARRIER ", _testingParams._mineHolder);
        }

        // Check if there is a cell below the grid position
        if (_cellDictionary.ContainsKey(gridPosition + Vector3Int.down))
        {
            GridCell foundCell = _cellDictionary[gridPosition + Vector3Int.down];

            foundCell._cellStatus = GridCell.CellStatus.isBarrier;
            SpawnDebugObject(_testingParams._testBarrier, foundCell._position, gridPosition, " BARRIER ", _testingParams._mineHolder);
        }
    }

    private void CreateMines()
    {
        List<GridCell> mineOrigins = new List<GridCell>();

        // Create the mine cluster origins
        for (int i = 0; i < _amountOfMines; i++)
        {
            Vector3Int randomPos = GetRandomGridPosition();
            GridCell foundCell = _cellDictionary[randomPos];

            bool mineCheck = CheckForMines(foundCell);

            if (mineCheck || foundCell._cellStatus != GridCell.CellStatus.isRegular || foundCell == _startRoom || foundCell == _endRoom)
            {
                // Continue the for loop until all the above are false
                i--;
                continue;
            }

            foundCell.SetStatus(GridCell.CellStatus.isMine);
            mineOrigins.Add(foundCell);
        }

        // Increase the size of the mine clusters
        for (int i = 0; i < mineOrigins.Count - 1; i++)
        {
            IncreaseMineCluster(mineOrigins[i], 0);
        }
    }

    public Vector3Int GetRandomGridPosition()
    {
        int randX = RandomGridPosition(_gridBounds.x);
        int randY = RandomGridPosition(_gridBounds.y);
        int randZ = RandomGridPosition(_gridBounds.z);

        Vector3Int randPos = new Vector3Int(randX, randY, randZ);
        return randPos;
    }

    private void LoopThroughMineNeighbours(GridCell mine, int clusterValue)
    {
        foreach (GridCell neighbour in GetCellNeighbours(mine))
        {
            if (neighbour._cellStatus != GridCell.CellStatus.isMine) continue;

            neighbour._clusterValue = clusterValue;
            LoopThroughMineNeighbours(neighbour, clusterValue);
        }
    }

    private void IncreaseMineCluster(GridCell currentMine, int index, List<GridCell> newClusterMines = null)
    {
        // Do not run code if the index parameter has hit the max cluster size.
        if (index > _mineClusterMaxSize) return;

        List<GridCell> validNeighbours = new List<GridCell>();
        List<GridCell> neighbouringMines = new List<GridCell>();

        List<GridCell> currentClusterMines = new List<GridCell>();;
        if (newClusterMines != null) currentClusterMines = newClusterMines;

        // Get all valid neighbours
        foreach (GridCell neighbourCell in GetCellNeighbours(currentMine))
        {
            // Check if the current cell's cluster value already meets the maximum cluster size. If so, exit this function.
            if (neighbourCell._clusterValue >= _mineClusterMaxSize) return;

            if (neighbourCell._cellStatus == GridCell.CellStatus.isMine) neighbouringMines.Add(neighbourCell);

            else validNeighbours.Add(neighbourCell);
        }

        // Do not continue to increase the cluster if the amount of valid neighbours is 0.
        if (validNeighbours.Count == 0) return;

        int randNeighbourValue = UnityEngine.Random.Range(0, validNeighbours.Count - 1);
        GridCell validCell = validNeighbours[randNeighbourValue];

        while (validCell == _startRoom || validCell == _endRoom)
        {
            randNeighbourValue = UnityEngine.Random.Range(0, validNeighbours.Count);
            validCell = validNeighbours[randNeighbourValue];
        }

        // Testing instantiation
        SpawnDebugObject(_testingParams._testMine, validCell._position, validCell._positionInGrid, " MINE ", _testingParams._mineHolder);

        validCell.SetStatus(GridCell.CellStatus.isMine);

        // Add current mine to the list of current cluster mines
        currentClusterMines.Add(validCell);

        foreach (GridCell mine in currentClusterMines)
        {
            mine._clusterValue = currentClusterMines.Count;
        }
        // Loop through all the neighbours of the valid cell and set their cluster value
        //LoopThroughMineNeighbours(validCell, currentClusterMines.Count);


        IncreaseMineCluster(validNeighbours[randNeighbourValue], currentClusterMines.Count, currentClusterMines);

    }

    private bool CheckForMines(GridCell markedCell)
    {
        foreach (GridCell neighbourCell in GetCellNeighbours(markedCell))
        {
            // If the checked neighbour cell doesn't equal a mine then continue, otherwise return as true
            if (neighbourCell._cellStatus != GridCell.CellStatus.isMine) continue;
            return true;
        }
        return false;
    }

    #endregion

    // Create the inital level layout & then extend it on each floor
    #region Generate layout

    private void GenerateLevelLayout()
    {
        // Create the linear path from the start room to the end room.
        CreateLinearPath();

        // Expand the linear path rooms to add more breadth to the level.
        ExtendLinearPath();

        // Configure rooms to be ladder rooms between floors.
        SetupLadderRooms();

        // Reserve any add-on rooms (if extant)
        if (_useAddOns) _addOnReserveSlots?.Invoke();

        // Generate a room on every room on the valid path.
        for (int i = 0; i < _totalPath.Count; i++)
        {
            if (_totalPath.Contains(_totalPath[i])) SetupCellAsRoom(_totalPath[i]);
        }

        // Generate add-on rooms (if extant)
        if (_useAddOns) _addOnGenerateReserves?.Invoke();
        
        Debug.Log("Finished spawning <color=#f5bd3b><b>rooms</b></color> after " + GetElapsedTime() + " seconds.");
        _spawnPlayer();
    }

    private void CreateLinearPath()
    {
        _linearPath = FindPath(_startRoom, _endRoom);
        _totalPath = _linearPath;

        if (_linearPath == null) return;

        for (int i = 0; i < _linearPath.Count; i++)
        {
            // Set this room as a valid room

            _linearPath[i]._setAsRoom = true;

            // Spawn debug objects for the linear path
            if (_linearPath[i] == _startRoom)
            {
                SpawnDebugObject(_testingParams._testRoom,_linearPath[i]._position, _linearPath[i]._positionInGrid, " START ", _testingParams._roomHolder);
            }
            else if (_linearPath[i] == _endRoom)
            {
                SpawnDebugObject(_testingParams._testRoom,_linearPath[i]._position, _linearPath[i]._positionInGrid, " END ", _testingParams._roomHolder);
            }
            else
            {
                SpawnDebugObject(_testingParams._testRoom,_linearPath[i]._position, _linearPath[i]._positionInGrid, " ", _testingParams._roomHolder);
            }
        }
    }

    private void ExtendLinearPath()
    {
        int totalExtension = Mathf.CeilToInt(_linearPath.Count * _extensionRatio);

        // Amount of extensions per floor

        for (int i = 0; i < _endRoom._positionInGrid.y + 1; i++)
        {
            ExtendOnThisFloor(i, totalExtension);
        }

        Debug.Log("Finished generating <color=#ffffff><b>level</b></color> layout after " + GetElapsedTime() + " seconds.");        
    }

    private void ExtendOnThisFloor(int floor, int amountOfTimes)
    {
        for (int cell = 0; cell < amountOfTimes; cell++)
        {
            List<GridCell> thisFloor = _totalPath.FindAll(valid => valid._positionInGrid.y == floor);
            if (thisFloor.Count == 0) return;

            // Room the end room if it is in current list of floors.
            // This means that a room cannot be next to the end room unless it is also next to another room
            if (thisFloor.Contains(_endRoom)) thisFloor.Remove(_endRoom);

            int rand = UnityEngine.Random.Range(0, thisFloor.Count);
            GridCell cellExtendingFrom = thisFloor[rand]; // NOTE: ERROR HAPPENS HERE. "ArgumentOutOfRangeException: Index was out of range"

            List<GridCell> cellNeighbours = GetCellNeighbours2D(cellExtendingFrom).FindAll(valid => !valid._setAsRoom);

            if (cellNeighbours.Count == 0) continue;

            int randNeighbour = UnityEngine.Random.Range(0, cellNeighbours.Count);
            GridCell newExtension = cellNeighbours[randNeighbour];

            SpawnDebugObject(_testingParams._testExtensionRoom, newExtension._position, newExtension._positionInGrid, " EXTEND ", _testingParams._roomHolder);

            newExtension._setAsRoom = true;

            _totalPath.Add(newExtension);

        }
    }

    private void SetupLadderRooms()
    {
        // Early exit if the level layout is 2D
        if (_endRoom._positionInGrid.y == 0) return;

        // Loop through all the Y values from 0 to the highest point (where the end room is)
        for (int i = 0; i < _endRoom._positionInGrid.y ; i++)
        {
            // Find all the rooms on this floor that are not a barrier or start or end room
            List<GridCell> thisFloorsRooms = _totalPath.FindAll(match => match._positionInGrid.y == i
            && match._cellStatus != GridCell.CellStatus.isBarrier);
            List<GridCell> validFloorRooms = new List<GridCell>();

            foreach (GridCell cell in thisFloorsRooms)
            {
                // A room that is set to go down cannot be added as one to go up, unless there is only 1 room on this floor (Edge case, shouldn't happen)
                if (cell._verticalityType == Verticality.DOWN && thisFloorsRooms.Count > 1 || cell == _startRoom || cell == _endRoom) continue;

                Vector3Int aboveCell = cell._positionInGrid + Vector3Int.up;

                if (_cellDictionary[aboveCell]._setAsRoom) validFloorRooms.Add(cell);
            }

            // Choose one of the possible cells which has an above room
            int randCell = UnityEngine.Random.Range(0, validFloorRooms.Count);

            // ArgumentOutOfRangeException: Index was out of range. Must be non-negative and les sthan the size of the collecton.
            // Parameter name: index
            // NOTE: This does not work ((SOMETIMES)), above is the error message
            // I think this doesn't work only when the level generation causes the linear rooms to go up more than twice in a row.
            // This theory has been proven to be false.

            if (validFloorRooms.Count == 0)
            {
                // Temporary fix until the problem has been addressed.
                Debug.LogError("<b>VALID FLOOR ROOMS ON FLOOR: " + i + " IS 0.\n\tABORTING PLAY TEST.</b>");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
            GridCell validCell = validFloorRooms[randCell];

            Vector3Int aboveCellKey = validCell._positionInGrid + Vector3Int.up;
            GridCell validAboveCell = _cellDictionary[aboveCellKey];

            // Set this cell as needing to be an above room (Or a both if it was already sety as a down room)
            if (validCell._verticalityType == Verticality.DOWN && validFloorRooms.Count == 1) validCell._verticalityType = Verticality.BOTH;
            else validCell._verticalityType = Verticality.UP;

            // Set the neighbour above as needing to be a down room
            validAboveCell._verticalityType = Verticality.DOWN;
        }

    }

    private void ImplementAddOnReserves(List<GridCell> reservedCells)
    {
        foreach (GridCell newCell in reservedCells)
        {
            _totalPath.Add(newCell);
        }
    }

    #endregion

    // Generate a room that matches the requirements by the level layout data
    #region Create regular room

    // Create a regular room on this cell
    private void SetupCellAsRoom(GridCell thisCell)
    {
        // Do not set up this cell as a room if it is not meant to be set as a room.
        if (!thisCell._setAsRoom || thisCell._setAsIrregular) return;
        _roomsGenerated++;
        List<GridCell> neighbouringRooms = GetNeighbouringRooms(thisCell);

        // Check if connected to the end room
        if (neighbouringRooms.Contains(_endRoom))
        {
            // If the end room has already got 1 connection, then remove it from being added to any other connection
            // This forces the boss room to only be able to have 1 door, and be accessed from 1 direction

            // End room has not been set yet.
            if (!_endRoom._endRoomConnection)
            {
                _endRoom._endRoomConnection = true;
                thisCell._endRoomConnection = true;
            }
            else
            {
                neighbouringRooms.Remove(_endRoom);
            }
        }

        // Enable doors for this room to all its neighbouring rooms
        if (thisCell == _endRoom && !thisCell._endRoomConnection)
        {
            // Only one door needs to be enabled if this is the end room
            int rand = UnityEngine.Random.Range(0, neighbouringRooms.Count);

            GridCell connection = neighbouringRooms[rand];
            connection._endRoomConnection = true;
            thisCell._endRoomConnection = true;

            // Difference of the neighbour cell to this cell
            Vector3Int positionDifference = thisCell._positionInGrid - connection._positionInGrid;
            EnableDoor(positionDifference, thisCell);

        }

        foreach (GridCell neighbourCell in neighbouringRooms)
        {
            // If this room is the end room and has been set as having a connection
            // Then skip all neighbour cells that do not also have that connection
            if (thisCell == _endRoom && !neighbourCell._endRoomConnection) continue;

            Vector3Int positionDifference = thisCell._positionInGrid - neighbourCell._positionInGrid;
            EnableDoor(positionDifference, thisCell);
        }

        // Configure the needed door configuration for this cell
        List<Compass> directionsNeeded = GetDoorConfiguration(thisCell);
        if (directionsNeeded.Count == 0) return;

        // Find all rooms with the correct door configuration
        List<GameObject> totalRooms = new List<GameObject>();

        if (thisCell == _endRoom)
        {
            totalRooms = _bossRoomPrefabs.FindAll(valid => valid.GetComponent<RoomData>().GetDoorConfiguration() == thisCell._neededDoorConfiguration);
        }
        else if (thisCell == _startRoom)
        {
            totalRooms = _spawnRoomPrefabs.FindAll(valid => valid.GetComponent<RoomData>().GetDoorConfiguration() == thisCell._neededDoorConfiguration);
        }
        else
        {
            totalRooms = _regularRoomPrefabs.FindAll(valid => valid.GetComponent<RoomData>().GetDoorConfiguration() == thisCell._neededDoorConfiguration
            && valid.GetComponent<RoomData>()._verticality == thisCell._verticalityType);
        }

        List<GameObject> filteredRooms = new List<GameObject>();

        // Filter room list by laboratory rooms here
        if (_useLaboratoryRooms && GetManhattanDistance(thisCell, _startRoom) <= _linearPath.Count * 0.2f && thisCell._positionInGrid.y == _startRoom._positionInGrid.y)
        {
            filteredRooms = totalRooms.FindAll(valid => valid.GetComponent<RoomData>()._isLaboratory);

        }
        else
        {
            filteredRooms = totalRooms;
        }

        // Pick one of the rooms out of all the possible rooms.
        int randRoom = UnityEngine.Random.Range(0, filteredRooms.Count);
        GameObject chosenRoom = filteredRooms[randRoom];
        GameObject newRoom = Instantiate(chosenRoom, thisCell._position, Quaternion.identity, _floorFolders[thisCell._positionInGrid.y].transform);

        RoomData chosenRoomData = newRoom.GetComponent<RoomData>();
        chosenRoomData._cellPosition = thisCell._positionInGrid;
        chosenRoomData.UpdateRoomDictionary();

        thisCell._roomData = chosenRoomData;
        thisCell._roomSetup = true;

        if (thisCell._verticalityType == Verticality.UP) chosenRoomData._setAsUp = true;
        if (thisCell._verticalityType == Verticality.DOWN) chosenRoomData._setAsDown = true;
        if (thisCell._verticalityType == Verticality.BOTH) {chosenRoomData._setAsUp = true; chosenRoomData._setAsDown = true;}

        CheckRoomRotation(directionsNeeded, thisCell, chosenRoomData);

        // Check to see if this room can become an irregular room
        if (_useIrregularRooms && _irregularDenominator >= 1 && thisCell != _startRoom && thisCell != _endRoom)
        {
            int chanceOfBeingIrregular = UnityEngine.Random.Range(0, _irregularDenominator);

            // The chance of this room becoming an irregular room
            if (chanceOfBeingIrregular == 0)
            {
                if (!ConvertRoomToDouble(thisCell))
                {
                    Debug.Log("Failed to create a double at: " + thisCell._positionInGrid);
                }

            }
        }

        // If the room does not become an irregular
        // Finalize the room
        FinalizeRoom(thisCell, newRoom, chosenRoomData);
    }

    private void EnableDoorsOnAll(List<GridCell> neighbouringRooms, GridCell thisCell)
    {
        thisCell._requiredDoorsSet = true;

        foreach (GridCell neighbourCell in neighbouringRooms)
        {
            // If this room is the end room and has been set as having a connection
            // Then skip all neighbour cells that do not also have that connection
            if (thisCell == _endRoom && !neighbourCell._endRoomConnection) continue;

            // Difference of the neighbour cell to this cell
            Vector3Int positionDifference = thisCell._positionInGrid - neighbourCell._positionInGrid;

            EnableDoor(positionDifference, thisCell);
        }
    }

    public void EnableDoor(Vector3Int positionDifference, GridCell currentCell)
    {
        currentCell._requiredDoorsSet = true;
        // Neighbour cell is left of the current
        if (positionDifference == Vector3.left) currentCell._requiredDoors._hasEastDoor = true;

        // Neighbour cell is right of the current
        else if (positionDifference == Vector3.right) currentCell._requiredDoors._hasWestDoor = true;

        // Neighbour cell is in front of the current
        else if (positionDifference == Vector3.forward) currentCell._requiredDoors._hasSouthDoor = true;

        // Neighbour cell is behind of the current
        else if (positionDifference == Vector3.back) currentCell._requiredDoors._hasNorthDoor = true;
    }

    // Set the door configuration of the cell and return a list of compasses that this cell requires
    public List<Compass> GetDoorConfiguration(GridCell thisCell, bool getIrregular = false)
    {
        List<Compass> directionsNeeded = new List<Compass>();
        List<Compass> irregularsNeeded = new List<Compass>();

        // Check the required doors and the irregular doors.
        if (thisCell._requiredDoors._hasNorthDoor) directionsNeeded.Add(Compass.North);
        if (thisCell._requiredDoors._hasEastDoor) directionsNeeded.Add(Compass.East);
        if (thisCell._requiredDoors._hasSouthDoor) directionsNeeded.Add(Compass.South);
        if (thisCell._requiredDoors._hasWestDoor) directionsNeeded.Add(Compass.West);

        // if getIrregular is enabled then add irregular doors to the irregularNeeded list
        if (getIrregular)
        {
            if (thisCell._requiredIrregularDoors._hasNorthDoor) irregularsNeeded.Add(Compass.North);
            if (thisCell._requiredIrregularDoors._hasEastDoor) irregularsNeeded.Add(Compass.East);
            if (thisCell._requiredIrregularDoors._hasSouthDoor) irregularsNeeded.Add(Compass.South);
            if (thisCell._requiredIrregularDoors._hasWestDoor) irregularsNeeded.Add(Compass.West);
        }
        // if Get Irregular is set to true, then it means this cell has already been setup for door configuration and thus does not need to run again
        else if (directionsNeeded.Count == 0 ) // This should not be possible
        {
            Debug.LogError(thisCell._positionInGrid + " HAS NO NEIGHBOURS. || Is room Irregular? = " + thisCell._setAsIrregular);
        }
        else if (directionsNeeded.Count == 1) thisCell._neededDoorConfiguration = DoorConfiguration.DeadEnd;
        else if (directionsNeeded.Count == 2) // thisCell._neededDoorConfiguration = Straight or Corner
        {
            // Check if North & South or East & West are enabled
            if (thisCell._requiredDoors._hasNorthDoor && thisCell._requiredDoors._hasSouthDoor
            || thisCell._requiredDoors._hasEastDoor && thisCell._requiredDoors._hasWestDoor)
            {
                thisCell._neededDoorConfiguration = DoorConfiguration.Straight;
            }
            else
            {
                thisCell._neededDoorConfiguration = DoorConfiguration.Corner;
            }
        }
        else if (directionsNeeded.Count == 3) thisCell._neededDoorConfiguration = DoorConfiguration.ThreeWay;
        else if (directionsNeeded.Count == 4) thisCell._neededDoorConfiguration = DoorConfiguration.FourWay;

        // Return the irregulars if getIrregular is enabled
        if (getIrregular) return irregularsNeeded;

        return directionsNeeded;
    }

    public void CheckRoomRotation(List<Compass> directionsNeeded, GridCell thisCell, RoomData roomData)
    {
        List<Compass> directionsRoomHas = new List<Compass>();
        List<Door> validDoors;

        IrregularRoomData irregularData = null;

        // Check to see if an irregular room is being used
        if (roomData is IrregularRoomData)
        {
            irregularData = (IrregularRoomData)roomData;
            validDoors = irregularData.GetValidIrregularDoors();
        }
        else
        {
            validDoors = roomData.GetValidDoors();
        }

        foreach (Door door in validDoors)
        {
            directionsRoomHas.Add(door._direction);
        }

        // Adjust room so that it is correctly rotated
        if (directionsNeeded != directionsRoomHas)
        {
            if (roomData is IrregularRoomData)
            {
                UpdateRoomRotation(thisCell, irregularData);
            }
            else
            {
                UpdateRoomRotation(thisCell, roomData);
            }

        }
    }

    private void UpdateRoomRotation(GridCell currentCell, RoomData chosenData)
    {
        // Recursion Handling
        int attempts = 0;

        bool doDoorsLineUp = RotateAndCheckDoors(currentCell, chosenData);

        // Do not remove this comment. It is a life saver. - Levi
        /*Debug.Log("Cell: " + currentCell._positionInGrid +  " should be: " + currentCell._requiredDoors._hasNorthDoor
    + " " + currentCell._requiredDoors._hasEastDoor
    + " " +  currentCell._requiredDoors._hasSouthDoor
    + " " + currentCell._requiredDoors._hasWestDoor);*/

        // Rotate the room if the doors do not line up
        while (!doDoorsLineUp )
        {
            doDoorsLineUp = RotateAndCheckDoors(currentCell, chosenData);

            // Recursion handler
            attempts++;
            if (attempts >= 10)
            {
                Debug.LogError("Infinite loop checking: " + currentCell._positionInGrid + " & " + chosenData.gameObject.name);
                break;
            }
        }
    }

    private bool RotateAndCheckDoors(GridCell cell, RoomData roomData)
    {
        List<Door> validDoors;

        // Check to see if an irregular room is being rotated or not
        if (roomData is IrregularRoomData)
        {
            IrregularRoomData irregularData = (IrregularRoomData)roomData;

            validDoors = irregularData.GetValidIrregularDoors();
            irregularData.RotateDoorsClockwise();
        }
        else
        {
            validDoors = roomData.GetValidDoors();

            roomData.RotateDoorsClockwise();
        }

        // Temporary bools for each direction
        bool northFound = false;
        bool eastFound = false;
        bool southFound = false;
        bool westFound = false;

        for (int i = 0; i < validDoors.Count; i++)
        {
            Door currentDoor = validDoors[i];

            if (currentDoor._direction == Compass.North) northFound = true;
            else if (currentDoor._direction == Compass.East) eastFound = true;
            else if (currentDoor._direction == Compass.South) southFound = true;
            else if (currentDoor._direction == Compass.West) westFound = true;
        }

        if (roomData is IrregularRoomData)
        {
            // Compare temporary bools to the required irregular door bools
            if (northFound == cell._requiredIrregularDoors._hasNorthDoor
            &&  eastFound  == cell._requiredIrregularDoors._hasEastDoor
            &&  southFound == cell._requiredIrregularDoors._hasSouthDoor
            &&  westFound  == cell._requiredIrregularDoors._hasWestDoor)
            return true;
        }
        else
        {
            // Compare temporary bools to the required door bools
            if (northFound == cell._requiredDoors._hasNorthDoor
            &&  eastFound  == cell._requiredDoors._hasEastDoor
            &&  southFound == cell._requiredDoors._hasSouthDoor
            &&  westFound  == cell._requiredDoors._hasWestDoor)
            return true;
        }

        // If the above statement has 1 or more false, then return false
        return false;
    }
    #endregion

    // Generate a regular room into an irregular room. Starting by converting it into a double. Regular -> Double -> L -> T
    #region Irregular room setup
    private bool ConvertRoomToDouble(GridCell thisCell)
    {
        // Find all neighbours which are not start or end rooms
        List<GridCell> floorNeighbours = GetCellNeighbours2D(thisCell).FindAll(valid => valid != _endRoom && valid != _startRoom 
        && !valid._setAsIrregular && !valid._setAsAddOn);

        // If this room has no neighbours to convert then it cannot be an irregular room
        // Send back to SetupCellAsRoom to revert back to its regular room status
        if (floorNeighbours.Count < 1) return false;

        int randNeighbour = UnityEngine.Random.Range(0, floorNeighbours.Count);
        GridCell partnerCell = floorNeighbours[randNeighbour];

        // Disable required door towards partner (and vice versa)
        Vector3Int thisPosDifference = thisCell._positionInGrid - partnerCell._positionInGrid;
        Vector3Int partnerPosDifference = partnerCell._positionInGrid - thisCell._positionInGrid;

        EnableIrregularDoors(thisPosDifference, thisCell);
        EnableIrregularDoors(partnerPosDifference, partnerCell);

        // Get all dead end rooms which fit the door configuration of this room
        // Unlike with regular rooms, Irregular rooms *have* to have all possible doors.
        // So checking for door configuration is unnecessary, as they will all have the correct door configuration.
        List<GameObject> validRoomsForThis = _deadEndRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == thisCell._verticalityType);
        List<GameObject> validRoomsForPartner = _deadEndRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == partnerCell._verticalityType);

        // If there are no valid dead end rooms for either grid cell, then this cannot be an irregular room.
        if (validRoomsForPartner.Count == 0 || validRoomsForThis.Count == 0)
        {
            DisableIrregularDoors(thisCell);
            DisableIrregularDoors(partnerCell);
            return false;
        }

        // If this point is reached, then both cells can be irregular rooms of type: Double

        int randThisRoom = UnityEngine.Random.Range(0, validRoomsForThis.Count);
        int randPartnerRoom = UnityEngine.Random.Range(0, validRoomsForPartner.Count);

        GameObject chosenRoomThis = validRoomsForThis[randThisRoom];
        GameObject chosenRoomPartner = validRoomsForPartner[randPartnerRoom];

        UpdateIrregularRoomPosition(thisCell, partnerCell);
        SetupIrregularDuo(thisCell, chosenRoomThis, partnerCell, chosenRoomPartner);

        // Check to see if this room should become a L room
        int chanceOfBeingIrregular = UnityEngine.Random.Range(0, Mathf.CeilToInt(_irregularDenominator / 2 ));

        // The chance of this room becoming an irregular room
        if (chanceOfBeingIrregular == 0)
        {
            if (!ConvertRoomTo_L((IrregularRoomData)thisCell._roomData))
            {
                Debug.Log("Failed to create an L at: " + thisCell._positionInGrid);
            }

        }

        return true;
    }
    
    private void SetupIrregularDuo(GridCell originCell, GameObject originGo, GridCell newCell, GameObject newGo, int irregularType = 0)
    {
        originCell._setAsIrregular = true;
        newCell._setAsIrregular = true;
        originCell._setAsRoom = true;
        newCell._setAsRoom = true;

        // Clone the previous room data of origin if it was already an irregular
        IrregularRoomData originDataClone = new IrregularRoomData();

        if (originCell._roomData is IrregularRoomData) originDataClone = (IrregularRoomData)originCell._roomData;
        
        // Delete the previous GameObject that this room was
        GameObject oldGameObject = originCell._roomData.gameObject;
        Destroy(oldGameObject);
        originCell._roomData = null;

        // If the partner cell was already set up as a room, delete it so it can be converted
        if (newCell._roomSetup)
        {
            GameObject oldGameobject = newCell._roomData.gameObject;
            Destroy(oldGameobject);
            newCell._roomData = null;
        }

        // Origin (this) room
        GameObject originRoomGo = Instantiate(originGo, originCell._position, Quaternion.identity, _floorFolders[originCell._positionInGrid.y].transform);
        IrregularRoomData originRoomData = originRoomGo.GetComponent<IrregularRoomData>();
        originRoomData._cellPosition = originCell._positionInGrid;
        originCell._roomData = originRoomData;

        // Update this cell's room data
        originRoomData.UpdateIrregularDictionary();
        originRoomData.UpdateRoomDoorDictionary();

        List<Compass> originDoorsNeeded = GetDoorConfiguration(originCell);
        List<Compass> originConnectionsNeeded = GetDoorConfiguration(originCell, true);

        // Partner Room
        GameObject newRoomGo = Instantiate(newGo, newCell._position, Quaternion.identity, _floorFolders[newCell._positionInGrid.y].transform);
        IrregularRoomData newRoomData = newRoomGo.GetComponent<IrregularRoomData>();
        newCell._roomData = newRoomData;
        newRoomData._cellPosition = newCell._positionInGrid;

        // Connect all irregular cells together
        GridCell irregularSource = null;
        IrregularRoomData irregularCache = null;
        if (irregularType != 0)
        {
            if (originCell == originDataClone._connectedCells._originCell)
            {
                irregularSource = originDataClone._connectedCells._doubleCell;
            }
            else
            {
                irregularSource = originDataClone._connectedCells._originCell;
            }

            irregularCache = (IrregularRoomData)irregularSource._roomData;
        }  

        switch (irregularType)
        {
            // First irregular type (Origin and Double)
            case 0:
                // Add both the origin cell (this) and the new double cell to the origin cell's connections
                originRoomData._connectedCells._originCell = originCell;
                originRoomData._connectedCells._doubleCell = newCell;

                // Update the double cell to have the same data as the origin cell
                newRoomData._connectedCells = originRoomData._connectedCells;
                break;
            // Second irregular type (Double and L)
            case 1:
                // Add the L Cell to the cache of the irregular cells.
                irregularCache._connectedCells._LCell = newCell;
               
                newRoomData._connectedCells = irregularCache._connectedCells;
                originRoomData._connectedCells = irregularCache._connectedCells;
                break;
            // Third irregular type (Double and T)
            case 2:
                irregularCache._connectedCells._TCell = newCell;

                newRoomData._connectedCells = irregularCache._connectedCells;
                originRoomData._connectedCells = irregularCache._connectedCells;

                break;         
        }

        if (!newCell._requiredDoorsSet)
        {
            List<GridCell> roomsToCheck = GetNeighbouringRooms(newCell).FindAll(valid => !valid._endRoomConnection);
            EnableDoorsOnAll(roomsToCheck, newCell);
        }

        // Handle this cells verticality
        if (originCell._verticalityType == Verticality.UP) originRoomData._setAsUp = true;
        else if (originCell._verticalityType == Verticality.DOWN) originRoomData._setAsDown = true;
        else if (originCell._verticalityType == Verticality.BOTH) {originRoomData._setAsUp = true; originRoomData._setAsDown = true;}

        // Handle the partner cells verticality
        if (newCell._verticalityType == Verticality.UP) newRoomData._setAsUp = true;
        else if (newCell._verticalityType == Verticality.DOWN) newRoomData._setAsDown = true;
        else if (newCell._verticalityType == Verticality.BOTH) {newRoomData._setAsUp = true; newRoomData._setAsDown = true;}
    
        // Update the partner's room data
        newRoomData.UpdateIrregularDictionary();
        newRoomData.UpdateRoomDoorDictionary();

        List<Compass> partnerDoorsNeeded = GetDoorConfiguration(newCell);
        List<Compass> partnerConnectionsNeeded = GetDoorConfiguration(newCell, true);

        // Adjust rotations on both this room and its partner
        CheckRoomRotation(originConnectionsNeeded, originCell, originRoomData);
        CheckRoomRotation(partnerConnectionsNeeded, newCell, newRoomData);

        // Disable unused doors
        //originCell._roomData.DisableUnusedDoors(originCell, GetCellNeighbours2D(originCell));
        //newCell._roomData.DisableUnusedDoors(newCell, GetCellNeighbours2D(newCell));

        FinalizeRoom(originCell, originRoomGo, originRoomData);
        FinalizeRoom(newCell, newRoomGo, newRoomData);
    }

    private void UpdateIrregularRoomPosition(GridCell referencePoint, GridCell toChange)
    {
        Vector3Int thisPosDifference = referencePoint._positionInGrid - toChange._positionInGrid;
        
        // Neighbour cell is left of the current
        if (thisPosDifference == Vector3.left)
        {
            // Update the position for the partner room based on its position from the origin cell
            // Partner cells should be positioned with the origin cell as a base.
            toChange._position = new Vector3(referencePoint._position.x + _cellDistanceConnected, referencePoint._position.y, referencePoint._position.z);
        }

        // Neighbour cell is right of the current
        else if (thisPosDifference == Vector3.right)
        {
            toChange._position = new Vector3(referencePoint._position.x - _cellDistanceConnected, referencePoint._position.y, referencePoint._position.z);
        }

        // Neighbour cell is in front of the current
        else if (thisPosDifference == Vector3.forward)
        {
            toChange._position = new Vector3(referencePoint._position.x , referencePoint._position.y, referencePoint._position.z - _cellDistanceConnected);
        }

        // Neighbour cell is behind of the current
        else if (thisPosDifference == Vector3.back)
        {
            toChange._position = new Vector3(referencePoint._position.x, referencePoint._position.y, referencePoint._position.z + _cellDistanceConnected);
        }

    }
    
    private bool ConvertRoomTo_L(IrregularRoomData originData)
    {
        GridCell originCell = originData._connectedCells._originCell;
        GridCell doubleCell = originData._connectedCells._doubleCell;

        // Find valid room to become the L room
        List<GridCell> originNeighbours = GetCellNeighbours2D(originCell);
        List<GridCell> doubleNeighbours = GetCellNeighbours2D(doubleCell);

        List<GridCell> allValidNeighbours = originNeighbours.Union(doubleNeighbours).ToList();

        // Remove cells which cannot become irregular.
        // Remove valid cells that would make this conversion into a three long room.
        foreach (GridCell cell in allValidNeighbours.ToList())
        {
            // Remove start & end rooms from valid neighbours.
            // Remove already existing irregular rooms as valid neighbours
            if (cell == _endRoom || cell == _startRoom || cell._setAsIrregular || cell._setAsAddOn) allValidNeighbours.Remove(cell);

            Vector3Int cellPosition = cell._positionInGrid;

            // Current irregular room setup is vertical
            if (originData._connectedCells._originCell._positionInGrid.x == originData._connectedCells._doubleCell._positionInGrid.x)
            {
                if (cellPosition.x == originData._connectedCells._originCell._positionInGrid.x)
                {
                    allValidNeighbours.Remove(cell);
                }
            }
            // Current irregular room setup is horizontal
            else
            {
                if (cellPosition.z == originData._connectedCells._originCell._positionInGrid.z)
                {
                    allValidNeighbours.Remove(cell);
                }
            }
        }

        // If there are no valid neighbours then backout of this conversion
        if (allValidNeighbours.Count == 0) return false;

        // Get UnityEngine.Random L room
        int rand = UnityEngine.Random.Range(0, allValidNeighbours.Count);
        GridCell LCell = allValidNeighbours[rand];

        // Check
        List<GridCell> neighbourRoomsToL = GetNeighbouringRooms(LCell);

        GridCell existingConnection = originCell;
        if (neighbourRoomsToL.Contains(doubleCell))
        {
            existingConnection = doubleCell;
        }

        Vector3Int existingToLCell = existingConnection._positionInGrid - LCell._positionInGrid;
        Vector3Int LCellToExisting = LCell._positionInGrid - existingConnection._positionInGrid;

        EnableIrregularDoors(existingToLCell, existingConnection);
        EnableIrregularDoors(LCellToExisting, LCell);

        // Filter valid corner rooms
        List<GameObject> validCornerRooms = _cornerRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == existingConnection._verticalityType);

        // Filter valid L rooms
        List<GameObject> validLRooms = _deadEndRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == LCell._verticalityType);

        // Check if there are any valid rooms 
        if (validCornerRooms.Count == 0 || validLRooms.Count == 0)
        {
            DisableIrregularDoors(existingConnection);
            DisableIrregularDoors(LCell);
            return false;
        }

        int randCorner = UnityEngine.Random.Range(0, validCornerRooms.Count);
        int randL = UnityEngine.Random.Range(0, validLRooms.Count);

        GameObject chosenCorner = validCornerRooms[randCorner];
        GameObject chosenL = validLRooms[randL];

        // Reposition the new irregular room part (L)
        UpdateIrregularRoomPosition(existingConnection, LCell);
        SetupIrregularDuo(existingConnection, chosenCorner, LCell, chosenL, 1);
        
        // Check to see if this room should become a T room
        int chanceOfBecomingT = UnityEngine.Random.Range(0, Mathf.CeilToInt(_irregularDenominator / 2 ));

        // The chance of this room becoming an irregular room
        if (chanceOfBecomingT == 0)
        {
            if (!ConvertRoomTo_T(originData, existingConnection, existingToLCell))
            {
                Debug.Log("Failed to create a T at: " + originCell._positionInGrid);
            }
        }

        return true;
    }

    private bool ConvertRoomTo_T(IrregularRoomData originData, GridCell cornerCell, Vector3Int cornerPositionDifference)
    {
        // Grab the room parrallel to the L room
        Vector3Int TCellGridPosition = cornerCell._positionInGrid + cornerPositionDifference;
        GridCell TCell = GetGridCell(TCellGridPosition.x, TCellGridPosition.y, TCellGridPosition.z);

        if (TCell == null || TCell == _endRoom || TCell == _startRoom || TCell._setAsIrregular || TCell._setAsAddOn)
        {
            return false;
        }

        Vector3Int cornerToT = cornerCell._positionInGrid - TCell._positionInGrid;
        Vector3Int tToCorner = TCell._positionInGrid - cornerCell._positionInGrid;

        EnableIrregularDoors(cornerToT, cornerCell);
        EnableIrregularDoors(tToCorner, TCell);

        // Filter valid Three way rooms
        List<GameObject> validThreeWayRooms = _threeWayRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == cornerCell._verticalityType);

        // Filter valid T rooms
        List<GameObject> validTRooms = _deadEndRoomPrefabs.FindAll(valid => valid.GetComponent<IrregularRoomData>()._verticality == TCell._verticalityType);

        // Check if there are any valid rooms 
        if (validThreeWayRooms.Count == 0 || validTRooms.Count == 0)
        {
            DisableIrregularDoors(cornerCell);
            DisableIrregularDoors(TCell);
            return false;
        }

        int randThreeWay = UnityEngine.Random.Range(0, validThreeWayRooms.Count);
        int randT = UnityEngine.Random.Range(0, validTRooms.Count);

        GameObject chosenThreeWay = validThreeWayRooms[randThreeWay];
        GameObject chosenT = validTRooms[randT];

        // Reposition the new irregular room part (L)
        UpdateIrregularRoomPosition(cornerCell, TCell);
        SetupIrregularDuo(cornerCell, chosenThreeWay, TCell, chosenT, 2);
        
        return true;
    }

    private void EnableIrregularDoors(Vector3Int positionDifference, GridCell currentCell)
    {
        // Neighbour cell is left of the current
        if (positionDifference == Vector3.left) currentCell._requiredIrregularDoors._hasEastDoor = true;

        // Neighbour cell is right of the current
        else if (positionDifference == Vector3.right) currentCell._requiredIrregularDoors._hasWestDoor = true;

        // Neighbour cell is in front of the current
        else if (positionDifference == Vector3.forward) currentCell._requiredIrregularDoors._hasSouthDoor = true;

        // Neighbour cell is behind of the current
        else if (positionDifference == Vector3.back) currentCell._requiredIrregularDoors._hasNorthDoor = true;
    }

    private void DisableIrregularDoors(GridCell currentCell)
    {
        currentCell._requiredIrregularDoors._hasNorthDoor = false;
        currentCell._requiredIrregularDoors._hasEastDoor = false;
        currentCell._requiredIrregularDoors._hasSouthDoor = false;
        currentCell._requiredIrregularDoors._hasWestDoor = false;
    }

    #endregion

    private GridCell _lastCheckedCell;
    public void FinalizeRoom(GridCell thisCell, GameObject roomGO, RoomData roomData)
    {
        // Correct this rooms rotation
        roomGO.transform.rotation = Quaternion.Euler(0, roomData._roomRotation, 0);

        // Correct this rooms name
        string roomGenerated = "(" + (_roomsGenerated + _addOnsGenerated) + "/" + _totalPath.Count + ") ";
        string roomInfo =  thisCell._positionInGrid + " " + roomGO.name.Replace("(Clone)", "");

        switch (roomData is IrregularRoomData)
        {
            case false:
                if (thisCell._setAsAddOn)
                {
                    roomGO.name = roomGenerated + "[AO-" + _addOnsGenerated + "] " + roomInfo;
                }
                else
                {
                    roomGO.name = roomGenerated + roomInfo;
                }
                break;
                
            case true:
                // Only increment the irregular rooms generated per full irregular room, not per part.
                IrregularRoomData convertedData = (IrregularRoomData)roomData;
                if (!convertedData.CheckIfConnectedPart(_lastCheckedCell)) _irregularRoomsGenerated++;

                _lastCheckedCell = thisCell;
                roomGO.name = roomGenerated + "[I-" + _irregularRoomsGenerated + "] " + roomInfo;
                break;
        }
    }

    #region Debug
    private void SetUpGridDots()
    {
        if (!_showDebugObjects) return;

        for (int gridX = 0; gridX < _gridBounds.x; gridX++)
        {
            for (int gridY = 0; gridY < _gridBounds.y; gridY++)
            {
                for (int gridZ = 0; gridZ < _gridBounds.z; gridZ++)
                {
                    Vector3Int position = new Vector3Int(gridX, gridY, gridZ);
                    GridCell cell = _cellDictionary[position];

                    string extraInfo = " " + cell._fCost.ToString() + " ";
                    SpawnDebugObject(_testingParams._testGridDot, cell._position, cell._positionInGrid, extraInfo, _testingParams._gridHolder.transform);

                }
            }
        }

    }

    private void SpawnDebugObject(GameObject gameObject, Vector3 position, Vector3Int gridPosition, string name, Transform parent)
    {
        // Only show debug objects if the bool is turned on
        if (!_showDebugObjects) return;

        GameObject debugObject = Instantiate(gameObject, position, Quaternion.identity, parent);
        debugObject.name = debugObject.name.Replace("(Clone)", name) + gridPosition;
    }

    private float GetElapsedTime()
    {
        float rawElapsedTime = Time.realtimeSinceStartup - _levelGenTimeElapsed;

        // Convert elapsed time to int and get just the decimal values of the elapsed time
        int intElapsed = (int)rawElapsedTime;
        float decimalElapsed = rawElapsedTime - intElapsed;

        // Values to locate the decimal point
        float temp = decimalElapsed;
        float decimalPlaceValue = 1f;

        // Finds position of first decimal that is not 0
        while (temp < 0.1f && temp != 0f)
        {
            temp *= 10;
            decimalPlaceValue /= 10;
        }

        float roundedDecimal = Mathf.Ceil(temp * 10f) / 10f * decimalPlaceValue;

        float roundedElapsedTime = intElapsed + roundedDecimal;

        return roundedElapsedTime;
    }
    #endregion

    #region A* Pathfinding
    private List<GridCell> FindPath(GridCell startCell, GridCell endCell)
    {
        _openList = new List<GridCell> {startCell};
        _closedList = new List<GridCell>();

        startCell._gCost = 0;
        startCell._hCost = CalculateDistanceCost(startCell, endCell);
        startCell.CalculateFCost();

        while (_openList.Count > 0 )
        {
            GridCell currentCell = GetLowestFCostCell(_openList);

            // Skip this cell if it is a barrier
            if (currentCell._cellStatus == GridCell.CellStatus.isBarrier) continue;

            // Early exit condition. If the end cell has been found.
            if (currentCell == endCell)
            {
                return CalculatePath(endCell);
            }

            _openList.Remove(currentCell);
            _closedList.Add(currentCell);

            // Check the neighbouring cells on the current cell
            foreach (GridCell neighbourCell in GetCellNeighbours(currentCell))
            {
                // Skip the current neighbour cell if it is already on the closed list
                if (_closedList.Contains(neighbourCell)) continue;
                // Skip the current neighbour cell if it is a barrier, then add it to the closed list.
                if (neighbourCell._cellStatus == GridCell.CellStatus.isBarrier)
                {
                    // Force this neighbours f cost to be a max value, so it will never be
                    // called by GetLowestFCostCell
                    neighbourCell._fCost = int.MaxValue;
                    _closedList.Add(neighbourCell);
                    _openList.Remove(neighbourCell);
                    continue;
                }

                int tentativeGCost = currentCell._gCost + CalculateDistanceCost(currentCell, neighbourCell);

                if (tentativeGCost < neighbourCell._gCost)
                {
                    neighbourCell._cameFromCell = currentCell;
                    neighbourCell._gCost = tentativeGCost;
                    neighbourCell._hCost = CalculateDistanceCost(neighbourCell, endCell);
                    neighbourCell.CalculateFCost();

                    // Increase the fcost by the mine cost if the cell is a mine
                    if (neighbourCell._cellStatus == GridCell.CellStatus.isMine)
                    {
                        neighbourCell._fCost += C_MINE_COST;
                    }

                    // Increase the fcost by the Y Cost if the cell has been flagged as being above a 2 column linear path cell
                    if (neighbourCell._increasedYCost && !neighbourCell._neighbourYCostIncreased)
                    {
                        neighbourCell._fCost += C_Y_COST;
                    }

                    // Check to see if the current cell is directly above the cell it came from
                    // If there is a valid cell above this cell, then flag it to have its F cost increased
                    if (currentCell != startCell && currentCell._positionInGrid == neighbourCell._positionInGrid + Vector3Int.down
                    && _cellDictionary.ContainsKey(neighbourCell._positionInGrid + Vector3Int.up) )
                    {
                        GridCell cellAboveThis = _cellDictionary[neighbourCell._positionInGrid + Vector3Int.up];

                        cellAboveThis._increasedYCost = true;

                        foreach (GridCell cell in GetCellNeighbours2D(cellAboveThis))
                        {
                           // cell._neighbourYCostIncreased = true;
                        }
                    }

                    if (!_openList.Contains(neighbourCell))
                    {
                        _openList.Add(neighbourCell);
                    }
                }
            }
        }

        // Out of cells on the open list
        // Path could not be found
        return null;
    }

    // Calculate the Manhattan distance heuristic cost for a cell (node)
    private int CalculateDistanceCost(GridCell a, GridCell b)
    {
        int manhattanDistance = GetManhattanDistance(a, b);

        return C_REGULAR_COST * manhattanDistance;
    }

    public int GetManhattanDistance(GridCell a, GridCell b)
    {
        int xDistance = Mathf.Abs(a._positionInGrid.x - b._positionInGrid.x);
        int yDistance = Mathf.Abs(a._positionInGrid.y - b._positionInGrid.y);
        int zDistance = Mathf.Abs(a._positionInGrid.z - b._positionInGrid.z);

        int manhattanDistance = (xDistance + yDistance + zDistance);

        return manhattanDistance;

    }

    private GridCell GetLowestFCostCell(List<GridCell> cellList)
    {
        // Set default lowest f cost cell as the first cell in the list
        GridCell lowestFCostCell = cellList[0];

        // Cycle through the entire list until the lowest f cost cell is found
        for (int i = 0; i < cellList.Count; i++)
        {
            if (cellList[i]._fCost < lowestFCostCell._fCost)
            {
                lowestFCostCell = cellList[i];
            }
        }

        return lowestFCostCell;

    }

    private List<GridCell> CalculatePath(GridCell endCell)
    {
        List<GridCell> path = new List<GridCell>();
        path.Add(endCell);

        GridCell currentCell = endCell;
        while (currentCell._cameFromCell != null)
        {
            path.Add(currentCell._cameFromCell);
            currentCell = currentCell._cameFromCell;
        }

        path.Reverse();
        return path;
    }
    #endregion

    #region Get Cell Info
    private List<GridCell> GetCellNeighbours(GridCell currentCell)
    {
        List<GridCell> neighbourList = new List<GridCell>();
        Vector3Int currentPosition = currentCell._positionInGrid;

        // Left
        if (currentPosition.x - 1 >= 0)
        {
            neighbourList.Add(GetGridCell(currentPosition.x - 1, currentPosition.y, currentPosition.z) );
        }

        // Right
        if (currentPosition.x + 1 < _gridBounds.x)
        {
            neighbourList.Add(GetGridCell(currentPosition.x + 1, currentPosition.y, currentPosition.z) );
        }

        // Backwards
        if (currentPosition.z - 1 >= 0)
        {
            neighbourList.Add(GetGridCell(currentPosition.x, currentPosition.y, currentPosition.z - 1) );
        }

        // Forwards
        if (currentPosition.z + 1 < _gridBounds.z)
        {
            neighbourList.Add(GetGridCell(currentPosition.x, currentPosition.y, currentPosition.z + 1) );
        }

        // Down
        if (currentPosition.y - 1 >= 0)
        {
            neighbourList.Add(GetGridCell(currentPosition.x, currentPosition.y - 1, currentPosition.z) );
        }

        // Up
        if (currentPosition.y + 1 < _gridBounds.y)
        {
            neighbourList.Add(GetGridCell(currentPosition.x, currentPosition.y + 1, currentPosition.z) );
        }

        //currentCell.SetNeighbours(neighbourList);
        return neighbourList;
    }

    public List<GridCell> GetCellNeighbours2D(GridCell thisCell)
    {
        List<GridCell> allNeighbours = GetCellNeighbours(thisCell);
        List<GridCell> neightbourListCopy = allNeighbours;

        List<GridCell> neightbourList2D = neightbourListCopy;
        int removedCells = 0;

        // Set up the 2D neighbour list
        foreach (GridCell neighbour in neightbourListCopy.ToList())
        {
            // Exit the loop early if the above and below cells have already been removed
            if (removedCells >= 2) break;

            // Check if neighbour is above or below the cell
            if (neighbour._positionInGrid == thisCell._positionInGrid + Vector3Int.up
            || neighbour._positionInGrid == thisCell._positionInGrid + Vector3Int.down)
            {
                removedCells++;
                neightbourList2D.Remove(neighbour);
            }
        }

        return neightbourList2D;
    }

    public List<GridCell> GetNeighbouringRooms(GridCell currentCell)
    {
        List<GridCell> all2DNeighbours = GetCellNeighbours2D(currentCell);
        List<GridCell> validNeighbours = new List<GridCell>();
        foreach (GridCell neighbour in all2DNeighbours)
        {
            if (neighbour._setAsRoom) validNeighbours.Add(neighbour);
        }

        return validNeighbours;
    }

    private GridCell GetGridCell(int x, int y, int z)
    {
        Vector3Int cellPos = new Vector3Int(x, y, z);

        if (_cellDictionary.Keys.Contains(cellPos)) return _cellDictionary[cellPos];
        
        return null;
    }

    #endregion
}
/*
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣷⣶⣤⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣿⣿⣿⣿⣷⡒⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⣿⣆⠙⡄⠀⠐⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣤⣤⣤⣤⣤⣤⣤⣤⣤⠤⢄⡀⠀⠀⣿⣿⣿⣿⣿⣿⡆⠘⡄⠀⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⢿⣿⣿⣿⣿⣿⣿⣿⣦⡈⠒⢄⢸⣿⣿⣿⣿⣿⣿⡀⠱⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⣿⣿⣿⣿⣿⣦⠀⠱⣿⣿⣿⣿⣿⣿⣇⠀⢃⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣷⡄⣹⣿⣿⣿⣿⣿⣿⣶⣾⣿⣶⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣴⣶⣿⣭⣍⡉⠙⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢀⣠⣶⣿⣿⣿⣿⣿⣿⣿⣿⣷⣦⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠻⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡷⢂⣓⣶⣶⣶⣶⣤⣤⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⢿⣿⣿⣿⠟⢀⣴⢿⣿⣿⣿⠟⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⠋⠉⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠤⠤⠤⠤⠙⣻⣿⣿⣿⣿⣿⣿⣾⣿⣿⡏⣠⠟⡉⣾⣿⣿⠋⡠⠊⣿⡟⣹⣿⢿⣿⣿⣿⠿⠛⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣤⣶⣤⣭⣤⣼⣿⢛⣿⣿⣿⣿⣻⣿⣿⠇⠐⢀⣿⣿⡷⠋⠀⢠⣿⣺⣿⣿⢺⣿⣋⣉⣉⣩⣴⣶⣤⣤⣄⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠻⠿⣿⣿⣿⣇⢻⣿⣿⡿⠿⣿⣯⡀⠀⢸⣿⠋⢀⣠⣶⠿⠿⢿⡿⠈⣾⣿⣿⣿⣿⡿⠿⠛⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠻⢧⡸⣿⣿⣿⠀⠃⠻⠟⢦⢾⢣⠶⠿⠏⠀⠰⠀⣼⡇⣸⣿⣿⠟⠉⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣴⣾⣶⣽⣿⡟⠓⠒⠀⠀⡀⠀⠠⠤⠬⠉⠁⣰⣥⣾⣿⣿⣶⣶⣷⡶⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠹⠟⣿⣿⡄⠀⠀⠠⡇⠀⠀⠀⠀⠀⢠⡟⠛⠛⠋⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠋⠹⣷⣄⠀⠐⣊⣀⠀⠀⢀⡴⠁⠣⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣤⣀⠤⠊⢁⡸⠀⣆⠹⣿⣧⣀⠀⠀⡠⠖⡑⠁⠀⠀⠀⠑⢄⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣦⣶⣿⣿⣟⣁⣤⣾⠟⠁⢀⣿⣆⠹⡆⠻⣿⠉⢀⠜⡰⠀⠀⠈⠑⢦⡀⠈⢾⠑⡾⠲⣄⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠖⠒⠚⠛⠛⠢⠽⢄⣘⣤⡎⠠⠿⠂⠀⠠⠴⠶⢉⡭⠃⢸⠃⠀⣿⣿⣿⠡⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⡤⠶⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣋⠁⠀⠀⠀⠀⠀⢹⡇⠀⠀⠀⠀⠒⠢⣤⠔⠁⠀⢀⡏⠀⠀⢸⣿⣿⠀⢻⡟⠑⠢⢄⡀⠀⠀⠀⠀
⠀⠀⠀⠀⢸⠀⠀⠀⡀⠉⠛⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣄⣀⣀⡀⠀⢸⣷⡀⣀⣀⡠⠔⠊⠀⠀⢀⣠⡞⠀⠀⠀⢸⣿⡿⠀⠘⠀⠀⠀⠀⠈⠑⢤⠀⠀
⠀⠀⢀⣴⣿⡀⠀⠀⡇⠀⠀⠀⠈⣿⣿⣿⣿⣿⣿⣿⣿⣝⡛⠿⢿⣷⣦⣄⡀⠈⠉⠉⠁⠀⠀⠀⢀⣠⣴⣾⣿⡿⠁⠀⠀⠀⢸⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⡜⠀⠀
⠀⢀⣾⣿⣿⡇⠀⢰⣷⠀⢀⠀⠀⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣦⣭⣍⣉⣉⠀⢀⣀⣤⣶⣾⣿⣿⣿⢿⠿⠁⠀⠀⠀⠀⠘⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠉⢦⠀
⢀⣼⣿⣿⡿⢱⠀⢸⣿⡀⢸⣧⡀⠀⢿⣿⣿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡭⠖⠁⠀⡠⠂⠀⠀⠀⠀⠀⠀⠀⠀⢠⠀⠀⠀⢠⠃⠀⠈⣀
⢸⣿⣿⣿⡇⠀⢧⢸⣿⣇⢸⣿⣷⡀⠈⣿⣿⣇⠈⠛⢿⣿⣿⣿⣿⣿⣿⠿⠿⠿⠿⠿⠿⠟⡻⠟⠉⠀⠀⡠⠊⠀⢠⠀⠀⠀⠀⠀⠀⠀⠀⣾⡄⠀⢠⣿⠔⠁⠀⢸
⠈⣿⣿⣿⣷⡀⠀⢻⣿⣿⡜⣿⣿⣷⡀⠈⢿⣿⡄⠀⠀⠈⠛⠿⣿⣿⣿⣷⣶⣶⣶⡶⠖⠉⠀⣀⣤⡶⠋⠀⣠⣶⡏⠀⠀⠀⠀⠀⠀⠀⢰⣿⣧⣶⣿⣿⠖⡠⠖⠁
⠀⣿⣿⣷⣌⡛⠶⣼⣿⣿⣷⣿⣿⣿⣿⡄⠈⢻⣷⠀⣄⡀⠀⠀⠀⠈⠉⠛⠛⠛⠁⣀⣤⣶⣾⠟⠋⠀⣠⣾⣿⡟⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⠷⠊⠀⢰⠀
⢰⣿⣿⠀⠈⢉⡶⢿⣿⣿⣿⣿⣿⣿⣿⣿⣆⠀⠙⢇⠈⢿⣶⣦⣤⣀⣀⣠⣤⣶⣿⣿⡿⠛⠁⢀⣤⣾⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⣸⣿⡿⠿⠋⠙⠒⠄⠀⠉⡄
⣿⣿⡏⠀⠀⠁⠀⠀⠀⠉⠉⠙⢻⣿⣿⣿⣿⣷⡀⠀⠀⠀⠻⣿⣿⣿⣿⣿⠿⠿⠛⠁⠀⣀⣴⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⠀⢠⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰
*/