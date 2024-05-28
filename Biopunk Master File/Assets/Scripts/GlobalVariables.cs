using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.AI.Navigation;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Keeps track of any variables, gameobjects and other such components that are widely referenced within scripts, to make them easier to reference.

// Edits since script completion:
// 05/03/24: Cut down script bloat by a lot, also making the script more modular.
*/
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

    [SerializeField] public static bool _gamePaused;

    [SerializeField] public static bool _startingItemGrabbed;
    [SerializeField] public static bool __startingDoorsOpened;

    private void Awake()
    {
        _playerMenu = _PlayerMenu;
        _headerText = _HeaderText;
        _resumeButton = _ResumeButton;
        _restartButton = _RestartButton;
        _navMeshSurface = GetComponent<NavMeshSurface>();
        _pickupCanvas = _PickupCanvas;
        _player = _Player;

        _musicManager = GameObject.FindGameObjectWithTag("Musicmanager");
    }
    public static void GenerateNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }

    // Handles incramenting/decramenting the timers for the various boost items. Runs every time the player clears a room with enemies within it.
    public static void TickCooldowns()
    {
        playerStatusEffects playerstat = _player.GetComponent<playerStatusEffects>();
        playerActiveItem playeractive = _player.GetComponent<playerActiveItem>();
        if (playerstat._amplifierDuration > 0)
        {
            playerstat._amplifierDuration--;
            PassiveItemManager._instance.UpdatePassive(PickupType.Amplifier, playerstat._amplifierDuration);
        }
        if (playerstat._serumDuration > 0)
        {
            playerstat._serumDuration--;
            PassiveItemManager._instance.UpdatePassive(PickupType.Serum, playerstat._serumDuration);
        }

        if (playeractive._activeItemCharge < playeractive._activeItemMaxCharge)
        {
            playeractive._activeItemCharge++;
        }
    }
}
