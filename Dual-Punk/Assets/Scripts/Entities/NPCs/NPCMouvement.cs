using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(NPCState))]
public class NPCMouvement : NetworkBehaviour
{
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runSpeed;
    [SerializeField] private float _reCalculationTime;
    [SerializeField] private LayerMask _layerMask;

    private NPCState _npcState;
    private Rigidbody2D _rb2d;
    private Vector2[] _possibleDirections;
    private Vector2 _moveDirection;
    private float _reCalculateTimer;
    private float _moveSpeed;


    private void Start()
    {
        _reCalculateTimer = _reCalculationTime;
        _possibleDirections = new Vector2[] { new Vector2(1,0), new Vector2(0,1), new Vector2(-1,0), new Vector2(0,-1),
            new Vector2(1,0.5f).normalized, new Vector2(-1,0.5f).normalized, new Vector2(1, -0.5f).normalized, new Vector2(-1,-0.5f) };
        
        _rb2d = GetComponent<Rigidbody2D>();
        _npcState = GetComponent<NPCState>();
    }


    private void Update()
    {
        if (!_npcState.Move || _reCalculateTimer < _reCalculationTime) {
            _reCalculateTimer += Time.deltaTime;
            return;
        }
        else {
            _reCalculateTimer = 0;
        }
        
        
        Vector2 direction = _npcState.Target.transform.position - transform.position;
        float distance = direction.magnitude;
        direction = direction.normalized;

        float maxAngle = 256;
        float currentAngle;

        for (int i = 0; i < _possibleDirections.Length; i++)
        {
            if ((currentAngle = Vector2.Angle(direction, _possibleDirections[i])) < maxAngle)
            {
                if (!Physics2D.Raycast(transform.position, _possibleDirections[i], distance * (1 - currentAngle/180), _layerMask))
                {
                    _moveDirection = _possibleDirections[i];
                    maxAngle = currentAngle;
                }
            }
        }

        if (_npcState.Run)
            _moveSpeed = _runSpeed;
        else
            _moveSpeed = _walkSpeed;
    }


    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero && _npcState.Move)
            _rb2d.MovePosition(_rb2d.position + _moveDirection * _moveSpeed * Methods.GetDirectionFactor(_moveDirection));
    }
}