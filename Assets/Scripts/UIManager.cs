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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Find AR components
        FindARComponents();
        
        // Disable all UI at start
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
        // Set the dice roll target by name
        if (perceptionDiceRoll != null && !string.IsNullOrEmpty(perceptionCheckName))
        {
            perceptionDiceRoll.SetPerceptionCheckName(perceptionCheckName);
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
        }
    }

    public void RegisterPerceptionCheck(PerceptionCheck perceptionCheck)
    {
        // Set the UI references for the perception check
        perceptionCheck.SetUIReferences(perceptionCheckText, perceptionInteractionUI, perceptionResultText, perceptionDiceRoll);
    }

    public void SetAllUIActive(bool active)
    {
        if (perceptionInteractionUI != null)
            perceptionInteractionUI.SetActive(active);
            
        // Clear text when hiding UI
        if (!active)
        {
            if (perceptionCheckText != null)
                perceptionCheckText.text = "";
            if (perceptionResultText != null)
                perceptionResultText.text = "";
        }
    }

    public void ShowInteractionUI()
    {
        SetAllUIActive(true);
    }

    public void HideInteractionUI()
    {
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
        }
    }
}