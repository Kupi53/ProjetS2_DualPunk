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
	[SerializeField] protected int _quantity;
    [SerializeField] protected int _delay;

    private Coroutine _regenerationCoroutine;
	
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
        while (IsEquipped)
        {
            HealthManager.Heal(_quantity, 0f);

            yield return new WaitForSeconds(_delay);
        }
    }
}