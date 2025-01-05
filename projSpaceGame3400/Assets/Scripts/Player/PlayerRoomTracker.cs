using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerRoomTracker : MonoBehaviour
{
    [Header("Vision Configuration")]
    [SerializeField] private float visionCheckDistance = 20f;
    [SerializeField] private LayerMask roomCheckLayers;
    [SerializeField] private bool showDebugRays = true;
    
    private Camera playerCamera;
    private RoomManager currentRoom;
    private RoomManager lastLookedAtRoom;
    private RoomVisibilityPoints visibilityPoints;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        CheckRoomLineOfSight();
    }

    private void CheckRoomLineOfSight()
    {
        Ray centerRay = new Ray(transform.position, transform.forward);
        RaycastHit centerHit;

        RoomManager lookedAtRoom = null;
        if (Physics.Raycast(centerRay, out centerHit, visionCheckDistance, roomCheckLayers))
        {
            lookedAtRoom = centerHit.collider.GetComponent<RoomManager>();
        }

        if (lookedAtRoom != null)
        {
            visibilityPoints = lookedAtRoom.GetComponent<RoomVisibilityPoints>();
            if (visibilityPoints != null)
            {
                Vector3[] points = visibilityPoints.GetCheckPointPositions();
                int visiblePoints = 0;

                foreach (Vector3 point in points)
                {
                    Vector3 directionToPoint = (point - transform.position).normalized;
                    Ray ray = new Ray(transform.position, directionToPoint);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.transform.IsChildOf(lookedAtRoom.transform))
                        {
                            visiblePoints++;
                        }

                        if (showDebugRays)
                        {
                            Debug.DrawLine(transform.position, hit.point, 
                                hit.collider.transform.IsChildOf(lookedAtRoom.transform) ? Color.green : Color.red);
                        }
                    }
                }

                bool canSeeRoom = visiblePoints > 0;

                if (lookedAtRoom != lastLookedAtRoom)
                {
                    if (lastLookedAtRoom != null)
                        lastLookedAtRoom.SetPlayerLookingAt(false);
                    
                    lastLookedAtRoom = lookedAtRoom;
                }

                lookedAtRoom.SetPlayerLookingAt(canSeeRoom);
            }
        }
        else if (lastLookedAtRoom != null)
        {
            lastLookedAtRoom.SetPlayerLookingAt(false);
            lastLookedAtRoom = null;
        }
    }

    public void SetCurrentRoom(RoomManager room)
    {
        if (currentRoom != room)
        {
            if (currentRoom != null)
            {
                currentRoom.SetPlayerLookingAt(false);
            }
            currentRoom = room;
        }
    }

    public RoomManager GetCurrentRoom() => currentRoom;
}