using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TotalAmmo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    private LocalPlayerReference References;

    void Start()
    {
        References = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }

    void Update()
    {
        Text.enabled = References.playerState.HoldingWeapon;
    }
}
