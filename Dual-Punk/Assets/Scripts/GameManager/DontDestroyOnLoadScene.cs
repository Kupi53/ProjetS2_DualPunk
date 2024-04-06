using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScene : MonoBehaviour
{
    private static DontDestroyOnLoadScene _instance;
    public GameObject[] objects;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            foreach (var element in objects)
            {
                DontDestroyOnLoad(element);
            }
        }
    }
}