using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.AI.Navigation;

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
    public static NavMeshSurface _navMeshSurface;
    [SerializeField] public Canvas _PlayerMenu;
    [SerializeField] public TextMeshProUGUI _HeaderText;
    [SerializeField] public GameObject _ResumeButton;
    [SerializeField] public GameObject _RestartButton;

    [SerializeField] public static GameObject _pickupCanvas;
    [SerializeField] public GameObject _PickupCanvas;

    [SerializeField] public static GameObject _player;
    [SerializeField] public GameObject _Player;


    [SerializeField] public static GameObject _stimulant;
    [SerializeField] public GameObject _Stimulant;


    private void Awake()
    {
        _playerMenu = _PlayerMenu;
        _headerText = _HeaderText;
        _resumeButton = _ResumeButton;
        _restartButton = _RestartButton;
        _navMeshSurface = GetComponent<NavMeshSurface>();
        _pickupCanvas = _PickupCanvas;
        _player = _Player;
        _stimulant = _Stimulant;

        _musicManager = GameObject.FindGameObjectWithTag("Musicmanager");
    }
    public static void GenerateNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
    
}
