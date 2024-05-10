using System.Linq;
using UnityEngine;

public class LadderHandler : MonoBehaviour
{
    [Header("Data")]
    [ShowOnly] public bool _isOpen = false;

    [Header("References")]
    public Transform _spawnPoint;
    private BoxCollider _doorExit;
    [SerializeField] private BoxCollider _doorCollider;

    [Header("Ladder Data")]
    public RoomData _roomData;
    private GridCell _connectedCell;
    public RoomData _connectedRoom;
    public LadderHandler _connectedLadder;
    
    private void Awake()
    {
        _doorExit = GetComponent<BoxCollider>();
        _isOpen = false;
        _doorExit.enabled = false;
        _doorCollider.enabled = true;
    }

    private void Start()
    {
        switch (_roomData._verticality)
        {
            case Verticality.UP:
                _connectedCell = Level_Generator._instance._cellDictionary[_roomData._cellPosition + Vector3Int.up];
                break;
            case Verticality.DOWN:
                _connectedCell = Level_Generator._instance._cellDictionary[_roomData._cellPosition + Vector3Int.down];
                break;
        }
        _connectedRoom = _connectedCell._roomData;
        // Get the opposite door that is connected to this door
        _connectedLadder = _connectedRoom._ladderObject;
    }
    
    public void SetRoomData(RoomData data)
    {
        _roomData = data;
    }

    public void ToggleLadder()
    {
        // When called, this method reverses _isOpen and _doorExit.Enabled
        _isOpen = !_isOpen;
        _doorExit.enabled = !_doorExit.enabled;
        _doorCollider.enabled = !_doorCollider.enabled;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Do nothing if the collision is not the player
        // Do nothing if the door is not set as open
        if (!_isOpen || collider.gameObject.tag != "Player") return;
        Transform player = collider.gameObject.transform;

        // teleport the player to the opposite rooms connected door with an offset
        player.position = _connectedLadder._spawnPoint.position;
        _connectedRoom.GetComponent<RoomStatus>()?.PlayerEntered();

        int floorIncrement = _connectedCell._positionInGrid.y;

        MiniMapHandler._instance.MoveIconFloor(floorIncrement);

        // Let the room this door is attatched to know that the player has left.
        _roomData.GetComponent<RoomStatus>()?.PlayerLeft();
    }

}
