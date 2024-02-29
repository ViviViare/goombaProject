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
using UnityEngine;

[Serializable]
public class LevelGenTestingParams
{
    [Header("Testing Prefabs")]
    public GameObject _testRoom;
    public GameObject _testMine;
    public GameObject _testBarrier;
    public GameObject _testExtensionRoom;
    public GameObject _testGridDot;

    [Header("Parents")]
    public Transform _roomHolder;
    public Transform _mineHolder;
    public Transform _gridHolder;
}