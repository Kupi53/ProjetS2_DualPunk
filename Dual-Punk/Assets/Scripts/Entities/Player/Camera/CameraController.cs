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
    [SerializeField] private float _minZoom;
    [SerializeField] private float _maxZoom;
    [SerializeField] private float _followTime;
    [SerializeField] private float _dashFollowTime;
    [SerializeField] private float _centerOnPlayerFactor;
    [SerializeField] private int _changeCenterZone;
    [SerializeField] private int _cameraZpos;

    private CinemachineVirtualCamera _vCam;
    private CinemachineBasicMultiChannelPerlin _cbmp;
    private Vector3 _offset;
    private Vector3 _refV2;
    private float _refV1;
    private float _shakeTimer;

    public Camera MainCamera { get; set; }
    public PlayerState PlayerState { get; set; }


    void Start()
    {
        _shakeTimer = 0;
        _refV1 = 0;
        _refV2 = Vector3.zero;
        _offset = new Vector3(0, 0, _cameraZpos);

        MainCamera = GetComponentInChildren<Camera>();
        _vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        _cbmp = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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

    private void FixedUpdate()
    {
        ChangeOffsetPosition(Input.mousePosition);

        if (PlayerState.Dashing)
        {
            // effet dash jsp
            transform.position = Vector3.SmoothDamp(transform.position, PlayerState.transform.position + _offset, ref _refV2, _followTime);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, PlayerState.transform.position + _offset, ref _refV2, _followTime);
        }

        if (PlayerState.Walking)
        {
            MainCamera.orthographicSize = Mathf.SmoothDamp(MainCamera.orthographicSize, _minZoom, ref _refV1, _followTime);
        }
        else
        {
            MainCamera.orthographicSize = Mathf.SmoothDamp(MainCamera.orthographicSize, _maxZoom, ref _refV1, _followTime);
        }
        _vCam.m_Lens.OrthographicSize = MainCamera.orthographicSize;
    }


    private void ChangeOffsetPosition(Vector3 mousePos)
    {
        //Changer en fonction de la resolution (je vais me suicider jpp de toutes ces merdes ca me rends fou)
        if (mousePos.y > 980 - _changeCenterZone || mousePos.y < _changeCenterZone || mousePos.x > 1920 - _changeCenterZone || mousePos.x < _changeCenterZone)
        {
            _offset = (PlayerState.transform.position * _centerOnPlayerFactor + PlayerState.MousePosition) / (1 + _centerOnPlayerFactor) - PlayerState.transform.position;
            _offset.z = _cameraZpos;
        }
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