using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalVariables : MonoBehaviour
{
    [Header("Statics")]
    public static int _roomsCleared;
    public static bool _inCombat;
    public static GameObject _musicManager;

    [Header("Variable Values")]
    [SerializeField] public static Canvas _playerMenu;
    [SerializeField] public static TextMeshProUGUI _headerText;
    [SerializeField] public static GameObject _resumeButton;
    [SerializeField] public static GameObject _restartButton;

    [SerializeField] public Canvas _PlayerMenu;
    [SerializeField] public TextMeshProUGUI _HeaderText;
    [SerializeField] public GameObject _ResumeButton;
    [SerializeField] public GameObject _RestartButton;


    private void Start()
    {
        _playerMenu = _PlayerMenu;
        _headerText = _HeaderText;
        _resumeButton = _ResumeButton;
        _restartButton = _RestartButton;

        _musicManager = GameObject.FindGameObjectWithTag("Musicmanager");
    }

}
