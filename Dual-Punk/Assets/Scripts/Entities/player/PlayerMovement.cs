using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using IngameDebugConsole;

/* Ce script gere le mouvement et les animations du joueur 
Il gere aussi les abilités (dash) mais ça doit être changé */

public class PlayerMovement : NetworkBehaviour
{
    public Rigidbody2D body;
    public Animator animator;
    public GameObject pointer;
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

    // Bool qui sert pour le state du joueur (par exemple pendant un dash ou certaines abilitiés le joueur ne doit pas pouvoir bouger)
    private bool enableMovement;
    private string currentState;
    private float slowingFactor;

    // Nombres decimaux pour gerer la vitesse de marche, course et de dash
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float dashTime;
    public float slowingAim;
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
        if (!IsOwner) return;

        // Prends Imputs et cooldown chaque frame
        if (enableMovement)
        {
            // Direction du deplacement
            moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            // Direction du pointeur
            pointerDirection = (pointer.transform.position - transform.position).normalized;

            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
                moveDirection *= 0.8f;
            else if (Input.GetAxis("Horizontal") == 0)
                moveDirection *= 0.6f;
        }

        if (Input.GetButton("Aim"))
        {
            PlayerState.Aiming = true;
            slowingFactor = slowingAim;
        }
        else 
        {
            PlayerState.Aiming = false;
            slowingFactor = 1.0f;
        }

        if (Input.GetButtonDown("Dash") && PlayerState.DashCooldown <= 0.0f)
        {
            enableMovement = false;
            PlayerState.Dashing = true;
            PlayerState.DashCooldown = PlayerState.DashCooldownMax;
        }
        else if (PlayerState.DashCooldown > 0.0f)
        {
            PlayerState.DashCooldown -= Time.deltaTime;
        }
    }


    void FixedUpdate()
    {
        if (enableMovement)
        {
            float moveAngle = (float)(Math.Atan2(moveDirection.y, moveDirection.x) * (180 / Math.PI));
            float pointerAngle = (float)(Math.Atan2(pointerDirection.y, pointerDirection.x) * (180 / Math.PI));

            if (IsHost)
            {
                if (moveDirection != new Vector2(0, 0))
                {
                    if (PlayerState.HoldingWeapon)
                    {
                        if (moveAngle > pointerAngle - 45 && moveAngle < pointerAngle + 45)
                            body.MovePosition(body.position + moveDirection * sprintSpeed * slowingFactor);
                        else
                            body.MovePosition(body.position + moveDirection * walkSpeed * slowingFactor);
                        ChangeAnimation(ChangeDirection(pointerAngle));
                    }
                    else
                    {
                        body.MovePosition(body.position + moveDirection * walkSpeed * slowingFactor);
                        ChangeAnimation(ChangeDirection(moveAngle));
                    }
                }

                if (PlayerState.HoldingWeapon)
                    ChangeAnimation(ChangeDirection(pointerAngle));
                else
                    ChangeAnimation(ChangeDirection(moveAngle));
            }

            else
            {
                if (moveDirection != new Vector2(0, 0))
                {
                    if (PlayerState.HoldingWeapon)
                    {
                        if (moveAngle > pointerAngle - 45 && moveAngle < pointerAngle + 45)
                            MovePositionServerRPC(body.position + moveDirection * sprintSpeed * slowingFactor);
                        else
                            MovePositionServerRPC(body.position + moveDirection * walkSpeed * slowingFactor);
                        ChangeAnimationServerRPC(ChangeDirection(pointerAngle));
                    }
                    else
                    {
                        body.MovePosition(body.position + moveDirection * walkSpeed * slowingFactor);
                        ChangeAnimationServerRPC(ChangeDirection(moveAngle));
                    }
                }

                if (PlayerState.HoldingWeapon)
                    ChangeAnimationServerRPC(ChangeDirection(pointerAngle));
                else
                    ChangeAnimationServerRPC(ChangeDirection(moveAngle));
            }
            
        }
        else if (PlayerState.Dashing)
        {
            if (PlayerState.DashTimer < dashTime)
            {
                body.MovePosition(body.position + moveDirection * (dashSpeed - PlayerState.DashTimer));
                PlayerState.DashTimer += Time.fixedDeltaTime;
            }
            else
            {
                enableMovement = true;
                PlayerState.Dashing = false;
                PlayerState.DashTimer = 0;
            }
        }
    }


    // movement du client
    [ServerRpc(RequireOwnership = false)]
    void MovePositionServerRPC(Vector2 pos){
        body.MovePosition(pos);
    }


    // Utilise dans anim mouvement, change l'animation en fonction des constantes Player_S, Player_...
    void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeAnimationServerRPC(string newState)
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