using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Heal(float amount);

    void Damage(float amount);
}
