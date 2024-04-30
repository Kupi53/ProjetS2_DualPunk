using FishNet.Object;
using UnityEngine;
using System;

public abstract class ImplantScript : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public HealthManager HealthManager { get; set; }

    public bool IsEquipped { get; set; } = false;
    
    protected SpriteRenderer _spriteRenderer;
    protected ObjectSpawner _objectSpawner;
    
    public abstract void Run();
    public abstract void ResetImplant();

    public void Drop()
    {
        ResetImplant();
        IsEquipped = false;
        transform.position = PlayerState.transform.position;
        transform.rotation = Quaternion.identity;
    }
    
    [ServerRpc]
    protected void RemoveAllOwnerShipRPC(NetworkObject networkObject)
    {
        networkObject.RemoveOwnership();
    }
    
    void Awake()
    {
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
    }
}
