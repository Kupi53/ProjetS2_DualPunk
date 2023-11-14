using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    public Animator animator;
    private string currentState;

    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";
    
    public float walkspeed;

    Vector2 direction;
    Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = PLAYER_S;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate(){
        pos = body.position;
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")*0.5f).normalized;
        Debug.Log(direction);
        body.MovePosition(pos + direction*walkspeed * Time.fixedDeltaTime);
        Anim_Movement(direction);
    }

    void ChangeAnimation(string newState){
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
   public void Anim_Movement(Vector2 direction){
        if (direction.x < 0.1f && direction.x > -0.1f && direction.y < 0.1f && direction.y > -0.1f){
            ChangeAnimation(PLAYER_S);
        }
        else if (direction.x >= 0.1f && direction.y < 0.1f && direction.y > -0.1f){
            ChangeAnimation(PLAYER_E);
        }
        else if (direction.x <= -0.1f && direction.y < 0.1f && direction.y > -0.1f){
            ChangeAnimation(PLAYER_W);
        }
        else if (direction.x < 0.1f && direction.x > -0.1f && direction.y >= 0.1f){
            ChangeAnimation(PLAYER_N);
        }
        else if (direction.x < 0.1f && direction.x > -0.1f && direction.y <= -0.1f){
            ChangeAnimation(PLAYER_S);
        }
        else if (direction.x >= 0.5f && direction.y >= 0.3f){
            ChangeAnimation(PLAYER_NE);
        }
        else if (direction.x >= 0.5f && direction.y <= -0.3f){
            ChangeAnimation(PLAYER_SE);
        }
        else if (direction.x <= -0.5f && direction.y >= 0.3f){
            ChangeAnimation(PLAYER_NW);
        }
        else if (direction.x <= -0.5f && direction.y <= -0.3f){
            ChangeAnimation(PLAYER_SW);
        }
    }
}