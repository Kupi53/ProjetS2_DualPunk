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

    private bool _stunAnimation;
    private SpriteRenderer _spriteRenderer;
    
    public override Vector3 TargetPoint
    {
        get => Target == null ? Vector3.zero : Target.transform.position;
        set => TargetPoint = value;
    }


    protected new void Awake()
    {
        base.Awake();

        CanAttack = false;
        Defending = false;

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _stunAnimation = false;
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

        if (Stun)
        {
            if (!_stunAnimation)
            {
                StartCoroutine(StunAnimation());
                _stunAnimation = true;
            }
        }
        else
        {
            StopCoroutine(StunAnimation());
            _stunAnimation = false;
        }
    }

    public IEnumerator StunAnimation()
    {
        while (Stun)
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.1f);
            yield return new WaitForSeconds(0.2f);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.2f);
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