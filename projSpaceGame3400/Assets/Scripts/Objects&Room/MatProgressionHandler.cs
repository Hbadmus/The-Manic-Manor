using UnityEngine;

public class MatProgressionHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer matRenderer;
    [SerializeField] private int totalObjects = 8;
    
    private int objectsFound = 0;
    private Material matInstance;
    private bool isPulsing = false;
    private bool hasBeenClicked = false;

    private void Start()
    {
        if (matRenderer == null)
            matRenderer = GetComponent<MeshRenderer>();
            
        matInstance = matRenderer.material;
        UpdateColor(0);
        matInstance.SetFloat("_OcclusionStrength", 0);
    }

    private void Update()
    {
        if (isPulsing && !hasBeenClicked)
        {
            float pulse = (Mathf.Sin(Time.time * 2) + 1) / 2; 
            UpdateColor(pulse);
        }
    }

    public void OnObjectFound()
    {
        objectsFound = Mathf.Clamp(objectsFound + 1, 0, totalObjects);
        float progress = (float)objectsFound / totalObjects;
        
        UpdateColor(progress);
        matInstance.SetFloat("_OcclusionStrength", progress);

        if (objectsFound >= totalObjects)
        {
            isPulsing = true;
        }
    }

    private void UpdateColor(float progress)
    {
        Color newColor = new Color(progress, progress, progress);
        matInstance.color = newColor;
    }

    public bool IsComplete()
    {
        return objectsFound >= totalObjects;
    }

    public void OnMatClicked()
    {
        if (IsComplete() && !hasBeenClicked)
        {
            hasBeenClicked = true;
            isPulsing = false;
            UpdateColor(1); 
            GameManager.Instance.StartWinSequence();
        }
    }
}