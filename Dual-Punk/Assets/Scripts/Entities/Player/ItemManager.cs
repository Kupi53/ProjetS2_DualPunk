using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.VisualScripting;


public class ItemManager : NetworkBehaviour
{
    private PlayerState _playerState;
    private List<GameObject> _items;

    #nullable enable
    private GameObject? _item;
    private WeaponScript? _weaponScript;
    #nullable disable

    private int _index;


    private void Start()
    {
        _index = 0;
        _items = new List<GameObject>();
        _playerState = GetComponent<PlayerState>();
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (_items.Count > 0)
        {
            if (Input.GetButtonDown("Switch"))
                _index = (_index + 1) % _items.Count;

            _item = _items[_index];
            _item.GetComponent<HighlightItem>().Highlight();

            if (_item.CompareTag("Weapon") && !(_weaponScript = _item.GetComponent<WeaponScript>()).InHand)
            {
                if (Input.GetButtonDown("Pickup") && !_playerState.HoldingWeapon)
                {
                    _index = 0;
                    //Intervetir avec l'arme en main
                    _playerState.Weapon = _item;
                    _playerState.HoldingWeapon = true;
                    _weaponScript.PlayerState = _playerState;
                    _weaponScript.InHand = true;
                }
            }
            else if (_item.CompareTag("Implant")) //Plus verifier que l'implant n'est pas sur une entite
            {
                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    //Mettre l'implant dans l'inventaire ou le remplacer avec un autre
                }
            }
            else
            {
                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    //Mettre l'item dans l'inventaire
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Implant") || collision.gameObject.CompareTag("Item"))
        {
            _items.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife") || collision.gameObject.CompareTag("Item"))
        {
            _index = 0;
            _items.Remove(collision.gameObject);
        }
    }
}