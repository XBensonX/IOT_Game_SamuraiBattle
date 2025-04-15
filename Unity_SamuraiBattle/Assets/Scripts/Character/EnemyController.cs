using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _hurtClip;

    [Header("Attack Time Duration")]
    [SerializeField] private float _minSec = 3f;
    [SerializeField] private float _maxSec = 7f;

    private float _sec = 0f;
    private float _targetTime; // Unit: secs

    private void Start()
    {
        _animator = GetComponent<Animator>();
        GetComponent<HPSystem>().enabled = true;

        _targetTime = Random.Range(_minSec, _maxSec);
    }

    private void Update()
    {
        _sec += Time.deltaTime;
        if (_sec >= _targetTime) // holding the button
        {
            Attack();
            _targetTime = Random.Range(_minSec, _maxSec); // Random to decide the next attack in time
            _sec = 0;
        }
        //Debug.Log(_sec + " " + _targetTime);
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");

        if (_attackClip)
        {
            GameManager.instance.PlaySFX_fromAudioClip(_attackClip);
        }
    }

    public void AttackedOnAnimationFrame()
    {
        if (!PlayerController.instance.isBlocking)
        {
            PlayerController.instance.Damage(1);
        }
        else
        {
            GameManager.instance.ShowBlockedState();
        }
    }

    public void Damage(int damage)
    {
        if (_hurtClip)
        {
            GameManager.instance.PlaySFX_fromAudioClip(_hurtClip);
        }

        GetComponent<HPSystem>().HP -= damage;
        if (GetComponent<HPSystem>().HP <= 0)
        {
            GameManager.instance.GameFinish("CONGRADUATION!", true);
        }
    }
}
