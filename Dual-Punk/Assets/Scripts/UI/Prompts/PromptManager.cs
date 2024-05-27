using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PromptManager : MonoBehaviour 
{
    private const float _DEFAULTPROMPTPOSITIONX = 0;
    private const float _DEFAULTPROMPTPOSITIONY = -400;
    public GameObject CurrentPromptShown;
    public GameObject CurrentIndicatorShown;
    public static PromptManager Instance;

    //

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //

    public bool SpawnPrompt(Prompt prompt, float xPos = _DEFAULTPROMPTPOSITIONX, float yPos = _DEFAULTPROMPTPOSITIONY)
    {
        if (CurrentPromptShown == null)
        {
            GameObject promptObject;
            switch (prompt.PromptType)
            {
                case PromptType.Closable:
                    promptObject = (GameObject)Instantiate(Resources.Load("ClosablePrompt"), new Vector2(xPos, yPos), quaternion.identity);
                    break;
                case PromptType.Unclosable:
                    promptObject = (GameObject)Instantiate(Resources.Load("UnclosablePrompt"), new Vector2(xPos, yPos), quaternion.identity);
                    break;
                case PromptType.Dialogue:
                    promptObject = (GameObject)Instantiate(Resources.Load("DialoguePrompt"), new Vector2(xPos, yPos), quaternion.identity);
                    break;
                default:
                    throw new System.Exception("prompt type not implemented");
            }
            if (promptObject != null)
            {
                CurrentPromptShown = promptObject;
                PromptController controller = promptObject.AddComponent<PromptController>();
                controller.Prompt = prompt;
                controller.Init();
                promptObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
                return true;
            }
            else return false;
        }
        else return false;
    }
    public void SpawnIndicator(Sprite _indicatorSprite, Vector3 position)
    {
        if (CurrentIndicatorShown is not null)
        {
            CloseCurrentIndicator();
        }
        GameObject indicatorObject = (GameObject)Instantiate(Resources.Load("Indicator"), position, quaternion.identity);
        indicatorObject.GetComponent<SpriteRenderer>().sprite = _indicatorSprite;
        CurrentIndicatorShown = indicatorObject;
    }

    public void CloseCurrentPrompt()
    {
        if (CurrentPromptShown != null)
        {
            Destroy(CurrentPromptShown);
        }
    }
    public void CloseCurrentIndicator()
    {
        if (CurrentIndicatorShown != null)
        {
            Destroy(CurrentIndicatorShown);
        }
    }
}