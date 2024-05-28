/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 06/05/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  MiniMapHandler.cs
//
//  Script to handle the minimap by reading the level generation and generating an icon for every room
//  As the player moves through the rooms it changes the visbility and colour of the icons to visualize their relation to the player (Such as: Visited, Not visited but adjacent or Currently active)
//  The room the player is currently in will always be centered.
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapHandler : MonoBehaviour
{
    public static MiniMapHandler _instance;
    private void Awake()
    {
        _instance = this;
    }
    [SerializeField] private List<GameObject> _roomSprites = new List<GameObject>();
    [SerializeField] private List<GameObject> _specialSprites = new List<GameObject>();
    [SerializeField] private int _rotationThreshold;
    [SerializeField] private RectTransform _miniMapMask;
    [SerializeField] private Transform _miniMapHolder;
    [SerializeField] private Transform _miniMapCenter;
    [SerializeField] private float _iconDistance;
    private List<RectTransform> _iconFolders = new List<RectTransform>();
    private Dictionary<Vector3Int, GameObject> _allIcons = new Dictionary<Vector3Int, GameObject>();
    
    [Header("Icon Customization")]
    [SerializeField] private iconConfig _playerIcon;
    [SerializeField] private Transform _playerIconGo;
    [SerializeField] private iconConfig _activeIcon;
    [SerializeField] private iconConfig _visitedIcon;
    [SerializeField] private iconConfig _adjacentIcon;
    [SerializeField] private iconConfig _bossIcon;
    [SerializeField] private iconConfig _itemIcon;
    [SerializeField] private iconConfig _ladderUpIcon, _ladderDownIcon;
    

    // Serializeable class to hold the configuration for each icon
    [System.Serializable]
    public class iconConfig
    {
        public GameObject _prefab;
        public Color _colour;
        [Range(0, 1)] public float _transparency;
    }
    
    private void OnEnable()
    {
        // Subscribe the event _finishedGeneratingRooms to the GenerateMiniMap method.
        Level_Generator._finishedGeneratingRooms += GenerateMiniMap;
    }

    // Unsubscribe from the level generator events
    private void OnDisable()
    {
        Level_Generator._finishedGeneratingRooms -= GenerateMiniMap;
    }

    private void Update()
    {
        // In update read the player's current rotation to correctly rotate the minimap player icon so that it always points in the direction the player is looking
        Vector3 camTransform = Camera.main.transform.forward;
        Vector3 forwardDirection = new Vector3(camTransform.x, 0f, camTransform.z);
        float angle = Mathf.Atan2(camTransform.x, camTransform.z) * Mathf.Rad2Deg;

        float snappedAngle = Mathf.Round(angle / 90) * 90;

        _playerIconGo.rotation = Quaternion.Euler(0f, 0f, -snappedAngle);
    }


    public void GenerateMiniMap()
    {
        // Create the folders for each floor of icons
        foreach (GameObject floor in Level_Generator._instance._floorFolders)
        {
            GameObject floorFolder = new GameObject();
            floorFolder.transform.SetParent(_miniMapHolder, false);
            RectTransform rectTransform = floorFolder.AddComponent<RectTransform>();

            _iconFolders.Add(rectTransform);
            floorFolder.name = "FLOOR " + _iconFolders.Count;
        }

        foreach (GridCell cell in Level_Generator._totalPath)
        {
            RoomData cellData = cell._roomData;

            GenerateRegularIcon(cell, cellData);
        }

        MoveIconFloor(0);
        MoveRoomToCenter(Level_Generator._instance._startRoom);
        foreach (GridCell neighbour in Level_Generator._instance.GetNeighbouringRooms(Level_Generator._instance._startRoom))
        {
            UpdateIconStatus(neighbour);
        }
    }

    private void GenerateRegularIcon(GridCell cell, RoomData data)
    {
        Vector3 iconPosition = new Vector3(cell._positionInGrid.x * _iconDistance, cell._positionInGrid.z * _iconDistance, 0);
        GameObject newIcon = Instantiate(_roomSprites[0], iconPosition, quaternion.identity);
        newIcon.transform.SetParent(_iconFolders[cell._positionInGrid.y].transform, false);

        MiniMapIconData iconData = newIcon.GetComponent<MiniMapIconData>();
        newIcon.name = ($"R_{cell._positionInGrid}");

        // Found Item Room
        if (data.GetComponent<ItemHandler>() != null)
        {
            CreateSpecialIcon(_itemIcon._prefab, _itemIcon._colour, iconData, MiniMapIconData.MMRoomType.Shop, newIcon, cell._positionInGrid, "SH");
        }
        // Found Boss Room
        else if (cell == Level_Generator._instance._endRoom)
        {
            CreateSpecialIcon(_bossIcon._prefab, _bossIcon._colour, iconData, MiniMapIconData.MMRoomType.Boss, newIcon, cell._positionInGrid, "BS");
        }
        // Found vertical room
        else if (cell._verticalityType == Verticality.UP)
        {
            CreateSpecialIcon(_ladderUpIcon._prefab, _ladderUpIcon._colour, iconData, MiniMapIconData.MMRoomType.Vertical, newIcon, cell._positionInGrid, "UP");
        }
        else if (cell._verticalityType == Verticality.DOWN)
        {
            CreateSpecialIcon(_ladderDownIcon._prefab, _ladderDownIcon._colour, iconData, MiniMapIconData.MMRoomType.Vertical, newIcon, cell._positionInGrid, "DN");
        }

        _allIcons.Add(cell._positionInGrid, newIcon);
        UpdateIconStatus(cell);
    }

    private void CreateSpecialIcon(GameObject iconPrefab, Color colour, MiniMapIconData iconData, MiniMapIconData.MMRoomType roomType, GameObject newIcon, Vector3Int position, string name)
    {
        GameObject newSpecialIcon = Instantiate(iconPrefab, Vector3.forward, quaternion.identity);
        newSpecialIcon.transform.SetParent(newIcon.transform, false);
        iconData._roomType = roomType;

        Image iconImg = newSpecialIcon.GetComponent<Image>();
        iconImg.color = colour;

        newIcon.name = ($"{name}1_{position}");
        newSpecialIcon.name = ($"{name}2_{position}");
        
    }

    public void MoveRoomToCenter(GridCell cell)
    {
        Vector3 newIcoPosition = _allIcons[cell._positionInGrid].transform.position;
        Vector3 centerPositon = _miniMapCenter.transform.position;
        Vector3 direction = centerPositon - newIcoPosition;

        _miniMapHolder.transform.position += direction;
    }

    public void MoveIconFloor(int floorNo)
    {
        foreach (RectTransform folder in _iconFolders)
        {
            if (folder == _iconFolders[floorNo]) continue;

            folder.gameObject.SetActive(false);
        }
        _iconFolders[floorNo].gameObject.SetActive(true);
    }

    public void UpdateIconStatus(GridCell cell)
    {
        RoomStatus cellStatus = cell._roomData.GetComponent<RoomStatus>();
        GameObject iconGameObject = _allIcons[cell._positionInGrid];

        MiniMapIconData iconData = iconGameObject.GetComponent<MiniMapIconData>();     
        Image iconImg = iconGameObject.GetComponent<Image>();
        
        float transparencyUsed = 0;
        Color colourUsed = Color.white;

        // Configure room status by order of significance.
        if (cellStatus._isActive)
        {
            colourUsed = _activeIcon._colour;
            transparencyUsed = _activeIcon._transparency;
        }
        else if (cellStatus._beenVisited)
        {
            colourUsed = _visitedIcon._colour;
            transparencyUsed = _visitedIcon._transparency;
        }
        else if (cellStatus._adjacentVisited)
        {
            colourUsed = _adjacentIcon._colour;
            transparencyUsed = _adjacentIcon._transparency;
        }
        // Player has not been adjacent to this room yet
        
        iconImg.color = colourUsed;
        iconImg.ChangeAlpha(transparencyUsed);

        CheckForChildIcon(iconData, transparencyUsed, iconImg);
    }

    private void CheckForChildIcon(MiniMapIconData iconData, float alpha, Image parentImage)
    {
        // If the icon has a child (such as a shop icon) then update its transparency as well
        if (iconData.transform.childCount == 0) return;

        Image childIcon = iconData.transform.GetChild(0).GetComponent<Image>();
        childIcon.ChangeAlpha(alpha);

        parentImage.color = (Color.white * parentImage.color)  * childIcon.color;
        parentImage.ChangeAlpha(alpha);
    }

}
