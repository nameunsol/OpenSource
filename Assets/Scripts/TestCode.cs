using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public static TestCode Instance;

    public int num = 5;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
