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

    public GameObject Player { get; set; }
    public Camera Camera { get; set; }

    public PlayerState _playerState;


    void Start()
    {
        _smoothTime = 0.25f;
        _velocity1 = 0;
        _velocity2 = Vector3.zero;
        _offset = new Vector3(0, 0, -7);
        _playerState = Player.GetComponent<PlayerState>();
    }


    void FixedUpdate()
    {
        Vector3 playerPosition = Player.transform.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref _velocity2, _smoothTime);

        if (Input.GetButton("Walk"))
        {
            Camera.orthographicSize = Mathf.SmoothDamp(Camera.orthographicSize, minZoom, ref _velocity1, _smoothTime);
        }
        else
        {
            Camera.orthographicSize = Mathf.SmoothDamp(Camera.orthographicSize, maxZoom, ref _velocity1, _smoothTime);
        }
    }
}