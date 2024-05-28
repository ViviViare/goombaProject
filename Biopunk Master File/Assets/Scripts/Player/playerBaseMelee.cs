/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// An updated script from our December prototype; handles melee attacking for the player's melee weapons.
// This script has been updated to both be neater, reduce bloat, and to work with our new systems (object pooling and new input system).

// Edits since script completion:
// 05/03/24: Updated to use actions instead of the original firing method.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using Unity.Rendering;
using UnityEngine.Rendering;

public class playerBaseMelee : MonoBehaviour
{
    [SerializeField] public Animator _meleeAnimator;
    [SerializeField] public Camera _playerCam;

    [Header("Melee Stats")]
    [SerializeField] public float _meleeSwingSpeed;
    [SerializeField] public float _meleeDamage;
    [SerializeField] public float _meleeRange;

    [SerializeField] public int _specialDamage;
    [SerializeField] public float _specialCooldown;
    [SerializeField] public bool _canUseSpecial = true;
    [SerializeField] public bool _isUsingSpecial;

    [SerializeField] public float _vampHealAmount;


    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _canSwing = true;

    [SerializeField] private float _swingCooldown = 2f;

    [SerializeField] public GameObject _player;

    [SerializeField] public GameObject _meleeHitSphereCenter;

    [SerializeField] List<GameObject> _objectsToDamage = new List<GameObject>();

    [SerializeField] private GameObject _overlayCam;

    

    public LeftOrRight _weaponsSide;
    private void OnEnable()
    {
        playerWeaponHandler._triggerAction += SwingMeleeWeapon;
    }
    private void OnDisable()
    {
        playerWeaponHandler._triggerAction -= SwingMeleeWeapon;
    }
    // Start is called before the first frame update. This automatically sets the script's required variables and objects at runtime.
    void Start()
    {
        //_meleeAnimator = this.GetComponent<Animator>();
        _playerCam = Camera.main;
        _overlayCam.SetActive(false);
    }

    // The below three methods all work in tandem with eachother, using animation events and triggers to perform a melee swing from the weapon.
    public void SwingMeleeWeapon(LeftOrRight direction)
    {
        if(_canSwing && _weaponsSide == direction)
        {
            _meleeAnimator.SetBool("attack", true);
            _isAttacking = true;
        }

        if (_canUseSpecial && direction == LeftOrRight.Left)
        {
            FireSpecial();
        }
    }

    public void StopSwing()
    {
        _meleeAnimator.SetBool("attack", false);
        _isAttacking = false;
    }

    public void MeleeRaycast()
    {
        Collider[] HitColliders = Physics.OverlapSphere(_meleeHitSphereCenter.transform.position, _meleeRange);
        foreach (var HitCollider in HitColliders)
        {
            if (HitCollider.gameObject.tag == "Player") continue;
            if (HitCollider.gameObject.GetComponent<IDamageable>() != null)
            {
                int calculatedDamage = (int)(_meleeDamage * _player.GetComponent<playerStats>()._playerDamageMultiplier);
                HitCollider.gameObject.GetComponent<IDamageable>().Damage(calculatedDamage);
            }
        }

        StartCoroutine(MeleeCooldown());
    }

    // The below three methods handle the claw's "special attack", which in this case is a dash that damages every enemy the player made contact with during the dash.
    
    // Only allows the player to use the special when it's off cooldown, otherwise does nothing
    public virtual void FireSpecial()
    {
        if (!_canUseSpecial) return;
        playerController dashScript = _player.GetComponent<playerController>();
        _objectsToDamage.Clear();
        StartCoroutine(ClawDash(dashScript._dashLength));
        dashScript.PlayerDash();
        StartCoroutine(SpecialCooldown());
    }

    // Although the claw's special calls the playercontroller's dash function for the actual dash, this below method handles the enabling/disable of the damage trigger while dashing
    private IEnumerator ClawDash(float dashtime)
    {
        float adjustedDashTime = (float)(dashtime + 0.1);
        _isUsingSpecial = true;
        _overlayCam.SetActive(true);
        yield return new WaitForSeconds(adjustedDashTime);
        _overlayCam.SetActive(false);
        _isUsingSpecial = false;
        foreach (GameObject obj in _objectsToDamage)
        {
            obj.GetComponent<IDamageable>().Damage(_specialDamage);
        }
        
    }

    // Only runs when the player is dashing (dictated by ClawDash above). Adds anything that the player collides with to a list of objects, which is then damaged at the end of ClawDash
    private void OnTriggerEnter(Collider other)
    {
        if (_isUsingSpecial == false) return;
        Debug.Log($"{other.name}. Special is on");
        if (other.gameObject.tag == "Player") return;
        Debug.Log($"{other.name} is not the player");

        if (other.gameObject.GetComponent<IDamageable>() == null) return;
        Debug.Log($"{other.name} has a damageable");


        if (_objectsToDamage.Contains(other.gameObject)) return;
        _objectsToDamage.Add(other.gameObject);
        Debug.Log("Added to List");
    }


    // Below method is called when a swing ends, and simply prevents further attacks based on a cooldown variable.
    public IEnumerator MeleeCooldown()
    {
        _canSwing = false;
        yield return new WaitForSeconds(_swingCooldown);
        _canSwing = true;
    }

    public IEnumerator SpecialCooldown()
    {
        _canUseSpecial = false;
        yield return new WaitForSeconds(_specialCooldown);
        _canUseSpecial = true;
    }

}
