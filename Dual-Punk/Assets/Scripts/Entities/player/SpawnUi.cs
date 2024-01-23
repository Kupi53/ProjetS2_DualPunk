using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUi : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    private GameObject LocalUI;
    void Start()
    {
        GameObject LocalUI = Instantiate(UI);
        LocalUI.GetComponent<LocalPlayerReference>().LOCALPLAYER = gameObject;
    }

}
