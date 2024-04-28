using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public class TimerGrenadeScript : InstantGrenadeScript
{
    [SerializeField] private float _stoppingFactor;

    private SpriteRenderer _spriteRenderer;
    private float _explosionTimer;
    private bool _stop;


    private new void Start()
    {
        base.Start();

        _stop = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        if (!IsServer) return;

        _linePosition += _moveDirection * _moveSpeed;

        if (!_stop)
        {
            float currentDistance = Vector3.Distance(_linePosition, _startPosition);
            float factor = currentDistance / _distanceUntilStop;

            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + _rotateSpeed * Time.deltaTime);
            _rb2d.MovePosition(_linePosition + _verticalDirection * (- factor * factor * _curveFactor + factor * _curveFactor));

            if (currentDistance > _distanceUntilStop)
            {
                _stop = true;
                _spriteRenderer.sortingOrder = 1;
            }
        }
        else
        {
            _rb2d.MovePosition(_linePosition);
            if (_moveSpeed > 0)
                _moveSpeed -= _stoppingFactor * Time.fixedDeltaTime;
            else
                _moveSpeed = 0;
        }

        if (_explosionTimer > 0) {
            _explosionTimer -= Time.fixedDeltaTime;
        } else {
            Destroy();
        }
    }


    public override void Setup(Vector3 startPosition, Vector3 moveDirection, Vector3 verticalDirection, float moveSpeed, float explosionTimer, float distanceUntilStop, float curveFactor)
    {
        _startPosition = startPosition;
        _moveDirection = moveDirection;
        _verticalDirection = verticalDirection;
        _moveSpeed = moveSpeed;
        _explosionTimer = explosionTimer;
        _distanceUntilStop = distanceUntilStop;
        _curveFactor = curveFactor;
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_stop)
            _moveSpeed = 0;
    }
}
