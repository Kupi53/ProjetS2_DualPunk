using FishNet.Object;
using UnityEngine;

public class IndicatorTrigger : NetworkBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private float indicatorPosX;
    [SerializeField] private float indicatorPosY;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == ClientManager.Connection)) return;
        PromptManager.Instance.SpawnIndicator(_icon, new Vector3(indicatorPosX, indicatorPosY, 2), this.gameObject);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == ClientManager.Connection)) return;
        PromptManager.Instance.CloseCurrentIndicator();
    }
}