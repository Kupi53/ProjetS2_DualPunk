using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;


public class TargetIndicatorScript : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;

#nullable enable
    public GameObject? Target { get; set; }
#nullable disable


    void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Target.transform.position + Vector3.up * 0.5f;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + _rotateSpeed * Time.deltaTime);
        }
    }
}
