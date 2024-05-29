using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class FloorNetworkWrapper : NetworkBehaviour
{
     public static FloorNetworkWrapper Instance;
     [SerializeField] private GameObject _floorManagerPrefab;
     public FloorManager LocalFloorManager {
        get
        {
            return GameObject.FindWithTag("FloorManager").GetComponent<FloorManager>();
        }
     }

     override public void OnStartNetwork(){
        base.OnStartNetwork();
        if (Instance==null)
        {
            Instance = this;
        }
        Instantiate(_floorManagerPrefab);
     }

    public void NewFloor(FloorType floorType)
    {
        if (!IsServer) return;
        
        int seed = UnityEngine.Random.Range(0,9999);
        SeedRPC(seed);

        if (LocalFloorManager.CurrentFloor != null)
        {
            DestroyCurrentFloorRPC();
        }
        SwitchToNewFloorRPC(floorType);
    }

    [ObserversRpc]
    private void SeedRPC(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }
    [ObserversRpc]
    private void DestroyCurrentFloorRPC()
    {
        if (LocalFloorManager.CurrentFloor != null)
        {
            LocalFloorManager.CurrentFloor.DestroyHolder();
        }
    }
    [ObserversRpc]
    private void SwitchToNewFloorRPC(FloorType floorType)
    {
        LocalFloorManager.CurrentFloor = LocalFloorManager.GenerateFloor(floorType);
        LocalFloorManager.SwitchRoom(LocalFloorManager.CurrentFloor.Entry);
        GameManager.Instance.LocalPlayer.transform.position = 
        LocalFloorManager.CurrentFloor.Entry.GetComponent<Grid>().CellToWorld(RoomExitTile.ComputeTargetCoordinates(
        Instance.LocalFloorManager.CurrentFloor.Entry._entryWallCardinal, 
        Instance.LocalFloorManager.CurrentFloor.Entry));
    }
}