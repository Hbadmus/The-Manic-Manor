using UnityEngine;

public class TransformableObjectClickHandler : MonoBehaviour
{
    private TransformableObject mainObject;

    public void Initialize(TransformableObject obj)
    {
        mainObject = obj;
    }

    public TransformableObject GetMainObject()
    {
        return mainObject;
    }
}