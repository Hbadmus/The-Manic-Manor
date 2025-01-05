using UnityEngine;
using System.Collections.Generic;

public class TransformableObject : MonoBehaviour
{
    public enum TransformationType
    {
        Material,      // For simple material swaps like wood to grey
        Texture,       // For things like calendar dates or paintings
        GameObject,    // For complex swaps like silverware to medical tools
        Light         // For changing light colors like the fireplace
    }

        [System.Serializable]
    public class MaterialPair
    {
        public MeshRenderer targetRenderer;
        public Material normalMaterial;
        public Material transformedMaterial;
    }

    [System.Serializable]
    public class MaterialState
    {
        public Material normalMaterial;
        public Material transformedMaterial;
        [Tooltip("Which material represents reality")]
        public bool isNormalMaterialReal = true;
    }

 [System.Serializable]
    public class GameObjectState
    {
        [Tooltip("Array of normal state objects (e.g., all regular candles)")]
        public GameObject[] normalObjects;
        [Tooltip("Array of transformed state objects (e.g., all occult candles)")]
        public GameObject[] transformedObjects;
        [Tooltip("Which set represents reality")]
        public bool isNormalObjectReal = true;
    }

    [System.Serializable]
    public class LightState
    {
        public Color normalColor = Color.red;
        public Color transformedColor = Color.green;
        [Tooltip("Which color represents reality")]
        public bool isNormalColorReal = true;
    }

    [Header("Configuration")]
    [SerializeField] private TransformationType transformationType;
    [SerializeField] private string objectName;
    [SerializeField] private bool hasBeenFound = false;
    
    [Header("State Configuration")]
    [SerializeField] private MaterialPair[] materialPairs; 
    [SerializeField] private GameObjectState objectState;
    [SerializeField] private LightState lightState;
    
    [Header("Components")]
    [SerializeField] private Light lightComponent;

    private bool isTransformed = false;

private void Start()
{
    
    isTransformed = false;  
    
    if (transformationType == TransformationType.GameObject)
    {
        foreach (var obj in objectState.normalObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        foreach (var obj in objectState.transformedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
    
    if (GameManager.Instance != null)
    {
        GameManager.Instance.RegisterTransformableObject(this);
    }

    if (transformationType == TransformationType.GameObject)
    {
        foreach (var obj in objectState.normalObjects)
        {
            if (obj != null)
            {
                var handler = obj.AddComponent<TransformableObjectClickHandler>();
                handler.Initialize(this);
            }
        }
        
        foreach (var obj in objectState.transformedObjects)
        {
            if (obj != null)
            {
                var handler = obj.AddComponent<TransformableObjectClickHandler>();
                handler.Initialize(this);
            }
        }
    }
}

    public void Transform()
    {
    if (hasBeenFound) 
    {
        Debug.Log($"{gameObject.name} - Can't transform because it's found");
        return;
    }

    isTransformed = !isTransformed;
    Debug.Log($"{gameObject.name} - Transforming. isTransformed = {isTransformed}");
    
        
        switch (transformationType)
        {
            case TransformationType.Material:
                foreach (var pair in materialPairs)
                {
                    if (pair.targetRenderer != null)
                    {
                        pair.targetRenderer.material = isTransformed ? pair.transformedMaterial : pair.normalMaterial;
                    }
                }
                break;

        case TransformationType.GameObject:
            Debug.Log($"{gameObject.name} - GameObject type transformation");
            foreach (var obj in objectState.normalObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(!isTransformed);
                    Debug.Log($"Normal object {obj.name} set to {!isTransformed}");
                }
            }
            foreach (var obj in objectState.transformedObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(isTransformed);
                    Debug.Log($"Transformed object {obj.name} set to {isTransformed}");
                }
            }
            break;

            case TransformationType.Light:
                if (lightComponent != null)
                {
                    lightComponent.color = isTransformed ? lightState.transformedColor : lightState.normalColor;
                }
                break;
        }
    }

    public void SetNormalState()
    {
        isTransformed = false;
        Transform();
    }

    public bool IsInRealState()
{
    switch (transformationType)
    {
        case TransformationType.Material:
            foreach (var pair in materialPairs)
            {
                if (pair.targetRenderer != null)
                {
                    var currentMaterial = pair.targetRenderer.material;
                    bool isCurrentlyReal = pair.normalMaterial == currentMaterial;
                    if (isCurrentlyReal != !isTransformed)
                    {
                        return false;
                    }
                }
            }
            return true;

        case TransformationType.Texture:

            foreach (var pair in materialPairs)
            {
                if (pair.targetRenderer != null)
                {
                    var currentMaterial = pair.targetRenderer.material;
                    bool isCurrentlyReal = pair.normalMaterial == currentMaterial;
                    if (isCurrentlyReal != !isTransformed)
                    {
                        return false; 
                    }
                }
            }
            return true;

        case TransformationType.GameObject:
            return objectState.isNormalObjectReal != isTransformed;

        case TransformationType.Light:
            return lightState.isNormalColorReal != isTransformed;

        default:
            return false;
    }
}


    public void CycleToNextState()
    {
        Transform();
    }

    public void MarkAsFound()
    {
        hasBeenFound = true;
    }

    public bool IsFound()
    {
        return hasBeenFound;
    }

    public bool IsTransformed()
    {
        return isTransformed;
    }
}