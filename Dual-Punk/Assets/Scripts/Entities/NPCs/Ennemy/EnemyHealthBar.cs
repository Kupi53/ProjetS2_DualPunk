using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthBar;

    private EnemyHealthManager _healthManager;
    private DisplayHealthBar[] _healthsBar;
    private float _width;
    private int _length;
    private int _index;


    private void Start()
    {
        _healthManager = transform.root.GetComponent<EnemyHealthManager>();
        _length = _healthManager.Lives.Length;
        _index = 0;

        _healthsBar = new DisplayHealthBar[_length];
        _width = GetComponent<RectTransform>().rect.width;
        _width = (_width - _length * 5) * (1 + _length) / 4;

        int totalLife = 0;
        foreach (int lifePoints in _healthManager.Lives)
        {
            totalLife += lifePoints;
        }

        for (int i = _length - 1; i > -1; i--)
        {
            GameObject healthBar = Instantiate(_healthBar);
            DisplayHealthBar displayHealth = healthBar.GetComponent<DisplayHealthBar>();

            healthBar.transform.SetParent(transform);
            healthBar.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(_width * _healthManager.Lives[i]/totalLife, 100);

            displayHealth.Current = _healthManager.Lives[i];
            displayHealth.MaxHealth = displayHealth.Current;
            _healthsBar[_length - i - 1] = displayHealth;
        }
    }


    private void Update()
    {
        if (_index < _healthManager.Index)
        {
            _healthsBar[_length - _index - 1].Current = 0;
            _index++;
        }

        _healthsBar[_length - _index - 1].Current = _healthManager.Lives[_index];
    }
}