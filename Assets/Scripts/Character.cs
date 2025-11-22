using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Threading.Tasks;

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

    public TMP_Text resultText;

    public TMP_Text UItext;

    public GameObject interactionUI;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool CanMove = true;

    private System.Random rand = new System.Random();

    private bool PortalActive = false;

    public GameObject PortalHome;

    public GameObject portalCPass;
    public GameObject portalPass;
    public GameObject portalCFail;

    public GameObject Fireplace;

    public int[] correctSequence = new int[5];
    private int currentIndex = 0;
    private CancellationTokenSource cts;

    private bool isInteracting = false;

    public void DieClicked(int number)
    {
        if (number == correctSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= correctSequence.Length)
            {
                Notification.text = "Portal Activated!";
                PortalActive = true;
                currentIndex = 0;
            } else
            {
                Notification.text = "Correct!";
            }
        }
        else
        {
            Notification.text = "Incorrect!";
            currentIndex = 0; 
        }
    }

    public void TextClick()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }
        cts = new CancellationTokenSource();
        ClearText(cts.Token);
    }

    async void ClearText(CancellationToken token)
    {
        try
        {
            await Task.Delay(Mathf.RoundToInt(5000), cts.Token);
            Notification.text = "";
        }
        catch (TaskCanceledException)
        {
            // Cancels exit
        }
    }

    void TeleportPlayer(GameObject destination)
    {
        characterController.enabled = false;
        characterController.transform.position = destination.transform.position;
        characterController.enabled = true;
    }

    int DiceRoll()
    {
        return rand.Next(1, 21);
    }

    IEnumerator WaitForUIButton()
    {
        interactionUI?.SetActive(true);
        isInteracting = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        resultText.text = "";
        
        // Disable camera movement script
        MonoBehaviour cameraScript = playerCamera.GetComponent<MonoBehaviour>();
        if (playerCamera != null)
            playerCamera.enabled = false;
        
        // Reset result text and wait for dice roll
        if (UItext != null)
        {
            UItext.text = "Click the button to roll the dice!";
            UItext.color = Color.white;
        }

        yield return new WaitUntil(() => resultText.text != "");

        PortalEvent(int.Parse(resultText.text));

        yield return new WaitForSeconds(2f);

        UItext.text = "";
        isInteracting = false;
        interactionUI?.SetActive(false);
        playerCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;


        

    }

    void PortalEvent(int result)
    {

        
        if (result == 20)
        {
            Notification.text = $"({result})" + " You are teleported to a secret location!";
            UItext.text = "Secret location discovered!";
            UItext.color = Color.orange;
            TeleportPlayer(portalCPass);
        }
        else if (result >= 12)
        {
            Notification.text = $"({result})" + " You are teleported to another realm!";
            UItext.text = "Tavern Entrance Discovered!";
            UItext.color = Color.green;
            TeleportPlayer(portalPass);
        }
        else if (result <= 11 && result >= 2)
        {
            Notification.text = $"({result})" + " Failure, nothing happens";
            UItext.text = "Failure, nothing happens.";
            UItext.color = Color.white;
        }
        else if (result == 1)
        {
            Notification.text = $"({result})" + " You are teleported to the Transitive Planes";
            UItext.text = "You have been sent to the Transitive Planes!";
            UItext.color = Color.red;
            TeleportPlayer(portalCFail);
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interactionUI?.SetActive(false);
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = CanMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = CanMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && CanMove && characterController.isGrounded)
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

        if (Input.GetKey(KeyCode.R) && CanMove)
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

        if (CanMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }


        if (Input.GetMouseButtonDown(0) && isInteracting == false)
            {
                if(crossHairText.text == "Interact with portal?")
                {
                    if(!PortalActive)
                    {
                        Notification.text = "Portal is offline.";
                    }
                    else{
                        StartCoroutine(WaitForUIButton());
                    }
                    
                } else if(crossHairText.text == "Interact with trophy?")
                {
                } else if(crossHairText.text == "Interact to return home")
                {
                    TeleportPlayer(PortalHome);
                } else if(crossHairText.text == "Interact with blue die?")
                {
                    DieClicked(3);
                } else if(crossHairText.text == "Interact with red die?")
                {
                    DieClicked(1);
                } else if(crossHairText.text == "Interact with green die?")
                {
                    DieClicked(4);
                } else if(crossHairText.text == "Interact with white die?")
                {
                    DieClicked(5);
                } else if(crossHairText.text == "Interact with yellow die?")
                {
                    DieClicked(2);
                }
                
                if(crossHairText.text != null)
                {
                    TextClick();
                }
            }
            
    }


}