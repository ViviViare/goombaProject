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
        if (GlobalVariables._roomsCleared == GlobalVariables._roomsCleared++)
        {
            _stimulantTimer++;
        }

        if (_stimulantTimer == _stimulantTimer + 1 && _usedStimulants)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerStats PlayerStats = player.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier -= _stimulantMultiplier;
            PlayerStats._playerAttackSpeedMultiplier -= _stimulantMultiplier;

            StatIncreaseUIHandler._instance.DecreaseDamageStat(_stimulantMultiplier * 100);
            StatIncreaseUIHandler._instance.DecreaseSpeedStat(_stimulantMultiplier * 100);
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
        
        StatIncreaseUIHandler._instance.IncreaseDamageStat(_stimulantMultiplier * 100);
        StatIncreaseUIHandler._instance.IncreaseSpeedStat(_stimulantMultiplier * 100);
           
        
    }
}
