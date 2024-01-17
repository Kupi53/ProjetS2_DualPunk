using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 MoveDirection;
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
}