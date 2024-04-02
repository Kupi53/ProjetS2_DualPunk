using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointerCollider : MonoBehaviour
{
    [SerializeField] private GameObject _pointer;
    private PointerScript _pointerScript;

    private void Start()
    {
        _pointerScript = _pointer.GetComponent<PointerScript>();
    }

    private 

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            _pointerScript.Target = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            _pointerScript.Target = null;
        }
    }
}
