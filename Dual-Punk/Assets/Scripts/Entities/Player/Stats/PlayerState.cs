using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerState : NetworkBehaviour
{
    private Vector3 _mousePosition;

    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public float AnimAngle { get; set; }
    public float ForcesEffect { get; set; }
    public float ForcesDecreaseSpeed { get; set; }
    public float CrawlTime { get; set; }
    public float CrawlTimer { get; set; }

    public bool Down { get; set; }
    public bool Moving { get; set; }
    public bool Walking { get; set; }
    public bool Dashing { get; set; }
    public bool Attacking { get; set; }
    public bool HoldingWeapon { get; set; }
    public bool CanBeTeleported { get; set; }

    public Vector3 MousePosition { get => _mousePosition; }

#nullable enable
    public WeaponScript? WeaponScript { get; set; }
    public PointerScript? PointerScript { get; set; }
    public CameraController? CameraController { get; set; }
#nullable disable



    private void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        Down = false;
        Moving = false;
        Walking = false;
        Dashing = false;
        Attacking = false;
        HoldingWeapon = false;
        AnimAngle = 0;
        ForcesEffect = 0.05f;
        ForcesDecreaseSpeed = 15;
        CrawlTime = 20;
        CrawlTimer = 0;
    }


    private void Update()
    {
        if (!IsOwner) return;

        _mousePosition = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0;
    }
}