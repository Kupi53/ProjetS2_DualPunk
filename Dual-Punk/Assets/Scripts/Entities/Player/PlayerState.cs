using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Classe base pour les joueurs
public class PlayerState : MonoBehaviour
{
    public static float MaxHealth = 100;
    public static float Health = MaxHealth;
    public static bool Aiming = false;
    public static bool Walking = true;
    public static bool Dashing = false;
    public static bool HoldingWeapon = false;
    public static float DashTimer = 0.0f;
    public static float DashCooldown = 0.0f;
    public static float DashCooldownMax = 1.0f;
}