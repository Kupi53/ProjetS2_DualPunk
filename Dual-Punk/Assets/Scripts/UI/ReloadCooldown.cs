using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
        if (References.playerState.HoldingWeapon)
        {
            if (References.weaponScript.isReloading)
            {
                multiplier = (MaxTop - MinTop) / References.weaponScript.reloadTime;
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -MinTop - multiplier * References.weaponScript.reloadTimer);
            }
        }
    }
}
