using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator _animator;

    [Header("Attack Time Duration")]
    [SerializeField] private float _minSec = 3f;
    [SerializeField] private float _maxSec = 7f;

    private float _sec = 0f;
    private float _targetTime; // Unit: secs

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _targetTime = Random.Range(_minSec, _maxSec);
    }

    private void Update()
    {
        _sec += Time.deltaTime;
        if (_sec >= _targetTime)
        {
            _animator.SetTrigger("Attack");
            _targetTime = Random.Range(_minSec, _maxSec);
            _sec = 0;
        }
        //Debug.Log(_sec + " " + _targetTime);
    }
}
