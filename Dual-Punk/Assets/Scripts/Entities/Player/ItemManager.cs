using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.VisualScripting;


public class ItemManager : NetworkBehaviour
{
    private PlayerState playerState;
    private List<GameObject> items;

    #nullable enable
    private GameObject? Item;
    private WeaponScript? weaponScript;
    #nullable disable

    private int index;


    private void Start()
    {
        index = 0;
        items = new List<GameObject>();
        playerState = gameObject.GetComponent<PlayerState>();
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (items.Count > 0)
        {
            if (Input.GetButtonDown("Switch"))
                index = (index + 1) % items.Count;

            Item = items[index];
            Item.GetComponent<HighlightItem>().Highlight();

            if (Item.CompareTag("Weapon") && !(weaponScript = Item.GetComponent<WeaponScript>()).InHand)
            {
                Item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup") && !playerState.HoldingWeapon)
                {
                    index = 0;
                    //Intervetir avec l'arme en main
                    playerState.Weapon = Item;
                    playerState.HoldingWeapon = true;
                    weaponScript.PlayerState = playerState;
                    weaponScript.InHand = true;
                }
            }
            else if (Item.CompareTag("Implant")) //Plus verifier que l'implant n'est pas sur une entite
            {
                Item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    index = 0;
                    //Mettre l'implant dans l'inventaire ou le remplacer avec un autre
                }
            }
            else
            {
                Item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    index = 0;
                    //Mettre l'item dans l'inventaire
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Implant") || collision.gameObject.CompareTag("Item"))
        {
            items.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife") || collision.gameObject.CompareTag("Item"))
        {
            index = 0;
            items.Remove(collision.gameObject);
        }
    }
}