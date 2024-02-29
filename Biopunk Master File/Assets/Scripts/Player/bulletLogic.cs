/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// A script for the player's projectiles that tells them what stats to inherit, when to do damage and when to despawn themselves.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class bulletLogic : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] public float _bulletSpeed;
    [SerializeField] public int _bulletDamage;
    [SerializeField] public float _bulletSize;

    [SerializeField] private float _despawnTimer = 3f;

    [SerializeField] private GameObject _player;

    /*
    // Traditional variable inheritance, as created for the December prototype, is not achievable with our new Object Pooler; as instead of creating a local instance of a bullet in
    // the firing method (which can then be fed variables from the player's weapon) it is spawned seperately via the new Object Pooler script. This would require a bit of work 
    // to feed variables into, and generally make the code messy and disorganized (as the object pooler should act completely seperately to the weapon scripts, instead of
    // relying on variables from said scripts when those variables are only useful for the player's projectiles.)

    // The below method fixes this issue, albeit in a bit of a longwinded way. Essentially, the player's weaponhandler script (which handles the firing methods for all of the player's weapons)
    // sets a "left or right" variable after every method; if the player fires their left weapon, this is set to "left" and vice versa. This variable is then fed into the bulletlogic script,
    // which then uses tags to grab the player's currently active left/right weapon (depending on whichever was last fired). It then grabs the relevant and required stats, such as damage, size
    // etcetera.

    // There may or may not be a more efficient or simple way to handle this, but this method is pretty robust and self-contained. 
    // This method also starts a "despawn" coroutine in case the bullet goes out of bounds or doesn't collide with anything; this comes with the added bonus of us being able to 
    // tweak the "despawn timer" value, which allows us to effectively control how far bullets travel.

    // Finally, it also checks to see if the "useTargetPoint" bool on the ranged weapon is true; if so, it will make the the bullet look towards (and subsequently travel towards) the
    // target point vector detailed in the "playerRangedAttack" script. This is to avoid the annoying issue encountered in the December prototype where the bullets would not travel
    // towards the player's crosshair, as they simply spawned and accelerated in the direction of the gun barrel (which made it so there would always be certain distances where a
    // bullet would veer off-course).
    */
    void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player.GetComponent<playerWeaponHandler>()._leftOrRight == playerWeaponHandler.LeftOrRight.Left)
        {
            GameObject leftGun = GameObject.FindGameObjectWithTag("Left Gun");
            playerRangedAttack leftGunStats = leftGun.GetComponent<playerRangedAttack>();
            
            _bulletSpeed = leftGunStats._gunBulletSpeed;
            _bulletDamage = leftGunStats._gunDamage;
            _bulletSize = leftGunStats._gunBulletSize;
            _despawnTimer = leftGunStats._bulletLifetime;
            
            if(leftGunStats._useTargetPoint == true)
            {
                this.gameObject.transform.LookAt(leftGunStats._targetPoint);
            }
        }
        else
        {
            GameObject rightGun = GameObject.FindGameObjectWithTag("Right Gun");
            playerRangedAttack rightGunStats = rightGun.GetComponent<playerRangedAttack>();
            
            _bulletSpeed = rightGunStats._gunBulletSpeed;
            _bulletDamage = rightGunStats._gunDamage;
            _bulletSize = rightGunStats._gunBulletSize;
            _despawnTimer= rightGunStats._bulletLifetime;
            
            if (rightGunStats._useTargetPoint == true)
            {
                this.gameObject.transform.LookAt(rightGunStats._targetPoint);
            }
        }
        this.transform.localScale = new Vector3(_bulletSize, _bulletSize, _bulletSize);
        StartCoroutine(BulletDespawnTimer());
    }
    // Update is called once per frame. This just makes the bullet go forward based on its speed.
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * _bulletSpeed;
    }

    // Below method checks to see if the collided object has an IDamageable component; if so, it deals damage based on the _bulletDamage stat and despawns itself.
    // If the collided object doesn't have an IDamageable component, the bullet simply despawns.
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(_bulletDamage);
            ObjectPooler.Despawn(this.gameObject);
        }
        else
        {
            ObjectPooler.Despawn(this.gameObject);
        }
    }

    IEnumerator BulletDespawnTimer()
    {
        yield return new WaitForSeconds(_despawnTimer);
        ObjectPooler.Despawn(this.gameObject);
    }
    void OnDisable()
    {
        Debug.Log("why");
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}
