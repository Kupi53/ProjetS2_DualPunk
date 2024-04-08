using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScene : MonoBehaviour
{
    public static DontDestroyOnLoadScene Instance;
    public GameObject[] objects;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            foreach (var element in objects)
            {
                DontDestroyOnLoad(element);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RemoveFromDontDestroyOnLoad()
    {
        foreach (var element in objects)
        {
            SceneManager.MoveGameObjectToScene(element, SceneManager.GetActiveScene());
        }
    }
}