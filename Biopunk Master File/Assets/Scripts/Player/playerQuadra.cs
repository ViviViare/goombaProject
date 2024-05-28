/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 26/02/24

// A smaller script that inherits from the playerRangedAttack script. This is used for our "Rusty Quadra" weapon, and handles its unique firing method.

// Edits since script completion:
// 05/03/24: Updated to use actions instead of the original firing method.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerQuadra : playerRangedAttack
{
    [Space]
    [Header("Quadra Vars")]
    [SerializeField] private List<GameObject> _gunBarrels = new List<GameObject>();
    [SerializeField] private int _gunBarrelsCount = 6;

    [SerializeField] public float _railgunCooldown;
    [SerializeField] public GameObject _railgunBullet;
    [SerializeField] public bool _railgunOnCooldown;

    [SerializeField] public float _railBulletSpeed = 200f;
    [SerializeField] public int _railDamage = 10;
    [SerializeField] public float _railLifetime = 3f;
    [SerializeField] public float _railBulletSize;

    [SerializeField] public GameObject _quadraObject;

    // Start populates a "_gunBarrels" list with a for loop, that goes through every child gameobject of the quadra gameobject and adds them to the list.
    // The only children of the quadra should be its multiple gun barrels, meaning that the list should be filled with each of the quadra's gun barrels.

    void Start()
    {
        for(int i = 0; i < _gunBarrelsCount; i++)
        {
            GameObject currentBarrel = this.gameObject.transform.GetChild(i).gameObject;
            _gunBarrels.Add(currentBarrel);
        }
    }

    // Handles firing. If the playerWeaponHandler broadcasts a "left", it will make the Quadra fire normally.
    // If it broadcasts a "right", it will make the Quadra fire its special move.

    public override void FireRangedWeapon(LeftOrRight direction)
    {
        if (GlobalVariables._gamePaused == true) return;
        if(_weaponsSide == direction)
        {
            FireQuarda();
        }
        else
        {
            FireRailgun();
        }
    }

    // When called, and if the player can shoot, this method essentially loops through the default RangedCoroutine on the playerRangedAttack script
    // an equal amount of times to the _gunBarrelsCount variable. With each iteration of the loop, the playerRangedAttack's _gunBarrel gameobject (which controls where a bullet
    // is spawned) is changed to the current index of _gunBarrels (which is whatever point in the loop the loop is currently at).
    // This is repeated until the loop ends, and results in every barrel of the quadra spawning a bullet, acting as a shotgun.

    // There are certain minor issues, namely that every bullet will still converge on the same point despite being a shotgun (thanks to simply looping the basic firing method)
    // Additionally, this means that the reloading system gets a little broken too, as the "_shotsFired" variable (which keeps track of how many bullets have been fired
    // to force a reload once that variable reaches the "_magSize" variable) incraments itself an amount equal to the _gunBarrelsCount variable.

    // Basically, it means if you want to change the magazine size for the quadra you have to multiply the _magSize variable by whatever _gunBarrelsCount is;
    // if you want the quadra to have a magazine size of three, you have to set the _magSize variable to 3*(_gunBarrelsCount). Otherwise the quadra will fire, immediately incrament
    // the _shotsFired variable by 6 and then force a reload due to _shotsFired being 6 and _magSize being 3.
    public void FireQuarda()
    {
        if(_canFire == true)
        {
            for (int i = 0; i < _gunBarrelsCount; i++)
            {
                _gunBarrel = _gunBarrels[i];
                StartCoroutine(RangedCoroutine());
            }
        }
    }

    // The Quadra's special attack; a longer-ranged single shot attack with a long cooldown that prevents the player from firing at all for a few seconds.
    // Serves as an option for the player to attack from further away, not having to risk taking damage from coming closer, at the cost of lower damage per second and a high cooldown.
    public void FireRailgun()
    {
        if (_railgunOnCooldown == true) return;
        if (_canFire == false) return;
        StartCoroutine(RailgunCooldown());
        _railgunOnCooldown = true;
        _canFire = false;
        muzzleVFX.Play();
        Vector3 rayOrigin = _playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, _playerCam.transform.forward, out hit))
        {
            _useTargetPoint = true;
            _targetPoint = hit.point;
            ObjectPooler.Spawn(_railgunBullet, _gunBarrel.transform.position, _gunBarrel.transform.rotation);
        }
        else
        {
            _useTargetPoint = false;
            ObjectPooler.Spawn(_railgunBullet, _gunBarrel.transform.position, _gunBarrel.transform.rotation);
        }
    }

    IEnumerator RailgunCooldown()
    {
        yield return new WaitForSeconds(_railgunCooldown);
        _railgunOnCooldown = false;
        _canFire = true;
    }
}
