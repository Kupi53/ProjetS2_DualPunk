using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyState : NPCState
{
    [SerializeField] private float _lockDistance;
    [SerializeField] private float _unlockDistance;

    public WeaponScript WeaponScript { get; set; }
    public bool Attack { get; set; }


    protected new void Awake()
    {
        base.Awake();

        Attack = false;
    }


    private void Update()
    {
        if (Target == null)
        {
            Move = false;

            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < targets.Length; i++)
            {
                if (Vector2.Distance(targets[i].transform.position, transform.position) < _lockDistance)
                {
                    Target = targets[i];
                    break;
                }
            }
        }
        else if (Vector2.Distance(transform.position, Target.transform.position) > _unlockDistance)
        {
            Target = null;
        }
        else
        {
            Move = true;
        }
    }
}