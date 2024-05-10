using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangedPickup : MonoBehaviour
{
    [Header("Bullet and Gun stats")]
    [SerializeField] public float _gunBulletSpeed = 200f;
    [SerializeField] public int _gunDamage = 10;
    [SerializeField] public float _bulletLifetime = 3f;
    [SerializeField] public float _gunBulletSize;

    [Header("Gun Cooldowns")]
    [SerializeField] private float _gunCoolDown;
    [SerializeField] private float _reloadCooldown;

    [Header("Magazine Size")]
    [SerializeField] private int _magSize = 5;

    [SerializeField] public int _gunIndex;
    [SerializeField] public bool _gunOrQuadra;

    public void SwapLeft(GameObject player)
    {
        if(_gunOrQuadra == false)
        {
            _gunIndex = 0;
        }
        else
        {
            _gunIndex = 2;
        }

        playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
        if (wepInv._currentLeftIndex == _gunIndex)
        {
            playerRangedAttack leftStats = wepInv._currentLeftWeapon.GetComponent<playerRangedAttack>();

            leftStats._gunBulletSpeed = _gunBulletSpeed;
            leftStats._gunDamage = _gunDamage;
            leftStats._bulletLifetime = _bulletLifetime;
            leftStats._gunBulletSize = _gunBulletSize;

            leftStats._gunCoolDown = _gunCoolDown;
            leftStats._reloadCooldown = _reloadCooldown;

            leftStats._magSize = _magSize;
        }
        else
        {
            wepInv._currentLeftWeapon.SetActive(false);
            wepInv._currentLeftWeapon = wepInv._playerWeapons[_gunIndex];
            wepInv._currentLeftWeapon.SetActive(true);
            wepInv._currentLeftIndex = _gunIndex;

            playerRangedAttack leftStats = wepInv._currentLeftWeapon.GetComponent<playerRangedAttack>();

            leftStats._gunBulletSpeed = _gunBulletSpeed;
            leftStats._gunDamage = _gunDamage;
            leftStats._bulletLifetime = _bulletLifetime;
            leftStats._gunBulletSize = _gunBulletSize;

            leftStats._gunCoolDown = _gunCoolDown;
            leftStats._reloadCooldown = _reloadCooldown;

            leftStats._magSize = _magSize;
        }
        ObjectPooler.Despawn(this.gameObject);
    }

    public void SwapRight(GameObject player)
    {
        if (_gunOrQuadra == false)
        {
            _gunIndex = 1;
        }
        else
        {
            _gunIndex = 3;
        }

        playerWeaponInventory wepInv = player.GetComponent<playerWeaponInventory>();
        if (wepInv._currentRightIndex == _gunIndex)
        {
            playerRangedAttack rightStats = wepInv._currentRightWeapon.GetComponent<playerRangedAttack>();

            rightStats._gunBulletSpeed = _gunBulletSpeed;
            rightStats._gunDamage = _gunDamage;
            rightStats._bulletLifetime = _bulletLifetime;
            rightStats._gunBulletSize = _gunBulletSize;

            rightStats._gunCoolDown = _gunCoolDown;
            rightStats._reloadCooldown = _reloadCooldown;

            rightStats._magSize = _magSize;
        }
        else
        {
            wepInv._currentRightWeapon.SetActive(false);
            wepInv._currentRightWeapon = wepInv._playerWeapons[_gunIndex];
            wepInv._currentRightWeapon.SetActive(true);
            wepInv._currentRightIndex = _gunIndex;

            playerRangedAttack rightStats = wepInv._currentRightWeapon.GetComponent<playerRangedAttack>();

            rightStats._gunBulletSpeed = _gunBulletSpeed;
            rightStats._gunDamage = _gunDamage;
            rightStats._bulletLifetime = _bulletLifetime;
            rightStats._gunBulletSize = _gunBulletSize;

            rightStats._gunCoolDown = _gunCoolDown;
            rightStats._reloadCooldown = _reloadCooldown;

            rightStats._magSize = _magSize;
        }
        ObjectPooler.Despawn(this.gameObject);
    }

}
