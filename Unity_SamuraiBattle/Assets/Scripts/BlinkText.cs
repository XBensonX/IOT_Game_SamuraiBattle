using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    [SerializeField] private float _blinkSpeed = 1f;

    private float _alphaVal;
    private bool _isFadeIn = false;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _alphaVal = _text.color.a;
    }

    private void Update()
    {
        if (!_isFadeIn) _alphaVal -= _blinkSpeed * Time.deltaTime;
        else _alphaVal += _blinkSpeed * Time.deltaTime;

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _alphaVal);

        if (!_isFadeIn && _alphaVal <= 0) _isFadeIn = true;
        else if (_isFadeIn && _alphaVal >= 1) _isFadeIn = false;
    }
}
