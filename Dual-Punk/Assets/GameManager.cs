using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Items"), LayerMask.NameToLayer("Items"));
    }
}