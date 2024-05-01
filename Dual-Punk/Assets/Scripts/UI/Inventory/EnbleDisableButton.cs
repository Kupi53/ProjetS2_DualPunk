using UnityEngine;

public class EnbleDisableButton : MonoBehaviour
{
    [SerializeField] private DescriptionManager descriptions;

    void Start()
    {
        descriptions.enabled = false;
    }

    public void ChangeActiveness()
    {
        descriptions.enabled = !descriptions.enabled;
    }
}