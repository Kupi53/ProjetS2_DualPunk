using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

/* Ce script gere le mouvement et les animations du joueur 
Il gere aussi les abilités (dash) mais ça doit être changé */

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    public Animator animator;
    public GameObject pointer;
    public SpriteRenderer spriteRenderer;
    private PlayerState playerState;

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
    private string currentState;
    private float slowingFactor;

    // Nombres decimaux pour gerer la vitesse de marche, course et de dash
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float dashTime;
    Vector2 moveDirection;
    Vector2 pointerDirection;


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
            // Direction du deplacement
            moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            // Direction du pointeur
            pointerDirection = (pointer.transform.position - transform.position).normalized;
            if (Input.GetAxis("Horizontal") == 0)
                moveDirection.Scale(new Vector2(1,0.75f));
        }

        if (Input.GetButton("Aim"))
        {
            slowingFactor = 0.8f;
        }
        else
        {
            slowingFactor = 1.0f;
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
            float moveAngle = (float)(Math.Atan2(moveDirection.y, moveDirection.x) * (180 / Math.PI));
            float pointerAngle = (float)(Math.Atan2(pointerDirection.y, pointerDirection.x) * (180 / Math.PI));

            if (moveDirection != new Vector2(0, 0))
            {
                if (moveAngle > pointerAngle - 45 && moveAngle < pointerAngle + 45)
                    body.MovePosition(body.position + moveDirection * sprintSpeed * 0.02f);
                else
                    body.MovePosition(body.position + moveDirection * walkSpeed * 0.02f);
            }
            ChangeAnimation(ChangeDirection(pointerAngle));
        }
        else if (PlayerState.dashing)
        {
            if (PlayerState.dashTimer < dashTime)
            {
                body.MovePosition(body.position + moveDirection * (dashSpeed - PlayerState.dashTimer));
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
    string ChangeDirection(float angle)
    {
        if (angle > -22 && angle <= 22)
        {
            return PLAYER_E;
        }
        else if (angle > 22 && angle <= 67)
        {
            return PLAYER_NE;
        }
        else if (angle > 67 && angle <= 112)
        {
            return PLAYER_N;
        }
        else if (angle > 112 && angle <= 157)
        {
            return PLAYER_NW;
        }
        else if ((angle > 157 &&  angle <= 180) || (angle >= -180 && angle <= -158))
        {
            return PLAYER_W;
        }
        else if (angle > -158 && angle <= -113)
        {
            return PLAYER_SW;
        }
        else if (angle > -113 && angle <= -68)
        {
            return PLAYER_S;
        }
        else
        {
            return PLAYER_SE;
        }
    }
}