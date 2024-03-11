using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorPickup : MonoBehaviour
{
    [SerializeField] public PickupType _pickupType;

    [SerializeField] public int _healAmount = 30;
    [SerializeField] public int _armourAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<playerController>() != null)
        {
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
        }
    }
}
