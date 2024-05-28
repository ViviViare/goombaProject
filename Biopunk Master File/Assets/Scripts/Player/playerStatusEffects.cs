using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Handles the timers and functionality of both "boosting" items in the game, the amplifier and the serum.

//
*/
public class playerStatusEffects : MonoBehaviour
{
    [SerializeField] float _amplifierMultiplier = 0.4f;
    [SerializeField] float _serumMultiplier = 0.4f;

    [SerializeField] public int _amplifierDuration;
    [SerializeField] public int _serumDuration;

    [SerializeField] public bool _ampActive = false;
    [SerializeField] public bool _serumActive = false;

    // If the _amplifierDuration is zero and the amplifier is currently active, it will revert the changes it has made to the player's base damage.
    // Likewise, if _serumDuration is zero and it is currently active, it will do the same for the changes it has made to the player's base speed.
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

    // Buffs the player's damage for an amount of rooms equal to whatever "duration" is.
    // SerumBoost does the same, save for buffing the player's speed instead of their damage.
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