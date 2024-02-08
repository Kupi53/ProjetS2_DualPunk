using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
        if(!IsServer) return;
        health = data.Health;
        damage = data.Damage;
        speed = data.Speed;
        player = GameObject.FindGameObjectWithTag("Ntmtest");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) return;
        PathFinding();
    }


    private IEnumerator VisualIndicator(Color color){
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDamage(float damage){
        if(!IsServer) return;
        health -= damage;
        StartCoroutine(VisualIndicator(Color.red));
        CheckDeath();
    }
    private void CheckDeath(){
        if(!IsServer) return;
        if (health <= 0){
            Destroy(gameObject);
        }
    }

    private void PathFinding(){
        if(!IsServer) return;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed* Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if(!IsServer) return;
        if(collider.GetComponent<PlayerState>()!=null){
            collider.GetComponent<PlayerState>().Damage(damage);
            Debug.Log("Ennemy is touching you !");
        }
    }
}
