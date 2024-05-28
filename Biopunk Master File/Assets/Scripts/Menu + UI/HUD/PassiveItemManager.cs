/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 02/05/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  PassiveItemManager.cs
//
//  This script manages the passive items so that when the player picks up a new passive item it 
//  slots itself into one of the possible two slots.
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveItemManager : MonoBehaviour
{
    public static PassiveItemManager _instance;
    private void Awake()
    {
        _instance = this;
    }
    
    #region Lists
    [SerializeField] private List<PassiveItemFill> _itemSlots = new List<PassiveItemFill>();
    private List<PassiveItemFill> _itemSlotsUnused = new List<PassiveItemFill>();
    private List<PassiveItemFill> _itemSlotsUsed = new List<PassiveItemFill>();
    [SerializeField] private List<ExposedDictionary<PickupType, Sprite>> _pickupImages = new List<ExposedDictionary<PickupType, Sprite>>();

    [System.Serializable]
    public class ExposedDictionary<key, value>
    {
        public key _key;
        public value _value;
    }
    #endregion
    
    private playerStatusEffects _passiveEffects;

    private void Start()
    {
        _itemSlotsUnused = _itemSlots;
        _passiveEffects = GlobalVariables._player.GetComponent<playerStatusEffects>();
    }

    public void NewPassive(PickupType newPickup)
    {
        Sprite pickupImage = GetDictionaryValue(newPickup);
        PassiveItemFill itemHolder = _itemSlotsUnused[0];
        itemHolder.gameObject.SetActive(true);
        itemHolder._currentPassive = newPickup;

        _itemSlotsUsed.Add(itemHolder);
        _itemSlotsUnused.Remove(itemHolder);
        
        SetupPassive(itemHolder, pickupImage, newPickup);

        switch (newPickup)
        {
            case PickupType.Amplifier:
                StatIncreaseUIHandler._instance.IncreaseDamageStat(40);
                break;
            case PickupType.Serum:
                StatIncreaseUIHandler._instance.IncreaseSpeedStat(40);
                break;
            default:
                break;
        }
    }

    public void UpdatePassive(PickupType typeToUpdate, float amount)
    {
        PassiveItemFill currentFill = GetFillHolder(typeToUpdate);

        currentFill.UpdateChargeAmount(amount);
    }


    // Setup the passive item UI based on whether it is an Amplifier or a Serum.
    private void SetupPassive(PassiveItemFill itemHolder, Sprite image, PickupType pickupType)
    {
        float maxDuration = default;
        if (pickupType == PickupType.Amplifier)
        {
            maxDuration = _passiveEffects._amplifierDuration;
        }
        else if (pickupType == PickupType.Serum)
        {
            maxDuration = _passiveEffects._serumDuration;
        }

        itemHolder.UpdatePassive(image, maxDuration);
    }

    private Sprite GetDictionaryValue(PickupType type)
    {
        // Loop through every entry in the pickup images list until the correct image is found
        foreach (var entry in _pickupImages)
        {
            if (entry._key != type) continue;

            return(entry._value);
        }

        return null;        
    }

    public void PassiveRanOut(PickupType pickupType)
    {
        PassiveItemFill fillHolder = GetFillHolder(pickupType);

        fillHolder.TogglePassiveSprite(false);

        _itemSlotsUsed.Remove(fillHolder);
        _itemSlotsUnused.Add(fillHolder);

        switch (pickupType)
        {
            case PickupType.Amplifier:
                StatIncreaseUIHandler._instance.DecreaseDamageStat(40);
                break;
            case PickupType.Serum:
                StatIncreaseUIHandler._instance.DecreaseSpeedStat(40);
                break;
            default:
                break;
        }
    }

    private PassiveItemFill GetFillHolder(PickupType pickupType)
    {
        foreach (PassiveItemFill currentFill in _itemSlotsUsed)
        {
            if (currentFill._currentPassive != pickupType) continue; 
            
            return currentFill;
            
        }

        return null;
    }



}
