using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    public TMP_Text crossHairText;
    public TMP_Text Notification;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    private System.Random rand = new System.Random();


    int DiceRoll()
    {
        return rand.Next(1, 20);
    }

    void TrophyEvent()
    {
        int result = DiceRoll();
        if (result == 20)
        {
            Notification.text = $"({result})" + " Critical Success! The trophy imparts clues and wisdom";
        }
        else if (result >= 12)
        {
            Notification.text = $"({result})" + " You have successfully interacted with the trophy! It imparts wisdom upon you";
        }
        else if (result <= 11 && result >= 2)
        {
            Notification.text = $"({result})" + " Failure, nothing happens";
        }
        else if (result == 1)
        {
            Notification.text = $"({result})" + " Critical failrue the trophy laughs at you";
        }
    }

    void PortalEvent()
    {
        int result = DiceRoll();
        if (result == 20)
        {
            Notification.text = $"({result})" + " You are teleported to a secret location!";
        }
        else if (result >= 12)
        {
            Notification.text = $"({result})" + " You are teleported to another realm!";
        }
        else if (result <= 11 && result >= 2)
        {
            Notification.text = $"({result})" + " Failure, nothing happens";
        }
        else if (result == 1)
        {
            Notification.text = $"({result})" + " You are teleported to the Transitive Planes";
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }


        if (Input.GetMouseButtonDown(0))
            {
                if(crossHairText.text == "Press E to interact with portal?")
                {
                    PortalEvent();
                } else if(crossHairText.text == "Interact with trophy?")
                {
                    TrophyEvent();
                }
            }

        if (Input.GetMouseButtonDown(1))
            {
                
            }
    }
}