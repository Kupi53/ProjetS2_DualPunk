using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PromptManager : MonoBehaviour 
{
    private const float _DEFAULTPROMPTPOSITIONX = 0;
    private const float _DEFAULTPROMPTPOSITIONY = -400;
    private List<GameObject> Prompts;
    //private bool 
    public GameObject CurrentPromptShown 
    {
        get
        {
            if (Prompts.Count == 0) return null;
            else return Prompts[Prompts.Count - 1];
        }
    }
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
        Prompts = new List<GameObject>();
    }

    //

    void Update()
    {
        CheckCurrentPromptTrigger();
        CheckCurrentIndicatorTrigger();
        CheckCurrentArrowTarget();
        CheckInventory();
        ManagePrompts();
    }

    //

    private void ManagePrompts()
    {
        for(int i = 0; i < Prompts.Count; i++)
        {
            if (Prompts[i] == null)
            {
                Prompts.RemoveAt(i);
                i--;
            }
        }
        if (CurrentPromptShown != null && CurrentPromptShown.activeSelf == false)
        {
            CurrentPromptShown.SetActive(true);
        }
    }

    public bool SpawnPrompt(Prompt prompt, GameObject Trigger, float xPos = _DEFAULTPROMPTPOSITIONX, float yPos = _DEFAULTPROMPTPOSITIONY)
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
            if (CurrentPromptShown != null) CurrentPromptShown.SetActive(false);
            Prompts.Add(promptObject);
            PromptController controller = promptObject.AddComponent<PromptController>();
            controller.Prompt = prompt;
            controller.Prompt.Trigger = Trigger;
            controller.Init();
            promptObject.transform.SetParent(GameObject.Find("BaseUI").transform, false);
            return true;
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
    public void SpawnIndicatorWithParent(Sprite _indicatorSprite, Vector3 position, GameObject Trigger, GameObject parent)
    {
        if (CurrentIndicatorShown is not null)
        {
            CloseCurrentIndicator();
        }
        GameObject indicatorObject = (GameObject)Instantiate(Resources.Load("Indicator"), parent.transform);
        indicatorObject.transform.position += position;
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
        }
    }
    public void ClosePrompt(Prompt prompt)
    {
        IEnumerable<GameObject> prompts = Prompts.Where(g => g != null && g.GetComponent<PromptController>().Prompt == prompt);
        if (prompts.Any())
        {
            Destroy(prompts.First());
        }
    }

    public void ClearPrompts()
    {
        foreach(GameObject prompt in Prompts)
        {
            Destroy(prompt);
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
        foreach (GameObject prompt in Prompts)
        {
            if (prompt != null && prompt.GetComponent<PromptController>().Prompt.Trigger == null)
            {
                Destroy(prompt);
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

    private void CheckCurrentArrowTarget()
    {
        if (CurrentArrowShown == null) return;
        else
        {
            if (CurrentArrowShown.GetComponent<PointerArrow>().Target == null)
            {
                CloseCurrentArrow();
            }
        }
    }

    private void CheckInventory()
    {
        
    }
}