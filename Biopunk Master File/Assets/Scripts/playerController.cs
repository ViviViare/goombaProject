/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 09/02/24

// This script handles player movement. Basic FPS controls; nothing too special.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private CharacterController _playerController;

    private Transform _playerCameraTransform;

    [SerializeField] private float _playerSpeed = 5f;

    // When first ran, this script locks the mouse cursor and gets references for objects and variables that are needed for the script to allow for player movement.

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerInput= GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions.FindAction("Move");
        _playerController = GetComponent<CharacterController>();
        _playerCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        PlayerMove();
        this.gameObject.GetComponent<playerStats>()._playerSpeed = _playerSpeed;
    }

    /*
    // To actually move the player, the script uses a vector2 that is read from the Input Actions created inside of Unity.
    // This vector2 is then converted into a vector3 (as our game is 3d, and requires a vector3 even when not using every single vector) which is then used to move the player.
    // Before actually moving the player, however, it also gets the direction of the player's camera so as to link movement to the direction the camera is facing;
    // forward is forward relative to the camera, not the scene

    // The use of the new input system, alongside input actions and input maps instead of standard "GetKey()" methods allows for our game to more easily be used with multiple input devices and methods;
    // rebinding of controls and use of gamepad will be simpler to implement thanks to the new input system.

    // Camera control and movement is not listed here. This is because it is handled in-engine using a cinemachine camera and another separate input action that reads a vector2 from a mouse
    // (or corresponding input device) and uses a delta (the difference made every time this vector2 changes) to tell the camera how to move.

    // This script's _playerSpeed variable is also updated with the _playerSpeed variable found in "playerStats.cs". This is to ensure that *all* of the player's stats are found in one place,
    // and are easily modifiable instead of having to potentially reference and modify both "playerStats.cs" and "playerController.cs" when obtaining an item that changes both the
    // player's speed and, say, the player's damage.
    */

    private void PlayerMove()
    {
        Vector2 direction = _moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(direction.x, 0, direction.y) * _playerSpeed * Time.deltaTime;
        movement = _playerCameraTransform.forward * movement.z + _playerCameraTransform.right * movement.x;
        movement.Normalize();
        movement.y = 0;
        _playerController.SimpleMove(movement * _playerSpeed);

    }
}
