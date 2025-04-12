using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private TextMeshProUGUI _blockedText;

    private void Start()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        if (PlayerController.instance.isAttacking)
        {
            _stateText.text = "Attacking!";
        }
        else
        {
            if (PlayerController.instance.isBlocking)
            {
                _stateText.text = "Blocking!";
            }
            else
            {
                _stateText.text = "";
            }
        }
    }

    public void ShowBlockedState()
    {
        _blockedText.text = "Blocked!";
        Invoke("ResetBlockedState", 2f);
    }

    private void ResetBlockedState()
    {
        _blockedText.text = string.Empty;
    }
}
