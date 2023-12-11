using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerWeapons : MonoBehaviour
{
    [SerializeField] public GameObject leftClaw;
    [SerializeField] public GameObject rightClaw;
    [SerializeField] public GameObject leftGun;
    [SerializeField] public GameObject rightGun;

    [SerializeField] public bool hasLeftGun;
    [SerializeField] public bool hasRightGun;
    [SerializeField] public bool hasLeftClaw;
    [SerializeField] public bool hasRightClaw;

    [SerializeField] TMPro.TextMeshProUGUI pickupText;

    [SerializeField] int pickupGunOrClaw;
    [SerializeField] bool canSelect;


    [SerializeField] Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        hasLeftClaw = true;
        hasRightClaw = true;
        hasLeftGun = false;
        hasRightGun = false;

        pickupText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        leftClaw.SetActive(hasLeftClaw);
        rightClaw.SetActive(hasRightClaw);
        leftGun.SetActive(hasLeftGun);
        rightGun.SetActive(hasRightGun);

        if (Input.GetKeyDown("e"))
        {
            Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, playerCam.transform.forward, out hit, 5))
            {
                weaponPickup weaponPick = hit.collider.GetComponent<weaponPickup>();
                if (weaponPick != null)
                {
                    pickupText.enabled = true;
                    pickupGunOrClaw = weaponPick.gunOrClaw;
                    canSelect = true;
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        if(canSelect == true)
        {
            if (Input.GetKey("1"))
            {
                ReplaceLeftArm();
            }
            else if (Input.GetKey("3"))
            {
                ReplaceRightArm();
            }
        }

    }

    public void ReplaceLeftArm()
    {
        if(pickupGunOrClaw == 0)
        {
            hasLeftGun = true;
            hasLeftClaw = false;
            pickupText.enabled = false;
            canSelect = false;
        }
        else if(pickupGunOrClaw == 1)
        {
            hasLeftGun = false;
            hasLeftClaw = true;
            pickupText.enabled = false;
            canSelect = false;
        }

    }

    public void ReplaceRightArm()
    {
        if (pickupGunOrClaw == 0)
        {
            hasRightGun = true;
            hasRightClaw = false;
            pickupText.enabled = false;
        }
        else if (pickupGunOrClaw == 1)
        {
            hasRightGun = false;
            hasRightClaw = true;
            pickupText.enabled = false;
        }
    }

  
}
