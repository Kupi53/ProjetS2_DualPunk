using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class HealIconRenderer : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    // [SerializeField]
    private float _cooldown = 20;

    private float _timer;
    private float _reloadingOffset;
    private float _transformMultiplier;

    private PlayerState _playerState;
    private RectTransform _rectTransform;


    void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _playerState = transform.root.GetComponent<LocalPlayerReference>().PlayerState;

        _timer = _cooldown;
        _reloadingOffset = _rectTransform.offsetMin.x;
        // Offset max est negatif car c'est la distance entre le coin haut droit de l'ancre et le coin haut droit de l'image
        _transformMultiplier = (_reloadingOffset + _rectTransform.offsetMax.x) / _cooldown;
    }

    
    void Update()
    {
        if (_timer >= _cooldown)
        {
            _image.enabled = false;
            if (Input.GetButtonDown("UseHeal") && _playerState.Health < _playerState.MaxHealth)
            {
                _timer = 0;
                _playerState.Heal(30);
            }
        }
        else
        {
            _image.enabled = true;
            _timer += Time.deltaTime;
            _rectTransform.offsetMax = new Vector2(-_reloadingOffset + _transformMultiplier * _timer, _rectTransform.offsetMax.y);
        }
    }
}