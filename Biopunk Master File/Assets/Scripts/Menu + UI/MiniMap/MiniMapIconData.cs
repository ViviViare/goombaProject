using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIconData : MonoBehaviour
{
    public List<Compass> _validSides = new List<Compass>();
    public MMRoomType _roomType = MMRoomType.Regular;

    public enum MMRoomType
    {
        Regular,
        Irregular,
        Shop,
        Boss,
        Vertical,
    }
}
