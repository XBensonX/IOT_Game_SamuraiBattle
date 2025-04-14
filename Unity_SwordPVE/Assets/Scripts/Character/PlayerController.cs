using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Enemy Settings")]
    [SerializeField] private EnemyController _enemy;
    [SerializeField] private float _distanceToEnemy = 3.0f;

    [Header("Katana Setting")]
    [SerializeField] private GameObject _katanaObj;
    public bool isBlocking = false;
    public bool isAttacking = false;
    public Vector3 originGyro_MPU6050 = Vector3.zero;

    private Vector3 _originPos;
    private Vector3 _originRot;

    [Header("Joystick Settings")]
    public float originJoystick;
    [SerializeField] private float _sensitivityJoystick = 50f;
    [SerializeField] private float _moveSpeed = 1.25f;

    private float _originPosY;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 _jointOriginalPos;
    private float _timer = 0;

    private void Start()
    {
        if (instance == null) instance = this;

        if (_katanaObj)
        { 
            _originPos = _katanaObj.transform.localPosition;
            _originRot = _katanaObj.transform.localEulerAngles;
        }
    }

    private void Update()
    {
        Move();
        LootAtEnemy();
        HeadBob();

        SyncKatanaRotation();

        // Attack's Priority is higher than Defense.
        if (MQTTDataHandler.instance.isAttackBtnPressed)
        {
            Attack();
        }
        else
        {
            if (MQTTDataHandler.instance.isJoystickPressed)
            {
                Defense();
            }
            else
            {
                isAttacking = false;
                isBlocking = false;

            }
        }
    }

    private void Move()
    {
        float val = MQTTDataHandler.instance.joystickVal.y;
        if (val >= originJoystick + _sensitivityJoystick) // right
        {
            transform.position += transform.right * Time.deltaTime * _moveSpeed;
        }
        else if (val <= originJoystick - _sensitivityJoystick) // left
        {
            transform.position -= transform.right * Time.deltaTime * _moveSpeed;
        }
    }

    private void LootAtEnemy()
    {
        transform.LookAt(_enemy.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);

        _originPosY = transform.position.y;
        Vector3 newPos = _enemy.transform.position - transform.forward * _distanceToEnemy;
        transform.position = new Vector3(newPos.x, _originPosY, newPos.z);
    }

    private void HeadBob()
    {
        _timer += Time.deltaTime * bobSpeed;
        joint.localPosition = new Vector3(_jointOriginalPos.x + Mathf.Sin(_timer) * bobAmount.x, 
                                          _jointOriginalPos.y + Mathf.Sin(_timer) * bobAmount.y, 
                                          _jointOriginalPos.z + Mathf.Sin(_timer) * bobAmount.z);
    }

    public void ResetKatanaPosAndRot()
    {
        if (_katanaObj)
        {
            _katanaObj.transform.localPosition = _originPos;
            _katanaObj.transform.localEulerAngles = _originRot;
        }
    }

    private void SyncKatanaRotation()
    {
        if (_katanaObj)
        {
            _katanaObj.transform.localEulerAngles = new Vector3(_originRot.x + MQTTDataHandler.instance.angle_MPU6050.x,
                                                                _originRot.y + MQTTDataHandler.instance.angle_MPU6050.z,
                                                                _originRot.z + MQTTDataHandler.instance.angle_MPU6050.y);
        }
    }

    public void DrawSword()
    {
        _katanaObj.SetActive(true);
    }

    private void Attack()
    {
        isAttacking = true;
        isBlocking = false;
    }

    private void Defense()
    {
        isAttacking = false;
        isBlocking = true;
    }
}
