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
    
    [Header("UI Elements")]
    public TMP_Text perceptionCheckText;
    public GameObject perceptionInteractionUI;
    public TMP_Text perceptionResultText;
    public DiceRoll perceptionDiceRoll;
    public TMP_Text rollTypeText;
    
    [Header("Dice Roll Target")]
    public string perceptionCheckName;

    private XROrigin xrOrigin;
    private Coroutine currentMessageCoroutine;
    private IDiceCheckable currentCheck;

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

    // Dice Check Management
    public void StartDiceCheck(IDiceCheckable check, string hint = "")
    {
        if (perceptionResultText != null && !string.IsNullOrEmpty(hint))
        {
            perceptionResultText.text = hint;
        }
        
        SetRollType(check.CheckType);
        
        // Use GetTransform() method
        Transform checkTransform = check.GetTransform();
        if (checkTransform != null)
        {
            PositionUIInWorldSpace(checkTransform);
        }
        
        ShowInteractionUI();
        
        currentCheck = check;
        check.IsWaitingForRoll = true;
    }
    
    public void ShowCheckResult(string checkType, int roll, int dc, string message, Color color)
    {
        string resultText = $"{checkType}: Rolled {roll} vs DC {dc}\n\n{message}";
        ShowMessage(resultText, color, 3f);
    }
    
    public void HandleDiceRoll(int result)
    {
        if (currentCheck != null && currentCheck.IsWaitingForRoll)
        {
            currentCheck.ProcessDiceRoll(result);
            currentCheck.IsWaitingForRoll = false;
            currentCheck = null;
        }
        else
        {
            Debug.LogWarning("No dice check waiting for roll!");
        }
    }

    // UI Display Methods
    public void SetRollType(string rollType)
    {
        if (rollTypeText != null)
        {
            rollTypeText.text = rollType;
        }
    }

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
    }

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

    public void ShowMessage(string message, Color color, float displayTime = 3f)
    {
        if (perceptionResultText != null)
        {
            perceptionResultText.text = message;
            perceptionResultText.color = color;
            
            if (perceptionInteractionUI != null)
            {
                perceptionInteractionUI.SetActive(false);
            }
            
            perceptionResultText.gameObject.SetActive(true);
            
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
        ClearRollType();
    }

    public void SetAllUIActive(bool active)
    {
        if (perceptionInteractionUI != null)
            perceptionInteractionUI.SetActive(active);
            
        if (!active)
        {
            if (perceptionCheckText != null)
                perceptionCheckText.text = "";
                
            if (perceptionResultText != null)
                perceptionResultText.text = "";
                
            if (rollTypeText != null)
                rollTypeText.text = "";
        }
    }

    public void PositionUIInWorldSpace(Transform targetTransform, float distance = 1f)
    {
        if (arCanvas != null && arCamera != null)
        {
            Vector3 uiPosition = arCamera.transform.position + arCamera.transform.forward * distance;
            arCanvas.transform.position = uiPosition;
            arCanvas.transform.LookAt(arCamera.transform);
            arCanvas.transform.Rotate(0, 180, 0);
            Debug.Log("UI positioned in world space");
        }
        else
        {
            Debug.LogError("Cannot position UI - arCanvas or arCamera is null");
        }
    }
}