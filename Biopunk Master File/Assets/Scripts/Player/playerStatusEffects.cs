using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStatusEffects : MonoBehaviour
{
    [SerializeField] float _amplifierMultiplier = 0.4f;
    [SerializeField] float _serumMultiplier = 0.4f;

    [SerializeField] public int _amplifierDuration;
    [SerializeField] public int _serumDuration;

    [SerializeField] public bool _ampActive = false;
    [SerializeField] public bool _serumActive = false;

    private void Update()
    {
        if (_amplifierDuration == 0 && _ampActive)
        {
            _ampActive = false;
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerDamageMultiplier -= _amplifierMultiplier;

            PassiveItemManager._instance.PassiveRanOut(PickupType.Amplifier);
        }

        if(_serumDuration == 0 && _serumActive)
        {
            _serumActive = false;
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier -= _serumMultiplier;
            PlayerStats._playerAttackSpeedMultiplier -= _serumMultiplier;

            PassiveItemManager._instance.PassiveRanOut(PickupType.Serum);
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