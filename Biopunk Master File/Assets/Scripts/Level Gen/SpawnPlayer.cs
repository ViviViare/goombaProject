/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 29/02/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  SpawnPlayer.cs
//
//  This script handles the functionality for spawning the player into the level
//  Event gets triggered by Level_Generator.cs
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{

    // Note this will have to change
    [SerializeField] private GameObject _playerPrefab;

    private void OnEnable()
    {
        Level_Generator._spawnPlayer += SpawnPlayerInLevel;
    }
    private void OnDisable()
    {
        Level_Generator._spawnPlayer -= SpawnPlayerInLevel;
    }

    private void SpawnPlayerInLevel()
    {
        // Get the spawn point from the start room
        Transform spawnPoint = Level_Generator._instance._startRoom._roomData.GetComponent<SpawnRoom>()._playerSpawnPoint;
        GameObject player = GlobalVariables._player;
        player.transform.position = spawnPoint.position;
        player.SetActive(true);

    }

}
