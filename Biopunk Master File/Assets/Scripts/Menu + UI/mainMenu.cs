/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 26/02/24

// Simple main menu controller that allows buttons to exit the game or load the test scene.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
