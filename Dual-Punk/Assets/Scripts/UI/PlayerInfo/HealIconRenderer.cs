using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class HealIconRenderer : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    // [SerializeField]
    
    private float _reloadingOffset;
    private float _transformMultiplier;

    private PlayerAbilities _playerAbilities;
    private RectTransform _rectTransform;


    private void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _playerAbilities = transform.root.GetComponent<LocalPlayerReference>().PlayerAbilities;
        
        _reloadingOffset = _rectTransform.offsetMin.x;
        // Offset max est negatif car c'est la distance entre le coin haut droit de l'ancre et le coin haut droit de l'image
        _transformMultiplier = (_reloadingOffset + _rectTransform.offsetMax.x) / _playerAbilities.HealCoolDown;
    }

    
    private void Update()
    {
        if (_playerAbilities.HealTimer >= _playerAbilities.HealCoolDown)
        {
            _image.enabled = false;
        }
        else
        {
            _image.enabled = true;
            _rectTransform.offsetMax = new Vector2(-_reloadingOffset + _transformMultiplier * _playerAbilities.HealTimer, _rectTransform.offsetMax.y);
        }
    }
}