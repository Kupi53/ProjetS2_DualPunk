using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    // Set in spawnui
    public PlayerState PlayerState { get; set; }
    public ConsumablesController ConsumablesController { get; set; }
}