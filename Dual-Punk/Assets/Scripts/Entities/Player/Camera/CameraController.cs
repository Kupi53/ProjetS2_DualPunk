using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml.Schema;
using Unity.Netcode;
using UnityEngine;
using Unity.Networking;
using Cinemachine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    private CinemachineVirtualCamera VCam;
    private CinemachineBasicMultiChannelPerlin _cbmp;

    private Vector3 _offset;
    private Vector3 _position;

    private float _smoothTime;
    private float _shakeTimer;

    public Camera MainCamera { get; set; }
    public GameObject Player { get; set; }

    private float _refV1;
    private Vector3 _refV2;


    void Start()
    {
        _shakeTimer = 0;
        _smoothTime = 0.25f;
        _offset = new Vector3(0, 0, -7);

        MainCamera = GetComponentInChildren<Camera>();
        VCam = GetComponentInChildren<CinemachineVirtualCamera>();
        _cbmp = VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _refV1 = 0;
        _refV2 = Vector3.zero;

        StopShake();
    }


    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0)
            {
                StopShake();
            }
        }
    }


    void FixedUpdate()
    {
        _position = Player.transform.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, _position, ref _refV2, _smoothTime);

        if (Input.GetButton("Walk")) {
            MainCamera.orthographicSize = Mathf.SmoothDamp(MainCamera.orthographicSize, minZoom, ref _refV1, _smoothTime);
        } else {
            MainCamera.orthographicSize = Mathf.SmoothDamp(MainCamera.orthographicSize, maxZoom, ref _refV1, _smoothTime);
        }
        VCam.m_Lens.OrthographicSize = MainCamera.orthographicSize;

    }

    public void ShakeCamera(float intensity, float time)
    {
        _shakeTimer = time;
        _cbmp.m_AmplitudeGain = intensity;
    }

    public void StopShake()
    {
        _shakeTimer = 0;
        _cbmp.m_AmplitudeGain = 0;
    }
}