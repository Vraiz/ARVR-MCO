using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Collections;

public class PortalScript : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    public GameObject passwordUI;
    public TMP_Text passwordResultText;
    
    private Renderer rend;
    private bool isInteracting = false;
    private Camera arCamera;
    private XROrigin xrOrigin;
    private string correctPassword = "BlueYellowRedGreenWhite";
    private string currentAttempt = "";
    private bool passwordRevealed = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        
        // Find AR components
        FindARComponents();
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
            
        if (passwordUI != null)
            passwordUI.SetActive(false);
    }

    void FindARComponents()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            arCamera = xrOrigin.Camera;
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    void Update()
    {
        // AR Touch input
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForARTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForARTouch(Vector2 touchPosition)
    {
        if (arCamera == null) return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                StartInteraction();
            }
        }
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        if (!isInteracting)
        {
            rend.material = onMaterial;
            playerText.text = "Tap to interact with portal";
        }
    }

    public void OnGazeExit()
    {
        if (!isInteracting)
        {
            rend.material = offMaterial;
            playerText.text = "";
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        
        // Position UI in AR space
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PositionUIInWorldSpace(this.transform);
        }
        
        if (interactionUI != null)
            interactionUI.SetActive(true);
            
        // Show password UI if password has been revealed
        if (passwordRevealed && passwordUI != null)
        {
            passwordUI.SetActive(true);
            currentAttempt = "";
            UpdatePasswordDisplay();
        }
    }

    public void AddColorToPassword(string color)
    {
        if (!passwordRevealed) return;
        
        currentAttempt += color;
        UpdatePasswordDisplay();
        
        // Check if password is complete
        if (currentAttempt.Length >= correctPassword.Length)
        {
            if (currentAttempt == correctPassword)
            {
                // Correct password
                if (passwordResultText != null)
                {
                    passwordResultText.text = "SUCCESS! Portal activated!";
                    passwordResultText.color = Color.green;
                }
                StartCoroutine(CompletePortalActivation());
            }
            else
            {
                // Wrong password
                if (passwordResultText != null)
                {
                    passwordResultText.text = "WRONG! Try again.";
                    passwordResultText.color = Color.red;
                }
                StartCoroutine(ResetPasswordAttempt());
            }
        }
    }

    void UpdatePasswordDisplay()
    {
        if (passwordResultText != null)
        {
            string display = "Password: ";
            for (int i = 0; i < currentAttempt.Length; i++)
            {
                if (i > 0) display += " -> ";
                display += currentAttempt[i].ToString();
            }
            passwordResultText.text = display;
            passwordResultText.color = Color.white;
        }
    }

    IEnumerator ResetPasswordAttempt()
    {
        yield return new WaitForSeconds(2f);
        currentAttempt = "";
        UpdatePasswordDisplay();
    }

    IEnumerator CompletePortalActivation()
    {
        yield return new WaitForSeconds(2f);
        
        // Portal activation effects
        rend.material = onMaterial;
        
        // Show success message in UIManager
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "Portal Activated! You can now travel through it.";
            UIManager.Instance.perceptionResultText.color = Color.magenta;
        }
        
        EndInteraction();
    }

    // Call this method when PerceptionCheck reveals the password
    public void RevealPassword()
    {
        passwordRevealed = true;
        
        // Show notification that password is available
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "Password revealed! You can now interact with the portal.";
            UIManager.Instance.perceptionResultText.color = Color.cyan;
            StartCoroutine(ClearPasswordNotification());
        }
    }

    IEnumerator ClearPasswordNotification()
    {
        yield return new WaitForSeconds(3f);
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "";
        }
    }

    public void EndInteraction()
    {
        isInteracting = false;
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
            
        if (passwordUI != null)
            passwordUI.SetActive(false);
        
        rend.material = offMaterial;
        playerText.text = "";
    }

    // Button methods for password input
    public void AddBlue() { AddColorToPassword("Blue"); }
    public void AddYellow() { AddColorToPassword("Yellow"); }
    public void AddRed() { AddColorToPassword("Red"); }
    public void AddGreen() { AddColorToPassword("Green"); }
    public void AddWhite() { AddColorToPassword("White"); }
}