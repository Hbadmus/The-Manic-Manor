using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraHolder : MonoBehaviour
{

    [SerializeField]
    private Transform cameraPosition;

    public MoveCameraHolder Instance { get; set; }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position; 
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Two MoveCameraHolders");
        }
        else
        {
            Instance = this;
        }
    }
}
