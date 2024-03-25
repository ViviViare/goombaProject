using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorPickup : MonoBehaviour
{
    [SerializeField] public PickupType _pickupType;

    [SerializeField] public int _healAmount = 30;
    [SerializeField] public int _armourAmount = 1;

    [SerializeField] public int _effectDuration;
    public int _pickupWeight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<playerController>() == null) return;
        if(_pickupType == PickupType.Health)
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerHealth += _healAmount;
        }
        else if (_pickupType == PickupType.Armour)
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<playerHealth>()._playerArmourStacks += _armourAmount;
        }
        else if (_pickupType == PickupType.Amplifier)
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().AmplifierBoost(_effectDuration);
        }
        else if (_pickupType == PickupType.Serum)
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<playerStatusEffects>().SerumBoost(_effectDuration);
        }
    }
}
