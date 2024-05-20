using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class EffectTileNetworkWrapper : NetworkBehaviour
{
     public static EffectTileNetworkWrapper Instance;

     override public void OnStartNetwork(){
        base.OnStartNetwork();
        if (Instance==null)
        {
            Instance = this;
        }
     }

     [ServerRpc(RequireOwnership = false)]
     public void EffectTileActionFromOtherPlayerRpc(NetworkConnection con, Vector3Int pos)
     {
        ETAFOPTargetRpc(con, pos);
     }
     [TargetRpc]
     private void ETAFOPTargetRpc(NetworkConnection con, Vector3Int pos)
     {
        EffectTilesController controller = FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.gameObject.GetComponentInChildren<EffectTilesController>();
        #nullable enable
        EffectTile? effectTile = controller.GetTileStoodOn(pos);
        Debug.Log("test1");
        if (effectTile != null)
        {
            Debug.Log("test");
            effectTile.Action(GameManager.Instance.LocalPlayer);
        }
     }
}