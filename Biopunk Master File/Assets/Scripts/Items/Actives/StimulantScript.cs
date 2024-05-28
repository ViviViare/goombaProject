using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulantScript : MonoBehaviour
{
    [SerializeField] public bool _usedStimulants;
    [SerializeField] public int _stimulantTimer;

    [SerializeField] public static float _stimulantMultiplier = 0.2f;

    // Keeps track of the stimulant's cooldowns and durations. If the player clears a room, it will increment the _stimulantTimer, which will revert the speed buffs it provides to the player.
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

    // Multiplies the player's speed mulitplier found within playerStats by _stimulantMultiplier.
    private void UseActive()
    {
        playerStats PlayerStats = GlobalVariables._player.gameObject.GetComponent<playerStats>();
        PlayerStats._playerSpeedMultiplier += _stimulantMultiplier;
        PlayerStats._playerAttackSpeedMultiplier += _stimulantMultiplier;
    }
}
