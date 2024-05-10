using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class playerInteract : MonoBehaviour
{
    [SerializeField] public float _interactRange = 5f;
    [SerializeField] public GameObject _currentInteractable;

    [SerializeField] public GameObject _player;

    [SerializeField] private CinemachineVirtualCamera _playerCam;

    private void Start()
    {
        _player = this.gameObject;
    }

    public void OnInteract()
    {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit interactable;
        if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out interactable, _interactRange))
        {
            if (interactable.collider.gameObject.GetComponent<rangedPickup>() != null || interactable.collider.gameObject.GetComponent<meleePickup>() != null)
            {
                _currentInteractable = interactable.collider.gameObject;
                GlobalVariables._pickupCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                _playerCam.enabled = false;
            }
            else if (interactable.collider.gameObject.GetComponent<activePickup>() != null)
            {
                if (interactable.collider.gameObject.GetComponent<activePickup>()._activeIndex == _player.gameObject.GetComponent<playerActiveInventory>()._currentActiveIndex) return;
                playerActiveInventory actInv = _player.GetComponent<playerActiveInventory>();
                actInv._currentActive.SetActive(false);
                actInv._currentActive = actInv._playerActives[interactable.collider.gameObject.GetComponent<activePickup>()._activeIndex];
                actInv._currentActive.SetActive(true);
                actInv._currentActiveIndex = interactable.collider.gameObject.GetComponent<activePickup>()._activeIndex;

                this.gameObject.GetComponent<playerActiveItem>()._activeItemMaxCharge = interactable.collider.GetComponent<activePickup>()._activeMaxCharge;
                this.gameObject.GetComponent<playerActiveItem>()._activeItemCharge = interactable.collider.GetComponent<activePickup>()._activeMaxCharge;
                this.gameObject.GetComponent<playerActiveItem>()._hasActiveItem = true;

                ActiveFillUpdate._instance.UpdateActive(interactable.collider.gameObject.GetComponent<activePickup>()._sprite, interactable.collider.GetComponent<activePickup>()._activeMaxCharge);
                ActiveFillUpdate._instance.UpdateChargeAmount(interactable.collider.GetComponent<activePickup>()._activeMaxCharge);

                ObjectPooler.Despawn(interactable.collider.gameObject);
            }
        }
    }

    public void SwapLeftWeapon()
    {
        if (_currentInteractable.GetComponent<rangedPickup>() != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            GlobalVariables._pickupCanvas.SetActive(false);
            _playerCam.enabled = true;
            _currentInteractable.GetComponent<rangedPickup>().SwapLeft(_player);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            GlobalVariables._pickupCanvas.SetActive(false);
            _playerCam.enabled = true;
            _currentInteractable.GetComponent<meleePickup>().SwapLeft(_player);
        }
    }

    public void SwapRightWeapon()
    {
        if (_currentInteractable.GetComponent<rangedPickup>() != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            GlobalVariables._pickupCanvas.SetActive(false);
            _playerCam.enabled = true;
            _currentInteractable.GetComponent<rangedPickup>().SwapRight(_player);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            GlobalVariables._pickupCanvas.SetActive(false);
            _playerCam.enabled = true;
            _currentInteractable.GetComponent<meleePickup>().SwapRight(_player);
        }
    }
}
