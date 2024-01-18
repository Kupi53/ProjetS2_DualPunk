using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    public Sprite pointerNormal;
    public Sprite pointer1;
    public Sprite pointer2;
    public Sprite pointerAim1;
    public Sprite pointerAim2;

    // Start is called before the first frame update
    void Start()
    {
        ChangePosition();
    }

    // Update is called once per frame
    void Update()
    {
        ChangePosition();

        
    }

    void ChangePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;
        transform.position = mousePos;
    }
}
