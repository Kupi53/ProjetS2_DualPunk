using UnityEngine;

public class EnbleDisableButton : MonoBehaviour
{
    [SerializeField] private DescriptionManager descriptions;

    void Start()
    {
        descriptions.enabled = true;
    }

    public void ChangeActiveness()
    {
        descriptions.enabled = !descriptions.enabled;
    }
}