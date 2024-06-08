using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyState : NPCState
{
    [SerializeField] private float _lockDistance;
    [SerializeField] private float _unlockDistance;

    public GameObject Target { get; set; }
    public DefenceType DefenceType { get; set; }
    public bool CanAttack { get; set; }

    public override Vector3 TargetPoint
    {
        get => Target == null ? Vector3.zero : Target.transform.position;
        set => TargetPoint = value;
    }


    protected new void Awake()
    {
        base.Awake();

        CanAttack = false;
        DefenceType = DefenceType.NotDefending;
    }


    private void Update()
    {
        if (Target == null)
        {
            Stop = true;

            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < targets.Length; i++)
            {
                if (Vector2.Distance(targets[i].transform.position, transform.position) < _lockDistance)
                {
                    Target = targets[i];
                    Stop = false;
                    break;
                }
            }
        }
        else if (Vector2.Distance(transform.position, Target.transform.position) > _unlockDistance)
        {
            Target = null;
        }
        else if ((Vector2.Distance(transform.position, Target.transform.position) > _unlockDistance/2 || !CanAttack) && DefenceType == DefenceType.NotDefending)
        {
            Run = true;
        }
        else
        {
            Run = false;
        }
    }


    private void OnDestroy()
    {
        if (ParentRoom is not null)
        {
            ParentRoom.Enemies.Remove(gameObject);
            ParentRoom.OnEnemyDeath();
        }
    }
}

public enum DefenceType
{
    NotDefending,
    ShouldDefend,
    Defending,
}