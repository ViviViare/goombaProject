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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<playerController>() == null) return;
        
        if (_pickupType == PickupType.Health && other.gameObject.GetComponent<playerHealth>()._playerHealth < other.gameObject.GetComponent<playerHealth>()._playerMaxHealth)
        {
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerHealth += _healAmount;
            other.gameObject.GetComponent<playerHealth>()._playerHealthBar.GetComponent<Slider>().value = other.gameObject.GetComponent<playerHealth>()._playerHealth;
            other.gameObject.GetComponent<playerHealth>()._playerHealth = Mathf.Clamp(other.gameObject.GetComponent<playerHealth>()._playerHealth, 0, other.gameObject.GetComponent<playerHealth>()._playerMaxHealth);
            other.gameObject.GetComponent<playerHealth>()._playerHealthText.GetComponent<TextMeshProUGUI>().text = ("Health: " + other.gameObject.GetComponent<playerHealth>()._playerHealth + "/" + other.gameObject.GetComponent<playerHealth>()._playerMaxHealth);
        }
        else if (_pickupType == PickupType.Armour && other.gameObject.GetComponent<playerHealth>()._playerArmourStacks < 3)
        {
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerArmourStacks += _armourAmount;
            other.gameObject.GetComponent<playerHealth>()._playerArmourBar.GetComponent<Slider>().value = other.gameObject.GetComponent<playerHealth>()._playerArmourStacks;
        }
        else if (_pickupType == PickupType.Amplifier && other.gameObject.GetComponent<playerStatusEffects>()._ampActive == false)
        {
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().AmplifierBoost(_effectDuration);
            PassiveItemManager._instance.NewPassive(_pickupType);
        }
        else if (_pickupType == PickupType.Serum && other.gameObject.GetComponent<playerStatusEffects>()._serumActive == false)
        {
            ObjectPooler.Despawn(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().SerumBoost(_effectDuration);
            PassiveItemManager._instance.NewPassive(_pickupType);
        }

        
    }
}
