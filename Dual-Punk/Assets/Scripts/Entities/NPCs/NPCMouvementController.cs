using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(NPCState))]
public class NPCMouvementController : NetworkBehaviour, IImpact
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _reCalculationTime;
    [SerializeField] private float _forcesEffect; // 0.05
    [SerializeField] private float _forcesDecreaseSpeed; //15
    [SerializeField] private LayerMask _layerMask;

    private List<(Vector2, float)> _forces;
    private NPCState _npcState;
    private Rigidbody2D _rb2d;

    private Vector2[] _possibleDirections;
    private Vector2 _moveDirection;
    private float _reCalculateTimer;
    private float _moveSpeed;


    private void Start()
    {
        _reCalculateTimer = _reCalculationTime;
        _forces = new List<(Vector2, float)>();
        _possibleDirections = new Vector2[] { new Vector2(1,0), new Vector2(0,1), new Vector2(-1,0), new Vector2(0,-1),
            new Vector2(1,0.5f).normalized, new Vector2(-1,0.5f).normalized, new Vector2(1, -0.5f).normalized, new Vector2(-1,-0.5f) };
        
        _rb2d = GetComponent<Rigidbody2D>();
        _npcState = GetComponent<NPCState>();
    }


    private void Update()
    {
        if (_npcState.Stop || _reCalculateTimer < _reCalculationTime)
        {
            _reCalculateTimer += Time.deltaTime;
            return;
        }
        else
        {
            _reCalculateTimer = 0;
        }
        
        Vector2 direction = _npcState.TargetPoint - transform.position;
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
        int i = 0;
        Vector2 resultingForce = Vector2.zero;
        Vector2 targetPosition = Vector2.zero;

        if (!_npcState.Stop)
        {
            targetPosition = _moveDirection * _moveSpeed * Methods.GetDirectionFactor(_moveDirection);
        }

        if (_forces.Count > 0)
        {
            while (i < _forces.Count)
            {
                if (_forces[i].Item2 <= 0)
                {
                    _forces.Remove(_forces[i]);
                }
                else
                {
                    resultingForce += _forces[i].Item1 * _forces[i].Item2;
                    _forces[i] = (_forces[i].Item1, _forces[i].Item2 - Time.deltaTime * _forcesDecreaseSpeed);
                    i++;
                }
            }

            resultingForce = resultingForce * Methods.GetDirectionFactor(resultingForce) * _forcesEffect;
        }

        _rb2d.MovePosition(_rb2d.position + resultingForce + targetPosition);
    }


    public void Impact(Vector2 direction, float intensity)
    {
        _forces.Add((direction.normalized, intensity));
    }
}