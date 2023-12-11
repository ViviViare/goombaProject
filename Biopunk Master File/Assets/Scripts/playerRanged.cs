using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerRanged : MonoBehaviour
{
    [SerializeField] GameObject playerBullet;

    [SerializeField] float gunSpeed = 200f;
    [SerializeField] int gunDamage = 10;
    [SerializeField] float gunRange;
    [SerializeField] float gunSize;

    [SerializeField] float gunCoolDown;

    [SerializeField] Camera playerCam;

    [SerializeField] string gunSide;

    [SerializeField] bool canShoot = true;

    [SerializeField] GameObject gunBarrel;
    void Start()
    {
        playerCam = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        if (this.name == "GunL")
        {
            gunSide = "left";
        }
        else if (this.name == "GunR")
        {
            gunSide = "right";
        }

        gunBarrel = this.gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (gunSide == "right")
        {
            if (Input.GetMouseButtonDown(1) && canShoot)
            {
                FireGun();
            }
        }
        else if (gunSide == "left")
        {
            if (Input.GetMouseButtonDown(0) & canShoot)
            {
                FireGun();
            }
        }
    }

    private void FireGun()
    {
        GameObject bullet = Instantiate(playerBullet, gunBarrel.transform.position, gunBarrel.transform.rotation);
        bullet.GetComponent<bulletLogic>().bulletDamage = gunDamage;
        bullet.GetComponent<bulletLogic>().bulletRange = gunRange;
        bullet.GetComponent<bulletLogic>().bulletSize = gunSize;
        bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, gunSpeed, 0));
    }
}
