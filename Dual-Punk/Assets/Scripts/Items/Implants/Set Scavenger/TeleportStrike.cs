using System.Collections;
using System.Linq;
using UnityEngine;
using FishNet.Object;

public class TeleportStrike : ImplantScript
{
    [SerializeField] protected float _range;
    [SerializeField] protected float _coolDown;
    [SerializeField] protected AudioClip _teleportationSound;
    [SerializeField] protected Animator _animator;

    private float _timeTeleportation;
    private float _timeCoolDown;
    private bool _canDash;
    
    void Awake()
    {
        Type = ImplantType.Boots;
        SetName = "Scavenger";

        _canDash = false;
        _timeTeleportation = _teleportationSound.length / 2;
        _timeCoolDown = 0f;

        _animator = gameObject.GetComponent<Animator>();
    }

    public override void Run()
    {
        if (_timeCoolDown < _coolDown)
            _timeCoolDown += Time.deltaTime;

        if (_timeCoolDown >= _coolDown)
            _canDash = true;
        
        GameObject nearestEnemy = GetNearestTarget(gameObject.transform.position);
        
        if (IsEquipped && _canDash && PlayerState.WeaponScript as MeleeWeaponScript && nearestEnemy != null)
        {
            _canDash = false;
            PlayerState.gameObject.GetComponent<MouvementsController>().EnableDash = false;

            if (Input.GetButtonDown("Dash") && !PlayerState.Down)
            {
                StartCoroutine(Teleportation(nearestEnemy));
            }
        }
    }

    private IEnumerator Teleportation(GameObject nearestEnemy)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        _animator.Play("Teleportation");
        
        AudioManager.Instance.PlayClipAt(_teleportationSound, gameObject.transform.position);
        
        PlayerState.gameObject.GetComponent<MouvementsController>().enabled = false;
        PlayerState.Dashing = true;
                    
        PlayerState.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        
        MeleeWeaponScript meleeWeaponScript = PlayerState.WeaponScript as MeleeWeaponScript;
        meleeWeaponScript.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        
        yield return new WaitForSeconds(_timeTeleportation);

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        _animator.Play("Default");
        PlayerState.gameObject.transform.position = nearestEnemy.transform.position;
        PlayerState.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        PlayerState.gameObject.GetComponent<MouvementsController>().EnableDash = true;
        PlayerState.gameObject.GetComponent<MouvementsController>().enabled = true;
        meleeWeaponScript.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        nearestEnemy.GetComponent<EnemyHealthManager>().Damage(meleeWeaponScript.CriticalDamage, 0f, true);
        AudioManager.Instance.PlayClipAt(meleeWeaponScript.CriticalSound, gameObject.transform.position);

        _timeCoolDown = 0;
    }
    
#nullable enable
    private GameObject? GetNearestTarget(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ennemy");

        return (from enemy in enemies
            let distance = Vector2.Distance(position, enemy.transform.position + Vector3.up / 2)
            where distance < _range
            orderby distance
            select enemy).FirstOrDefault();
    }
#nullable disable

    public override void ResetImplant()
    {
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}