using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script gere le mouvement et les animations du joueur 
Il gere aussi les abilités (dash) mais ça doit être changé */

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private PlayerState playerState;
    private string currentState;

    // Constantes qui sont les noms des sprites du joueur
    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";

    // Bool qui sert pour le state du joueur (par exemple pendant un dash ou certaines abilitiés le joueur ne doit pas pouvoir bouger)
    private bool enableMovement;
    public float walkspeed;
    public float dashSpeed;
    public float dashTime;
    public float deceleration;
    Vector2 direction;


    // Start is called before the first frame update
    void Start()
    {
        // De base le joueur face en bas
        currentState = PLAYER_S;
        enableMovement = true;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Prends Imputs et cooldown chaque frame
        if (enableMovement)
        {
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
                direction *= 0.8f;
            else if (Input.GetAxis("Horizontal") == 0)
                direction *= 0.6f;
        }

        if (Input.GetButtonDown("Dash") && PlayerState.dashCooldown <= 0.0f)
        {
            enableMovement = false;
            PlayerState.dashing = true;
            PlayerState.dashCooldown = PlayerState.dashCooldownMax;
        }
        else if (PlayerState.dashCooldown > 0.0f)
        {
            PlayerState.dashCooldown -= Time.deltaTime;
        }
    }


    void FixedUpdate()
    {
        if (enableMovement)
        {
            if (direction != new Vector2(0, 0))
                body.MovePosition(body.position + direction * walkspeed);
            Anim_Movement(direction);
        }
        else if (PlayerState.dashing)
        {
            if (PlayerState.dashTimer < dashTime)
            {
                body.MovePosition(body.position + direction * (dashSpeed - PlayerState.dashTimer * deceleration));
                PlayerState.dashTimer += Time.fixedDeltaTime;
            }
            else
            {
                enableMovement = true;
                PlayerState.dashing = false;
                PlayerState.dashTimer = 0;
            }
        }
    }


    // Utilise dans anim mouvement, change l'animation en fonction des constantes Player_S, Player_...
    void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    // On passe la direction actuelle du joueur et en fonction, appelle changeAnimation 
    // Avec la constante (nom du sprite) adaptée
    void Anim_Movement(Vector2 direction)
    {
        if (direction.x < 0.1f && direction.x > -0.1f && direction.y < 0.1f && direction.y > -0.1f)
        {
            ChangeAnimation(PLAYER_S);
        }
        else if (direction.x >= 0.1f && direction.y < 0.1f && direction.y > -0.1f)
        {
            ChangeAnimation(PLAYER_E);
        }
        else if (direction.x <= -0.1f && direction.y < 0.1f && direction.y > -0.1f)
        {
            ChangeAnimation(PLAYER_W);
        }
        else if (direction.x < 0.1f && direction.x > -0.1f && direction.y >= 0.1f)
        {
            ChangeAnimation(PLAYER_N);
        }
        else if (direction.x < 0.1f && direction.x > -0.1f && direction.y <= -0.1f)
        {
            ChangeAnimation(PLAYER_S);
        }
        else if (direction.x >= 0.5f && direction.y >= 0.3f)
        {
            ChangeAnimation(PLAYER_NE);
        }
        else if (direction.x >= 0.5f && direction.y <= -0.3f)
        {
            ChangeAnimation(PLAYER_SE);
        }
        else if (direction.x <= -0.5f && direction.y >= 0.3f)
        {
            ChangeAnimation(PLAYER_NW);
        }
        else if (direction.x <= -0.5f && direction.y <= -0.3f)
        {
            ChangeAnimation(PLAYER_SW);
        }
    }
}