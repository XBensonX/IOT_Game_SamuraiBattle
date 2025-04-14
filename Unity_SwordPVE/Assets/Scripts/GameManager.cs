using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private Canvas _titleUI;
    [SerializeField] private AudioClip _titleClip;
    [SerializeField] private Canvas _gamOverUI;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private TextMeshProUGUI _titleTextInGameOverUI;
    [SerializeField] private Canvas _hud;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private HPSystem _playerHPSystem;

    [Header("InGame")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _inGameClip;
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _defenseClip;
    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private TextMeshProUGUI _blockedText;

    [HideInInspector] public bool isInGame = false;
    [HideInInspector] public bool isGameOver = false;

    private void Start()
    {
        if (instance == null) instance = this;
        _bgmSource.clip = _titleClip;
        _bgmSource.Play();
    }

    private void Update()
    {
        if (!isInGame && MQTTDataHandler.instance.isHallTrigger)
        {
            isInGame = true;
            
            _titleUI.enabled = false;

            _hud.gameObject.SetActive(true);
            _enemyController.enabled = true;
            _playerHPSystem.enabled = true;
            PlayerController.instance.DrawSword();

            // audio
            _bgmSource.clip = _inGameClip;
            _bgmSource.Play();
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

    public void PlaySFX_fromAudioClip(AudioClip clip)
    {
        if (clip)
        {
            AudioSource _audioObj = Instantiate(_audioSource, _bgmSource.transform);
            _audioObj.clip = clip;
            _audioObj.Play();
            Destroy(_audioObj, 2f);
        }
    }

    public void AttackState()
    {
        PlaySFX_fromAudioClip(_attackClip);
    }

    public void ShowBlockedState()
    {
        _blockedText.text = "Blocked!";
        Invoke("ResetBlockedState", 2f);

        PlaySFX_fromAudioClip(_defenseClip);
    }

    private void ResetBlockedState()
    {
        _blockedText.text = string.Empty;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameFinish(string title, bool isWin)
    {
        isGameOver = true;
        _gamOverUI.gameObject.SetActive(true);

        _titleTextInGameOverUI.text = title;
        _titleTextInGameOverUI.color = isWin ? Color.green : Color.red;

        _hud.gameObject.SetActive(false);
        _enemyController.enabled = false;
        _playerHPSystem.enabled = false;

        // audio
        if (isWin) _bgmSource.clip = _winClip;
        else _bgmSource.clip = _loseClip;
        _bgmSource.Play();
    }
}
