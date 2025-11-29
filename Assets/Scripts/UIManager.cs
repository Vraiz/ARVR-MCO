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
    public void ShowMessage(string message, Color color, float displayTime = 3f)
    {
        if (perceptionResultText != null)
        {
            perceptionResultText.text = message;
            perceptionResultText.color = color;
            ShowInteractionUI();
            
            // Cancel previous message if any
            if (currentMessageCoroutine != null)
                StopCoroutine(currentMessageCoroutine);
                
            currentMessageCoroutine = StartCoroutine(HideMessageAfterDelay(displayTime));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideInteractionUI();
    }

    public void ShowInteractionUI()
    {
        SetAllUIActive(true);
    }

    public void HideInteractionUI()
    {
        SetAllUIActive(false);
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