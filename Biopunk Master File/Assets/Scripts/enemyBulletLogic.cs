using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBulletLogic : MonoBehaviour
{
    [SerializeField] public float bulletSpeed;
    [SerializeField] public int bulletDamage;
    [SerializeField] public float bulletRange;
    [SerializeField] public float bulletSize;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<playerStatus>())
        {
            collision.gameObject.GetComponent<playerStatus>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.tag != "Enemy" || collision.gameObject.tag != "Bullet")
        {
            Destroy(this.gameObject);
        }
    }
}
