using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPSystem : MonoBehaviour
{
    [SerializeField] private int _maxHP = 3;
    private int _hp = 0;

    [Header("UI")]
    [SerializeField] private GameObject _hpGroupInUI;
    [SerializeField] private Image _image;
    [SerializeField] private float _gap = 20f;
    private List<Image> _images = new List<Image>();

    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    private void Start()
    {
        _hp = _maxHP;
        for (int i = 0; i < _hp; i++)
        {
            _images.Add(Instantiate(_image, _hpGroupInUI.transform));
            _images[i].GetComponent<RectTransform>().localPosition
                += new Vector3(i * (_images[i].GetComponent<RectTransform>().sizeDelta.x + _gap), 0, 0);
        }
    }

    private void Update()
    {
        Debug.Log(_hp);
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _maxHP; i++) _images[i].enabled = false;
        for (int i = 0; i < _hp; i++) _images[i].enabled = true;
    }
}
