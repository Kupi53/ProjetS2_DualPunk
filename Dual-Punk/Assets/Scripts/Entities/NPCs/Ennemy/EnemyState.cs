using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyState : NPCState
{
    [SerializeField] private Vector3 _detectionOffset;
    [SerializeField] private float _lockDistance;
    [SerializeField] private float _unlockDistance;

    private SpriteRenderer _spriteRenderer;
    private bool _stunAnimation;

    public GameObject Target { get; set; }
    public DefenceType DefenceType { get; set; }
    public bool CanAttack { get; set; }


    protected new void Awake()
    {
        base.Awake();

        CanAttack = false;
        DefenceType = DefenceType.NotDefending;

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _stunAnimation = false;
    }


    private void Update()
    {
        float distance;

        if (Target == null)
        {
            Stop = true;
            float maxDistance = _lockDistance;
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < targets.Length; i++)
            {
                Vector3 targetPoint = targets[i].transform.position + _detectionOffset;
                distance = Vector2.Distance(targetPoint, transform.position + _detectionOffset);
                
                if (distance < maxDistance / 4 || distance < maxDistance &&
                    (!Physics2D.Raycast(transform.position, targetPoint - transform.position - _detectionOffset, distance, LayerMask)
                    || DefenceType > DefenceType.NotDefending))
                {
                    maxDistance = distance;
                    Target = targets[i];
                    Stop = false;
                }
            }
            return;
        }

        TargetPoint = Target.transform.position;
        distance = Vector2.Distance(transform.position, TargetPoint);

        if (distance > _unlockDistance)
        {
            Target = null;
            return;
        }

        Run = (distance > _unlockDistance / 2 || !CanAttack) && DefenceType == DefenceType.NotDefending;

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