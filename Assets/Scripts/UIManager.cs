using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("AR Canvas References")]
    public Canvas arCanvas;
    public Camera arCamera;
    
    [Header("Perception Check UI Elements")]
    public TMP_Text perceptionCheckText;
    public GameObject perceptionInteractionUI;
    public TMP_Text perceptionResultText;
    public DiceRoll perceptionDiceRoll;

    [Header("Dice Roll Target")]
    public string perceptionCheckName;

    private XROrigin xrOrigin;

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
            Debug.Log("UIManager Instance already exists, destroying duplicate");
            Destroy(gameObject);
            return;
        }
        
        // Find AR components
        FindARComponents();
        
        // Log the state of UI elements
        Debug.Log($"perceptionCheckText: {perceptionCheckText != null}");
        Debug.Log($"perceptionInteractionUI: {perceptionInteractionUI != null}");
        Debug.Log($"perceptionResultText: {perceptionResultText != null}");
        Debug.Log($"perceptionDiceRoll: {perceptionDiceRoll != null}");
        
        // Disable all UI at start
        SetAllUIActive(false);
    }

    void FindARComponents()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            arCamera = xrOrigin.Camera;
            Debug.Log($"Found XR Origin and camera: {arCamera != null}");
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
            Debug.Log($"Using main camera: {arCamera != null}");
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
        
        // Setup AR canvas if available
        SetupARCanvas();
    }

    void SetupARCanvas()
    {
        if (arCanvas != null)
        {
            // Set canvas to work with AR
            arCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            arCanvas.worldCamera = arCamera;
            Debug.Log("AR Canvas setup complete");
        }
        else
        {
            Debug.LogError("AR Canvas is null in UIManager!");
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
        }
    }

    public void ShowInteractionUI()
    {
        Debug.Log("ShowInteractionUI called");
        SetAllUIActive(true);
    }

    public void HideInteractionUI()
    {
        Debug.Log("HideInteractionUI called");
        SetAllUIActive(false);
    }

    // Method to position UI in world space for AR
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