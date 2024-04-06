using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NapalmScript : MonoBehaviour
{
    [SerializeField] public int _napalmDamage;
    [SerializeField] public float _napalmRange;

    private void OnEnable()
    {
        playerActiveItem._activeAction += UseActive;
    }
    private void OnDisable()
    {
        playerActiveItem._activeAction -= UseActive;
    }


    private void UseActive()
    {
        Collider[] HitColliders = Physics.OverlapSphere(GlobalVariables._player.gameObject.transform.position, _napalmRange);
        foreach (var HitCollider in HitColliders)
        {
            if (HitCollider.gameObject.tag == "Player") return;
            if (HitCollider.gameObject.GetComponent<IDamageable>() != null)
            {
                HitCollider.gameObject.GetComponent<IDamageable>().Damage(_napalmDamage);
            }
        }

    }
}
