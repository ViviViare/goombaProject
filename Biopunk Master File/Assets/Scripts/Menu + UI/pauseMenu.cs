using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCam;


    private PlayerInput _playerInput;
    private InputAction _pauseAction;

    [SerializeField] private bool _playerDead;


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

    public void OpenPauseMenu(InputAction.CallbackContext obj)
    {
        GlobalVariables._gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        _playerCam.enabled = false;
        GlobalVariables._playerMenu.enabled = true;

        GlobalVariables._resumeButton.SetActive(true);
        GlobalVariables._restartButton.SetActive(false);

        GlobalVariables._headerText.text = "Paused";
    }

    public void ClosePauseMenu()
    {
        GlobalVariables._gamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        _playerCam.enabled = true;
        GlobalVariables._playerMenu.enabled = false;
    }
 
    public void OpenDeathMenu()
    {
        GlobalVariables._gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 0;
        
        _playerDead = true;

        _playerCam.enabled = false;
        GlobalVariables._playerMenu.enabled = true;

        GlobalVariables._resumeButton.SetActive(false);
        GlobalVariables._restartButton.SetActive(true);

        GlobalVariables._headerText.text = "You have died";
    }

    public void ExitToDesktop()
    {
        GlobalVariables._gamePaused = false;
        Application.Quit();
    }

    public void ExitToMenu()
    {
        GlobalVariables._gamePaused = false;
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        GlobalVariables._gamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
