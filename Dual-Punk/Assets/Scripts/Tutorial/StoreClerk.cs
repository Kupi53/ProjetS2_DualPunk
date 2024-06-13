using GameKit.Utilities;
using Unity.Mathematics;
using UnityEngine;

public class StoreClerk : MonoBehaviour
{
    public void OnDialogueEnd()
    { 
        ObjectSpawner.Instance.SpawnObjectFromIdRpc("0009", GameManager.Instance.LocalPlayer.transform.position, quaternion.identity);
        ObjectSpawner.Instance.SpawnObjectFromIdRpc("0020", GameManager.Instance.LocalPlayer.transform.position, quaternion.identity);
    }
}