using UnityEngine;
using System.Collections;

public class MaterialResizeOnClick : MonoBehaviour
{
    [Header("Resize Settings")]
    public GameObject objectToResize; // Drag the object you want to resize here
    public float expandedYScale = 0.6f;
    public float collapsedYScale = 0.1f;
    public float resizeDuration = 0.5f;
    
    [Header("UI Message")]
    public string clickMessage = "The water flows with magic...";
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isExpanded = true;
    private bool isResizing = false;
    private float resizeTimer = 0f;

    void Start()
    {
        // If no object is assigned, use this object
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
        // Handle resize animation
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

    // This gets called by ARGazeDetector when object is tapped
    public void HandleClick()
    {
        Debug.Log("✓ CLICK DETECTED on: " + gameObject.name + " (will resize: " + (objectToResize != null ? objectToResize.name : "NULL") + ")");
        
        // Toggle size
        ToggleSize();
        
        // Show message
        ShowMessage();
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

    void ShowMessage()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage(clickMessage, Color.cyan, 3f);
            Debug.Log("Message shown: " + clickMessage);
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
    }

    public void OnGazeExit()
    {
        Debug.Log("Gaze exit: " + gameObject.name);
    }
}