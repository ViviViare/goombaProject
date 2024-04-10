using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulantScript : MonoBehaviour
{
    [SerializeField] public bool _usedStimulants;
    [SerializeField] public int _stimulantTimer;

    [SerializeField] public static float _stimulantMultiplier = 0.2f;

    private void Update()
    {
        if (_stimulantTimer == _stimulantTimer + 1 && _usedStimulants)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerStats PlayerStats = player.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier -= _stimulantMultiplier;
            PlayerStats._playerAttackSpeedMultiplier -= _stimulantMultiplier;
            _stimulantTimer = 0;
        }
    }

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
        playerStats PlayerStats = GlobalVariables._player.gameObject.GetComponent<playerStats>();
        PlayerStats._playerSpeedMultiplier += _stimulantMultiplier;
        PlayerStats._playerAttackSpeedMultiplier += _stimulantMultiplier;
    }
}
