using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyScript : MonoBehaviour
{
    public float _maxHealth = 100;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        health = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private void OnDamage(float damage){
        health -= damage;
        CheckDeath();
    }
    private void CheckDeath(){
        if (health <= 0){
            Destroy(gameObject);
        }
    }
}
