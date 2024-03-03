using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class DashIconRenderer : MonoBehaviour
{
    [SerializeField] private RawImage _image;

    private float _dashEnabledRight;
    private float _dashDisabledRight;
    private float _transformMultiplier;

    private PlayerState _playerState;
    private RectTransform _rectTransform;


    void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _dashEnabledRight = -_rectTransform.offsetMax.x;
        _dashDisabledRight = _rectTransform.offsetMin.x;
        _playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
        _transformMultiplier = (_dashEnabledRight - _dashDisabledRight) / _playerState.DashCooldownMax;
    }
    
    void Update()
    {
        if (_playerState.DashCooldown > 0)
        {
            _rectTransform.offsetMax = new Vector2(-_dashDisabledRight - _transformMultiplier * (_playerState.DashCooldownMax - _playerState.DashCooldown), _rectTransform.offsetMax.y);
        }
    }
}