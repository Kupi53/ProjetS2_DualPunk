using FishNet.Object;
using GameKit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


[RequireComponent(typeof(EnemyHealthManager))]
public class EnemyHealthIndicator : NetworkBehaviour
{
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _healthsBarLayout;
    [SerializeField] private GameObject _floatingTextPrefab;
    [SerializeField] private GameObject _floatingTextsParent;

    private EnemyHealthManager _healthManager;
    private DisplayHealthBar[] _healthsBar;
    private float _width;
    private int _length;
    private int _index;


    private void Start()
    {
        _healthManager = GetComponent<EnemyHealthManager>();
        _length = _healthManager.Lives.Length;
        _index = 0;

        _healthsBar = new DisplayHealthBar[_length];
        _width = _healthsBarLayout.GetComponent<RectTransform>().rect.width;
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

            healthBar.transform.SetParent(_healthsBarLayout.transform);
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


    private IEnumerator VisualIndicator(Color color, float time)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(time);
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    public void DisplayDamageIndicator(int amount)
    {
        if (amount == 0) return;

        GameObject floatingText = Instantiate(_floatingTextPrefab);
        floatingText.transform.SetParent(_floatingTextsParent.transform);
        floatingText.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        floatingText.GetComponent<RectTransform>().SetScale(new Vector3(1, 1, 0));

        StartCoroutine(VisualIndicator(Color.black, 0.1f));
        Spawn(floatingText);
    }
}