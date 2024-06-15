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
    private bool _isTeleporting;
    private GameObject _oldMeleeWeaponScript;

    private SpriteRenderer _spriteRenderer { get => gameObject.GetComponent<SpriteRenderer>(); }
    private HealthManager HealthManager { get => PlayerState.gameObject.GetComponent<HealthManager>(); }
    private SpriteRenderer PlayerSpriteRenderer { get => PlayerState.gameObject.GetComponent<SpriteRenderer>(); }
    private MouvementsController MouvementsController { get => PlayerState.gameObject.GetComponent<MouvementsController>(); }

    
    void Awake()
    {
        Type = ImplantType.Boots;
        SetName = "Scavenger";

        _canDash = false;
        _timeTeleportation = _teleportationSound.length / 2;
        _timeCoolDown = 0f;
        _oldMeleeWeaponScript = null;

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
            MouvementsController.EnableDash = false;

            if (Input.GetButtonDown("Dash") && !PlayerState.IsDown)
            {
                StartCoroutine(Teleportation(nearestEnemy));
            }
        }
    }

    private IEnumerator Teleportation(GameObject nearestEnemy)
    {
        _timeCoolDown = 0;
        _canDash = false;

        _spriteRenderer.enabled = true;
        _animator.Play("Teleportation");
        
        AudioManager.Instance.PlayClipAt(_teleportationSound, gameObject.transform.position, "Player");
        
        PlayerState.Dashing = true;
        PlayerSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        HealthManager.Teleportation = true;
        
        MeleeWeaponScript meleeWeaponScript = PlayerState.WeaponScript as MeleeWeaponScript;
        _oldMeleeWeaponScript = meleeWeaponScript.gameObject;
        foreach (var renderer in _oldMeleeWeaponScript.GetComponentsInChildren<SpriteRenderer>())
            renderer.color = new Color(1f, 1f, 1f, 0f);
        
        
        yield return new WaitForSeconds(_timeTeleportation);

        _spriteRenderer.enabled = false;
        _animator.Play("Default");

        PlayerState.gameObject.transform.position = nearestEnemy.transform.position;
        PlayerSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        MouvementsController.EnableDash = true;
        HealthManager.Teleportation = false;

        foreach (var renderer in _oldMeleeWeaponScript.GetComponentsInChildren<SpriteRenderer>())
            renderer.color = new Color(1f, 1f, 1f, 1f);

        nearestEnemy.GetComponent<EnemyHealthManager>().Damage(meleeWeaponScript.CriticalDamage, 0f, true, 0f);

        AudioManager.Instance.PlayClipAt(meleeWeaponScript.CriticalSound, gameObject.transform.position, "Player");
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
        _spriteRenderer.enabled = false;
        _animator.Play("Default");

        PlayerSpriteRenderer.enabled = true;
        MouvementsController.EnableDash = true;
        HealthManager.Teleportation = false;

        if (_oldMeleeWeaponScript != null)
            _oldMeleeWeaponScript.GetComponent<SpriteRenderer>().enabled = true;

        _timeCoolDown = 0;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}