using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image image;

    private float HealthMaxRight;
    private float HealthMinRight;
    private float transformMultiplier;
    private RectTransform rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = image.GetComponent<RectTransform>();
        HealthMaxRight = rectTransform.offsetMin.x;
        HealthMinRight = -rectTransform.offsetMax.x;
        transformMultiplier = (HealthMinRight - HealthMaxRight) / PlayerState.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.offsetMax = new Vector2(-HealthMaxRight - transformMultiplier * (PlayerState.MaxHealth - PlayerState.Health), rectTransform.offsetMax.y);
    }

    [ContextMenu("DecreaseHealth")]
    public void DecreaseHealth()
    {
        Debug.Log("HealthDecreased");
        PlayerState.Health -= 9;
        if (PlayerState.Health < 0)
            PlayerState.Health = 0;
    }

    [ContextMenu("IncreaseHealth")]
    public void IncreaseHealth()
    {
        Debug.Log("HealthIncreased");
        PlayerState.Health += 9;
        if (PlayerState.Health > PlayerState.MaxHealth)
            PlayerState.Health = PlayerState.MaxHealth;
    }
}
