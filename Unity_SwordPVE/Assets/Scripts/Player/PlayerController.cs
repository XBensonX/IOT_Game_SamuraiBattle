using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private EnemyController _enemy;
    [SerializeField] private float _distanceToEnemy = 3.0f;

    public float originJoystick;
    [SerializeField] private float _sensitivityJoystick = 50f;
    [SerializeField] private float _speed = 2f;

    private float _originPosY;

    private void Start()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        Move();
        LootAtEnemy();
    }

    private void Move()
    {
        float val = MQTTDataHandler.instance.joystickVal;
        if (val >= originJoystick + _sensitivityJoystick) // right
        {
            transform.position += transform.right * Time.deltaTime * _speed;
        }
        else if (val <= originJoystick - _sensitivityJoystick) // left
        {
            transform.position -= transform.right * Time.deltaTime * _speed;
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
}
