using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour
{

    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = PLAYER_S;
    }

    void ChangeAnimation(string newState){
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;

    }

    public void Anim_Movement(Vector2 direction){
    }
}