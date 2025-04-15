using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KatanaController : MonoBehaviour
{
    [SerializeField] private float triggerAngle = 60f;
    //[SerializeField] private GameObject enemyObj;
    [SerializeField] private Image _tameshigiriImg;
    [SerializeField] private float _cooldown = 10f;
    private float _cooldown_current = 0;

    [Header("UI")]
    [SerializeField] private Image _cooldownImg;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private TextMeshProUGUI _skillReadyText;

    private bool isInEnemyBody = false;
    private bool hasDamaged = false;
      

    private void Update()
    {
        if (!hasDamaged && PlayerController.instance.isAttacking && isInEnemyBody)
        {
            hasDamaged = true;
            PlayerController.instance.enemyController.Damage(1);
            GameManager.instance.AttackState();
        }

        // Tameshigiri(©~¦X±Ù) skill
        Debug.Log(_cooldown_current + "/" + _cooldown);
        if (!isSkillAlreadyCooldown()) return;
        _cooldownImg.enabled = false;
        _cooldownText.enabled = false;
        _skillReadyText.enabled = true;

        if (GameManager.instance.isInGame && !MQTTDataHandler.instance.isHallTrigger)
        {
            if (PlayerController.instance.isAttacking)
            {
                if (!PlayerController.instance.isTameshigiri) PlayerController.instance.isTameshigiri = true;
            }
        }
        
        if (PlayerController.instance.isTameshigiri && PlayerController.instance.isAttacking && MQTTDataHandler.instance.isHallTrigger)
        {
            _tameshigiriImg.gameObject.SetActive(true);
            PlayerController.instance.enemyController.Damage(2);
            GameManager.instance.AttackState();
            PlayerController.instance.isTameshigiri = false;
            _cooldown_current = _cooldown;
            Invoke("TameshigiriEnd", 2f);
        }
    }

    private void TameshigiriEnd()
    {
        _tameshigiriImg.gameObject.SetActive(false);
    }

    private bool isSkillAlreadyCooldown()
    {
        if (_cooldown_current <= 0) return true;

        _cooldown_current -= Time.deltaTime;

        _cooldownImg.enabled = true;
        _cooldownText.enabled = true;
        _skillReadyText.enabled = false;
        _cooldownText.text = ((int)_cooldown_current).ToString();

        return false;
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
