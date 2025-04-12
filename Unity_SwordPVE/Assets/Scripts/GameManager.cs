using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
        
    [SerializeField] private Canvas _titleUI;
    [SerializeField] private Canvas _gamOverUI;
    [SerializeField] private Canvas _hud;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private HPSystem _hpSystem;

    [Header("InGame")]
    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private TextMeshProUGUI _blockedText;

    public bool isInGame = false;
    public bool isGameOver = false;

    private void Start()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        if (!isInGame && MQTTDataHandler.instance.isHallTrigger)
        {
            isInGame = true;
            
            _titleUI.enabled = false;

            _hud.gameObject.SetActive(true);
            _enemyController.enabled = true;
            _hpSystem.enabled = true;
            PlayerController.instance.DrawSword();
        }

        if (isInGame && !MQTTDataHandler.instance.isHallTrigger)
        {
            // TODO: Maybe have Tameshigiri(©~¦X±Ù) skill
            if (!PlayerController.instance.isAttacking || isGameOver)
            {
                ResetGame();
            }
        }

        if (isInGame) ShowStateInGame();
    }

    private void ShowStateInGame()
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

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameFinish()
    {
        isGameOver = true;
        _gamOverUI.gameObject.SetActive(true);

        _hud.gameObject.SetActive(false);
        _enemyController.enabled = false;
        _hpSystem.enabled = false;
    }
}
