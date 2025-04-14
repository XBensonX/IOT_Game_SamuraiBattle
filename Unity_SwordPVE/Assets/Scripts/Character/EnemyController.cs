using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private AudioClip _enemyClip;

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
        if (_sec >= _targetTime)
        {
            Attack();
            _targetTime = Random.Range(_minSec, _maxSec);
            _sec = 0;
        }
        //Debug.Log(_sec + " " + _targetTime);
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");

        if (_enemyClip)
        {
            GameManager.instance.PlaySFX_fromAudioClip(_enemyClip);
        }
    }

    public void AttackedOnAnimationFrame()
    {
        if (!PlayerController.instance.isBlocking)
        {
            PlayerController.instance.GetComponent<HPSystem>().HP--;
            if (PlayerController.instance.GetComponent<HPSystem>().HP <= 0)
            {
                GameManager.instance.GameFinish("YOU DIED", false);
            }
        }
        else
        {
            GameManager.instance.ShowBlockedState();
        }
    }

    public void Damage()
    {
        GetComponent<HPSystem>().HP--;
        if (GetComponent<HPSystem>().HP <= 0)
        {
            GameManager.instance.GameFinish("CONGRADUATION!", true);
        }
    }
}
