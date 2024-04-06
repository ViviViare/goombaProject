using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedipakScript : MonoBehaviour
{
    [SerializeField] public int _medkitHeal = 30;


    private void OnEnable()
    {
        playerActiveItem._activeAction += UseActive;
    }
    private void OnDisable()
    {
        playerActiveItem._activeAction -= UseActive;
    }


    private void UseActive()
    {
        GlobalVariables._player.gameObject.GetComponent<playerHealth>()._playerHealth += _medkitHeal;
    }
}
