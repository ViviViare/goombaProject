/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 06/05/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  MiniMapIconData.cs
//
//  Data script to hold the enum for the room type as well as the valid amount of sides that an icon has.
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/
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
