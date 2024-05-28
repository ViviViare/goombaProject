/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Updated ranged attack script from our December prototype, debloated and polished up to work with our new systems (new input system, object pooler)

// Edits since script completion:
// 05/03/24: Updated to use actions instead of the original firing method.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class playerRangedAttack : MonoBehaviour
{
    [Header("Bullet and Gun stats")]
    [SerializeField] GameObject _playerBullet;
    [SerializeField] public float _gunBulletSpeed = 200f;
    [SerializeField] public int _gunDamage = 10;
    [SerializeField] public float _bulletLifetime = 3f;
    [SerializeField] public float _gunBulletSize;

    [Header("Gun Cooldowns")]
    [SerializeField] public float _gunCoolDown;
    [SerializeField] public float _reloadCooldown;

    [Header("Magazine Size")]
    [SerializeField] public int _shotsFired = 0;
    [SerializeField] public int _magSize = 5;


    [SerializeField] public Camera _playerCam;

    [SerializeField] public bool _canFire = true;
    [SerializeField] public bool _canUseSpecial;

    [SerializeField] public float _specialDuration;
    [SerializeField] public float _specialCooldown;

    [SerializeField] public GameObject _gunBarrel;

    [SerializeField] public Vector3 _targetPoint;
    [SerializeField] public bool _useTargetPoint;
    [SerializeField] public bool _bulletHasTracking;

    [SerializeField] public ParticleSystem muzzleVFX;

    [SerializeField] public AudioClip _clipToPlay;
    [SerializeField] public AudioClip _specialSound;

    public LeftOrRight _weaponsSide;

    private void OnEnable()
    {
        playerWeaponHandler._triggerAction += FireRangedWeapon;
    }
    private void OnDisable()
    {
        playerWeaponHandler._triggerAction -= FireRangedWeapon;
    }

    // Grabs a referance to the player's camera and the weapon gameobject's gun barrel object, to fire bullets from.
    void Start()
    {
        _playerCam = Camera.main;
        _gunBarrel = this.gameObject.transform.GetChild(0).gameObject;
    }

    public virtual void FireRangedWeapon(LeftOrRight direction)
    {
        if (GlobalVariables._gamePaused == true) return;
        if (_canFire && _weaponsSide == direction)
        {
            StartCoroutine(RangedCoroutine());
        }
        else if (_weaponsSide != direction)
        {
            StartSpecial();
        }
    }

    // Handles the actual firing of the player's ranged weapon. First, it fires a raycast from the player's camera; if this raycast collides with any object, it sets the
    // gun's "targetPoint" vector to whatever the hit point is, and tells the gun to use that target vector. Otherwise, it tells the gun to not use the target point vector, and spawns a bullet
    // using the object pooler script.

    // Further detail on what this target point is used for can be found commented on the "bulletLogic" script.

    // After the bullet is spawned, it also incraments a "_shotsFired" variable. Upon this _shotsFired variable reaching the weapon's designated "_magSize" variable,
    // it triggers a "reloading" coroutine automatically, to give our weapons a magazine and reloading system.
    // Weapons have a natural "between-shot" cooldown as well, which is shorter than the reloading cooldown. This is to prevent a player from firing too many bullets too quickly.
    public IEnumerator RangedCoroutine()
    {
        muzzleVFX.Play();
        AudioSource.PlayClipAtPoint(_clipToPlay, this.gameObject.transform.position);
        _canFire = false;
        Vector3 rayOrigin = _playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, _playerCam.transform.forward, out hit))
        {
            if (_bulletHasTracking == true)
            {
                _useTargetPoint = true;
            }
            _targetPoint = hit.point;
            ObjectPooler.Spawn(_playerBullet, _gunBarrel.transform.position, _gunBarrel.transform.rotation);
        }
        else
        {
            _useTargetPoint = false;
            ObjectPooler.Spawn(_playerBullet, _gunBarrel.transform.position, _gunBarrel.transform.rotation);
        }
        _shotsFired++;

        yield return new WaitForSeconds(_gunCoolDown);

        if (_shotsFired >= _magSize)
        {
            StartCoroutine(ReloadCooldown());
        }
        else
        {
            _canFire = true;
        }
    }

    // Below methods handle the base gun's special attack.

    // If the special attack is off-cooldown, starts the RangedSpecial coroutine.
    private void StartSpecial()
    {
        if (_canUseSpecial == false) return;
        _canUseSpecial = false;
        StartCoroutine(RangedSpecial());
    }

    // Resets the player's _shotsFired variable, increases the gun's magazine size and reduces the gun's cooldown between shots.
    // This special essentially acts as an "overcharge" for the gun.
    public IEnumerator RangedSpecial()
    {
        float origGunCooldown = _gunCoolDown;
        int origMagSize = _magSize;

        AudioSource.PlayClipAtPoint(_specialSound, this.transform.position, 100);

        _shotsFired = 0;

        _gunCoolDown = _gunCoolDown / 2;
        _magSize = _magSize * 2;

        yield return new WaitForSeconds(_specialDuration);

        _gunCoolDown = origGunCooldown;
        _magSize = origMagSize;

        StartCoroutine(SpecialCooldown());
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator SpecialCooldown()
    {
        yield return new WaitForSeconds(_specialCooldown);
        _canUseSpecial = true;
    }

    private IEnumerator ReloadCooldown()
    {
        _canFire = false;
        yield return new WaitForSeconds(_reloadCooldown);
        _shotsFired = 0;
        _canFire = true;
    }
}
