using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable : IDestroyable
{
    void Heal(int amount, float time);

    void Damage(int amount, float time, bool warriorLuckBullet);

    void SetHealth(int amount);
}
