using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
using System.Data.SqlTypes;


public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D body;
    public Animator animator;
    public PlayerState playerState;
    public SpriteRenderer spriteRenderer;

    // Constantes qui sont les noms des sprites du joueur
    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";
    const string PLAYER_IDLE = "Player Idle";
    const string PLAYERWEAPON_N = "PlayerWeapon N";
    const string PLAYERWEAPON_E = "PlayerWeapon E";
    const string PLAYERWEAPON_S = "PlayerWeapon S";
    const string PLAYERWEAPON_W = "PlayerWeapon W";
    const string PLAYERWEAPON_NE = "PlayerWeapon NE";
    const string PLAYERWEAPON_NW = "PlayerWeapon NW";
    const string PLAYERWEAPON_SE = "PlayerWeapon SE";
    const string PLAYERWEAPON_SW = "PlayerWeapon SW";

    // Bool qui sert pour le state du joueur (par exemple pendant un dash ou certaines abilitiés le joueur ne doit pas pouvoir bouger)
    private bool enableMovement;
    private string currentState;
    private float dashTimer;

    // Nombres decimaux pour gerer la vitesse de marche, course et de dash
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float dashTime;
    public float moveBackFactor;

    private Vector2 moveDirection;
    private Vector2 pointerDirection;


    // Start is called before the first frame update
    void Start()
    {
        dashTimer = 0;
        enableMovement = true;
        currentState = PLAYER_IDLE;
        animator = GetComponent<Animator>();
        playerState = gameObject.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        // Prends Imputs chaque frame
        if (enableMovement)
        {
            // Direction du deplacement
            moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            // Direction du pointeur
            if (playerState.Pointer != null)
                pointerDirection = (playerState.Pointer.transform.position - transform.position).normalized;
            
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
                moveDirection *= 0.8f;
            else if (Input.GetAxis("Horizontal") == 0)
                moveDirection *= 0.6f;

            if (Input.GetButtonDown("Sprint"))
            {
                playerState.Walking = !playerState.Walking;
            }
        }

        if (Input.GetButtonDown("Dash") && playerState.DashCooldown <= 0.0f)
        {
            enableMovement = false;
            playerState.Dashing = true;
            playerState.DashCooldown = playerState.DashCooldownMax;
        }
        else if (playerState.DashCooldown > 0.0f)
        {
            playerState.DashCooldown -= Time.deltaTime;
        }
    }


    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (enableMovement)
        {
            // Vitesse de deplacement en fonction des variables publiques en dessous
            float moveSpeed;
            // Facteur qui depend aussi de certaines variables en dessous qui ralenti ou accelere le deplacement dans certaines situations
            float moveFactor = 1.0f;

            float moveAngle = (float)(Math.Atan2(moveDirection.y, moveDirection.x) * (180 / Math.PI));
            float pointerAngle = (float)(Math.Atan2(pointerDirection.y, pointerDirection.x) * (180 / Math.PI));


            if (playerState.Walking) {
                moveSpeed = walkSpeed;
            } else {
                moveSpeed = sprintSpeed;
            }

            if (playerState.HoldingWeapon && !SameDirection(moveAngle, pointerAngle, 60)) {
                moveFactor *= moveBackFactor;
            }

            if (moveDirection != new Vector2(0, 0))
            {
                body.MovePosition(body.position + moveDirection * moveSpeed * moveFactor);

                if (!playerState.HoldingWeapon)
                    ChangeAnimation(ChangeDirection(moveAngle));
            }

            else if (!playerState.HoldingWeapon)
                animator.Play(PLAYER_IDLE);

            if (playerState.HoldingWeapon)
                ChangeAnimation(ChangeDirection(pointerAngle));
        }

        else if (playerState.Dashing)
        {
            if (dashTimer < dashTime)
            {
                body.MovePosition(body.position + moveDirection * (dashSpeed - dashTimer));
                dashTimer += Time.fixedDeltaTime;
            }
            else
            {
                enableMovement = true;
                playerState.Dashing = false;
                dashTimer = 0;
            }
        }
    }


    bool SameDirection(float angle1, float angle2, int margin) 
    {
        if (angle1 + margin > 180 && (angle2 > angle1 - margin || angle2 < -360 + angle1 + margin))
            return true;
        else if (angle1 - margin < -180 && (angle2 < angle1 + margin || angle2 > 360 - angle1 - margin))
            return true;
        else if (angle2 < angle1 + margin && angle2 > angle1 - margin)
            return true;
        return false;
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
    
    string ChangeDirectionWeapon(float angle)
    {
        if (angle > -22 && angle <= 22)
        {
            return PLAYERWEAPON_E;
        }
        else if (angle > 22 && angle <= 67)
        {
            return PLAYERWEAPON_NE;
        }
        else if (angle > 67 && angle <= 112)
        {
            return PLAYERWEAPON_N;
        }
        else if (angle > 112 && angle <= 157)
        {
            return PLAYERWEAPON_NW;
        }
        else if ((angle > 157 &&  angle <= 180) || (angle >= -180 && angle <= -158))
        {
            return PLAYERWEAPON_W;
        }
        else if (angle > -158 && angle <= -113)
        {
            return PLAYERWEAPON_SW;
        }
        else if (angle > -113 && angle <= -68)
        {
            return PLAYERWEAPON_S;
        }
        else
        {
            return PLAYERWEAPON_SE;
        }
    }
}