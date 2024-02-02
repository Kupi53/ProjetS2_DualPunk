using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyState : MonoBehaviour
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
        health = data.Health;
        damage = data.Damage;
        speed = data.Speed;
        player = GameObject.FindGameObjectWithTag("Player");
    }




    // Update is called once per frame
    void Update()
    {
        PathFinding();

    }



    public void OnDamage(float damage){
        health -= damage;
        CheckDeath();
    }
    private void CheckDeath(){
        if (health <= 0){
            Destroy(gameObject);
        }
    }

    private void PathFinding(){
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed* Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if(collider.GetComponent<PlayerState>()!=null){
            collider.GetComponent<PlayerState>().Damage(damage);
            Debug.Log("Ennemy is touching you !");
        }
    }
}
