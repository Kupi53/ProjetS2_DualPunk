using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoInMag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private LocalPlayerReference _references;

    void Start()
    {
        _references = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }

    void Update()
    {
        if (_references.PlayerState.HoldingWeapon && _references.FireArmScript != null)
        { 
            _text.text = _references.FireArmScript.AmmoLeft.ToString();
            _text.enabled = true;
        }
        else
            _text.enabled = false;
    }
}
