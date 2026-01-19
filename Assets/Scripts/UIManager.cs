//UIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("AR Canvas References")]
    public Canvas arCanvas;
    public Camera arCamera;
    
    [Header("Perception Check UI Elements")]
    public TMP_Text perceptionCheckText;
    public GameObject perceptionInteractionUI;
    public TMP_Text perceptionResultText; // KEEP THIS FOR PORTALSCRIPT
    public DiceRoll perceptionDiceRoll;
    
    [Header("New Roll Type Display")]
    public TMP_Text rollTypeText; // Add this field

    [Header("Dice Roll Target")]
    public string perceptionCheckName;

    private XROrigin xrOrigin;
    private Coroutine currentMessageCoroutine;

    void Awake()
    {
        Debug.Log("UIManager Awake called");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("UIManager Instance set");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        FindARComponents();
        SetAllUIActive(false);
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

    void Start()
    {
        Debug.Log("UIManager Start called");
        
        // Set the dice roll target by name
        if (perceptionDiceRoll != null && !string.IsNullOrEmpty(perceptionCheckName))
        {
            perceptionDiceRoll.SetPerceptionCheckName(perceptionCheckName);
            Debug.Log($"Set perception check name: {perceptionCheckName}");
        }
        
        SetupARCanvas();
    }

    void SetupARCanvas()
    {
        if (arCanvas != null)
        {
            arCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            arCanvas.worldCamera = arCamera;
            Debug.Log("AR Canvas setup complete");
        }
        else
        {
            Debug.LogError("AR Canvas is null in UIManager!");
        }
    }

    // NEW METHOD: Set Roll Type Text
    public void SetRollType(string rollType)
    {
        if (rollTypeText != null)
        {
            rollTypeText.text = rollType;
        }
    }

    // NEW METHOD: Clear Roll Type Text
    public void ClearRollType()
    {
        if (rollTypeText != null)
        {
            rollTypeText.text = "";
        }
    }

    public void RegisterPerceptionCheck(PerceptionCheck perceptionCheck)
    {
        Debug.Log($"Registering PerceptionCheck: {perceptionCheck != null}");
        
        // Set the UI references for the perception check
        if (perceptionCheck != null)
        {
            perceptionCheck.SetUIReferences(perceptionCheckText, perceptionInteractionUI, perceptionResultText, perceptionDiceRoll);
            Debug.Log("PerceptionCheck UI references set");
        }
    }

    // UNIVERSAL UI METHODS - For any interactable object
    public void ShowInteractionHint(string hint)
    {
        if (perceptionCheckText != null)
        {
            perceptionCheckText.text = hint;
        }
    }

    public void ClearInteractionHint()
    {
        if (perceptionCheckText != null)
        {
            perceptionCheckText.text = "";
        }
    }

    // ShowMessage method for MaterialResizeOnClick and other scripts
    // In UIManager.cs, update the ShowMessage method to be more robust:
    public void ShowMessage(string message, Color color, float displayTime = 3f)
    {
        if (perceptionResultText != null)
        {
            perceptionResultText.text = message;
            perceptionResultText.color = color;
            
            // Show ONLY the result text, not the full interaction UI
            if (perceptionInteractionUI != null)
            {
                // Hide the main UI but keep result text visible
                perceptionInteractionUI.SetActive(false);
            }
            
            // Make sure result text is active
            perceptionResultText.gameObject.SetActive(true);
            
            // Cancel previous message if any
            if (currentMessageCoroutine != null)
                StopCoroutine(currentMessageCoroutine);
                
            currentMessageCoroutine = StartCoroutine(HideMessageAfterDelay(displayTime));
            Debug.Log("Message shown: " + message);
        }
        else
        {
            Debug.LogError("perceptionResultText is null in UIManager!");
        }
    }

    
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Hide the result text
        if (perceptionResultText != null)
        {
            perceptionResultText.text = "";
            perceptionResultText.gameObject.SetActive(false);
        }
        
        ClearRollType();
    }
    public void ShowInteractionUI()
    {
        SetAllUIActive(true);
    }

    public void HideInteractionUI()
    {
        SetAllUIActive(false);
        ClearRollType(); // Clear roll type when hiding UI
    }

    public void SetAllUIActive(bool active)
    {
        if (perceptionInteractionUI != null)
            perceptionInteractionUI.SetActive(active);
        else
            Debug.LogWarning("perceptionInteractionUI is null in SetAllUIActive");
            
        // Clear text when hiding UI
        if (!active)
        {
            if (perceptionCheckText != null)
                perceptionCheckText.text = "";
            else
                Debug.LogWarning("perceptionCheckText is null in SetAllUIActive");
                
            if (perceptionResultText != null)
                perceptionResultText.text = "";
            else
                Debug.LogWarning("perceptionResultText is null in SetAllUIActive");
                
            if (rollTypeText != null)
                rollTypeText.text = "";
            else
                Debug.LogWarning("rollTypeText is null in SetAllUIActive");
        }
    }

    public void PositionUIInWorldSpace(Transform targetTransform, float distance = 1f)
    {
        if (arCanvas != null && arCamera != null)
        {
            // Position UI in front of the camera
            Vector3 uiPosition = arCamera.transform.position + arCamera.transform.forward * distance;
            arCanvas.transform.position = uiPosition;
            arCanvas.transform.LookAt(arCamera.transform);
            arCanvas.transform.Rotate(0, 180, 0); // Flip to face camera
            Debug.Log("UI positioned in world space");
        }
        else
        {
            Debug.LogError("Cannot position UI - arCanvas or arCamera is null");
        }
    }
}