using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentHealth : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        text.text = PlayerState.Health.ToString();
    }
}
