using GameKit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextScript : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _rangeX;
    [SerializeField] private float _rangeY;

    private RectTransform _rectTransform;
    private float _timer;


    void Start()
    {
        _timer = 0;
        _rectTransform = GetComponent<RectTransform>();

        _rectTransform.SetPosition(true, new Vector3(Random.Range(-_rangeX, _rangeX), Random.Range(-_rangeY, _rangeY), 0));
    }

    void Update()
    {
        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        _timer += Time.deltaTime;
        float scale = 1 - _timer / _lifeTime;
        _rectTransform.SetScale(new Vector3(scale, scale, 1));
    }
}
