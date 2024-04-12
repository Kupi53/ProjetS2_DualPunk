using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DirectPathFinding : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _lockDistance;
    [SerializeField] private float _unlockDistance;
    [SerializeField] private float _reCalculationTime;

#nullable enable
    private GameObject? _target;
#nullable disable
    private Rigidbody2D _rb2d;

    private Vector2[] _possibleDirections;
    private Vector2 _moveDirection;
    private float _moveSpeed;
    private float _reCalculateTimer;


    void Start()
    {
        _reCalculateTimer = _reCalculationTime;

        _possibleDirections = new Vector2[] { new Vector2(1,0), new Vector2(0,1), new Vector2(-1,0), new Vector2(0,-1),
            new Vector2(1,0.5f).normalized, new Vector2(-1,0.5f).normalized, new Vector2(1, -0.5f).normalized, new Vector2(-1,-0.5f) };

        _rb2d = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (_reCalculateTimer < _reCalculationTime)
        {
            _reCalculateTimer += Time.deltaTime;
            return;
        }
        else
        {
            _reCalculateTimer = 0;
        }

        if (_target == null)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < targets.Length; i++)
            {
                if (Vector2.Distance(targets[i].transform.position, transform.position) < _lockDistance)
                {
                    _target = targets[i];
                    break;
                }
            }

            if (_target == null)
                return;
        }


        Vector2 direction = _target.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance > _unlockDistance)
        {
            _target = null;
            return;
        }

        direction = direction.normalized;
        float maxAngle = 256;
        float minAngle = 256;
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
                if (currentAngle < minAngle)
                {
                    minAngle = currentAngle;
                }
            }
        }

        if (_moveDirection != Vector2.zero)
        {
            if (Vector2.Angle(_moveDirection, direction) < minAngle + 10)
                _moveSpeed = _runSpeed;
            else
                _moveSpeed = _walkSpeed;
        }
    }


    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero && _target != null)
            _rb2d.MovePosition(_rb2d.position + _moveDirection * _moveSpeed * Methods.GetDirectionFactor(_moveDirection));
    }
}