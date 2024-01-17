using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/* ce script gere le mouvement et les animations du joueur 
Il gere aussi les abilités (dash) mais ça doit être changé */

public class PlayerMovement : NetworkBehaviour
{
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    public Animator animator;
    private string currentState;
    
// constantes qui sont les noms des sprites du joueur
    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";
    
    public float walkspeed;
// bool qui sert pour le state du joueur (par exemple pendant un dash ou certaines abilitiés le joueur ne doit pas pouvoir bouger)
    private bool enableMovement = true;

    Vector2 direction;
    Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        // de base le joueur face en bas
        currentState = PLAYER_S;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate(){
        // inputs
        if (IsOwner){
            if (Input.GetButtonDown("Dash") && AbilitiesState.Instance.dashCooldown <= 0.0f){
                Debug.Log("dashing");
                enableMovement = false;
                AbilitiesState.Instance.dashing = true;
                AbilitiesState.Instance.dashCooldown = AbilitiesState.Instance.dashCooldownMax;
                AbilitiesState.Instance.dashTimer = 0.25f;
            }
            // functions
            Movement();
            Cooldown();
        }
    }

// gere le mouvement et gère aussi le dash mais ça doit etre separé
    void Movement(){
        if (AbilitiesState.Instance.dashing){
            if (AbilitiesState.Instance.dashTimer>0.15f){
                Debug.Log("dashing rn");
                pos = body.position;
                body.MovePosition(pos + direction * walkspeed * Time.fixedDeltaTime * 4.0f);
                AbilitiesState.Instance.dashTimer-=Time.fixedDeltaTime;
            }
            else if (AbilitiesState.Instance.dashTimer > 0){
                pos = body.position;
                body.MovePosition(pos + direction * walkspeed * Time.fixedDeltaTime * 2.0f);
                AbilitiesState.Instance.dashTimer-=Time.fixedDeltaTime;
            }
            else {
                enableMovement = true;
                AbilitiesState.Instance.dashing = false;
            }
        }
        if (enableMovement){
            pos = body.position;
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")*0.5f).normalized;
            body.MovePosition(pos + direction*walkspeed * Time.fixedDeltaTime);
            Anim_Movement(direction);
        }
    }

// gere le cooldown des abilités (pour l'instant seulement le dash) mais ça doit etre changé
    void Cooldown(){
        // dash
        if (AbilitiesState.Instance.dashCooldown > 0.0f){
            AbilitiesState.Instance.dashCooldown -= Time.fixedDeltaTime;
        }
    }

    // utilisé dans anim mouvement, change l'animation en fonction des constantes Player_S, Player_...
    void ChangeAnimation(string newState){
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    // on passe la direction actuelle du joueur et en fonction, appelle changeAnimation 
    // avec la constante (nom du sprite) adaptée
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

