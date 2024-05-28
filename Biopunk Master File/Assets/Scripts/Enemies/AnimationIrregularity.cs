using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationIrregularity : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private float _variance = 3;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        float rand = Mathf.Abs( Random.Range(-_variance, _variance) );
        float adjustedRand = 1 + (rand / 10);

        _animator.speed = adjustedRand;
    }
}
