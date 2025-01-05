using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Room Configuration")]
    [SerializeField] private string roomName;
    [SerializeField] private BoxCollider roomTrigger;
    [SerializeField] private List<TransformableObject> transformableObjects = new List<TransformableObject>();
    
    [Header("Debug")]
    [SerializeField] private bool playerInRoom = false;
    [SerializeField] private bool playerLookingAtRoom = false;
    private bool gameStarted = false;

    private void Awake()
    {
        if (roomTrigger == null)
            roomTrigger = GetComponent<BoxCollider>();
        
        roomTrigger.isTrigger = true;
    }

        private void Start()
    {
      
        Invoke(nameof(EnableTransformations), 0.5f);
    }

    private void EnableTransformations()
    {
        gameStarted = true;
    }

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entered room");
        playerInRoom = true;
        PlayerRoomTracker tracker = other.GetComponent<PlayerRoomTracker>();
        if (tracker != null)
            tracker.SetCurrentRoom(this);
    }
}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoom = false;
            if (!playerLookingAtRoom)
            {
                foreach (var obj in transformableObjects)
                {
                    obj.CycleToNextState();
                }
            }
        }
    }

    public void SetPlayerLookingAt(bool isLooking)
    {
    playerLookingAtRoom = isLooking;
        
        if (gameStarted && !playerLookingAtRoom && !playerInRoom)
        {
            foreach (var obj in transformableObjects)
            {
                obj.CycleToNextState();
            }
        }
    }

    public string GetRoomName() => roomName;
    public bool IsPlayerInRoom() => playerInRoom;
    public bool IsPlayerLookingAt() => playerLookingAtRoom;
}