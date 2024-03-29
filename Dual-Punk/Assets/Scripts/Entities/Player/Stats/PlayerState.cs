using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerState : MonoBehaviour
{
    private Vector3 _mousePosition;

    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float AnimAngle { get; set; }
    public float DashCooldown { get; set; }
    public float DashCooldownMax { get; set; }

    public bool Moving { get; set; }
    public bool Walking { get; set; }
    public bool Dashing { get; set; }
    public bool Attacking { get; set; }
    public bool HoldingWeapon { get; set; }

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
        Moving = false;
        Walking = false;
        Dashing = false;
        Attacking = false;
        HoldingWeapon = false;
        AnimAngle = 0;
        DashCooldown = 0;
        DashCooldownMax = 1;
    }

    void Update()
    {
        _mousePosition = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0;
    }
}