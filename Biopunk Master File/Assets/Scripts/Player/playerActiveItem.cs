using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActiveItem : MonoBehaviour
{
    [SerializeField] public bool _hasActiveItem = false;
    [SerializeField] public string _currentItem;

    [SerializeField] public int _activeItemMaxCharge;
    [SerializeField] public int _activeItemCharge;

    [SerializeField] public int _medkitHeal = 30;

    [SerializeField] public int _napalmDamage;
    [SerializeField] public float _napalmRange;

    [SerializeField] public bool _usedStimulants;

    private void Update()
    {
        if(GlobalVariables._roomsCleared == GlobalVariables._roomsCleared++)
        {
            _activeItemCharge++;
        }

        if(_activeItemCharge == _activeItemCharge + 1 && _usedStimulants)
        {
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier -= 0.2f;
            PlayerStats._playerAttackSpeedMultiplier -= 0.2f;
        }
    }

    public void OnActiveUsage()
    {
        if (_activeItemCharge < _activeItemMaxCharge || _hasActiveItem == false) return;

        _activeItemCharge = 0;

        if(_currentItem == "MediPak")
        {
            this.gameObject.GetComponent<playerHealth>()._playerHealth += _medkitHeal;
        }
        else if(_currentItem == "Stimulant")
        {
            playerStats PlayerStats = this.gameObject.GetComponent<playerStats>();
            PlayerStats._playerSpeedMultiplier += 0.2f;
            PlayerStats._playerAttackSpeedMultiplier += 0.2f;
        }
        else if(_currentItem == "Experimental Napalm")
        {
            Collider[] HitColliders = Physics.OverlapSphere(this.gameObject.transform.position, _napalmRange);
            foreach (var HitCollider in HitColliders)
            {
                if (HitCollider.gameObject.tag == "Player") return;
                if(HitCollider.gameObject.GetComponent<IDamageable>() != null)
                {
                    HitCollider.gameObject.GetComponent<IDamageable>().Damage(_napalmDamage);
                }
            }
        }
    }
}
