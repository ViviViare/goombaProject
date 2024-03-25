using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStatusEffects : MonoBehaviour
{
    [SerializeField] float _amplifierMultiplier = 0.4f;
    [SerializeField] float _serumMultiplier = 0.4f;

    [SerializeField] int _amplifierDuration;
    [SerializeField] int _serumDuration;

    [SerializeField] bool _ampActive = false;
    [SerializeField] bool _serumActive = false;

    private void Update()
    {
        if (GlobalVariables._roomsCleared == GlobalVariables._roomsCleared + 1)
        {
            _amplifierDuration--;
            _serumDuration--;
        }

        if (_amplifierDuration == 0 && _ampActive)
        {
            _ampActive = false;
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerDamageMultiplier -= _amplifierMultiplier;
        }

        if(_serumDuration == 0 && _serumActive)
        {
            _serumActive = false;
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier -= _serumMultiplier;
            PlayerStats._playerAttackSpeedMultiplier -= _serumMultiplier;
        }
    }


    public void AmplifierBoost(int duration)
    {
        this.gameObject.GetComponent<playerStats>()._playerDamageMultiplier += _amplifierMultiplier;
        _amplifierDuration = duration;
        _ampActive = true;
    }

    public void SerumBoost(int duration)
    {
        this.gameObject.GetComponent<playerStats>()._playerSpeedMultiplier += _amplifierMultiplier;
        this.gameObject.GetComponent<playerStats>()._playerAttackSpeedMultiplier += _amplifierMultiplier;
        _serumDuration = duration;
        _serumActive = true;
    }
}