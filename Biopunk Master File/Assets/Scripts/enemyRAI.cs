using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class enemyRAI : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject enemBullet;
    [SerializeField] GameObject gunBarrel;

    [SerializeField] float enemySpeed = 2f;
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyBulletSpeed;
    [SerializeField] float enemyRange;
    [SerializeField] float enemyProjectileSize;
    [SerializeField] float enemyFleeDistance = 12f;

    [SerializeField] bool isTriggered = true;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttacking;

    [SerializeField] NavMeshAgent rangedAgent;

    [SerializeField] private Coroutine rangedCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        rangedAgent = this.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        rangedAgent.speed = enemySpeed;
        gunBarrel = this.gameObject.transform.GetChild(1).gameObject;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        if (isTriggered == true)
        {
            rangedAgent.destination = target.GetComponent<Transform>().position;
            float distanceToPlayer = Vector3.Distance(target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);
            if (distanceToPlayer <= enemyRange && canAttack)
            {
                canAttack = false;
                isAttacking = true;
                rangedCoroutine = StartCoroutine(FireGun());
            }
            else if (distanceToPlayer > enemyRange && isAttacking == true)
            {
                isAttacking = false;
                canAttack = true;
                StopCoroutine(rangedCoroutine);
            }
            if (distanceToPlayer <= enemyFleeDistance)
            {
                transform.Translate(UnityEngine.Vector3.back * enemySpeed * 1.5f * Time.deltaTime);
            }

        }
    }

    public IEnumerator FireGun()
    {
        while (target.GetComponent<playerStatus>().playerHP > 0)
        {
            GameObject bullet = Instantiate(enemBullet, gunBarrel.transform.position, gunBarrel.transform.rotation);
            bullet.GetComponent<enemyBulletLogic>().bulletDamage = (int)enemyDamage;
            bullet.GetComponent<enemyBulletLogic>().bulletRange = enemyRange;
            bullet.GetComponent<enemyBulletLogic>().bulletSize = enemyProjectileSize;
            bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, enemyBulletSpeed, 0));
            yield return new WaitForSeconds(2f);
        }

    }
}
