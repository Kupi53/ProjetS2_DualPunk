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
        StartCoroutine(Spawner());
    }

    // Update is called once per frame
    void Update()
    {
        PathFinding();

    }

    private IEnumerator Spawner(){
        yield return new WaitForSeconds(0.0001f);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private IEnumerator VisualIndicator(Color color){
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDamage(float damage){
        health -= damage;
        StartCoroutine(VisualIndicator(Color.red));
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
