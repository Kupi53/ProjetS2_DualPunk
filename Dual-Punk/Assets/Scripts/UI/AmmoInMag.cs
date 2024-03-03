using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoInMag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    private LocalPlayerReference References;

    void Start()
    {
        References = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }

    void Update()
    {
        if (References.playerState.HoldingWeapon && References.fireArmScript != null)
        { 
            Text.text = References.fireArmScript.magSize.ToString();
            Text.enabled = true;
        }
        else
            Text.enabled = false;
    }
}
