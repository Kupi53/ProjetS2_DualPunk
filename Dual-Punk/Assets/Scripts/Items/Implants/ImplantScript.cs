using FishNet.Object;
using UnityEngine;
using System;

public enum ImplantType{
    Neuralink,
    ExoSqueleton,
    Arm,
    Boots
}
public abstract class ImplantScript : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public bool IsEquipped { get; set; } = false;
    public ImplantType Type { get; protected set; }
    public int? SetNumber { get; protected set; }
    
    protected SpriteRenderer _spriteRenderer;
    protected ObjectSpawner _objectSpawner;
    
    
    public abstract void Run();
    public abstract void ResetImplant();

    public void Drop()
    {
        ResetImplant();

        switch(Type)
        {
            case ImplantType.Neuralink:
                PlayerState.gameObject.GetComponent<ImplantController>().NeuralinkImplant = null;
                break;
            case ImplantType.ExoSqueleton:
                PlayerState.gameObject.GetComponent<ImplantController>().ExoSqueletonImplant = null;
                break;
            case ImplantType.Arm:
                PlayerState.gameObject.GetComponent<ImplantController>().ArmImplant = null;
                break;
            case ImplantType.Boots:
                PlayerState.gameObject.GetComponent<ImplantController>().BootsImplant = null;
                break;
        }

        IsEquipped = false;
        transform.position = PlayerState.transform.position;
        transform.rotation = Quaternion.identity;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
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
