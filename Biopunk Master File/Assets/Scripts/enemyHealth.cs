using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    [SerializeField] int health = 100;

    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public IEnumerator DamageFlash()
    {
        this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        yield return new WaitForSeconds(0.2f);
        this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    public void takeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Damaged for " + damageAmount + " points of damage.");
        StartCoroutine(DamageFlash());
    }


}
