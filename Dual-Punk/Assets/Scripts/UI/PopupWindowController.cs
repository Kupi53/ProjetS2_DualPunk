using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindowController : MonoBehaviour
{
    private Button _closeButton;
    void Start()
    {
        _closeButton = GetComponentInChildren<Button>();
        _closeButton.onClick.AddListener(() => {
            GameObject.Destroy(gameObject);
        });
    }

    void Update()
    {
        
    }
}
