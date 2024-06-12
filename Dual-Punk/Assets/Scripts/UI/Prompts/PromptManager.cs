using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PromptManager : MonoBehaviour 
{
    private const float _DEFAULTPROMPTPOSITIONX = 0;
    private const float _DEFAULTPROMPTPOSITIONY = -400;
    public GameObject CurrentPromptShown;
    public GameObject CurrentIndicatorShown;
    public GameObject CurrentArrowShown;
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

    void Update()
    {
        CheckCurrentPromptTrigger();
        CheckCurrentIndicatorTrigger();
    }

    //

    public bool SpawnPrompt(Prompt prompt, GameObject Trigger, float xPos = _DEFAULTPROMPTPOSITIONX, float yPos = _DEFAULTPROMPTPOSITIONY)
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
                controller.Prompt.Trigger = Trigger;
                controller.Init();
                promptObject.transform.SetParent(GameObject.Find("BaseUI").transform, false);
                return true;
            }
            else return false;
        }
        else return false;
    }
    public void SpawnIndicator(Sprite _indicatorSprite, Vector3 position, GameObject Trigger)
    {
        if (CurrentIndicatorShown is not null)
        {
            CloseCurrentIndicator();
        }
        GameObject indicatorObject = (GameObject)Instantiate(Resources.Load("Indicator"), position, quaternion.identity);
        indicatorObject.GetComponent<SpriteRenderer>().sprite = _indicatorSprite;
        Indicator indicator = indicatorObject.AddComponent<Indicator>();
        indicator.Trigger = Trigger;
        CurrentIndicatorShown = indicatorObject;
    }

    public void SpawnPointerArrow(GameObject target)
    {
        if (CurrentArrowShown is not null)
        {
            CloseCurrentArrow();
        }
        GameObject arrowObject = (GameObject)Instantiate(Resources.Load("PointerArrow"));
        CurrentArrowShown = arrowObject;
        arrowObject.GetComponent<PointerArrow>().Target = target;
        arrowObject.transform.SetParent(GameObject.Find("BaseUI").transform, false);
    }

    public void CloseCurrentPrompt()
    {
        if (CurrentPromptShown != null)
        {
            Destroy(CurrentPromptShown);
            CurrentPromptShown = null;
        }
    }
    public void CloseCurrentIndicator()
    {
        if (CurrentIndicatorShown != null)
        {
            Destroy(CurrentIndicatorShown);
            CurrentIndicatorShown = null;
        }
    }

    public void CloseCurrentArrow()
    {
        if (CurrentArrowShown != null)
        {
            Destroy(CurrentArrowShown);
            CurrentArrowShown = null;
        }
    }

    private void CheckCurrentPromptTrigger()
    {
        if (CurrentPromptShown == null) return;
        else
        {
            if (CurrentPromptShown.GetComponent<PromptController>().Prompt.Trigger == null)
            {
                CloseCurrentPrompt();
            }
        }
    }

    private void CheckCurrentIndicatorTrigger()
    {
        if (CurrentIndicatorShown == null) return;
        else
        {
            if (CurrentIndicatorShown.GetComponent<Indicator>().Trigger == null)
            {
                CloseCurrentIndicator();
            }
        }
    }
}