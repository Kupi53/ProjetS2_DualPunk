using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class RegenerationImplant : ImplantScript
{
    private HealthManager _healthManager{
        get {
            return PlayerState.gameObject.GetComponent<HealthManager>();
        }
    }
	[SerializeField] protected int _quantity;
    [SerializeField] protected int _delay;
    private Coroutine _regenerationCoroutine;

    void Awake(){
        Type = ImplantType.Neuralink;
    }
    public override void Run()
    {
        if (_regenerationCoroutine == null)
        {
            _regenerationCoroutine = StartCoroutine(RegenerationLoop());
        }
    }

    public override void ResetImplant()
    {
        if (_regenerationCoroutine != null)
        {
            StopCoroutine(_regenerationCoroutine);
            _regenerationCoroutine = null;
        }

        IsEquipped = false;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    private IEnumerator RegenerationLoop()
    {
        Debug.Log("caca");
        while (IsEquipped)
        {
            Debug.Log("pipi");
            _healthManager.Heal(_quantity, 0f);

            yield return new WaitForSeconds(_delay);
        }
    }
}