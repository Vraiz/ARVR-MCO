using UnityEngine;
using System.Collections;

public class MaterialResizeOnClick : MonoBehaviour, IDiceCheckable
{
    [Header("Resize Settings")]
    public GameObject objectToResize;
    public float expandedYScale = 0.6f;
    public float collapsedYScale = 0.1f;
    public float resizeDuration = 0.5f;
    
    [Header("Dice Check Settings")]
    public int difficultyClass = 12;
    public string checkType = "Arcana Check";
    public string successMessage = "Arcana Check Passed! The water flows with magic...";
    public string failMessage = "Arcana Check Failed! The magic remains dormant.";
    
    public int DifficultyClass => difficultyClass;
    public string CheckType => checkType;
    public bool IsWaitingForRoll { get; set; } = false;
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isExpanded = true;
    private bool isResizing = false;
    private float resizeTimer = 0f;

    void Start()
    {
        if (objectToResize == null)
        {
            objectToResize = gameObject;
            Debug.LogWarning("No objectToResize assigned. Using this GameObject: " + gameObject.name);
        }
        
        originalScale = objectToResize.transform.localScale;
        targetScale = originalScale;
        Debug.Log("MaterialResize ready for: " + objectToResize.name);
    }

    void Update()
    {
        if (isResizing && objectToResize != null)
        {
            resizeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(resizeTimer / resizeDuration);
            objectToResize.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            
            if (progress >= 1f)
            {
                isResizing = false;
                resizeTimer = 0f;
                originalScale = objectToResize.transform.localScale;
                Debug.Log("Resize completed for: " + objectToResize.name);
            }
        }
    }

    public void HandleClick()
    {
        Debug.Log("✓ CLICK DETECTED on: " + gameObject.name);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartDiceCheck(this, "Tap for " + checkType + " (DC: " + difficultyClass + ")");
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in HandleClick!");
        }
    }

    public void ProcessDiceRoll(int diceRoll)
    {
        if (!IsWaitingForRoll) 
        {
            Debug.Log("Dice check not waiting for roll");
            return;
        }
        
        IsWaitingForRoll = false;
        bool success = diceRoll >= difficultyClass;
        
        Debug.Log($"{checkType}: Rolled {diceRoll} vs DC {difficultyClass} - {(success ? "SUCCESS" : "FAIL")}");
        
        string resultMessage = "";
        Color resultColor = Color.white;
        
        if (success)
        {
            ToggleSize();
            resultMessage = successMessage;
            resultColor = Color.green;
        }
        else
        {
            resultMessage = failMessage;
            resultColor = Color.red;
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCheckResult(checkType, diceRoll, difficultyClass, resultMessage, resultColor);
        }
    }

    void ToggleSize()
    {
        if (isResizing || objectToResize == null) return;
        
        if (isExpanded)
        {
            targetScale = new Vector3(originalScale.x, collapsedYScale, originalScale.z);
            isExpanded = false;
            Debug.Log("Collapsing: " + objectToResize.name);
        }
        else
        {
            targetScale = new Vector3(originalScale.x, expandedYScale, originalScale.z);
            isExpanded = true;
            Debug.Log("Expanding: " + objectToResize.name);
        }
        
        isResizing = true;
        resizeTimer = 0f;
    }

    // IDiceCheckable implementation
    public Transform GetTransform()
    {
        return this.transform;
    }

    // Gaze methods
    public void OnGazeEnter()
    {
        Debug.Log("Gaze enter: " + gameObject.name);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetRollType(checkType);
            UIManager.Instance.ShowInteractionHint("Tap for " + checkType + " (DC: " + difficultyClass + ")");
        }
    }

    public void OnGazeExit()
    {
        Debug.Log("Gaze exit: " + gameObject.name);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClearRollType();
            UIManager.Instance.ClearInteractionHint();
        }
    }
}