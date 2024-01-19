using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BulletScript : NetworkBehaviour

{
    public Vector2 MoveDirection;
    public int MoveSpeed;
    public Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.MovePosition(body.position + MoveDirection * MoveSpeed * Time.deltaTime);

        if (!GetComponent<Renderer>().isVisible)
        {
            if (IsHost){
                Destroy(gameObject);
            }
            else{
                DestroyBulletServerRPC();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    void DestroyBulletServerRPC()
    {
        Destroy(gameObject);
    }
}