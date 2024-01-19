using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashIconRender : MonoBehaviour
{
    // dashdisabletop = le "Top" du rectangleTransform pour que la DashIcon soit Recouverte par DashCooldownIcon
    private float DashDisabledTop = 150f;
    private float transformMultiplier;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        /* le rapport entre dashdisabledtop et le dashcooldownMax
        le but est que pour dashcooldown max on puisse faire dashcooldown * transform = dashdisabledTop
        comme ça on part de Top = 150 (dashicon recouverte) et on va a dashcooldown = 0 donc Top = 0 
        au fur et à mesure que dashcooldown tend vers 0 */
        transformMultiplier = DashDisabledTop / PlayerState.DashCooldownMax;
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // si dashcooldown
        if (PlayerState.DashCooldown > 0)
        {
            // dashdisabledicon fait son truc
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -150 + PlayerState.DashCooldown * transformMultiplier);
        }
    }
}
