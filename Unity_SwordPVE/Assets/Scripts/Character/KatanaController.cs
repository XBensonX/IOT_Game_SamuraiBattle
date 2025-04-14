using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatanaController : MonoBehaviour
{
    [SerializeField] private float triggerAngle = 60f;
    [SerializeField] private GameObject enemyObj;
    [SerializeField] private Image _tameshigiriImg;

    private bool isInEnemyBody = false;
    private bool hasDamaged = false;

    private void Update()
    {
        if (!hasDamaged && PlayerController.instance.isAttacking && isInEnemyBody)
        {
            hasDamaged = true;
            enemyObj.GetComponent<EnemyController>().Damage(1);
            GameManager.instance.AttackState();
        }

        // Tameshigiri(©~¦X±Ù) skill
        if (PlayerController.instance.isTameshigiri && PlayerController.instance.isAttacking && MQTTDataHandler.instance.isHallTrigger)
        {
            _tameshigiriImg.gameObject.SetActive(true);
            enemyObj.GetComponent<EnemyController>().Damage(2);
            GameManager.instance.AttackState();
            PlayerController.instance.isTameshigiri = false;
            Invoke("TameshigiriEnd", 2f);
        }
    }

    private void TameshigiriEnd()
    {
        _tameshigiriImg.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDamaged) return;
        if (other.gameObject.GetComponent<EnemyController>())
        {
            isInEnemyBody = true;
            //enemyObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyController>())
        {
            hasDamaged = false;
            isInEnemyBody = false;
            //enemyObj = null;
        }
    }
}
