using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Component.Animating;


public class MetalBladeScript : MeleeWeaponScript
{
    private NetworkAnimator _networkAnimator;
    private string _currentAnimation;


    private new void Start()
    {
        _currentAnimation = "drop";
        _networkAnimator = GetComponentInChildren<NetworkAnimator>();

        base.Start();
    }

    private new void Update()
    {
        base.Update();

        if (InHand && (_currentAnimation == "drop" || _currentAnimation == "dropHighlight"))
        {
            ChangeAnimation("normal");
        }
    }


    public override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        base.MovePosition(position, direction, targetPoint);

        int currentSign = Math.Sign(transform.localScale.y);
        if (_attack == 0 && Math.Sign(targetPoint.x - position.x) != currentSign)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 0);
        }

        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
        transform.position = position + WeaponOffset + direction * _weaponDistance;
        _attackPoint.transform.position = position + WeaponOffset + direction * _range / 3;
    }


    protected override void ResetPosition()
    {
        ChangeAnimation("drop");
    }

    private void ChangeAnimation(string newAnim)
    {
        if (_currentAnimation != newAnim)
        {
            _currentAnimation = newAnim;
            _networkAnimator.Play(newAnim);
        }
    }


    protected override void Defend(Vector3 direction)
    {
        ChangeAnimation("defend");

        base.Defend(direction);
    }

    protected override void Attack(Vector3 direction, bool damagePlayer)
    {
        ChangeAnimation("attack" + (_attack + 1).ToString());

        base.Attack(direction, damagePlayer);
    }
}