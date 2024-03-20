using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Pathfinding;

public class EnnemyState : NetworkBehaviour
{
    public float health;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    [SerializeField]
    private EnnemyData data;
    private GameObject player;
    private IAstarAI ai;


    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;
        health = data.Health;
        damage = data.Damage;
        speed = data.Speed;
        player = GameObject.FindGameObjectWithTag("Ntmtest");
<<<<<<< HEAD
=======

        // Obtenir ou attacher le composant IAstarAI (par exemple, AIPath)
        ai = GetComponent<IAstarAI>();
        if (ai == null)
        {
            return;
        }

        // Assigner la destination initiale
        if (player != null)
        {
            ai.destination = player.transform.position;
        }
    }
>>>>>>> parent of 88e0c82 (Revert "Merge branch 'prototype' into Arnaud")

        // Obtenir ou attacher le composant IAstarAI (par exemple, AIPath)
        ai = GetComponent<IAstarAI>();
        if (ai == null)
        {
            return;
        }

        // Assigner la destination initiale
        if (player != null)
        {
            ai.destination = player.transform.position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        PathFinding();
    }


    private IEnumerator VisualIndicator(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDamage(float damage)
    {
        if (!IsServer) return;
        health -= damage;
        StartCoroutine(VisualIndicator(Color.red));
        CheckDeath();
    }
    private void CheckDeath()
    {
        if (!IsServer) return;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

<<<<<<< HEAD
    private void PathFinding()
    {
        if (!IsServer) return;

        // Générer un nouveau chemin vers la position du joueur
=======
    private void PathFinding(){
        if(!IsServer) return;
        
        // GÃ©nÃ©rer un nouveau chemin vers la position du joueur
>>>>>>> parent of 88e0c82 (Revert "Merge branch 'prototype' into Arnaud")
        if (player != null)
        {
            ai.destination = player.transform.position;
        }
    }

<<<<<<< HEAD
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer) return;
=======
    private void OnTriggerEnter2D(Collider2D collider){
        if(!IsServer) return;
>>>>>>> parent of 88e0c82 (Revert "Merge branch 'prototype' into Arnaud")
        PlayerState playerState = collider.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.Damage(damage);
            Debug.Log("Ennemy is touching you !");
        }
    }
}