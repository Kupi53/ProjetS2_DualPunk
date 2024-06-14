using FishNet.Object;
using UnityEngine;

public class IndicatorTrigger : NetworkBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private float indicatorPosX;
    [SerializeField] private float indicatorPosY;
    private GameObject parent;

    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == ClientManager.Connection)) return;
        PromptManager.Instance.SpawnIndicatorWithParent(_icon, new Vector3(indicatorPosX, indicatorPosY, 2), this.gameObject, parent);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == ClientManager.Connection)) return;
        PromptManager.Instance.CloseCurrentIndicator();
    }

    public void SpawnIndicatorWithParent(GameObject parent)
    {
        PromptManager.Instance.SpawnIndicatorWithParent(_icon, new Vector3(indicatorPosX, indicatorPosY, 2), this.gameObject, parent);
    }
}