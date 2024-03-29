using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Meme but mais different de PlayerState, ici on donne les stats des capacites du joueur non de son etat
public class PlayerAbilities : MonoBehaviour
{
    private PlayerState _playerState;
    private IDamageable _damageable;

    // Heal
    [SerializeField]
    private float _healCooldown;
    private float _healTimer;

    public float HealCoolDown { get => _healCooldown; }
    public float HealTimer { get => _healTimer; }

    // CombatUseableItem
    [SerializeField]
    private float _itemCooldown;
    private float _itemTimer;
    
    public float ItemCoolDown { get => _itemCooldown; }
    public float ItemTimer { get => _itemTimer; }


    void Start()
    {
        _healTimer = _healCooldown;
        _itemTimer = _itemCooldown;
        _playerState = GetComponent<PlayerState>();
        _damageable = GetComponent<IDamageable>();
    }


    void Update()
    {
        if (_healTimer >= _healCooldown)
        {
            if (Input.GetButtonDown("UseHeal") && _playerState.Health < _playerState.MaxHealth)
            {
                _healTimer = 0;
                _damageable.Heal(30);
            }
        }
        else
        {
            _healTimer += Time.deltaTime;
        }
    }
}
