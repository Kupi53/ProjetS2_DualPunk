using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
//using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _crawlBar;

    private RectTransform _healthRectTransform;
    private RectTransform _crawlRectTransform;
    private PlayerState _playerState;

    private float _maxRight;
    private float _minRight;
    private float _healthMultiplier;
    private float _crawlMultiplier;


    void Start()
    {
        _healthRectTransform = _healthBar.GetComponent<RectTransform>();
        _crawlRectTransform = _crawlBar.GetComponent<RectTransform>();
        _playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;

        _maxRight = _healthRectTransform.offsetMin.x;
        _minRight = -_healthRectTransform.offsetMax.x;
        _crawlMultiplier = (_minRight - _maxRight) / _playerState.CrawlTime;
    }


    void Update()
    {
        if (_playerState.IsDown)
        {
            _healthBar.enabled = false;
            _crawlBar.enabled = true;
            _crawlRectTransform.offsetMax = new Vector2(-_maxRight - _crawlMultiplier * _playerState.CrawlTimer, _crawlRectTransform.offsetMax.y);
        }
        else
        {
            _healthBar.enabled = true;
            _crawlBar.enabled = false;
            _healthMultiplier = (_minRight - _maxRight) / _playerState.MaxHealth;
            _healthRectTransform.offsetMax = new Vector2(-_maxRight - _healthMultiplier * (_playerState.MaxHealth - _playerState.Health), _healthRectTransform.offsetMax.y);
        }
    }
}
