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
     public void EffectTileActionFromOtherPlayerRpc(NetworkConnection con)
     {
        ETAFOPTargetRpc(con);
     }
     [TargetRpc]
     private void ETAFOPTargetRpc(NetworkConnection con)
     {
        EffectTilesController controller = FloorManager.Instance.CurrentRoom.gameObject.GetComponentInChildren<EffectTilesController>();
        GameObject otherPlayer;
        if (GameManager.Instance.LocalPlayer == GameManager.Instance.Player1){
            otherPlayer = GameManager.Instance.Player2;
        }
        else{
            otherPlayer = GameManager.Instance.Player1;
        }
        #nullable enable
        EffectTile? effectTile = controller.GetTileStoodOn(otherPlayer);
        if (effectTile != null)
        {
            effectTile.Action(GameManager.Instance.LocalPlayer);
        }
     }
}