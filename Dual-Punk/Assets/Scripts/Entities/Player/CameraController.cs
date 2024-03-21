using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml.Schema;
using Unity.Netcode;
using UnityEngine;
using Unity.Networking;


public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    private float _smoothTime;
    private float _velocity1;

    private Vector3 _offset;
    private Vector3 _velocity2;
    private Vector3 _position;

    private Camera _camera;

    public PlayerState PlayerState { get; set; }


    void Start()
    {
        _smoothTime = 0.25f;
        _velocity1 = 0;
        _velocity2 = Vector3.zero;
        _offset = new Vector3(0, 0, -7);

        _camera = GetComponent<Camera>();
    }


    void FixedUpdate()
    {
        _position = PlayerState.transform.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, _position, ref _velocity2, _smoothTime);

        if (Input.GetButton("Walk"))
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, minZoom, ref _velocity1, _smoothTime);
        }
        else
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, maxZoom, ref _velocity1, _smoothTime);
        }
    }
}