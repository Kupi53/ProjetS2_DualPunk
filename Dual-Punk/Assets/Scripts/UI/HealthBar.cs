using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _image;

    private float _healthMaxRight;
    private float _healthMinRight;
    private float _transformMultiplier;
    private RectTransform _rectTransform;
    private PlayerState _playerState;


    void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _healthMaxRight = _rectTransform.offsetMin.x;
        _healthMinRight = -_rectTransform.offsetMax.x;
        _playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }


    void Update()
    {
        _transformMultiplier = (_healthMinRight - _healthMaxRight) / _playerState.MaxHealth;
        _rectTransform.offsetMax = new Vector2(-_healthMaxRight - _transformMultiplier * (_playerState.MaxHealth - _playerState.Health), _rectTransform.offsetMax.y);
    }


    [ContextMenu("DecreaseHealth")]
    public void DecreaseHealth()
    {
        Debug.Log("HealthDecreased");
        _playerState.Health -= 9;
        if (_playerState.Health < 0)
            _playerState.Health = 0;
    }

    [ContextMenu("IncreaseHealth")]
    public void IncreaseHealth()
    {
        Debug.Log("HealthIncreased");
        _playerState.Health += 9;
        if (_playerState.Health > _playerState.MaxHealth)
            _playerState.Health = _playerState.MaxHealth;
    }
}
