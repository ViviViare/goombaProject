using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float _despawnTimer = 0.6f;

    private void OnEnable()
    {
        StartCoroutine(DespawnTimer());
        _particle.Play();
    }

    private void OnDisable()
    {
        _particle.Clear();
        StopAllCoroutines();
    }

    private IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(_despawnTimer);
        ObjectPooler.Despawn(this.gameObject);
    }
}
