using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IgnoreCollisions : MonoBehaviour
{
    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Items"), LayerMask.NameToLayer("Items"));
    }
}
