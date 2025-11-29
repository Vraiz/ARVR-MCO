//MaterialResizeOnClick.cs
using UnityEngine;
using System.Collections;

public class MaterialResizeOnClick : MonoBehaviour
{
    [Header("Resize Settings")]
    public GameObject objectToResize;
    public float expandedYScale = 0.6f;
    public float collapsedYScale = 0.1f;
    public float resizeDuration = 0.5f;
    
    [Header("Arcana Check Settings")]
    public int difficultyClass = 12;
    public string successMessage = "Arcana Check Passed! The water flows with magic...";
    public string failMessage = "Arcana Check Failed! The magic remains dormant.";
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isExpanded = true;
    private bool isResizing = false;
    public bool waitingForRoll = false; // CHANGED TO PUBLIC
    private float resizeTimer = 0f;

    void Start()
    {
        if (objectToResize == null)
        {
            objectToResize = this.gameObject;
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
        
        // Start arcana check process
        StartArcanaCheck();
    }

    void StartArcanaCheck()
    {
        if (UIManager.Instance != null)
        {
            // Set roll type FIRST before showing UI
            UIManager.Instance.SetRollType("Arcana Check");
            UIManager.Instance.PositionUIInWorldSpace(this.transform);
            UIManager.Instance.ShowInteractionUI();
            
            // Set up UI for dice roll
            if (UIManager.Instance.perceptionResultText != null)
            {
                UIManager.Instance.perceptionResultText.text = "Tap the button to roll for Arcana Check!";
                UIManager.Instance.perceptionResultText.color = Color.white;
            }
            
            waitingForRoll = true;
            Debug.Log("Arcana check started - waiting for dice roll");
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in StartArcanaCheck!");
        }
    }

    // This method will be called by a dice roll button
    public void ProcessArcanaCheck(int diceRoll)
    {
        if (!waitingForRoll) 
        {
            Debug.Log("Arcana check not waiting for roll");
            return;
        }
        
        waitingForRoll = false;
        bool success = diceRoll >= difficultyClass;
        
        Debug.Log($"Arcana Check: Rolled {diceRoll} vs DC {difficultyClass} - {(success ? "SUCCESS" : "FAIL")}");
        
        // HIDE the dice roll UI but KEEP the result text visible
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractionUI(); // This hides the roll button but keeps result text
        }
        
        string resultMessage = "";
        Color resultColor = Color.white;
        
        if (success)
        {
            ToggleSize();
            resultMessage = $"Roll: {diceRoll} (DC: {difficultyClass})\n\n{successMessage}";
            resultColor = Color.green;
        }
        else
        {
            resultMessage = $"Roll: {diceRoll} (DC: {difficultyClass})\n\n{failMessage}";
            resultColor = Color.red;
        }
        
        ShowMessage(resultMessage, resultColor);
    }    

private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractionUI();
            UIManager.Instance.ClearRollType();
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

    void ShowMessage(string message, Color color)
    {
        if (UIManager.Instance != null)
        {
            // Use ShowMessage which will handle the UI properly
            // This will show the message for 3 seconds then hide everything
            UIManager.Instance.ShowMessage(message, color, 3f);
            Debug.Log("Arcana Check Message shown: " + message);
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
    }
    // Gaze methods
    public void OnGazeEnter()
    {
        Debug.Log("Gaze enter: " + gameObject.name);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetRollType("Arcana Check");
            // Also show hint text
            UIManager.Instance.ShowInteractionHint("Tap for Arcana Check (DC: " + difficultyClass + ")");
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