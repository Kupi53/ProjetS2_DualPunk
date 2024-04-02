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

    private ConsumablesController _consumablesController;
    private RectTransform _rectTransform;


    private void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _consumablesController = transform.root.GetComponent<LocalPlayerReference>().ConsumablesController;
        
        _reloadingOffset = _rectTransform.offsetMin.x;
        // Offset max est negatif car c'est la distance entre le coin haut droit de l'ancre et le coin haut droit de l'image
        _transformMultiplier = (_reloadingOffset + _rectTransform.offsetMax.x) / _consumablesController.HealCoolDown;
    }

    
    private void Update()
    {
        if (_consumablesController.HealTimer >= _consumablesController.HealCoolDown)
        {
            _image.enabled = false;
        }
        else
        {
            _image.enabled = true;
            _rectTransform.offsetMax = new Vector2(-_reloadingOffset + _transformMultiplier * _consumablesController.HealTimer, _rectTransform.offsetMax.y);
        }
    }
}