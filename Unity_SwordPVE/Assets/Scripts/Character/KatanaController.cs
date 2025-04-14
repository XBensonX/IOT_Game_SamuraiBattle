using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaController : MonoBehaviour
{
    [SerializeField] private float triggerAngle = 60f;

    private bool isInEnemyBody = false;
    private bool hasDamaged = false;
    private GameObject enemyObj;

    private void Update()
    {
        if (!hasDamaged && PlayerController.instance.isAttacking && isInEnemyBody)
        {
            hasDamaged = true;
            enemyObj.GetComponent<EnemyController>().Damage();
            GameManager.instance.AttackState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDamaged) return;
        if (other.gameObject.GetComponent<EnemyController>())
        {
            isInEnemyBody = true;
            enemyObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyController>())
        {
            hasDamaged = false;
            isInEnemyBody = false;
            enemyObj = null;
        }
    }
}
