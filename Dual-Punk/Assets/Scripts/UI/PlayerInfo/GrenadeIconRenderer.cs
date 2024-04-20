using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GrenadeIconRenderer : MonoBehaviour
{
    [SerializeField] private RawImage _image;

    private ConsumablesController _consumablesController;
    private RectTransform _rectTransform;

    private float _reloadingOffset;
    private float _transformMultiplier;


    void Start()
    {
        _rectTransform = _image.GetComponent<RectTransform>();
        _consumablesController = transform.root.GetComponent<LocalPlayerReference>().ConsumablesController;

        _reloadingOffset = _rectTransform.offsetMin.x;
        // Offset max est negatif car c'est la distance entre le coin haut droit de l'ancre et le coin haut droit de l'image
        _transformMultiplier = (_reloadingOffset + _rectTransform.offsetMax.x) / _consumablesController.ItemCoolDown;
    }

    void Update()
    {
        if (_consumablesController.ItemTimer >= _consumablesController.ItemCoolDown) {
            _image.enabled = false;
        } else {
            _image.enabled = true;
            _rectTransform.offsetMax = new Vector2(-_reloadingOffset + _transformMultiplier * _consumablesController.ItemTimer, _rectTransform.offsetMax.y);
        }
    }
}
