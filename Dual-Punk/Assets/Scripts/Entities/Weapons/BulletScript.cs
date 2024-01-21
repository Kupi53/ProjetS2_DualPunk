using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage = 3;
    public Vector3 MoveDirection;
    [SerializeField]
    public int MoveSpeed;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += MoveDirection * MoveSpeed * Time.deltaTime;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    
    }

    public void OnTriggerStay2D(Collider2D collider){
        if (collider.GetComponent<Health>() != null){
            Health health = collider.GetComponent<Health>();
            health.Damage(damage);
            Destroy(gameObject);
        }   
    }

}