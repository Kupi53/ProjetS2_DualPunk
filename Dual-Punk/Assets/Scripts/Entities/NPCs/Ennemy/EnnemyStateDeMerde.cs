using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using FishNet.Object;
using Unity.Mathematics;

public class EnnemyStateDeMerde : NetworkBehaviour
{
    public float health;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    [SerializeField]
    private EnnemyData data;
    private GameObject player1;
    private GameObject player2;
    private AIPath ai;


    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;
        health = data.Health;
        damage = data.Damage;
        speed = data.Speed;


        // Obtenir ou attacher le composant AIPath
        ai = GetComponent<AIPath>();
        if (ai == null)
        {
            return;
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
            Loot();
            Destroy(gameObject);
        }
    }

    private void Loot()
    {
        int lootRolls = UnityEngine.Random.Range(0, data.MaxLootRollAmount);
        for (int i =0; i<lootRolls; i++)
        {
            string idToSpawn = data.LootTable.PickLoot();
            ObjectSpawner.Instance.SpawnObjectFromIdRpc(idToSpawn, gameObject.transform.position, new quaternion());
        }
    }

    private void PathFinding()
    {
        if (!IsServer) return;

        // Générer un nouveau chemin vers la position du joueur
        player1 = GameObject.Find("Player1(Clone)");
        player2 = GameObject.Find("Player2(Clone)");
        
        if (player1 != null && player2 != null)
        {
            float distancePlayer1 = Vector3.Distance(transform.position, player1.transform.position);
            float distancePlayer2 = Vector3.Distance(transform.position, player2.transform.position);

            if (distancePlayer1 <= distancePlayer2)
            {
                ai.destination = player1.transform.position;
            }
            else
            {
                ai.destination = player2.transform.position;
            }
        }
        else if (player1 != null)
        {
            ai.destination = player1.transform.position;
        }
        else if (player2 != null)
        {
            ai.destination = player2.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer) return;
        PlayerState playerState = collider.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.GetComponent<IDamageable>().Damage(10, 0.1f);
        }
    }
}