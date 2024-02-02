using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ReloadCooldown : MonoBehaviour
{
    [SerializeField] private RawImage Image;

    private float MaxTop;
    private float MinTop;
    private float multiplier;
    private RectTransform rectTransform;
    private LocalPlayerReference References;


    void Start()
    {
        rectTransform = Image.GetComponent<RectTransform>();
        MaxTop = -rectTransform.offsetMax.y;
        MinTop = rectTransform.offsetMin.y;
        References = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }

    private void Update()
    {
        Image.enabled = false;
        if (References.playerState.HoldingWeapon && References.weaponScript.reloading)
        {
            Image.enabled = true;
            multiplier = (MaxTop - MinTop) / References.weaponScript.reloadTime;
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -MinTop - multiplier * References.weaponScript.reloadTimer);
        }
        else if (References.playerState.HoldingKnife && References.knifeScript.attack != 0)
        {
            Image.enabled = true;
            multiplier = (MaxTop - MinTop) / References.knifeScript.resetCooldown;
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -MinTop - multiplier * References.knifeScript.resetCooldownTimer);
        }
    }
}
