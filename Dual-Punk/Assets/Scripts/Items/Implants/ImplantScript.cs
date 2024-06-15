using FishNet.Object;
using UnityEngine;
using System;

public enum ImplantType
{
    Neuralink,
    ExoSqueleton,
    Arm,
    Boots
}

public abstract class ImplantScript : NetworkBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected ObjectSpawner _objectSpawner;

    public PlayerState PlayerState { get; set; }
    public ImplantType Type { get; protected set; }
    public string SetName { get; protected set; }
    public bool IsEquipped { get; set; } = false;

    
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

        ObjectSpawner.Instance.ImplantSetIsEquippedRPC(gameObject, false);
        transform.position = PlayerState.transform.position;
        transform.rotation = Quaternion.identity;
        ObjectSpawner.Instance.ImplantRendererSetEnabledRPC(gameObject, true);
        if (!GameManager.Instance.InTutorial && !GameManager.Instance.InFinalFight)
        {
            ObjectSpawner.Instance.ObjectParentToRoomRpc(gameObject);
        }
        else
        {
            ObjectSpawner.Instance.ObjectParentToGameObjectRpc(gameObject, null, gameObject.transform.position);
        }
        RemoveAllOwnerShipRPC(gameObject.GetComponent<NetworkObject>());
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
