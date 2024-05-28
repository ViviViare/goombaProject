using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GlobalVariables._player.GetComponent<pauseMenu>().OpenWinMenu();
    }
}
