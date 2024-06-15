using System.Collections;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    public Prompt Prompt;
    [SerializeField] private PromptTriggerType _type;
    [SerializeField] private float _delay;

    void Start ()
    {
        if (_type == PromptTriggerType.Automatic)
        {
            TriggerPromptAfterSeconds(_delay);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        if (_type == PromptTriggerType.OnCollision)
        {
            Debug.Log(other);
            Spawn();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!enabled) return;
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        if (_type == PromptTriggerType.OnButton)
        {
            if (Input.GetButtonDown("Interact") && other.gameObject.GetComponent<MouvementsController>().EnableMovement)
            {
                Spawn();
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) return;
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        if (_type == PromptTriggerType.OnCollision)
        {
            PromptManager.Instance.ClosePrompt(Prompt);
        }
    }

    private IEnumerator TriggerPromptAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Spawn();
    }

    public void Spawn()
    {
        PromptManager.Instance.SpawnPrompt(Prompt, this.gameObject);
    }
}

public enum PromptTriggerType
{
    Automatic,
    OnCollision,
    OnButton,
    Tutorial
}