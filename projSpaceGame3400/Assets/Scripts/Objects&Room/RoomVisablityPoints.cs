using UnityEngine;

public class RoomVisibilityPoints : MonoBehaviour
{
    [Header("Visibility Check Points")]
    [SerializeField] private Transform[] checkPoints;
    [SerializeField] private bool showDebugLines = true;

    private void OnDrawGizmos()
    {
        if (showDebugLines && checkPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform point in checkPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);
                }
            }
        }
    }

    public Vector3[] GetCheckPointPositions()
    {
        if (checkPoints == null) return new Vector3[0];
        
        Vector3[] positions = new Vector3[checkPoints.Length];
        for (int i = 0; i < checkPoints.Length; i++)
        {
            if (checkPoints[i] != null)
            {
                positions[i] = checkPoints[i].position;
            }
        }
        return positions;
    }
}