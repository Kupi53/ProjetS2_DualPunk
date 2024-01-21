using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RunningIconRenderer : MonoBehaviour
{
    public RawImage image;

    void Update()
    {
        if (!PlayerState.Walking)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
        }
    }
}
