using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMelee : MonoBehaviour
{
    [SerializeField] private Animator clawAnimator;
    [SerializeField] Camera playerCam;

    [SerializeField] public int clawSwingSpeed;
    [SerializeField] public int clawDamage;
    [SerializeField] public int clawRange;

    [SerializeField] bool isAttacking;

    [SerializeField] public string clawSide;

    // Start is called before the first frame update
    void Start()
    {
        clawAnimator = this.GetComponent<Animator>();
        playerCam = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();

        if(this.name == "ClawL")
        {
            clawSide = "left";
        }
        else if(this.name == "ClawR")
        {
            clawSide = "right";
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(clawSide == "right")
        {
            if (Input.GetMouseButtonDown(1) & isAttacking == false)
            {
                SwingClaw();
            }
        }
        else if(clawSide == "left") 
        {
            if (Input.GetMouseButtonDown(0) & isAttacking == false)
            {
                SwingClaw();
            }
        }
    }

    public void SwingClaw()
    {
        clawAnimator.SetBool("attack", true);
        isAttacking = true;
    }
    public void StopSwing()
    {
        clawAnimator.SetBool("attack", false);
        isAttacking = false;
    }

    public void ClawRaycast()
    {
        Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit wepSwing;
        if (Physics.Raycast(rayOrigin, playerCam.transform.forward, out wepSwing, clawRange))
        {
            enemyHealth enemyHP = wepSwing.collider.GetComponent<enemyHealth>();
            if (enemyHP != null)
            {
                enemyHP.takeDamage(clawDamage);
            }
        }
    }
}
