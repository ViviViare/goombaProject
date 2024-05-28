using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class floorPickup : MonoBehaviour
{
    [SerializeField] public PickupType _pickupType;

    [SerializeField] public int _healAmount = 30;
    [SerializeField] public int _armourAmount = 1;

    [SerializeField] public int _effectDuration;

    [SerializeField] public int _pickupWeight;

    [SerializeField] public AudioClip _clipToPlay;



    // The below mess of a method handles what happens when the player walks over a floor pickup.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<playerController>() == null) return;
        
        if (_pickupType == PickupType.Health && other.gameObject.GetComponent<playerHealth>()._playerHealth < other.gameObject.GetComponent<playerHealth>()._playerMaxHealth)
        {
            // If the pickup is a health pickup, it will heal the player for whatever the health pickup's heal value is and play a sound effect.
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerHealth += _healAmount;
            other.gameObject.GetComponent<playerHealth>()._playerHealthBar.GetComponent<Slider>().value = other.gameObject.GetComponent<playerHealth>()._playerHealth;
            other.gameObject.GetComponent<playerHealth>()._playerHealth = Mathf.Clamp(other.gameObject.GetComponent<playerHealth>()._playerHealth, 0, other.gameObject.GetComponent<playerHealth>()._playerMaxHealth);
            other.gameObject.GetComponent<playerHealth>()._playerHealthText.GetComponent<TextMeshProUGUI>().text = ("Health: " + other.gameObject.GetComponent<playerHealth>()._playerHealth + "/" + other.gameObject.GetComponent<playerHealth>()._playerMaxHealth);
        }
        else if (_pickupType == PickupType.Armour && other.gameObject.GetComponent<playerHealth>()._playerArmourStacks < 3)
        {
            // If the pickup is an armour pickup, it will add one stack of protective armour to the player (limited to three stacks)
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerArmourStacks += _armourAmount;
            other.gameObject.GetComponent<playerHealth>()._playerArmourBar.GetComponent<Slider>().value = other.gameObject.GetComponent<playerHealth>()._playerArmourStacks;
        }
        else if (_pickupType == PickupType.Amplifier && other.gameObject.GetComponent<playerStatusEffects>()._ampActive == false)
        {
            // If the pickup is an amplifier, it will buff the player's damage by 40% until they clear a room.
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().AmplifierBoost(_effectDuration);
            PassiveItemManager._instance.NewPassive(_pickupType);
        }
        else if (_pickupType == PickupType.Serum && other.gameObject.GetComponent<playerStatusEffects>()._serumActive == false)
        {
            // If the pickup is a serum, it will buff the player's speed by 40% until they clear a room.
            AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().SerumBoost(_effectDuration);
            PassiveItemManager._instance.NewPassive(_pickupType);
        }

        
    }
}
