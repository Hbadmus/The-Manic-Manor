using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 6f;

    [SerializeField]
    private float groundDrag = 5f;

    [SerializeField]
    private Transform orientation;

    float horizontalInput;
    float verticalInput;
    float zInput; 
    bool isMoving;


    Vector3 moveDirection;
    Rigidbody rb;
    private static bool canMove = true;

    // The Oscillator to create slight lurching in the movement
    [SerializeField] float oscillationVal = 0f;
    [SerializeField] float oscillationFreq = 8f;
    float lurchVal = 0f;
    [SerializeField] float lurchStrength = 0.05f;

    // values for footstep sounds
    //float lastStepTime;
    [SerializeField]
    //float stepDelayTime = .5f;


    //FindObjectOfType<AudioManager>().Play("Footstep" + (UnityEngine.Random.Range(0, 2) + 1));

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = groundDrag;
        isMoving = false;
        //lastStepTime = 0f;

    }

    // FixedUpdate handles all physics-related movement
    private void FixedUpdate()
    {
        MovePlayer();
        //StepTimer();

    }


    // Update is called once per frame
    void Update()
    {

        // update the oscillation value
        oscillationVal = MathF.Sin(oscillationFreq * Time.time);
        lurchVal = oscillationVal * moveSpeed /* + UnityEngine.Random.Range(-1f, 1f)*/  * lurchStrength;

        MyInput();

    }

private void MyInput()
{
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Mathf.Max(0, Input.GetAxisRaw("Vertical"));
    isMoving = horizontalInput != 0 || verticalInput != 0 && canMove;
}

    private void CalcLurch()
    {
        if (isMoving)
        {
            // update the oscillation value
            oscillationVal = MathF.Sin(oscillationFreq * Time.time);
            lurchVal = oscillationVal * moveSpeed /* + UnityEngine.Random.Range(-1f, 1f)*/  * lurchStrength;
        }
    }

    private void MovePlayer()
    {

        if (canMove)
        {
            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            //                                      pos fwd                               pos right

            rb.AddForce(moveDirection.normalized * (moveSpeed + lurchVal) * 10f, ForceMode.Force);

        }

    }

    /*
    public void StepTimer()
    {
        if (lastStepTime + stepDelayTime <= Time.time && isMoving && canMove)
        {
            AudioManager.instance.Play("Footstep" + (UnityEngine.Random.Range(0, 2) + 1));
            lastStepTime = Time.time;
        }
    }*/



    public static void setCanMove(bool value)
    {
        canMove = value;
    }

    public static bool getCanMove()
    {
        return canMove;
    }
}
