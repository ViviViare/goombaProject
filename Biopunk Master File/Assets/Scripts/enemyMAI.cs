using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyMAI : MonoBehaviour
{
    [SerializeField] GameObject target;

    [SerializeField] float enemySpeed = 2f;
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyRange;
    [SerializeField] float enemyTriggerRange;

    [SerializeField] bool isTriggered = true;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttacking;

    [SerializeField] NavMeshAgent meleeAgent;

    [SerializeField] private Coroutine meleeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        meleeAgent = this.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        meleeAgent.speed = enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered == true)
        {
            meleeAgent.destination = target.GetComponent<Transform>().position;
            float distanceToPlayer = Vector3.Distance(target.GetComponent<Transform>().position, this.GetComponent<Transform>().position);
            if (distanceToPlayer <= enemyRange && canAttack)
            {
                meleeCoroutine = StartCoroutine(MeleeAttack());
                canAttack = false;
                isAttacking = true;
            }
            else if (distanceToPlayer > enemyRange && isAttacking == true)
            {
                isAttacking = false;
                canAttack = true;
                StopCoroutine(meleeCoroutine);
            }
        }
    }

    public IEnumerator MeleeAttack()
    {
        while (target.GetComponent<playerStatus>().playerHP > 0)
        {
            target.GetComponent<playerStatus>().TakeDamage(enemyDamage);
            yield return new WaitForSeconds(2f);
        }
    }
}
