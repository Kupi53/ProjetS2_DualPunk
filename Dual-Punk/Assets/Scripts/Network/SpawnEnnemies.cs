using UnityEngine;
using UnityEngine.Networking;
using FishNet.Object;


public class SpawnEnnemies : NetworkBehaviour
{
    [SerializeField] private GameObject randomEnnemy; //add prefab in inspector
    
    public override void OnStartServer()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject bot = Instantiate(randomEnnemy, new Vector3(1 + i, -1 * i, 0), Quaternion.identity);
            Spawn(bot);
        }
    }
}
