using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DisplayHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _sliderHealthObject;
    [SerializeField] private GameObject _sliderEffectObject;
    [SerializeField] private float _effectSmoothTime;

    private Slider _sliderHealth;
    private Slider _sliderEffect;
    private float _vel;

    public float Current { get; set; }
    public float MaxHealth { get; set; }


    private void Start()
    {
        _vel = 0;
        _sliderHealth = _sliderHealthObject.GetComponent<Slider>();
        _sliderEffect = _sliderEffectObject.GetComponent<Slider>();
    }

    private void Update()
    {
        _sliderHealth.value = Current / MaxHealth;
        _sliderEffect.value = Mathf.SmoothDamp(_sliderEffect.value, _sliderHealth.value, ref _vel, _effectSmoothTime);
    }
}
