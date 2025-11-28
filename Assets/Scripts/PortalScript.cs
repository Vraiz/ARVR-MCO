using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Collections;

public class PortalScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    public GameObject passwordUI;
    public TMP_Text passwordDisplayText;
    
    [Header("Password Settings")]
    public int[] correctSequence = new int[] { 3, 2, 1, 4, 5 }; // Blue, Yellow, Red, Green, White
    
    private Renderer rend;
    private bool isInteracting = false;
    private Camera arCamera;
    private XROrigin xrOrigin;
    private bool isGazed = false;
    private bool passwordRevealed = false;
    private bool portalActive = false;
    private int currentSequenceIndex = 0;

    void Start()
    {
        rend = GetComponent<Renderer>();
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
        if (isGazed && !isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            StartInteraction();
        }

        // Back button to exit interaction
        if (isInteracting && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)))
        {
            EndInteraction();
        }
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        isGazed = true;
        if (!isInteracting)
        {
            rend.material = onMaterial;
            if (playerText != null)
            {
                if (portalActive)
                    playerText.text = "Portal is active!";
                else if (passwordRevealed)
                    playerText.text = "Tap to enter password";
                else
                    playerText.text = "Tap to interact with portal";
            }
        }
    }

    public void OnGazeExit()
    {
        isGazed = false;
        if (!isInteracting)
        {
            rend.material = offMaterial;
            if (playerText != null)
                playerText.text = "";
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        
        // Show appropriate UI based on portal state
        if (portalActive)
        {
            // Portal is active - show success message
            if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
            {
                UIManager.Instance.perceptionResultText.text = "Portal is already active! You completed the puzzle!";
                UIManager.Instance.perceptionResultText.color = Color.green;
            }
        }
        else if (passwordRevealed)
        {
            // Show password UI
            if (passwordUI != null)
                passwordUI.SetActive(true);
            
            ResetPasswordAttempt();
            
            // Update text
            if (playerText != null)
                playerText.text = "Enter the password sequence";
        }
        else
        {
            // Show normal interaction UI
            if (interactionUI != null)
                interactionUI.SetActive(true);
            
            // Update text
            if (playerText != null)
                playerText.text = "Portal requires password to activate";
                
            // Show message about needing password
            if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
            {
                UIManager.Instance.perceptionResultText.text = "You need to discover the password first!";
                UIManager.Instance.perceptionResultText.color = Color.yellow;
                StartCoroutine(ClearUIMessage(3f));
            }
        }
        
        // Change material to show active interaction
        rend.material = onMaterial;
    }

    // Password input methods
    // In the AddColorToPassword method, add more debugging:
    public void AddColorToPassword(string color)
    {
        if (!passwordRevealed || !isInteracting) 
        {
            Debug.Log($"Portal: Cannot add color - passwordRevealed: {passwordRevealed}, isInteracting: {isInteracting}");
            return;
        }
        
        int colorNumber = ColorNameToNumber(color);
        Debug.Log($"Portal: Adding color {color} -> {colorNumber}, expecting {correctSequence[currentSequenceIndex]} at index {currentSequenceIndex}");
        
        // DEBUG: Check UIManager status
        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager.Instance is NULL!");
        }
        else if (UIManager.Instance.perceptionResultText == null)
        {
            Debug.LogError("UIManager.Instance.perceptionResultText is NULL!");
        }
        else
        {
            Debug.Log("UIManager and perceptionResultText are available");
        }
        
        if (colorNumber == correctSequence[currentSequenceIndex])
        {
            currentSequenceIndex++;
            UpdatePasswordDisplay();
            
            string progressMessage = $"Correct! {color} added. ({currentSequenceIndex}/{correctSequence.Length})";
            DisplayResultMessage(progressMessage, Color.yellow);
            StartCoroutine(ClearUIMessage(2f));
            // ... rest of the code
        }
        else
        {
            // Wrong color - reset
            currentSequenceIndex = 0;
            // ... existing code
            
            string errorMessage = $"Wrong sequence! {color} is incorrect. Starting over.";
            DisplayResultMessage(errorMessage, Color.red);
            StartCoroutine(ClearUIMessage(3f));
            StartCoroutine(ResetPasswordAfterDelay(2f));
        }}
    int ColorNameToNumber(string color)
    {
        switch(color.ToLower())
        {
            case "blue": return 3;
            case "yellow": return 2;
            case "red": return 1;
            case "green": return 4;
            case "white": return 5;
            default: 
                Debug.LogError($"Unknown color: {color}");
                return 0;
        }
    }

    string NumberToColorName(int number)
    {
        switch(number)
        {
            case 1: return "Red";
            case 2: return "Yellow";
            case 3: return "Blue";
            case 4: return "Green";
            case 5: return "White";
            default: return "Unknown";
        }
    }

    void UpdatePasswordDisplay()
    {
        if (passwordDisplayText != null)
        {
            string display = "Sequence: ";
            for (int i = 0; i < currentSequenceIndex; i++)
            {
                if (i > 0) display += " -> ";
                display += NumberToColorName(correctSequence[i]);
            }
            
            if (currentSequenceIndex < correctSequence.Length)
            {
                display += $"\nProgress: {currentSequenceIndex}/{correctSequence.Length}";
                display += $"\nNext: {NumberToColorName(correctSequence[currentSequenceIndex])}";
            }
            else
            {
                display += "\nComplete!";
            }
            
            passwordDisplayText.text = display;
            passwordDisplayText.color = Color.white;
        }
    }

    IEnumerator ResetPasswordAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetPasswordAttempt();
    }

    void ResetPasswordAttempt()
    {
        currentSequenceIndex = 0;
        if (passwordDisplayText != null)
        {
            passwordDisplayText.text = "Enter password sequence...\n(Blue -> Yellow -> Red -> Green -> White)";
            passwordDisplayText.color = Color.gray;
        }
    }

    IEnumerator PasswordCorrect()
    {
        portalActive = true;
        
        if (passwordDisplayText != null)
        {
            passwordDisplayText.text = "CORRECT! Portal activated!";
            passwordDisplayText.color = Color.green;
        }
        
        // Show success message in UIManager result text
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "🎉 PORTAL ACTIVATED! 🎉\nYou solved the password puzzle!";
            UIManager.Instance.perceptionResultText.color = Color.magenta;
        }
        
        // Portal activation effects
        rend.material = onMaterial;
        
        yield return new WaitForSeconds(3f);
        
        EndInteraction();
    }

    IEnumerator ClearUIMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "";
        }
    }

    // Method called by PerceptionCheck when password is revealed
    public void RevealPassword()
    {
        passwordRevealed = true;
        
        // Show notification that password is available
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "Password revealed: Blue -> Yellow -> Red -> Green -> White\nYou can now activate the portal!";
            UIManager.Instance.perceptionResultText.color = Color.cyan;
            StartCoroutine(ClearUIMessage(5f));
        }
        
        // Update gaze text if currently gazed
        if (isGazed && playerText != null)
        {
            playerText.text = "Tap to enter password";
        }

        Debug.Log("Portal: Password revealed - ready for input!");
    }

    public void EndInteraction()
    {
        isInteracting = false;
        
        // Hide UI
        if (interactionUI != null)
            interactionUI.SetActive(false);
        if (passwordUI != null)
            passwordUI.SetActive(false);
        
        // Reset materials and text based on gaze state
        if (isGazed)
        {
            rend.material = onMaterial;
            if (playerText != null)
            {
                if (portalActive)
                    playerText.text = "Portal is active!";
                else if (passwordRevealed)
                    playerText.text = "Tap to enter password";
                else
                    playerText.text = "Tap to interact with portal";
            }
        }
        else
        {
            rend.material = offMaterial;
            if (playerText != null)
                playerText.text = "";
        }
    }

    // UI Button methods
    public void ClosePortalUI()
    {
        EndInteraction();
    }
    
        // Add this method to PortalScript.cs
    private void DisplayResultMessage(string message, Color color)
    {
        // Try UIManager first
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = message;
            UIManager.Instance.perceptionResultText.color = color;
            Debug.Log($"Displayed via UIManager: {message}");
        }
        // Fallback to local text display
        else if (passwordDisplayText != null)
        {
            passwordDisplayText.text = message;
            passwordDisplayText.color = color;
            Debug.Log($"Displayed via passwordDisplayText: {message}");
        }
        // Last resort - debug log
        else
        {
            Debug.Log($"RESULT: {message}");
        }
    }
    // Password button methods
    public void AddBlue() { AddColorToPassword("Blue"); }
    public void AddYellow() { AddColorToPassword("Yellow"); }
    public void AddRed() { AddColorToPassword("Red"); }
    public void AddGreen() { AddColorToPassword("Green"); }
    public void AddWhite() { AddColorToPassword("White"); }
    public void ClearPassword() { ResetPasswordAttempt(); }
}