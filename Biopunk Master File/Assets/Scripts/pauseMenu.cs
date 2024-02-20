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

    [SerializeField] private Canvas _playerMenu;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _restartButton;

    private PlayerInput _playerInput;
    private InputAction _pauseAction;

    [SerializeField] private bool _playerDead;


    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _pauseAction = _playerInput.actions.FindAction("Pause");
        _playerMenu.enabled = false;
    }

    void Update()
    {
        if(!_playerDead)
        {
            _pauseAction.started += OpenPauseMenu;
        }
    }

    private void OpenPauseMenu(InputAction.CallbackContext obj)
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        _playerCam.enabled = false;
        _playerMenu.enabled = true;
        _resumeButton.SetActive(true);
        _restartButton.SetActive(false);
        _headerText.text = "Paused";
    }

    public void ClosePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        _playerCam.enabled = true;
        _playerMenu.enabled = false;
    }
 
    public void OpenDeathMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        _playerDead = true;
        _playerCam.enabled = false;
        _playerMenu.enabled = true;
        _resumeButton.SetActive(false);
        _restartButton.SetActive(true);
        _headerText.text = "You have died";
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
