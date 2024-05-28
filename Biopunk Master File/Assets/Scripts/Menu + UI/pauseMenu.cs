using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 26/02/24

// Script handles the player's pause menu and all its derivatives (such as the menu that opens when a player dies, which is functionally the same menu as when they pause with minor adjustments.)

// Edits since script completion:
*/

public class pauseMenu : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCam;


    private PlayerInput _playerInput;
    private InputAction _pauseAction;

    [SerializeField] public bool _playerDead;

    [SerializeField] public AudioClip _uiOpen;
    [SerializeField] public AudioClip _uiClose;


    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _pauseAction = _playerInput.actions.FindAction("Pause");
        GlobalVariables._playerMenu.enabled = false;
    }

    void Update()
    {
        if(!_playerDead)
        {
            _pauseAction.started += OpenPauseMenu;
        }
    }

    // Opens the pause menu, and sets the timescale to zero as well as disabling the player's ability to move, attack or look around.
    public void OpenPauseMenu(InputAction.CallbackContext obj)
    {
        if (_playerDead) return;

        GlobalVariables._gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        _playerCam.enabled = false;
        GlobalVariables._playerMenu.enabled = true;

        GlobalVariables._resumeButton.SetActive(true);
        GlobalVariables._restartButton.SetActive(false);

        GlobalVariables._headerText.text = "Paused";

    }

    // Closes the pause menu, reverting all the above changes and returning the game to normal speed.
    public void ClosePauseMenu()
    {
        if (_playerDead) return;

        GlobalVariables._gamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        _playerCam.enabled = true;
        GlobalVariables._playerMenu.enabled = false;

    }

    // Opens the death menu, and sets the timescale to zero as well as disabling the player's ability to move, attack or look around.
    // The player has no choice or option to unpause at this point, and can only restart the game.
    public void OpenDeathMenu()
    {
        _playerDead = true;

        GlobalVariables._gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 0;

        _playerCam.enabled = false;
        GlobalVariables._playerMenu.enabled = true;

        GlobalVariables._resumeButton.SetActive(false);
        GlobalVariables._restartButton.SetActive(true);

        GlobalVariables._headerText.text = "You have died";
    }

    // Opens the win menu, and sets the timescale to zero as well as disabling the player's ability to move, attack or look around.
    // The player has no choice or option to unpause at this point, and can only restart the game.
    public void OpenWinMenu()
    {
        _playerDead = true;

        GlobalVariables._gamePaused = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;

        _playerCam.enabled = false;
        GlobalVariables._playerMenu.enabled = true;

        GlobalVariables._resumeButton.SetActive(false);
        GlobalVariables._restartButton.SetActive(true);

        GlobalVariables._headerText.text = "You win!";
    }

    // Self explanatory methods used to allow the buttons in the pause menu to perform their respective functions, such as unpausing the game and restarting the game.

    public void ExitToDesktop()
    {
        Time.timeScale = 1;
        GlobalVariables._gamePaused = false;
        Application.Quit();
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1;
        GlobalVariables._gamePaused = false;
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        GlobalVariables._gamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

}
