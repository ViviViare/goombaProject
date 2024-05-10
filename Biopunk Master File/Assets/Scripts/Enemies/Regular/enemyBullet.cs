using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] public float _bulletSpeed;
    [SerializeField] public int _bulletDamage;
    [SerializeField] public float _bulletSize;

    [SerializeField] private float _despawnTimer = 3f;

    [SerializeField] private GameObject _player;

    void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        this.transform.LookAt(_player.transform.position);
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
    void OnTriggerEnter(Collider collision)
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
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}
