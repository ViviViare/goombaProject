/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// An updated script from our December prototype; handles melee attacking for the player's melee weapons.
// This script has been updated to both be neater, reduce bloat, and to work with our new systems (object pooling and new input system).

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBaseMelee : MonoBehaviour
{
    [SerializeField] public Animator _meleeAnimator;
    [SerializeField] public Camera _playerCam;

    [Header("Melee Stats")]
    [SerializeField] public int _meleeSwingSpeed;
    [SerializeField] public int _meleeDamage;
    [SerializeField] public int _meleeRange;

    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _canSwing = true;

    [SerializeField] private float _swingCooldown = 2f;

    // Start is called before the first frame update. This automatically sets the script's required variables and objects at runtime.
    void Start()
    {
        _meleeAnimator = this.GetComponent<Animator>();
        _playerCam = Camera.main;
    }

    // The below three methods all work in tandem with eachother, using animation events and triggers to perform a melee swing from the weapon.
    public void SwingMeleeWeapon()
    {
        if(_canSwing)
        {
            _meleeAnimator.SetBool("attack", true);
            _isAttacking = true;
        }
    }
    public void StopSwing()
    {
        _meleeAnimator.SetBool("attack", false);
        _isAttacking = false;
    }

    public void MeleeRaycast()
    {
        Vector3 rayOrigin = _playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit wepSwing;
        if (Physics.Raycast(rayOrigin, _playerCam.transform.forward, out wepSwing, _meleeRange))
        {
            IDamageable damageable = wepSwing.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(_meleeDamage);
            }
        }
        StartCoroutine(MeleeCooldown());
    }

    // Below method is called when a swing ends, and simply prevents further attacks based on a cooldown variable.
    public IEnumerator MeleeCooldown()
    {
        _canSwing = false;
        yield return new WaitForSeconds(_swingCooldown);
        _canSwing = true;
    }
}
