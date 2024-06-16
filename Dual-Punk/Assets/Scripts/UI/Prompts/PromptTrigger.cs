using System.Collections;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    public Prompt Prompt;
    [SerializeField] private PromptTriggerType _type;
    [SerializeField] private float _delay;
    private bool onTrigger;
    private GameObject playerOn;

    void Start ()
    {
        onTrigger = false;
        playerOn = null;
        if (_type == PromptTriggerType.Automatic)
        {
            TriggerPromptAfterSeconds(_delay);
        }
    }

    void Update()
    {
        if (enabled && _type == PromptTriggerType.OnButton && onTrigger && Input.GetButtonDown("Interact") && playerOn.GetComponent<MouvementsController>().EnableMovement)
        {
            Spawn();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        if (_type == PromptTriggerType.OnCollision)
        {
            Spawn();
        }
        else if (_type == PromptTriggerType.OnButton)
        {
            onTrigger = true;
            playerOn = other.gameObject;
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
        else if (_type == PromptTriggerType.OnButton)
        {
            onTrigger = false;
            playerOn = null;
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